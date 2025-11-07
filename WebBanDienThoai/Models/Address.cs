using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDienThoai.Models
{
    [Table("ADDRESS")]
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Required, MaxLength(255)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = "Việt Nam";

        [MaxLength(20)]
        public string PostalCode { get; set; }

        public bool IsDefault { get; set; } = false;

        // Navigation
        public Customer Customer { get; set; }
    }
}
