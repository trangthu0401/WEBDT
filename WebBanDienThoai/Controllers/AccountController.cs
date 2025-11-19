using Microsoft.AspNetCore.Mvc;
using WebBanDienThoai.Models;
using WebBanDienThoai.ViewModels; // <-- Giả định modelView được đổi tên thành ViewModels
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
using BCrypt.Net; // <--- Giữ lại BCrypt cho bảo mật

namespace WebBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        // *** HỢP NHẤT: Giữ tên DbContext từ bản Incoming/Program.cs ***
        private readonly DemoWebBanDienThoaiDbContext _context;

        public AccountController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // === HÀM TRỢ GIÚP MỚI ĐỂ LẤY DANH MỤC (TỪ BẢN BUG2) ===
        private async Task<(List<BrandCount> Brands, int TotalCount)> GetCategoryDataAsync()
        {
            var dsHang = await _context.Brands
                .AsNoTracking()
                .Where(b => b.Products.Any(p => p.IsActive == true))
                .Select(b => new BrandCount
                {
                    brandId = b.BrandId,
                    BrandName = b.BrandName,
                    Count = b.Products.Count(p => p.IsActive == true)
                })
                .ToListAsync();

            var totalProductCount = await _context.Products
                .CountAsync(p => p.IsActive == true && p.ProductVariants.Any(v => v.IsActive == true));

            return (dsHang, totalProductCount);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var account = await _context.Accounts
                    // HỢP NHẤT: Giữ logic kiểm tra Email/Phone từ bản Current
                    .FirstOrDefaultAsync(a => (a.Email == model.EmailOrPhone || a.Phone == model.EmailOrPhone) && a.IsActive == true);

                // *** HỢP NHẤT: GIỮ BẢO MẬT BCrypt từ bản Current ***
                if (account != null && BCrypt.Net.BCrypt.Verify(model.Password, account.Password))
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == account.AccountId);

                    // Logic tạo Customer nếu chưa tồn tại (giống bản Current trước đó)
                    if (customer == null)
                    {
                        // Tạm thời tạo Customer nếu chưa có (để tránh lỗi)
                        customer = new Customer
                        {
                            AccountId = account.AccountId,
                            FullName = account.Email.Split('@')[0]
                            // Các trường khác sẽ là null/default
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();
                    }

                    // *** HỢP NHẤT: Sử dụng Claim ID và FullName từ bản Incoming ***
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.Email),
                        new Claim("FullName", customer.FullName), // Claim FullName
                        new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()), // Claim CustomerId
                        new Claim(ClaimTypes.Role, account.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ. Vui lòng thử lại.");
                }
            }
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (existingAccount != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // *** HỢP NHẤT: GIỮ BẢO MẬT BCrypt từ bản Current ***
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newAccount = new Account
                {
                    Email = model.Email,
                    Password = hashedPassword, // Mật khẩu đã Hash
                    Role = "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                var newCustomer = new Customer
                {
                    AccountId = newAccount.AccountId,
                    FullName = model.FullName,
                    CustomerType = "Thường",
                    Gender = "Khác"
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                // Chuyển hướng đến trang Login
                TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /Account/Logout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // Lấy CustomerId (ClaimTypes.NameIdentifier) thay vì AccountID
            var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var customer = await _context.Customers
                                        .Include(c => c.Account)
                                        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null || customer.Account == null)
            {
                return NotFound();
            }

            // === TÍCH HỢP TÍNH NĂNG MỚI: Lấy dữ liệu danh mục ===
            var (dsHang, totalProductCount) = await GetCategoryDataAsync();

            var model = new ProfileViewModel
            {
                Email = customer.Account.Email,
                FullName = customer.FullName,
                Phone = customer.Phone,
                BirthDate = customer.BirthDate,
                Gender = customer.Gender,

                // === Gán dữ liệu danh mục ===
                BrandCounts = dsHang,
                TotalProductCount = totalProductCount
            };

            return View(model);
        }

        // POST: /Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            // === Tải lại dữ liệu danh mục khi ModelState lỗi (tính năng mới) ===
            if (!ModelState.IsValid)
            {
                var (dsHangError, totalCountError) = await GetCategoryDataAsync();
                model.BrandCounts = dsHangError;
                model.TotalProductCount = totalCountError;
                return View(model);
            }

            var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerIdStr) || !int.TryParse(customerIdStr, out int customerId))
            {
                return Unauthorized();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound("Không tìm thấy hồ sơ khách hàng.");
            }

            customer.FullName = model.FullName;
            customer.Phone = model.Phone;
            customer.BirthDate = model.BirthDate;
            customer.Gender = model.Gender;

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                // Cập nhật Claim FullName trong Cookie
                var identity = (ClaimsIdentity)User.Identity;
                var oldClaim = identity.FindFirst("FullName");
                if (oldClaim != null)
                {
                    identity.RemoveClaim(oldClaim);
                }
                identity.AddClaim(new Claim("FullName", model.FullName));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể lưu thay đổi. Thử lại sau.");
            }

            // === Tải lại dữ liệu danh mục sau khi lưu (tính năng mới) ===
            var (dsHang, totalProductCount) = await GetCategoryDataAsync();
            model.BrandCounts = dsHang;
            model.TotalProductCount = totalProductCount;

            return View(model);
        }
    }
}