using Stripe.Climate;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
    public int OrderDetailId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

    public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
    public decimal? UnitPrice { get; set; }

        // Navigation
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
