// Trong file WebBanDienThoai.Models.ViewModels (hoặc file ViewModels chung của bạn)

public class AddVariantViewModel
{
    // Bắt buộc
    public int ProductId { get; set; }

    // Thông số biến thể
    public string Color { get; set; }
    public string Storage { get; set; } // Ví dụ: 128GB
    public string Ram { get; set; }     // Ví dụ: 8GB

    // Giá và Tồn kho
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; } // Giá khuyến mãi (có thể null)
    public int Stock { get; set; }

    // File ảnh upload (quan trọng cho enctype="multipart/form-data")
    public IFormFile? ImageFile { get; set; }
}