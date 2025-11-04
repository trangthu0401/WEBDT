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

        // Ngăn 5: Chứa danh sách SP bán chạy
        public List<BestSellingProductViewModel> TopSellingProducts { get; set; }
    }
}
