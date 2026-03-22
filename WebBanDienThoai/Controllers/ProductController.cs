using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DemoWebBanDienThoaiDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? brandId, string? searchId, int pageIndex = 1)
        {
            var productsQuery = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchId))
            {
                var term = searchId.Trim();
                productsQuery = productsQuery.Where(p =>
                    p.ProductId.ToString().Contains(term) ||
                    (p.Name != null && p.Name.Contains(term)));
            }

            if (brandId.HasValue && brandId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.BrandId == brandId.Value);
            }

            const int pageSize = 10;
            var totalItems = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            pageIndex = Math.Max(1, pageIndex);
            if (pageIndex > totalPages && totalPages > 0)
            {
                pageIndex = totalPages;
            }

            // Min/Sum nullable + ?? 0: SQL trả NULL khi không có biến thể — EF Core dịch ổn (không dùng Select/DefaultIfEmpty trên navigation)
            var productList = await productsQuery
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductAdminListViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name ?? "N/A",
                    MainImage = p.MainImage,
                    BrandId = p.BrandId,
                    BrandName = p.Brand != null ? p.Brand.BrandName ?? "N/A" : "N/A",
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    LowestPrice = p.ProductVariants.Min(v => (decimal?)v.Price) ?? 0m,
                    TotalStock = p.ProductVariants.Sum(v => (int?)v.Stock) ?? 0
                })
                .ToListAsync();

            var brandCounts = await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.BrandName)
                .Select(b => new BrandCountViewModel
                {
                    brandId = b.BrandId,
                    BrandName = b.BrandName ?? "N/A",
                    IsActive = brandId.HasValue && b.BrandId == brandId.Value,
                    Count = _context.Products.Count(p => p.BrandId == b.BrandId)
                })
                .ToListAsync();

            var totalProductCount = await _context.Products.CountAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = productList,
                BrandCounts = brandCounts,
                TotalProductCount = totalProductCount
            };

            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = Math.Max(1, totalPages);
            ViewBag.SearchId = searchId;
            ViewBag.BrandId = brandId;

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                BrandList = await _context.Brands
                    .OrderBy(b => b.BrandName)
                    .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                    .ToListAsync(),
                Product = new Product()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.BrandList = await _context.Brands
                    .OrderBy(b => b.BrandName)
                    .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                    .ToListAsync();
                return View(viewModel);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string? mainImagePath = null;
                if (viewModel.MainImageFile != null)
                {
                    mainImagePath = await UploadFile(viewModel.MainImageFile);
                }

                string? variantImagePath = null;
                if (viewModel.VariantImageFile != null)
                {
                    variantImagePath = await UploadFile(viewModel.VariantImageFile);
                }

                // 1) Lưu Product trước
                var newProduct = viewModel.Product!;
                newProduct.CreatedDate = DateTime.Now;
                newProduct.IsActive = true;
                newProduct.MainImage = mainImagePath;

                _context.Products.Add(newProduct);

                // 2) Lấy Identity ID
                await _context.SaveChangesAsync();

                // 3) Gán ProductId đã sinh cho biến thể, rồi lưu tiếp
                var newVariant = new ProductVariant
                {
                    ProductId = newProduct.ProductId,
                    Color = viewModel.VariantColor?.Trim() ?? string.Empty,
                    Storage = viewModel.VariantStorage?.Trim() ?? string.Empty,
                    RAM = string.IsNullOrWhiteSpace(viewModel.VariantRam) ? "-" : viewModel.VariantRam.Trim(),
                    Price = viewModel.VariantPrice,
                    Stock = viewModel.VariantStock,
                    ImageUrl = variantImagePath ?? mainImagePath,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.ProductVariants.Add(newVariant);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["StatusMessage"] = "Thêm sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Lỗi lưu: " + ex.Message);
            }

            viewModel.BrandList = await _context.Brands
                .OrderBy(b => b.BrandName)
                .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                .ToListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                Product = product,
                BrandList = await _context.Brands
                    .OrderBy(b => b.BrandName)
                    .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                    .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.Product.ProductId)
            {
                return NotFound();
            }

            ModelState.Remove("MainImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    var productFromDb = await _context.Products.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.ProductId == id);
                    if (productFromDb == null)
                    {
                        return NotFound();
                    }

                    string? mainImagePath = productFromDb.MainImage;
                    if (viewModel.MainImageFile != null)
                    {
                        mainImagePath = await UploadFile(viewModel.MainImageFile);
                    }

                    viewModel.Product.MainImage = mainImagePath;
                    viewModel.Product.CreatedDate = productFromDb.CreatedDate;
                    viewModel.Product.UpdatedDate = DateTime.Now;

                    _context.Update(viewModel.Product);

                    // Ẩn sản phẩm → ẩn toàn bộ biến thể (chỉ áp dụng khi chuyển sang không hiển thị)
                    if (!viewModel.Product.IsActive)
                    {
                        var variants = await _context.ProductVariants
                            .Where(v => v.ProductId == id)
                            .ToListAsync();
                        foreach (var v in variants)
                        {
                            v.IsActive = false;
                            v.UpdatedDate = DateTime.Now;
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }

            viewModel.BrandList = await _context.Brands
                .OrderBy(b => b.BrandName)
                .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                .ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                if (product.ProductVariants != null && product.ProductVariants.Count > 0)
                {
                    _context.ProductVariants.RemoveRange(product.ProductVariants);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Đã xóa sản phẩm.";
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult Test()
        {
            return Content("✅ ProductController đang hoạt động!");
        }

        [AllowAnonymous]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                var productCount = await _context.Products.CountAsync();
                var brandCount = await _context.Brands.CountAsync();
                return Content($"✅ Database OK! Products: {productCount}, Brands: {brandCount}");
            }
            catch (Exception ex)
            {
                return Content($"❌ Lỗi database: {ex.Message}");
            }
        }

        private async Task<string?> UploadFile(IFormFile file)
        {
            var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadDir, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/products/" + fileName;
        }
    }
}
