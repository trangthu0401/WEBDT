using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Brands")]
    public class Brand
{
        [Key]
    public int BrandId { get; set; }

        [Required, MaxLength(100)]
    public string BrandName { get; set; } = null!;

        // Navigation
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
}
