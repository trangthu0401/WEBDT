using System;
using System.Collections.Generic;

namespace WebBanDienThoai.Models.ViewModels
{
    public class CustomerDetailViewModel
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }

        // SỬA: Thêm dấu ? vì cột Phone có thể NULL trong DB
        public string? Phone { get; set; }

        // SỬA: Thêm dấu ? vì cột Gender có thể NULL trong DB
        public string? Gender { get; set; }

        public DateTime? BirthDate { get; set; } // Đã đúng
        public string CustomerType { get; set; }

        // Thông tin liên quan
        public int AccountID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Danh sách địa chỉ khách hàng
        // Khởi tạo để tránh lỗi NULL nếu danh sách rỗng
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public int AddressID { get; set; }
        public string Street { get; set; }

        // SỬA: Thêm dấu ? vì các cột này cho phép NULL trong DB
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public bool IsDefault { get; set; }
    }
}