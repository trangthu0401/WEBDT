using Microsoft.AspNetCore.Mvc;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.modelView;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;

namespace WebBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        private readonly DemoWebBanDienThoaiContext _context;

        public AccountController(DemoWebBanDienThoaiContext context)
        {
            _context = context;
        }

        // === HÀM TRỢ GIÚP MỚI ĐỂ LẤY DANH MỤC ===
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
                    .FirstOrDefaultAsync(a => a.Email == model.EmailOrPhone && a.Password == model.Password && a.IsActive == true);

                if (account != null)
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == account.AccountId);
                    if (customer == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin khách hàng.");
                        return View(model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.Email),
                        new Claim("FullName", customer.FullName),
                        new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                        new Claim(ClaimTypes.Role, account.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
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

                var newAccount = new Account
                {
                    Email = model.Email,
                    Password = model.Password,
                    Role = "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                var newCustomer = new Customer
                {
                    AccountId = newAccount.AccountId,
                    FullName = model.FullName
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                var loginModel = new LoginViewModel { EmailOrPhone = model.Email, Password = model.Password };
                return await Login(loginModel);
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
            var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerIdStr))
            {
                return RedirectToAction("Login");
            }

            var customer = await _context.Customers
                                .Include(c => c.Account)
                                .FirstOrDefaultAsync(c => c.CustomerId == int.Parse(customerIdStr));

            if (customer == null)
            {
                return NotFound();
            }

            // === THÊM: Lấy dữ liệu danh mục ===
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
            // === THÊM: Tải lại dữ liệu danh mục khi lỗi ===
            if (!ModelState.IsValid)
            {
                var (dsHangError, totalCountError) = await GetCategoryDataAsync();
                model.BrandCounts = dsHangError;
                model.TotalProductCount = totalCountError;
                return View(model);
            }

            var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == int.Parse(customerIdStr));

            if (customer == null)
            {
                return NotFound();
            }

            customer.FullName = model.FullName;
            customer.Phone = model.Phone;
            customer.BirthDate = model.BirthDate;
            customer.Gender = model.Gender;

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

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

            // === THÊM: Tải lại dữ liệu danh mục sau khi lưu ===
            var (dsHang, totalProductCount) = await GetCategoryDataAsync();
            model.BrandCounts = dsHang;
            model.TotalProductCount = totalProductCount;

            return View(model);
        }
    }
}