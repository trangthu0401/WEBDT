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
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>(); // Model đã có
        public int? BrandId { get; set; }
        public int TotalProductCount { get; set; }
    }

    public class FavoritesViewModel
    {
        public List<ProductListViewModel> FavoriteProducts { get; set; } = new List<ProductListViewModel>();

        // === THÊM 2 DÒNG SAU ===
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int TotalProductCount { get; set; }
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

        // === THÊM 2 DÒNG SAU ===
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int TotalProductCount { get; set; }
    }
}