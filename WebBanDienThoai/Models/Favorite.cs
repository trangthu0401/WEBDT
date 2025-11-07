using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("Favorites")]
    public class Favorite
    {
        [Key]
    public int FavoriteId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Customer Customer { get; set; }
        public ICollection<FavoriteDetail> FavoriteDetails { get; set; }
    }
}
