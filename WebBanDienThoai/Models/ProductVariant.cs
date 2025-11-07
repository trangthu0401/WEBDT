using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("ProductVariants")]
    public class ProductVariant
    {
        [Key]
    public int VariantId { get; set; }

        [ForeignKey("Product")]
    public int ProductId { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        [MaxLength(20)]
        public string Storage { get; set; }

        [MaxLength(20)]
        public string RAM { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountPrice { get; set; }

        public int Stock { get; set; } = 0;

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

        // Navigation
        public Product Product { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<FavoriteDetail> FavoriteDetails { get; set; }
        public ICollection<ReviewDetail> ReviewDetails { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
