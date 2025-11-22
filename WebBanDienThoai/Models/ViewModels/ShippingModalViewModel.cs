using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanDienThoai.Models.ViewModels
{
    public class ShippingModalViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đơn vị vận chuyển")]
        public string Carrier { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày dự kiến giao hàng")]
        public DateTime? EstimatedDate { get; set; }
    }
}