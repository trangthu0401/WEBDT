using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
    public int OrderId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

    public DateTime? OrderDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalAmount { get; set; }

        // ======================================================
        // CÁC CỘT ĐỊA CHỈ "ĐÓNG BĂNG" TẠI THỜI ĐIỂM ĐẶT HÀNG
        // (Đây là các trường đã được thêm vào sau migration)
        // ======================================================

        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [MaxLength(100)]
        public string ShippingFullName { get; set; }

        [Required(ErrorMessage = "SĐT người nhận là bắt buộc")]
        [MaxLength(15)]
        public string ShippingPhone { get; set; }

        [Required(ErrorMessage = "Địa chỉ đường là bắt buộc")]
        [MaxLength(255)]
        public string ShippingStreet { get; set; }

        [MaxLength(100)]
        public string ShippingDistrict { get; set; } // Cho phép null

        [MaxLength(100)]
        public string ShippingCity { get; set; } // Cho phép null

        [Required]
        [MaxLength(100)]
        public string ShippingCountry { get; set; } = "Việt Nam";

        // ======================================================

        // Navigation Properties (Thuộc tính điều hướng)
        public Customer Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public Shipping Shipping { get; set; } // Quan hệ 1-1
    }
}
