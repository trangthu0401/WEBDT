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
        public string BrandName { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; }
    }
}
