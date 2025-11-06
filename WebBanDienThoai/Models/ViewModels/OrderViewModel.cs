using System;

namespace WebBanDienThoai.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        // Định dạng mã đơn hàng, ví dụ: "HD00001"
        public string OrderCode => $"HD{OrderId:D5}";

        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}