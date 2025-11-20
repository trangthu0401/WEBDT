using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Cần thêm cái này cho IFormFile

namespace WebBanDienThoai.Models.ViewModels
{
    // === DÙNG CHO TRANG INDEX (DANH SÁCH) SẢN PHẨM ===
    public class ProductIndexViewModel
    {
        // FIX: Đã đổi từ ProductListViewModel sang ProducAdmintListViewModel
        public List<ProducAdmintListViewModel> Products { get; set; } = new List<ProducAdmintListViewModel>();
        public List<BrandCountViewModel> BrandCounts { get; set; } = new List<BrandCountViewModel>();
        public int TotalProductCount { get; set; }
    }

    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<SelectListItem> BrandList { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Vui lòng chọn ảnh cho sản phẩm")]
        public IFormFile? MainImageFile { get; set; }

        [Required(ErrorMessage = "Màu sắc là bắt buộc")]
        public string VariantColor { get; set; }

        [Required(ErrorMessage = "Dung lượng là bắt buộc")]
        public string VariantStorage { get; set; }
        public string? VariantRam { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        public decimal VariantPrice { get; set; }

        [Required(ErrorMessage = "Tồn kho là bắt buộc")]
        public int VariantStock { get; set; }
        public IFormFile? VariantImageFile { get; set; }
    }

    public class ProductEditViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<SelectListItem> BrandList { get; set; } = new List<SelectListItem>();
        public IFormFile? MainImageFile { get; set; }
    }

    // ViewModel con cho 1 hàng trong bảng Admin (Chứa TotalStock, LowestPrice)
    public class ProducAdmintListViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string MainImage { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public decimal FirstVariantPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public int TotalStock { get; set; }
    }

    public class BrandCountViewModel
    {
        public int brandId { get; set; }
        public string BrandName { get; set; }
        public bool IsActive { get; set; }
        public int Count { get; set; }
    }
}