using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanDienThoai.Models.ViewModels
{
    // ViewModel này chứa TOÀN BỘ thông tin cho trang Chi tiết Đơn hàng
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // --- Thông tin người nhận (Đã "đóng băng" trong bảng Order) ---
        public string ShippingFullName { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingFullAddress { get; set; } // Địa chỉ đầy đủ (gộp)

        // --- Thông tin thanh toán (Từ bảng Payment) ---
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        // --- Thông tin giao vận (Từ bảng Shipping) ---
        public string? Carrier { get; set; } // Đơn vị vận chuyển
        public string? TrackingNumber { get; set; } // Mã vận đơn

        // --- Danh sách các sản phẩm trong đơn hàng ---
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    // Một ViewModel phụ để chứa thông tin 1 dòng sản phẩm
    public class OrderItemViewModel
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string Storage { get; set; }
        public string RAM { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal UnitPrice { get; set; } // Giá tại thời điểm mua

        public int Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalItemPrice => UnitPrice * Quantity; // Tổng tiền của dòng này
    }
}