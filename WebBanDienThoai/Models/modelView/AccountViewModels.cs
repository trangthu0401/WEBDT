using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanDienThoai.Models.modelView // <-- TÔI ĐÃ SỬA DÒNG NÀY
{
    // Dùng cho Login.cshtml
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ tôi")]
        public bool RememberMe { get; set; }
    }

    // Dùng cho Register.cshtml
    public class RegisterViewModel
    {
        // === DÒNG MỚI THÊM VÀO ===
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }
        // === KẾT THÚC DÒNG MỚI ===

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }

    // Dùng cho Profile.cshtml
    public class ProfileViewModel
    {
        [Display(Name = "Email (Không thể thay đổi)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }
    }
}