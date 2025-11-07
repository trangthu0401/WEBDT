using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
    public int CartItemId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

    public int Quantity { get; set; }

        // Navigation
        public Customer Customer { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
