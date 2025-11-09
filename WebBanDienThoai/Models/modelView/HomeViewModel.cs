using System;
using System.Collections.Generic;

namespace WebBanDienThoai.Models.modelView
{
    public class BrandCount
    {
        public int brandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ProductListViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsFavorited { get; set; } = false;
    }

    public class ManageProductsViewModel
    {
        public List<ProductListViewModel> Products { get; set; } = new List<ProductListViewModel>();
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int? BrandId { get; set; }
        public int TotalProductCount { get; set; }
    }

    public class FavoritesViewModel
    {
        public List<ProductListViewModel> FavoriteProducts { get; set; } = new List<ProductListViewModel>();
    }

    // === PHẦN CODE MỚI ĐƯỢC THÊM VÀO ===
    // (Dùng cho tính năng gợi ý tìm kiếm có ảnh, tên, giá)
    public class ProductSuggestionViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public decimal Price { get; set; }
    }
}