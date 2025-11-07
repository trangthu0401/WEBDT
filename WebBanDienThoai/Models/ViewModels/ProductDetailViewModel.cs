using System.Collections.Generic;

namespace WebBanDienThoai.Models.ViewModels
{
    // ViewModel này sẽ chứa thông tin chi tiết của 1 Product
    // và danh sách các Variant của nó
    public class ProductDetailViewModel
    {
        // Chứa thông tin chung của sản phẩm (Tên, Hãng, Chipset...)
        public Product Product { get; set; }

        // Chứa danh sách các biến thể (Màu, RAM, Giá, Tồn kho...)
        // ViewModel này sẽ khởi tạo một danh sách rỗng để tránh lỗi null
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        // Bạn có thể thêm các thuộc tính khác nếu cần (ví dụ: danh sách ảnh review...)
    }
}