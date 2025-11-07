using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
    public int NotificationId { get; set; }

        [ForeignKey("Account")]
        public int AccountID { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Account Account { get; set; }
    }
}
