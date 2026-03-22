using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebBanDienThoai.Models; // Cần using Models

namespace WebBanDienThoai.Models.ViewModels
{
    // === 1. DÙNG CHO TRANG INDEX (HIỂN THỊ) ===
    // (Model chính cho Views/ProductVariant/Index.cshtml)
  
    public class ProductVariantIndexViewModel
    {
        public Product Product { get; set; } = new Product(); // Thông tin sản phẩm cha
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>(); // Danh sách các con

        // Gộp Form Create vào đây để View có thể render Modal
        public ProductVariantCreateViewModel CreateForm { get; set; } = new ProductVariantCreateViewModel();
    }

    // === 2. DÙNG CHO FORM CREATE (THÊM MỚI) ===
    // (Model cho Modal "Thêm mới Biến thể")
    public class ProductVariantCreateViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Màu sắc là bắt buộc")]
        [StringLength(50)]
        public string Color { get; set; }  // ← Non-nullable

        [Required(ErrorMessage = "Dung lượng là bắt buộc")]
        [StringLength(20)]
        public string Storage { get; set; }  // ← Non-nullable

        [StringLength(20)]
        public string Ram { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá phải > 0")]  // ← Force > 0
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Tồn kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không âm")]  // ← Force >= 0
        public int Stock { get; set; }

        public IFormFile ImageFile { get; set; }
    }
    // === 3. DÙNG CHO FORM EDIT (SỬA) ===
    // (Model cho Modal "Sửa Biến thể")
    public class ProductVariantEditViewModel
    {
        public int ProductId { get; set; } // Để biết đường quay lại
        public int VariantId { get; set; } // Để biết sửa cái nào

        [Required]
        public string Color { get; set; }
        [Required]
        public string Storage { get; set; }
        public string? Ram { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}