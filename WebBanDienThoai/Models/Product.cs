using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
    public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [ForeignKey("Brand")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một hãng sản xuất.")]
        public int BrandId { get; set; }

        [MaxLength(50)]
        public string? Chipset { get; set; }

        [MaxLength(30)]
        public string? OperatingSystem { get; set; }

    public short? BatteryCapacity { get; set; }

        public bool ChargerIncluded { get; set; } = true;

        [Column(TypeName = "decimal(4, 2)")]
    public decimal? ScreenSize { get; set; }

        [MaxLength(40)]
        public string? ScreenTech { get; set; }

    public short? RefreshRate { get; set; }

        [MaxLength(100)]
        public string? RearCamera { get; set; }

        [MaxLength(50)]
        public string? FrontCamera { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
    public decimal? Weight { get; set; }

        [MaxLength(50)]
        public string? Dimensions { get; set; }

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [MaxLength(255)]
        public string? MainImage { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

        // Navigation
        public virtual Brand? Brand { get; set; }
        public virtual ICollection<ProductVariant>? ProductVariants { get; set; } = new List<ProductVariant>();
    }
}
