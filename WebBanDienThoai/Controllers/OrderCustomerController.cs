using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    [Authorize]
    public class OrderCustomerController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;
        private readonly IConfiguration _configuration;

        public OrderCustomerController(DemoWebBanDienThoaiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Helper lấy ID chuẩn từ Cookie
        private int GetCurrentCustomerId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int id))
                return 0;
            return id;
        }

        // ============================================================
        // 1. VIEW CHECKOUT (GET)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Checkout(string itemIds)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == 0) return RedirectToAction("Login", "Account");

            // Parse chuỗi itemIds thành List<int>
            List<int> selectedIds = new List<int>();
            if (!string.IsNullOrEmpty(itemIds))
            {
                selectedIds = itemIds.Split(',')
                                     .Select(s => int.TryParse(s, out int n) ? n : 0)
                                     .Where(n => n > 0)
                                     .ToList();
            }

            if (!selectedIds.Any())
            {
                return RedirectToAction("Index", "Cart"); // Không chọn gì thì quay lại giỏ
            }

            // Lọc sản phẩm trong giỏ dựa trên ID được chọn
            var cartItems = await _context.CartItems
                .Include(c => c.ProductVariant).ThenInclude(v => v.Product)
                .Where(c => c.CustomerID == customerId && selectedIds.Contains(c.CartItemId))
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            // Lấy thông tin khách và địa chỉ mặc định
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerID == customerId);
            var defaultAddr = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.CustomerID == customerId && a.IsDefault);

            // Map ViewModel
            var model = new CheckoutViewModel
            {
                // QUAN TRỌNG: Lưu danh sách ID đã chọn vào ViewModel để dùng cho bước Đặt hàng
                SelectedCartItemIds = selectedIds,

                CartItems = cartItems.Select(c => new CartItemViewModel
                {
                    CartItemId = c.CartItemId,
                    VariantId = c.VariantId,
                    ProductName = c.ProductVariant.Product.Name,
                    VariantName = $"{c.ProductVariant.Color} - {c.ProductVariant.Storage}",
                    ImageUrl = c.ProductVariant.ImageUrl ?? c.ProductVariant.Product.MainImage,
                    Price = (c.ProductVariant.DiscountPrice ?? c.ProductVariant.Price),
                    Quantity = c.Quantity
                }).ToList(),

                SubTotal = cartItems.Sum(x => (x.ProductVariant.DiscountPrice ?? x.ProductVariant.Price) * x.Quantity),

                AddressId = defaultAddr?.AddressID ?? 0,
                ReceiverName = customer.FullName,
                ReceiverPhone = customer.Phone,

                // Tách địa chỉ (nếu có)
                ReceiverAddressDetail = defaultAddr?.Street ?? "",
                ReceiverDistrict = defaultAddr?.District ?? "",
                ReceiverCity = defaultAddr?.City ?? ""
            };

            // VietQR Config
            ViewBag.BankId = _configuration["VietQR:BankId"];
            ViewBag.AccountNo = _configuration["VietQR:AccountNo"];
            ViewBag.AccountName = _configuration["VietQR:AccountName"];
            ViewBag.Template = _configuration["VietQR:Template"];

            return View(model);
        }

        // ============================================================
        // 2. XỬ LÝ ĐẶT HÀNG (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == 0) return Json(new { success = false, message = "Hết phiên đăng nhập." });

            // Validate: Phải có ID sản phẩm được chọn
            if (model.SelectedCartItemIds == null || !model.SelectedCartItemIds.Any())
            {
                return Json(new { success = false, message = "Danh sách sản phẩm thanh toán không hợp lệ." });
            }

            // Validate: Địa chỉ
            if (string.IsNullOrEmpty(model.ReceiverAddressDetail) || string.IsNullOrEmpty(model.ReceiverCity))
            {
                return Json(new { success = false, message = "Thiếu thông tin địa chỉ nhận hàng." });
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // B1: Lấy lại giỏ hàng từ DB NHƯNG CHỈ LẤY NHỮNG ITEM ĐƯỢC CHỌN
                    var dbCartItems = await _context.CartItems
                        .Include(c => c.ProductVariant)
                        .Where(c => c.CustomerID == customerId && model.SelectedCartItemIds.Contains(c.CartItemId)) // <--- FIX LỖI Ở ĐÂY
                        .ToListAsync();

                    if (!dbCartItems.Any())
                        return Json(new { success = false, message = "Sản phẩm chọn mua không còn trong giỏ hàng." });

                    // B2: Tính tiền & Check tồn kho
                    decimal totalAmount = 0;
                    foreach (var item in dbCartItems)
                    {
                        if (item.Quantity > item.ProductVariant.Stock)
                        {
                            return Json(new { success = false, message = $"Sản phẩm mã {item.VariantId} không đủ hàng. Kho còn: {item.ProductVariant.Stock}" });
                        }

                        totalAmount += (item.ProductVariant.DiscountPrice ?? item.ProductVariant.Price) * item.Quantity;
                    }

                    // B3: Trừ mã giảm giá
                    decimal discountVal = 0;
                    if (!string.IsNullOrEmpty(model.CouponCode) && model.CouponCode.ToUpper() == "RABIT10")
                        discountVal = 50000;

                    decimal finalTotal = totalAmount - discountVal;
                    if (finalTotal < 0) finalTotal = 0;

                    // B4: Tạo Đơn Hàng
                    var newOrder = new Order
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        Status = "Chờ xác nhận",
                        TotalAmount = finalTotal,

                        ShippingFullName = model.ReceiverName,
                        ShippingPhone = model.ReceiverPhone,

                        // Gộp địa chỉ để lưu
                        ShippingStreet = model.ReceiverAddressDetail + (string.IsNullOrEmpty(model.ReceiverWard) ? "" : ", " + model.ReceiverWard),
                        ShippingDistrict = model.ReceiverDistrict,
                        ShippingCity = model.ReceiverCity,
                        ShippingCountry = "Việt Nam"
                    };

                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    // B5: Tạo Chi Tiết & Trừ Kho
                    foreach (var item in dbCartItems)
                    {
                        _context.OrderDetails.Add(new OrderDetail
                        {
                            OrderId = newOrder.OrderId,
                            VariantId = item.VariantId,
                            Quantity = item.Quantity,
                            UnitPrice = item.ProductVariant.DiscountPrice ?? item.ProductVariant.Price
                        });

                        item.ProductVariant.Stock -= item.Quantity;
                        _context.ProductVariants.Update(item.ProductVariant);
                    }

                    // B6: Tạo Payment
                    string pStatus = "Chưa thanh toán";
                    DateTime? pDate = null;
                    if (model.PaymentMethod == "Banking")
                    {
                        pStatus = "Đã thanh toán";
                        pDate = DateTime.Now;
                    }

                    _context.Payments.Add(new Payment
                    {
                        OrderId = newOrder.OrderId,
                        PaymentMethod = model.PaymentMethod,
                        PaymentStatus = pStatus,
                        PaymentDate = pDate
                    });

                    // B7: XÓA GIỎ HÀNG (CHỈ XÓA NHỮNG MÓN ĐÃ MUA)
                    _context.CartItems.RemoveRange(dbCartItems);
                    // Các món không nằm trong list dbCartItems vẫn được giữ lại trong DB

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Json(new { success = true, orderId = newOrder.OrderId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
                }
            }
        }

        // Check Coupon
        [HttpPost]
        public IActionResult CheckCoupon(string code)
        {
            if (code?.ToUpper() == "RABIT10") return Json(new { success = true, discount = 50000, message = "Đã giảm 50k" });
            return Json(new { success = false, message = "Mã không hợp lệ" });
        }
    }
}