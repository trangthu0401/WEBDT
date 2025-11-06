using Stripe.Climate;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Shipping")]
    public class Shipping
    {
        [Key]
        public int ShippingId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [MaxLength(100)]
        public string Carrier { get; set; }

        [MaxLength(100)]
        public string TrackingNumber { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? EstimatedDelivery { get; set; }

        public DateTime? DeliveredDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public string Note { get; set; }

        // Navigation
        public Order Order { get; set; }
    }
}