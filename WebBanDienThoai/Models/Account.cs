using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    // Giữ lại Data Annotation và tên lớp đơn
    [Table("ACCOUNT")]
    public class Account
    {
        [Key]
        public int AccountID { get; set; } // Giữ lại tên AccountID

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty; // Khởi tạo để tránh lỗi Non-nullable

        [Required, MaxLength(255)]
        // Khởi tạo Password để khắc phục lỗi "Non-nullable property 'Password' must contain a non-null value"
        public string Password { get; set; } = string.Empty;

        // Thêm cột Phone đã được yêu cầu
        [MaxLength(15)]
        public string? Phone { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Customer";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation Properties

        // Giữ lại 1-1 (Customer) vì đây là mối quan hệ đã cấu hình trong DbContext
        // Thêm ? để tránh cảnh báo Nullability
        public Customer? Customer { get; set; }

        // Giữ ICollection, khởi tạo để tránh cảnh báo Nullability
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}