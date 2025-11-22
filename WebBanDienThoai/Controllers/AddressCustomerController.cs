using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    [Authorize] // Bắt buộc đăng nhập
    public class AddressCustomerController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public AddressCustomerController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        private int GetCurrentCustomerId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out int id))
                return 0;
            return id;
        }

        // 1. Lấy danh sách (Partial View)
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var customerId = GetCurrentCustomerId();
            var addresses = await _context.Addresses
                .Where(a => a.CustomerID == customerId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();

            return PartialView("_AddressListPartial", addresses);
        }

        // 2. Lấy chi tiết
        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            var customerId = GetCurrentCustomerId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressID == id && a.CustomerID == customerId);

            if (address == null) return NotFound();

            return Json(new { success = true, data = address });
        }

        // 3. Thêm mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressCheckoutViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            try
            {
                if (model.IsDefault)
                {
                    var oldDefaults = await _context.Addresses
                        .Where(a => a.CustomerID == customerId && a.IsDefault).ToListAsync();
                    foreach (var addr in oldDefaults) addr.IsDefault = false;
                }
                else
                {
                    if (!await _context.Addresses.AnyAsync(a => a.CustomerID == customerId))
                        model.IsDefault = true;
                }

                var address = new Address
                {
                    CustomerID = customerId,
                    City = model.City,
                    District = model.District,
                    Street = model.Street,
                    IsDefault = model.IsDefault,
                    PostalCode = "700000"
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm địa chỉ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 4. Cập nhật
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] AddressCheckoutViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressID == model.AddressId && a.CustomerID == customerId);

            if (address == null) return Json(new { success = false, message = "Không tìm thấy địa chỉ." });

            try
            {
                if (model.IsDefault)
                {
                    var oldDefaults = await _context.Addresses
                        .Where(a => a.CustomerID == customerId && a.IsDefault && a.AddressID != model.AddressId)
                        .ToListAsync();
                    foreach (var addr in oldDefaults) addr.IsDefault = false;
                }

                address.Street = model.Street;
                address.District = model.District;
                address.City = model.City;
                address.IsDefault = model.IsDefault;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 5. Xóa
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var customerId = GetCurrentCustomerId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressID == id && a.CustomerID == customerId);

            if (address == null) return Json(new { success = false, message = "Không tìm thấy." });

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}