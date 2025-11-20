using System;
using System.Collections.Generic;
using WebBanDienThoai.Models;

namespace WebBanDienThoai.Models.ViewModels
{
    public class BrandCount
    {
        public int BrandId { get; set; }
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
        public int SoldCount { get; set; }
    }

    public class ManageProductsViewModel
    {
        public List<ProductListViewModel> Products { get; set; } = new();
        public List<ProductListViewModel> TopSellingProducts { get; set; } = new();
        public List<BrandCount> BrandCounts { get; set; } = new();
        public int? BrandId { get; set; }
        public int TotalProductCount { get; set; }
        public string CurrentSearch { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SelectedRam { get; set; }
        public string? SelectedStorage { get; set; }
    }

    public class FavoritesViewModel
    {
        public List<ProductListViewModel> FavoriteProducts { get; set; } = new();
        public List<BrandCount> BrandCounts { get; set; } = new();
        public int TotalProductCount { get; set; }
        public int? BrandId { get; set; }
        public string CurrentSearch { get; set; } = string.Empty;
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
        public ProductVariant ProductDetail { get; set; } = null!;
        public List<ProductVariant> AllVariants { get; set; } = new();
        public List<ProductVariant> RelatedProducts { get; set; } = new();
        public List<BrandCount> BrandCounts { get; set; } = new();
        public int TotalProductCount { get; set; }
        public int? BrandId { get; set; }
        public string CurrentSearch { get; set; } = string.Empty;
    }
}