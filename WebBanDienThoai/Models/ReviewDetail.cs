using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("ReviewDetails")]
    public class ReviewDetail
    {
        [Key]
    public int ReviewDetailId { get; set; }

        [ForeignKey("Review")]
        public int ReviewId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        [Range(1, 5)]
    public int? Rating { get; set; }

        public string Comment { get; set; }

        // Navigation
        public Review Review { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
