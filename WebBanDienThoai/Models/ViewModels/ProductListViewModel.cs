using System;
using System.Collections.Generic;

namespace WebBanDienThoai.Models.ViewModels
{
    // Lớp mới để lưu tên hãng và số lượng
    public class BrandCount
    {
        public int brandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    // ViewModel cho mỗi dòng sản phẩm trong bảng (Product List)
    public class ProductListViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MainImage { get; set; } // SỬA: Cho phép MainImage là null

        public int? BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public decimal LowestPrice { get; set; } // Dùng cho hiển thị chung
        public int TotalStock { get; set; } // Dùng cho hiển thị chung

        // THÊM 2 TRƯỜNG MỚI: Chỉ dùng để edit trong modal (giá trị của biến thể đầu tiên)
        public decimal FirstVariantPrice { get; set; }
        public int FirstVariantStock { get; set; }
    }

    // ViewModel cho toàn bộ trang quản lý
    public class ManageProductsViewModel
    {
        public List<ProductListViewModel> Products { get; set; } = new List<ProductListViewModel>();
        public List<BrandCount> BrandCounts { get; set; } = new List<BrandCount>();
        public int? BrandId { get; set; }
        public int TotalProductCount { get; set; }
    }

    public class ProductListViewModelComparer : IEqualityComparer<ProductListViewModel>
    {
        public bool Equals(ProductListViewModel? x, ProductListViewModel? y)
        {
            if (x is null || y is null) return false;
            return x.ProductId == y.ProductId;
        }
        public int GetHashCode(ProductListViewModel obj)
        {
            return obj.ProductId.GetHashCode();
        }
    }
}