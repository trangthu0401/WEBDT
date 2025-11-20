using System.Collections.Generic;

namespace WebBanDienThoai.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Ngăn 1: Chứa Tổng doanh thu
        public decimal TotalRevenue { get; set; }

        // Ngăn 2: Chứa Tổng sản phẩm
        public int ProductCount { get; set; }

        // Ngăn 3: Chứa Tổng người dùng
        public int UserCount { get; set; }

        // Ngăn 4: Chứa Tổng đơn hàng
        public int OrderCount { get; set; }

        // Ngăn 5: Chứa danh sách SP bán chạy (FIX: Khởi tạo để tránh Nullability)
        public List<BestSellingProductViewModel> TopSellingProducts { get; set; } = new List<BestSellingProductViewModel>();
    }

    // ViewModel cho sản phẩm bán chạy
    public class BestSellingProductViewModel
    {
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
    }

}