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
        public int FirstVariantId { get; set; }
    }

    public class ManageProductsViewModel
    {
        public List<ProductListViewModel> Products { get; set; } = new List<ProductListViewModel>();
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int? BrandId { get; set; }
        public int TotalProductCount { get; set; }
        public string currentSearch { get; set; }

        // === THÊM CÁC BIẾN CHO BỘ LỌC ===
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SelectedRam { get; set; }
        public string SelectedStorage { get; set; }
    }

    // Các class khác giữ nguyên, không đổi
    public class FavoritesViewModel
    {
        public List<ProductListViewModel> FavoriteProducts { get; set; } = new List<ProductListViewModel>();
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int TotalProductCount { get; set; }
        public int? BrandId { get; set; }
        public string currentSearch { get; set; }
    }

    public class ProductSuggestionViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDetailViewModel
    {
        public ProductVariant ProductDetail { get; set; }
        public List<ProductVariant> AllVariants { get; set; }
        public List<ProductVariant> RelatedProducts { get; set; }
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int TotalProductCount { get; set; }
        public int? BrandId { get; set; }
        public string currentSearch { get; set; }
    }
}