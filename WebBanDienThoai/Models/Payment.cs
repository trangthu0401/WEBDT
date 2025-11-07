using Stripe.Climate;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
    public int PaymentId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        // Navigation
        public Order Order { get; set; }
    }
}
