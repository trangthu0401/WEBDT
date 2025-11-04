// Thêm các using này vào đầu file
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebBanDienThoai.Models; // Giả sử Model 'Product' nằm ở đây

namespace WebBanDienThoai.Models.ViewModels
{
    public class CreateProductViewModel
    {
        // 1. Dùng để binding với các thông tin chung của sản phẩm
        public Product Product { get; set; }

        // 2. Dùng để hiển thị danh sách hãng cho dropdown
        public IEnumerable<SelectListItem> BrandList { get; set; }

        // 3. Thông tin cho biến thể ĐẦU TIÊN
        // Chúng ta đưa các trường này ra ngoài vì Product model không chứa chúng

        [Required(ErrorMessage = "Vui lòng nhập Màu sắc")]
        public string VariantColor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Dung lượng")]
        public string VariantStorage { get; set; }

        public string VariantRam { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Giá bán")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal VariantPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
        public int VariantStock { get; set; }


        // 4. Các file được tải lên từ form

        // Ảnh đại diện cho Product
        public IFormFile MainImageFile { get; set; }

        // Ảnh riêng cho biến thể (nếu có)
        public IFormFile VariantImageFile { get; set; }

        // Constructor để khởi tạo, tránh lỗi null
        public CreateProductViewModel()
        {
            Product = new Product();
            BrandList = new List<SelectListItem>();
        }
    }
}