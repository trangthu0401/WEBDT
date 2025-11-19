using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models.ViewModels;
using WebBanDienThoai.Models; // Đảm bảo đã import Model

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

        // GET: /Order/Index (Giữ nguyên)
        public async Task<IActionResult> Index(
            string searchId,
            string statusFilter,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1)
        {
            int pageSize = 10;
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchId))
            {
                if (int.TryParse(searchId, out int id))
                {
                    query = query.Where(o => o.OrderId == id);
                }
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
                query = query.Where(o => o.OrderDate < endDate.Value.AddDays(1));
            }

            int totalItems = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.ShippingFullName,
                    ShippingAddress = o.ShippingDistrict + ", " + o.ShippingCity,
                    OrderDate = o.OrderDate,
                    Status = o.Status
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

        // GET: /Order/Details (Giữ nguyên)
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shipping)
                .Where(o => o.OrderId == id)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // --- Ánh xạ (Map) sang ViewModel (Giữ nguyên) ---
            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                OrderStatus = order.Status,
                OrderDate = order.OrderDate ?? DateTime.Now,
                TotalAmount = order.TotalAmount ?? 0,

                // Thông tin giao hàng (từ bảng Order)
                ShippingFullName = order.ShippingFullName,
                ShippingPhone = order.ShippingPhone,
                ShippingFullAddress = $"{order.ShippingStreet}, {order.ShippingDistrict}, {order.ShippingCity}",

                // Thông tin thanh toán (từ bảng Payment)
                PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMethod ?? "N/A",
                PaymentStatus = order.Payments.FirstOrDefault()?.PaymentStatus ?? "N/A",

                // Thông tin giao vận (từ bảng Shipping)
                Carrier = order.Shipping?.Carrier ?? "N/A",
                TrackingNumber = order.Shipping?.TrackingNumber ?? "N/A",

                // Lấy danh sách sản phẩm
                OrderItems = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    VariantId = od.VariantId,
                    ProductName = $"{od.ProductVariant.Product.Name} ({od.ProductVariant.Color}, {od.ProductVariant.Storage})",
                    ImageUrl = od.ProductVariant.Product.MainImage,
                    Color = od.ProductVariant.Color,
                    Storage = od.ProductVariant.Storage,
                    RAM = od.ProductVariant.RAM,
                    UnitPrice = od.UnitPrice ?? 0,
                    Quantity = od.Quantity ?? 0
                }).ToList()
            };

            return View(viewModel);
        }

        // ======================================================
        // ACTION 2: CẬP NHẬT TRẠNG THÁI (FIXED)
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdvanceStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            string newStatus = order.Status;
            bool canAdvance = true;
            bool shouldUpdateShipping = false;

            switch (order.Status)
            {
                case "Đang xử lý":
                case "Chờ xác nhận":
                    newStatus = "Đang giao";
                    shouldUpdateShipping = true;
                    break;

                case "Đang giao":
                    // ĐỒNG BỘ TRẠNG THÁI CUỐI CÙNG
                    newStatus = "Đã giao";
                    shouldUpdateShipping = true;
                    break;

                case "Đã giao":
                case "Đã hủy":
                    canAdvance = false;
                    break;
            }

            if (canAdvance && order.Status != newStatus)
            {
                try
                {
                    // 1. Kiểm tra và trừ tồn kho (CHỈ ÁP DỤNG KHI CHUYỂN SANG ĐANG GIAO)
                    if (newStatus == "Đang giao")
                    {
                        if (!await CheckAndDeductStock(orderId))
                        {
                            return RedirectToAction("Details", new { id = orderId });
                        }
                    }

                    // 2. Cập nhật trạng thái Order
                    order.Status = newStatus;
                    _context.Orders.Update(order);

                    // 3. Cập nhật bảng Shipping (Nếu cần)
                    if (shouldUpdateShipping)
                    {
                        var shipping = await _context.Shippings.FirstOrDefaultAsync(s => s.OrderId == orderId);

                        if (shipping == null && newStatus == "Đang giao")
                        {
                            // Tạo mới bản ghi Shipping (Thêm Note để tránh lỗi NULL)
                            _context.Shippings.Add(new Shipping
                            {
                                OrderId = orderId,
                                Carrier = "Viettel Post",
                                TrackingNumber = "VT" + orderId + DateTime.Now.ToString("ddMM"),
                                ShippedDate = DateTime.Now,
                                EstimatedDelivery = DateTime.Now.AddDays(3),
                                Status = "Đang vận chuyển",
                                Note = $"Bắt đầu vận chuyển đơn hàng #{orderId}"
                            });
                        }
                        else if (shipping != null && newStatus == "Đã giao")
                        {
                            // Cập nhật DeliveredDate khi hoàn thành
                            shipping.Status = "Đã giao";
                            shipping.DeliveredDate = DateTime.Now;
                            _context.Shippings.Update(shipping);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{orderId} thành '{newStatus}'.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Lỗi đồng bộ dữ liệu. Đơn hàng có thể đã được cập nhật bởi người khác.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi lưu dữ liệu. Chi tiết lỗi: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái này.";
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        // ======================================================
        // HÀM NGHIỆP VỤ: CHECK VÀ TRỪ TỒN KHO (FIXED)
        // ======================================================
        private async Task<bool> CheckAndDeductStock(int orderId)
        {
            var orderItems = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Select(od => new { od.VariantId, od.Quantity })
                .ToListAsync();

            foreach (var item in orderItems)
            {
                var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                int quantity = item.Quantity ?? 0;

                if (variant == null)
                {
                    TempData["ErrorMessage"] = $"Lỗi dữ liệu: Không tìm thấy biến thể sản phẩm (VariantId: {item.VariantId}) trong kho.";
                    return false;
                }

                if (variant.Stock < quantity)
                {
                    var productInfo = await _context.Products
                        .Where(p => p.ProductId == variant.ProductId)
                        .Select(p => new { p.Name, variant.Color, variant.Storage, variant.Stock })
                        .FirstOrDefaultAsync();

                    TempData["ErrorMessage"] = $"Không đủ tồn kho cho sản phẩm: {productInfo?.Name} ({productInfo?.Color}, {productInfo?.Storage}). Tồn kho hiện tại: {productInfo?.Stock}.";
                    return false;
                }

                variant.Stock -= quantity;
                _context.ProductVariants.Update(variant);
            }

            return true;
        }
    }
}