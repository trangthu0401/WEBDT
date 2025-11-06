using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public CustomerController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // ĐÃ SỬA LẠI HÀM INDEX
        public async Task<IActionResult> Index(string search, string typeFilter, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Customers
                .Include(c => c.Account)
                .AsQueryable();

            // --- Lọc theo từ khóa
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.CustomerID.ToString().Contains(search));

            // --- Lọc theo loại khách hàng
            if (!string.IsNullOrEmpty(typeFilter))

                query = query.Where(c => c.CustomerType == typeFilter);

            int totalItems = await query.CountAsync();

            var customers = await query
                .OrderBy(c => c.CustomerID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    FullName = c.FullName,
                    Gender = c.Gender,
                    CustomerType = c.CustomerType,
                    IsActive = c.Account.IsActive
                })
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerID == id)
                .Select(c => new CustomerDetailViewModel
                {
                    CustomerID = c.CustomerID,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Gender = c.Gender,
                    BirthDate = c.BirthDate,
                    CustomerType = c.CustomerType,
                    AccountID = c.Account.AccountID,
                    Email = c.Account.Email,
                    Role = c.Account.Role,
                    CreatedAt = c.Account.CreatedAt,
                    IsActive = c.Account.IsActive,
                    Addresses = c.Addresses.Select(a => new AddressViewModel
                    {
                        AddressID = a.AddressID,
                        Street = a.Street,
                        District = a.District,
                        City = a.City,
                        Country = a.Country,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound();

            return View(customer);
        }

    }
}
