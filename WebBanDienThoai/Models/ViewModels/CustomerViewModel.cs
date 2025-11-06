namespace WebBanDienThoai.Models.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }
        public string CustomerCode => $"0000{CustomerID}"; // hiển thị 00001
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string CustomerType { get; set; } // Thường / VIP

        public bool IsActive { get; set; }
    }
}
