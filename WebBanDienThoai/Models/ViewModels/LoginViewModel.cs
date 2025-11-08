using System.ComponentModel.DataAnnotations;

namespace WebBanDienThoai.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc SĐT")]
        [Display(Name = "Email hoặc Số điện thoại")]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ tôi?")]
        public bool RememberMe { get; set; }

        // === THUỘC TÍNH MỚI ===
        [Display(Name = "Bạn muốn đăng nhập với quyền Admin?")]
        public bool LoginAsAdmin { get; set; }
    }
}