using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
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

        // GET: /Order/Index
        public async Task<IActionResult> Index(
            string searchId,
            string statusFilter,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1)
        {
            int pageSize = 10;

            // Bắt đầu câu query
            var query = _context.Orders
                .AsQueryable(); ;

            // --- 1. Lọc theo ID đơn hàng ---
            if (!string.IsNullOrEmpty(searchId))
            {
                if (int.TryParse(searchId, out int id))
                {
                    query = query.Where(o => o.OrderId == id);
                }
            }

            // --- 2. Lọc theo Trạng thái ---
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // --- 3. Lọc theo Ngày (OrderDate) ---
            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // Thêm 1 ngày và tìm < (để bao gồm cả ngày endDate)
                query = query.Where(o => o.OrderDate < endDate.Value.AddDays(1));
            }

            // --- 4. Phân trang (Lấy tổng số trước khi phân trang) ---
            int totalItems = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate) // Sắp xếp mới nhất lên đầu
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

            // --- 5. Gửi dữ liệu filter và phân trang sang View ---
            ViewBag.SearchId = searchId;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd"); // Định dạng cho input type="date"
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Lấy các dòng sản phẩm
                    .ThenInclude(od => od.ProductVariant) // Lấy thông tin biến thể (màu, dung lượng...)
                        .ThenInclude(pv => pv.Product) // Lấy thông tin sản phẩm gốc (tên, ảnh)
                .Include(o => o.Payments) // Lấy thông tin thanh toán
                .Include(o => o.Shipping) // Lấy thông tin giao vận
                .Where(o => o.OrderId == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // --- Ánh xạ (Map) sang ViewModel ---
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
                    // Lấy tên SP + Màu + Dung lượng
                    ProductName = $"{od.ProductVariant.Product.Name} ({od.ProductVariant.Color}, {od.ProductVariant.Storage})",
                    ImageUrl = od.ProductVariant.Product.MainImage, // (Hoặc ImageUrl của biến thể nếu có)
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
        // ACTION 2: CẬP NHẬT TRẠNG THÁI (CHỈ TIẾN, KHÔNG LÙI)
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdvanceStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            string newStatus = order.Status; // Giữ nguyên nếu không thay đổi

            // Logic "Chỉ tiến, không lùi"
            switch (order.Status)
            {
                case "Chờ xác nhận":
                    newStatus = "Đang giao";
                    break;

                case "Đang giao":
                    newStatus = "Đã giao";

                    // Cập nhật luôn bảng Shipping (nếu có)
                    var shipping = await _context.Shippings.FirstOrDefaultAsync(s => s.OrderId == orderId);
                    if (shipping != null)
                    {
                        shipping.Status = "Đã giao";
                        shipping.DeliveredDate = DateTime.Now;
                        _context.Shippings.Update(shipping);
                    }
                    break;

                case "Đã giao":
                    // Không làm gì, không thể đi tiếp
                    break;

                case "Đã hủy":
                    // Không làm gì, không thể cập nhật
                    break;
            }

            // Chỉ cập nhật nếu trạng thái thực sự thay đổi
            if (order.Status != newStatus)
            {
                order.Status = newStatus;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{orderId} thành '{newStatus}'.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái này.";
            }

            // Quay lại đúng trang chi tiết
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}