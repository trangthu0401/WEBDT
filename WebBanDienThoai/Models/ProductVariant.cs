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
        public string Color { get; set; } = null!;

        [MaxLength(20)]
        public string Storage { get; set; } = null!;

        [MaxLength(20)]

        public string RAM { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountPrice { get; set; }

        public int Stock { get; set; } = 0;

        [MaxLength(255)]
    public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

        // Navigation
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<FavoriteDetail> FavoriteDetails { get; set; } = new List<FavoriteDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ReviewDetail> ReviewDetails { get; set; } = new List<ReviewDetail>();
}
}
