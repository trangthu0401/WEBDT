using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("FavoriteDetails")]
    public class FavoriteDetail
    {
        [Key]
        public int FavoriteDetailId { get; set; }

        [ForeignKey("Favorite")]
        public int FavoriteId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        // Navigation
        public Favorite Favorite { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}