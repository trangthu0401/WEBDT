// Thêm các using cần thiết
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models; // Cần Models
using WebBanDienThoai.Models.ViewModels; // <-- Cần tạo các ViewModel này

namespace WebBanDienThoai.Controllers
{
    // Ghi chú: Đây là Controller mới, tách từ AdminController
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        // Ghi chú: Đổi tên từ DemoWebBanDienThoaiContext -> DemoWebBanDienThoaiDbContext
        private readonly DemoWebBanDienThoaiDbContext _context;

        public DashboardController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // --- 2. TRANG TỔNG QUAN (DASHBOARD) ---
        // GET: /Dashboard/Index (Hoặc /Admin/Index tùy vào Route của bạn)
        public async Task<IActionResult> Index()
        {
            try
            {
                // Mặc định là tháng hiện tại
                var currentDate = DateTime.Now;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var dashboardViewModel = new DashboardViewModel
                {
                    TotalRevenue = await GetTotalRevenue(currentMonth, currentYear),
                    ProductCount = await GetProductCount(),
                    UserCount = await GetUserCount(),
                    OrderCount = await GetOrderCount(currentMonth, currentYear),
                    TopSellingProducts = await GetTopSellingProducts(5, currentMonth, currentYear)
                };
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải dữ liệu Dashboard: {ex.Message}");
                return View(new DashboardViewModel { TopSellingProducts = new List<BestSellingProductViewModel>() });
            }
        }

        // --- HÀM HỖ TRỢ (PRIVATE) CHO DASHBOARD ---

        // Các hàm helper cho Dashboard - ĐÃ THÊM THAM SỐ THÁNG/NĂM
        private async Task<decimal> GetTotalRevenue(int month, int year)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.HasValue &&
                           o.OrderDate.Value.Month == month &&
                           o.OrderDate.Value.Year == year)
                .SumAsync(o => o.TotalAmount ?? 0m);
        }

        private async Task<int> GetProductCount()
        {
            return await _context.Products.CountAsync();
        }

        private async Task<int> GetUserCount()
        {
            return await _context.Customers.CountAsync();
        }

        private async Task<int> GetOrderCount(int month, int year)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.HasValue &&
                           o.OrderDate.Value.Month == month &&
                           o.OrderDate.Value.Year == year)
                .CountAsync();
        }

        // Hàm helper lấy Top 5 SP bán chạy - ĐÃ THÊM LỌC THEO THÁNG
        private async Task<List<BestSellingProductViewModel>> GetTopSellingProducts(int topN = 5, int? month = null, int? year = null)
        {
            // Lấy query OrderDetails cơ bản
            var orderDetailsQuery = _context.OrderDetails
                .Include(od => od.Order) // Include Order để lấy ngày đặt hàng
                .Where(od => od.VariantId != null);

            // Lọc theo tháng/năm nếu có
            if (month.HasValue && year.HasValue)
            {
                orderDetailsQuery = orderDetailsQuery.Where(od =>
                    od.Order.OrderDate.HasValue &&
                    od.Order.OrderDate.Value.Month == month.Value &&
                    od.Order.OrderDate.Value.Year == year.Value);
            }

            var topVariantsInfo = await orderDetailsQuery
                .GroupBy(od => od.VariantId)
                .Select(g => new {
                    VariantId = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topN)
                .ToListAsync();

            var variantIds = topVariantsInfo.Select(v => v.VariantId).ToList();

            var variants = await _context.ProductVariants
                                     .Include(v => v.Product)
                                         .ThenInclude(p => p.Brand)
                                     .Where(v => variantIds.Contains(v.VariantId))
                                     .ToListAsync();

            var result = new List<BestSellingProductViewModel>();
            foreach (var topInfo in topVariantsInfo)
            {
                var variant = variants.FirstOrDefault(v => v.VariantId == topInfo.VariantId);
                if (variant != null && variant.Product != null)
                {
                    result.Add(new BestSellingProductViewModel
                    {
                        ProductName = (variant.Product.Name ?? "N/A") +
                                      (!string.IsNullOrEmpty(variant.Color) ? $" ({variant.Color}" : "") +
                                      (!string.IsNullOrEmpty(variant.Storage) ? $" - {variant.Storage})" : (!string.IsNullOrEmpty(variant.Color) ? ")" : "")),
                        ImageUrl = variant.ImageUrl ?? variant.Product.MainImage,
                        QuantitySold = topInfo.TotalQuantity,
                        Price = variant.DiscountPrice ?? variant.Price,
                        Stock = variant.Stock 
                    });
                }
            }
            return result.OrderByDescending(r => r.QuantitySold).ToList();
        }

        // API để load lại top products theo tháng
        [HttpGet]
        public async Task<IActionResult> GetTopProductsByMonth(int year, int month)
        {
            try
            {
                var topProducts = await GetTopSellingProducts(5, month, year);
                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lấy dữ liệu sản phẩm bán chạy: " + ex.Message);
            }
        }

        // GET: /Dashboard/GetRevenueByDay?year=2025&month=10 (API cho biểu đồ doanh thu)
        [HttpGet]
        public async Task<IActionResult> GetRevenueByDay(int year, int month)
        {
            try
            {
                // 1. Lấy dữ liệu thô từ CSDL
                var revenueData = await _context.Orders
                    .Where(o => o.OrderDate.HasValue &&
                                o.OrderDate.Value.Year == year &&
                                o.OrderDate.Value.Month == month)
                    .GroupBy(o => o.OrderDate.Value.Day)
                    .Select(g => new
                    {
                        Day = g.Key,
                        Total = g.Sum(o => o.TotalAmount)
                    })
                    .ToDictionaryAsync(k => k.Day, v => v.Total);

                // 2. Tạo nhãn cho tất cả các ngày trong tháng
                int daysInMonth = DateTime.DaysInMonth(year, month);
                var labels = new List<string>();
                var data = new List<decimal>();

                for (int i = 1; i <= daysInMonth; i++)
                {
                    labels.Add("Ngày " + i);
                    if (revenueData.ContainsKey(i))
                    {
                        data.Add(revenueData[i] ?? 0m); // Fix: Convert decimal? to decimal
                    }
                    else
                    {
                        data.Add(0);
                    }
                }

                // 3. Trả về JSON
                return Json(new { labels, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ: " + ex.Message);
            }
        }

    } // <-- Đóng class DashboardController
}