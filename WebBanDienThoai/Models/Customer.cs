using Stripe;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("CUSTOMER")]
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [ForeignKey("Account")]
        public int AccountID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; } = "Nam";

        public DateTime? BirthDate { get; set; }

        [Required, MaxLength(20)]
        public string CustomerType { get; set; } = "Thường";

        // Navigation
        public Account Account { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
