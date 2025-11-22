using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.ViewModels;
using BCrypt.Net;

namespace WebBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public AccountController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a =>
                    (a.Email == model.EmailOrPhone || (a.Customer != null && a.Customer.Phone == model.EmailOrPhone))
                    && a.IsActive == true);

            if (account != null && BCrypt.Net.BCrypt.Verify(model.Password, account.Password))
            {
                // TẠO DANH SÁCH QUYỀN HẠN (CLAIM)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim(ClaimTypes.Role, account.Role),
                    
                    // QUAN TRỌNG 1: Để HomeController lấy được ID người dùng
                    new Claim(ClaimTypes.NameIdentifier, account.Customer?.CustomerID.ToString() ?? account.AccountID.ToString()),
                    
                    // QUAN TRỌNG 2: Đặt tên claim là "FullName" để khớp với View @User.FindFirstValue("FullName")
                    new Claim("FullName", account.Customer?.FullName ?? account.Email),
                    
                    // Giữ lại ID tài khoản nếu cần dùng
                    new Claim("AccountID", account.AccountID.ToString())
                };

                await SignInUserAsync(claims, model.RememberMe);

                // Logic chuyển hướng
                if (account.Role == "Admin" && model.LoginAsAdmin)
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email, SĐT hoặc mật khẩu không chính xác.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Accounts.AnyAsync(a => a.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }
            if (await _context.Customers.AnyAsync(c => c.Phone == model.Phone))
            {
                ModelState.AddModelError("Phone", "SĐT đã được sử dụng.");
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

                var newCustomer = new Customer
                {
                    AccountID = newAccount.AccountID,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Gender = model.Gender,
                    BirthDate = model.BirthDate,
                    CustomerType = "Thường"
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        [Route("/Account/Profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                Console.WriteLine("🔍 Profile GET action called");

                var accountId = int.Parse(User.FindFirstValue("AccountID"));

                var account = await _context.Accounts
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.AccountID == accountId);

                if (account == null || account.Customer == null)
                {
                    Console.WriteLine("❌ Account or Customer not found");
                    return NotFound();
                }

                var model = new AccountViewModels
                {
                    Email = account.Email,
                    FullName = account.Customer.FullName,
                    Phone = account.Customer.Phone,
                    Gender = account.Customer.Gender,
                    BirthDate = account.Customer.BirthDate
                };

                Console.WriteLine("✅ Profile data loaded successfully");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Profile GET: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [Route("/Account/Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AccountViewModels model)
        {
            try
            {
                Console.WriteLine("🔍 Profile POST action called");

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ ModelState invalid");
                    return View(model);
                }

                var accountId = int.Parse(User.FindFirstValue("AccountID"));

                var account = await _context.Accounts
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.AccountID == accountId);

                if (account == null || account.Customer == null)
                {
                    Console.WriteLine("❌ Account or Customer not found in POST");
                    return NotFound();
                }

                // Kiểm tra số điện thoại đã tồn tại chưa (trừ chính tài khoản hiện tại)
                if (!string.IsNullOrEmpty(model.Phone) &&
                    await _context.Customers.AnyAsync(c => c.Phone == model.Phone && c.CustomerID != account.Customer.CustomerID))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng bởi tài khoản khác.");
                    Console.WriteLine("❌ Phone number already exists");
                    return View(model);
                }

                // Cập nhật thông tin
                account.Customer.FullName = model.FullName;
                account.Customer.Phone = model.Phone;
                account.Customer.Gender = model.Gender;
                account.Customer.BirthDate = model.BirthDate;

                _context.Customers.Update(account.Customer);
                await _context.SaveChangesAsync();

                // Cập nhật claim FullName nếu có thay đổi
                if (User.FindFirstValue("FullName") != model.FullName)
                {
                    await UpdateFullNameClaim(model.FullName);
                }

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                Console.WriteLine("✅ Profile updated successfully");
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Profile POST: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View(model);
            }
        }

        // Hàm hỗ trợ cập nhật claim FullName
        private async Task UpdateFullNameClaim(string fullName)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var existingClaim = identity.FindFirst("FullName");

            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            identity.AddClaim(new Claim("FullName", fullName));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Hàm hỗ trợ đăng nhập
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

        // POST: ToggleActive (cho admin)
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

                string statusMessage = account.IsActive ? "mở khóa" : "khóa";
                TempData["SuccessMessage"] = $"Đã {statusMessage} tài khoản '{account.Email}' thành công.";
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái tài khoản.";
            }

            return RedirectToAction("Details", "Customer", new { id = customerId });
        }

        // Test route - tạm thời để kiểm tra
        [HttpGet]
        [Route("/Account/Test")]
        public IActionResult Test()
        {
            Console.WriteLine("✅ Test route works!");
            return Content("Test route works! - AccountController is working");
        }
    }
}