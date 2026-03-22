using System;

namespace WebBanDienThoai.Models.ViewModels
{
    /// <summary>
    /// Một dòng hiển thị trên trang Admin / Product / Index.
    /// </summary>
    public class ProductAdminListViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>Đường dẫn ảnh đại diện (có thể null hoặc rỗng).</summary>
        public string? MainImage { get; set; }

        public int BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        /// <summary>Giá thấp nhất trong các biến thể; 0 nếu chưa có biến thể.</summary>
        public decimal LowestPrice { get; set; }

        /// <summary>Tổng tồn kho các biến thể; 0 nếu không có biến thể.</summary>
        public int TotalStock { get; set; }
    }
}
