using System.Collections.Generic;
using System.Linq;

namespace WebBanDienThoai.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }      // Giá hiện tại
        public decimal? OldPrice { get; set; }  // Giá gốc
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public bool IsSelected { get; set; } = true; // Mặc định chọn

        public decimal TotalPrice => Price * Quantity;
        public decimal SavingAmount => (OldPrice.HasValue && OldPrice > Price) ? (OldPrice.Value - Price) * Quantity : 0;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Chỉ tính tổng tiền của các item được chọn (IsSelected = true)
        public decimal GrandTotal => Items.Where(x => x.IsSelected).Sum(x => x.TotalPrice);

        // Tổng tiền tiết kiệm
        public decimal TotalSavings => Items.Where(x => x.IsSelected).Sum(x => x.SavingAmount);

        public int TotalQuantity => Items.Count();
        public int SelectedCount => Items.Count(x => x.IsSelected);
    }
}