using System.Collections.Generic;

namespace WebBanDienThoai.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Thông tin hiển thị và binding
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }

        // Các trường địa chỉ tách biệt để lưu DB chuẩn xác
        public string ReceiverAddressDetail { get; set; } // Số nhà, tên đường
        public string ReceiverWard { get; set; }          // Phường/Xã
        public string ReceiverDistrict { get; set; }      // Quận/Huyện
        public string ReceiverCity { get; set; }          // Tỉnh/Thành

        public int AddressId { get; set; } // ID địa chỉ đang chọn

        // Giỏ hàng & Tiền
        public List<int> SelectedCartItemIds { get; set; } = new List<int>();
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal ShippingFee { get; set; } = 0;
        public decimal GrandTotal => SubTotal - DiscountAmount + ShippingFee;

        // Input Post lên
        public string PaymentMethod { get; set; } = "COD";
        public string Note { get; set; }
        public string CouponCode { get; set; }
    }

    // Model dùng cho API Thêm/Sửa địa chỉ
    public class AddressCheckoutViewModel
    {
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public bool IsDefault { get; set; }
    }
}