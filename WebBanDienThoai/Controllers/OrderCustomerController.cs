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

        // Helper: Lấy ID khách hàng hiện tại
        private int GetCurrentCustomerId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int id))
                return 0;
            return id;
        }

        // ============================================================
        // 1. LỊCH SỬ MUA HÀNG (HISTORY) - ĐÃ SỬA LỖI VIEWMODEL
        // ============================================================
        public async Task<IActionResult> History(string searchId, string statusFilter, string paymentMethod, string datePreset, DateTime? startDate, DateTime? endDate)
        {
            int userId = GetCurrentCustomerId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // 1. Query dữ liệu
            var query = _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductVariant).ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Where(o => o.CustomerID == userId)
                .AsQueryable();

            // 2. Áp dụng bộ lọc
            if (!string.IsNullOrEmpty(searchId) && int.TryParse(searchId, out int id))
            {
                query = query.Where(o => o.OrderId == id);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                string method = paymentMethod.ToUpper().Trim();
                if (method == "QR CODE")
                    query = query.Where(o => o.Payments.Any(p => p.PaymentMethod.Contains("QR") || p.PaymentMethod.Contains("Chuyển khoản") || p.PaymentMethod.Contains("Banking")));
                else if (method == "COD")
                    query = query.Where(o => !o.Payments.Any(p => p.PaymentMethod.Contains("QR") || p.PaymentMethod.Contains("Chuyển khoản") || p.PaymentMethod.Contains("Banking")));
            }

            // 3. Lấy danh sách
            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();

            // 4. Truyền dữ liệu bộ lọc qua ViewBag (Thay vì ViewModel để tránh lỗi thiếu file)
            // Đây là cách giải quyết vấn đề "Không có ViewModel" của bạn
            ViewBag.SearchId = searchId;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.DatePreset = datePreset;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(orders); // Trả về List<Order> trực tiếp
        }

        // ============================================================
        // 2. CHI TIẾT ĐƠN HÀNG (DETAILS) - HỖ TRỢ POPUP
        // ============================================================
        public async Task<IActionResult> Details(int id)
        {
            int userId = GetCurrentCustomerId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductVariant).ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            // BẢO MẬT: Không cho xem đơn của người khác
            if (order.CustomerID != userId) return Forbid();

            // [QUAN TRỌNG] Kiểm tra nếu gọi từ AJAX (Popup) thì trả về PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_OrderDetailPartial", order);
            }

            // Nếu truy cập trực tiếp bằng URL thì trả về trang Full
            return View(order);
        }

        // ============================================================
        // 3. THANH TOÁN (CHECKOUT)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Checkout(string itemIds)
        {
            int customerId = GetCurrentCustomerId();
            if (customerId == 0) return RedirectToAction("Login", "Account");

            List<int> selectedIds = new List<int>();
            if (!string.IsNullOrEmpty(itemIds))
            {
                selectedIds = itemIds.Split(',')
                                     .Select(s => int.TryParse(s, out int n) ? n : 0)
                                     .Where(n => n > 0)
                                     .ToList();
            }

            if (!selectedIds.Any()) return RedirectToAction("Index", "Cart");

            var cartItems = await _context.CartItems
                .Include(c => c.ProductVariant).ThenInclude(v => v.Product)
                .Where(c => c.CustomerID == customerId && selectedIds.Contains(c.CartItemId))
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerID == customerId);
            var defaultAddr = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.CustomerID == customerId && a.IsDefault);

            var model = new CheckoutViewModel
            {
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
                ReceiverAddressDetail = defaultAddr?.Street ?? "",
                ReceiverDistrict = defaultAddr?.District ?? "",
                ReceiverCity = defaultAddr?.City ?? ""
            };

            ViewBag.BankId = _configuration["VietQR:BankId"];
            ViewBag.AccountNo = _configuration["VietQR:AccountNo"];
            ViewBag.AccountName = _configuration["VietQR:AccountName"];
            ViewBag.Template = _configuration["VietQR:Template"];

            return View(model);
        }

        // ============================================================
        // 4. ĐẶT HÀNG (PLACE ORDER)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutViewModel model)
        {
            int customerId = GetCurrentCustomerId();
            if (customerId == 0) return Json(new { success = false, message = "Hết phiên đăng nhập." });

            if (model.SelectedCartItemIds == null || !model.SelectedCartItemIds.Any())
                return Json(new { success = false, message = "Vui lòng chọn sản phẩm." });

            if (string.IsNullOrEmpty(model.ReceiverAddressDetail) || string.IsNullOrEmpty(model.ReceiverCity))
                return Json(new { success = false, message = "Vui lòng nhập địa chỉ." });

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbCartItems = await _context.CartItems
                        .Include(c => c.ProductVariant)
                        .Where(c => c.CustomerID == customerId && model.SelectedCartItemIds.Contains(c.CartItemId))
                        .ToListAsync();

                    if (!dbCartItems.Any())
                        return Json(new { success = false, message = "Giỏ hàng thay đổi." });

                    decimal totalAmount = 0;
                    foreach (var item in dbCartItems)
                    {
                        if (item.Quantity > item.ProductVariant.Stock)
                            return Json(new { success = false, message = $"Hết hàng: {item.VariantId}" });
                        totalAmount += (item.ProductVariant.DiscountPrice ?? item.ProductVariant.Price) * item.Quantity;
                    }

                    decimal discountVal = 0;
                    if (!string.IsNullOrEmpty(model.CouponCode) && model.CouponCode.ToUpper() == "RABIT10")
                        discountVal = 50000;

                    var newOrder = new Order
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        Status = "Chờ xác nhận",
                        TotalAmount = (totalAmount - discountVal) < 0 ? 0 : (totalAmount - discountVal),
                        ShippingFullName = model.ReceiverName,
                        ShippingPhone = model.ReceiverPhone,
                        ShippingStreet = model.ReceiverAddressDetail + (string.IsNullOrEmpty(model.ReceiverWard) ? "" : ", " + model.ReceiverWard),
                        ShippingDistrict = model.ReceiverDistrict,
                        ShippingCity = model.ReceiverCity,
                        ShippingCountry = "Việt Nam"
                    };

                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

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

                    string pStatus = "Chưa thanh toán";
                    DateTime? pDate = null;
                    if (model.PaymentMethod.Contains("Banking") || model.PaymentMethod.Contains("QR"))
                    {
                        pStatus = "Đã thanh toán"; pDate = DateTime.Now;
                    }

                    _context.Payments.Add(new Payment
                    {
                        OrderId = newOrder.OrderId,
                        PaymentMethod = model.PaymentMethod,
                        PaymentStatus = pStatus,
                        PaymentDate = pDate
                    });

                    _context.CartItems.RemoveRange(dbCartItems);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Json(new { success = true, orderId = newOrder.OrderId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
            }
        }

        // ============================================================
        // 5. HỦY ĐƠN HÀNG (CANCEL ORDER)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId, string reason)
        {
            int userId = GetCurrentCustomerId();
            if (userId == 0) return Json(new { success = false, message = "Hết phiên đăng nhập." });

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails).ThenInclude(od => od.ProductVariant)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerID == userId);

                if (order == null) return Json(new { success = false, message = "Đơn hàng không tồn tại." });

                if (order.Status != "Chờ xác nhận" && order.Status != "Chờ xử lý")
                {
                    return Json(new { success = false, message = "Không thể hủy đơn hàng này." });
                }

                foreach (var detail in order.OrderDetails)
                {
                    if (detail.ProductVariant != null)
                    {
                        detail.ProductVariant.Stock += detail.Quantity ?? 0;
                        _context.ProductVariants.Update(detail.ProductVariant);
                    }
                }

                order.Status = "Đã hủy";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã hủy đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CheckCoupon(string code)
        {
            if (code?.ToUpper() == "RABIT10") return Json(new { success = true, discount = 50000 });
            return Json(new { success = false });
        }
    }
}