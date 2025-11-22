using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public OrderController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. DANH SÁCH ĐƠN HÀNG (ĐÃ FIX LỖI DATA IS NULL)
        // ============================================================
        public async Task<IActionResult> Index(string searchId, string statusFilter, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchId) && int.TryParse(searchId, out int id))
                query = query.Where(o => o.OrderId == id);

            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(o => o.Status == statusFilter);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate < endDate.Value.AddDays(1));

            int totalItems = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,

                    // FIX LỖI: Dùng toán tử ?? để xử lý nếu dữ liệu bị NULL
                    CustomerName = o.ShippingFullName ?? "Khách lẻ",

                    // FIX LỖI: Cộng chuỗi an toàn. Nếu District null thì thay bằng rỗng
                    ShippingAddress = (o.ShippingDistrict ?? "") + ", " + (o.ShippingCity ?? ""),

                    OrderDate = o.OrderDate,
                    Status = o.Status ?? "Chờ xác nhận",
                    TotalAmount = o.TotalAmount ?? 0
                })
                .ToListAsync();

            ViewBag.SearchId = searchId;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(orders);
        }

        // ============================================================
        // 2. CHI TIẾT ĐƠN HÀNG
        // ============================================================
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.ProductVariant).ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                OrderStatus = order.Status,
                OrderDate = order.OrderDate ?? DateTime.Now,
                TotalAmount = order.TotalAmount ?? 0,
                ShippingFullName = order.ShippingFullName,
                ShippingPhone = order.ShippingPhone,

                // Xử lý hiển thị địa chỉ chi tiết
                ShippingFullAddress = $"{order.ShippingStreet}, {order.ShippingDistrict}, {order.ShippingCity}",

                PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMethod ?? "N/A",
                PaymentStatus = order.Payments.FirstOrDefault()?.PaymentStatus ?? "N/A",
                Carrier = order.Shipping?.Carrier ?? "Chưa có",
                TrackingNumber = order.Shipping?.TrackingNumber ?? "---",
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    VariantId = od.VariantId,
                    ProductName = $"{od.ProductVariant.Product.Name} ({od.ProductVariant.Color}, {od.ProductVariant.Storage})",
                    ImageUrl = od.ProductVariant.ImageUrl ?? od.ProductVariant.Product.MainImage,
                    UnitPrice = od.UnitPrice ?? 0,
                    Quantity = od.Quantity ?? 0
                }).ToList()
            };

            return View(viewModel);
        }

        // ============================================================
        // 3. BẮT ĐẦU GIAO HÀNG (Từ Modal: Chờ xác nhận -> Đang giao)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShipping(ShippingModalViewModel model)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderId);

            if (order == null) return NotFound();

            // Chỉ xử lý khi chuyển từ "Chờ xác nhận/Đang xử lý" -> "Đang giao"
            if (order.Status == "Chờ xác nhận" || order.Status == "Đang xử lý")
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Trừ kho (QUAN TRỌNG)
                        if (!await CheckAndDeductStock(order.OrderId))
                        {
                            return RedirectToAction("Details", new { id = order.OrderId });
                        }

                        // 2. Update trạng thái Order
                        order.Status = "Đang giao";
                        _context.Orders.Update(order);

                        // 3. Tạo bản ghi Shipping
                        var shipping = new Shipping
                        {
                            OrderId = order.OrderId,
                            Carrier = model.Carrier, // Lấy từ Modal
                            TrackingNumber = GetTrackingPrefix(model.Carrier) + DateTime.Now.ToString("ddMM") + order.OrderId,
                            ShippedDate = DateTime.Now,
                            EstimatedDelivery = model.EstimatedDate, // Lấy từ Modal
                            Status = "Đang vận chuyển",
                            Note = $"Đã bàn giao cho {model.Carrier}"
                        };
                        _context.Shippings.Add(shipping);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = $"Đơn hàng #{order.OrderId} đã được giao cho {model.Carrier}.";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                    }
                }
            }

            return RedirectToAction("Details", new { id = model.OrderId });
        }

        // ============================================================
        // 4. HOÀN TẤT ĐƠN HÀNG (Đang giao -> Đã giao)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelivered(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order != null && order.Status == "Đang giao")
            {
                // 1. Update Order
                order.Status = "Đã giao";

                // 2. Update Shipping DeliveredDate
                if (order.Shipping != null)
                {
                    order.Shipping.Status = "Giao thành công";
                    order.Shipping.DeliveredDate = DateTime.Now;
                }

                // 3. Update Payment (Nếu là COD -> Đã thanh toán)
                var payment = order.Payments.FirstOrDefault();
                if (payment != null && payment.PaymentMethod == "COD" && payment.PaymentStatus != "Đã thanh toán")
                {
                    payment.PaymentStatus = "Đã thanh toán";
                    payment.PaymentDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xác nhận giao hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Trạng thái đơn hàng không hợp lệ.";
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        // ============================================================
        // 5. HỦY ĐƠN HÀNG (Hoàn trả kho)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId, string cancelReason)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null || order.Status == "Đã giao") return RedirectToAction("Details", new { id = orderId });

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Nếu đang giao (đã trừ kho) -> Phải cộng lại kho
                    if (order.Status == "Đang giao")
                    {
                        foreach (var item in order.OrderDetails)
                        {
                            var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                            if (variant != null) variant.Stock += item.Quantity ?? 0;
                        }
                    }

                    order.Status = "Đã hủy";
                    // Có thể lưu cancelReason vào bảng Note hoặc Log nếu cần

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Đã hủy đơn hàng.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi hủy đơn: " + ex.Message;
                }
            }
            return RedirectToAction("Details", new { id = orderId });
        }

        // --- Helper: Kiểm tra và trừ kho ---
        private async Task<bool> CheckAndDeductStock(int orderId)
        {
            var orderItems = await _context.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync();
            foreach (var item in orderItems)
            {
                var variant = await _context.ProductVariants.Include(v => v.Product).FirstOrDefaultAsync(v => v.VariantId == item.VariantId);
                if (variant == null || variant.Stock < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {variant?.Product?.Name ?? "Unknown"} không đủ tồn kho.";
                    return false;
                }
                variant.Stock -= item.Quantity ?? 0;
            }
            return true;
        }

        // --- Helper: Prefix mã vận đơn ---
        private string GetTrackingPrefix(string carrier) => carrier switch
        {
            "Giao Hàng Nhanh" => "GHN",
            "Giao Hàng Tiết Kiệm" => "GHTK",
            "Viettel Post" => "VT",
            "J&T Express" => "JT",
            "Shopee Express" => "SPX",
            _ => "SHIP"
        };
    }
}