using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanDienThoai.ViewModels
{
    public class RegisterViewModel
    {
        // ============================
        // 1. THÔNG TIN CÁ NHÂN BỔ SUNG
        // ============================

        [Required(ErrorMessage = "Vui lòng nhập họ tên đầy đủ")]
        [Display(Name = "Họ tên đầy đủ")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [Display(Name = "Giới tính")]
        public string Gender { get; set; } // Giả định kiểu string cho Dropdown (Nam/Nữ/Khác)

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; } // Dùng DateTime? vì có thể không bắt buộc hoặc để an toàn

        // ============================
        // 2. THÔNG TIN ĐĂNG NHẬP
        // ============================

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}