// GHI CHÚ: Đã thêm/sửa các using
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

// GHI CHÚ: Xóa using Org.BouncyCastle.Crypto.Generators; (Thừa)
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.ViewModels; // GHI CHÚ: Sửa lại namespace cho đúng

namespace WebBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        // GHI CHÚ: Sửa lại tên DbContext (bỏ "Db")
        private readonly DemoWebBanDienThoaiDbContext _context;

        // GHI CHÚ: Sửa lại tên DbContext (bỏ "Db")
        public AccountController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int accountId, int customerId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            account.IsActive = !account.IsActive;

            try
            {
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                // GHI CHÚ: Tối ưu lại thông báo (dùng toán tử 3 ngôi)
                string statusMessage = account.IsActive ? "mở khóa" : "khóa";
                TempData["SuccessMessage"] = $"Đã {statusMessage} tài khoản '{account.Email}' thành công.";
            }
            catch (DbUpdateException ex)
            {
                // GHI CHÚ: Nên log lỗi
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái tài khoản.";
            }

            return RedirectToAction("Details", "Customer", new { id = customerId });
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // GHI CHÚ: Sửa lỗi Logic (Login bằng SĐT)
            // 1. Phải Include(a => a.Customer)
            // 2. Thêm điều kiện check a.Customer.Phone
            var account = await _context.Accounts
                .Include(a => a.Customer) // <-- Thêm Include
                .FirstOrDefaultAsync(a =>
                    (a.Email == model.EmailOrPhone || (a.Customer != null && a.Customer.Phone == model.EmailOrPhone))
                    && a.IsActive == true);

            if (account != null && BCrypt.Net.BCrypt.Verify(model.Password, account.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim("AccountID", account.AccountID.ToString()),
                    new Claim(ClaimTypes.Role, account.Role)
                };

                await SignInUserAsync(claims, model.RememberMe);



                // ==============================================
                // === SỬA LẠI LOGIC CHUYỂN HƯỚNG ===
                // ==============================================

                // 1. Người dùng tick vào ô "Đăng nhập với quyền Admin"
                if (model.LoginAsAdmin)
                {
                    // 1a. Kiểm tra xem họ CÓ PHẢI là Admin không
                    if (account.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Dashboard"); // (ĐÚNG) Admin -> Dashboard
                    }
                    else
                    {
                        // 1b. (SAI) Customer tick ô Admin -> Đăng xuất & Báo lỗi
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        ModelState.AddModelError(string.Empty, "Bạn không có quyền đăng nhập với tư cách Quản trị viên.");
                        return View(model);
                    }
                }

                // 2. Người dùng KHÔNG tick ô "Admin" (Đăng nhập bình thường)
                // (Cả Customer và Admin [nếu quên tick] đều sẽ vào trang Home)
                return RedirectToAction("Index", "Home");
                // ==============================================
                // === KẾT THÚC SỬA ===
                // ==============================================
            }

            ModelState.AddModelError(string.Empty, "Email, SĐT hoặc mật khẩu không chính xác.");
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _context.Accounts.AnyAsync(a => a.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newAccount = new Account
            {
                Email = model.Email,
                Password = hashedPassword,
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                // GHI CHÚ: Logic tự động tạo Customer (giống trong Profile)
                // Điều này giúp Profile (GET) không cần kiểm tra null
                var newCustomer = new Customer
                {
                    AccountID = newAccount.AccountID,
                    FullName = model.Email.Split('@')[0], // Tên mặc định
                    Gender = "Khác",
                    CustomerType = "Thường"
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            // GHI CHÚ: Sửa (Exception) -> (DbUpdateException) để bắt lỗi CSDL cụ thể hơn
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message); // Log lỗi
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi CSDL. Vui lòng thử lại.");
                return View(model);
            }
        }

        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var accountIdString = User.FindFirstValue("AccountID");
            if (string.IsNullOrEmpty(accountIdString))
            {
                return Unauthorized("Không tìm thấy thông tin tài khoản.");
            }
            var accountId = int.Parse(accountIdString);

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return Unauthorized();

            // GHI CHÚ: Đã sửa logic.
            // Nhờ logic mới trong Register (POST), customer sẽ luôn tồn tại.
            // Bỏ khối "if (customer == null)"
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == accountId);
            if (customer == null)
            {
                // Dự phòng (nếu tài khoản được tạo trước khi sửa logic Register)
                return NotFound("Lỗi: Không tìm thấy hồ sơ. Vui lòng liên hệ hỗ trợ.");
            }

            var viewModel = new ProfileViewModel
            {
                Email = account.Email,
                FullName = customer.FullName,
                Phone = customer.Phone,
                Gender = customer.Gender,
                BirthDate = customer.BirthDate
            };

            return View(viewModel);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            // GHI CHÚ: Gán lại Email (vì nó là readonly và không được post về)
            model.Email = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accountIdString = User.FindFirstValue("AccountID");
            var accountId = int.Parse(accountIdString);

            var customerToUpdate = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == accountId);
            if (customerToUpdate == null)
            {
                return NotFound("Không tìm thấy hồ sơ khách hàng.");
            }

            customerToUpdate.FullName = model.FullName;
            customerToUpdate.Phone = model.Phone;
            customerToUpdate.Gender = model.Gender;
            customerToUpdate.BirthDate = model.BirthDate;

            try
            {
                _context.Customers.Update(customerToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message); // Log lỗi
                ModelState.AddModelError("", "Không thể lưu thay đổi. Vui lòng thử lại.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(List<Claim> claims, bool isPersistent)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
        }

        

        
    }
}