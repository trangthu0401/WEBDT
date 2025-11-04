namespace WebBanDienThoai.Models.ViewModels
{
    public class BestSellingProductViewModel
    {
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; } // Dùng để tính trạng thái (Còn hàng, Sắp hết)
        public List<BestSellingProductViewModel> TopSellingProducts { get; set; } = new List<BestSellingProductViewModel>();
    }
}
