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

        // --- INDEX: QUẢN LÝ SẢN PHẨM ---
        public async Task<IActionResult> Index(int? brandId, string searchId, int pageIndex = 1)
        {
            try
            {
                // ĐƠN GIẢN HÓA QUERY ĐỂ TEST TRƯỚC
                var productsQuery = _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.ProductVariants)
                                            .AsQueryable();

                // 1. Lọc
                if (!string.IsNullOrEmpty(searchId))
                {
                    productsQuery = productsQuery.Where(o =>
                        o.ProductId.ToString().Contains(searchId) ||
                        o.Name.Contains(searchId));
                }

                if (brandId.HasValue && brandId.Value > 0)
                {
                    productsQuery = productsQuery.Where(p => p.BrandId == brandId.Value);
                }

                // 2. Phân trang
                int pageSize = 10;
                int totalItems = await productsQuery.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                pageIndex = Math.Max(1, pageIndex);
                if (pageIndex > totalPages && totalPages > 0) pageIndex = totalPages;

                // 3. Truy vấn đơn giản hóa
                var productList = await productsQuery
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProducAdmintListViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name ?? "N/A",
                        MainImage = p.MainImage,
                        BrandId = p.BrandId,
                        BrandName = p.Brand != null ? p.Brand.BrandName ?? "N/A" : "N/A",
                        CreatedDate = p.CreatedDate,
                        IsActive = p.IsActive,

                        // Tính giá thấp nhất - đơn giản hóa
                        LowestPrice = p.ProductVariants.Any() ? p.ProductVariants.Min(v => v.Price) : 0,

                        // Tính tổng tồn kho - đơn giản hóa
                        TotalStock = p.ProductVariants.Sum(v => v.Stock)
                    })
                    .ToListAsync();

                // 4. Lấy danh sách hãng
                var brandCounts = await _context.Brands
                    .Select(b => new BrandCountViewModel
                    {
                        brandId = b.BrandId,
                        BrandName = b.BrandName ?? "N/A",
                        IsActive = brandId.HasValue && b.BrandId == brandId.Value,
                        Count = _context.Products.Count(p => p.BrandId == b.BrandId)
                    })
                    .OrderBy(b => b.BrandName)
                    .ToListAsync();

                var totalProductCount = await _context.Products.CountAsync();

                // 5. Tạo ViewModel
                var viewModel = new ProductIndexViewModel
                {
                    Products = productList,
                    BrandCounts = brandCounts,
                    TotalProductCount = totalProductCount
                };

                ViewBag.PageIndex = pageIndex;
                ViewBag.TotalPages = totalPages;
                ViewBag.SearchId = searchId;
                ViewBag.BrandId = brandId;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // HIỂN THỊ LỖI CHI TIẾT ĐỂ DEBUG
                return Content($"🔥 LỖI TRONG Product/Index: {ex.Message}<br><br>" +
                              $"Stack Trace: {ex.StackTrace}<br><br>" +
                              $"Inner Exception: {ex.InnerException?.Message}");
            }
        }

        // --- CÁC ACTION KHÁC GIỮ NGUYÊN ---
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                BrandList = await _context.Brands.OrderBy(b => b.BrandName)
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
            if (ModelState.IsValid)
            {
                try
                {
                    string? mainImagePath = null;
                    if (viewModel.MainImageFile != null) mainImagePath = await UploadFile(viewModel.MainImageFile);

                    string? variantImagePath = null;
                    if (viewModel.VariantImageFile != null) variantImagePath = await UploadFile(viewModel.VariantImageFile);

                    // 1. Lưu Product
                    Product newProduct = viewModel.Product!;
                    newProduct.CreatedDate = DateTime.Now;
                    newProduct.IsActive = true;
                    newProduct.MainImage = mainImagePath;

                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync();

                    // 2. Lưu Variant
                    var newVariant = new ProductVariant
                    {
                        ProductId = newProduct.ProductId,
                        Color = viewModel.VariantColor,
                        Storage = viewModel.VariantStorage,
                        RAM = viewModel.VariantRam,
                        Price = viewModel.VariantPrice,
                        Stock = viewModel.VariantStock,
                        ImageUrl = variantImagePath ?? mainImagePath,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.ProductVariants.Add(newVariant);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Thêm sản phẩm thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu: " + ex.Message);
                }
            }

            viewModel.BrandList = await _context.Brands.OrderBy(b => b.BrandName)
                                            .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                                            .ToListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var viewModel = new ProductEditViewModel
            {
                Product = product,
                BrandList = await _context.Brands.OrderBy(b => b.BrandName)
                                          .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                                          .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.Product.ProductId) return NotFound();
            ModelState.Remove("MainImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    var productFromDb = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                    if (productFromDb == null) return NotFound();

                    string? mainImagePath = productFromDb.MainImage;
                    if (viewModel.MainImageFile != null) mainImagePath = await UploadFile(viewModel.MainImageFile);

                    viewModel.Product.MainImage = mainImagePath;
                    viewModel.Product.CreatedDate = productFromDb.CreatedDate;
                    viewModel.Product.UpdatedDate = DateTime.Now;

                    _context.Update(viewModel.Product);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật: " + ex.Message);
                }
            }

            viewModel.BrandList = await _context.Brands.OrderBy(b => b.BrandName)
                                            .Select(b => new SelectListItem { Value = b.BrandId.ToString(), Text = b.BrandName })
                                            .ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.ProductVariants).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product != null)
            {
                if (product.ProductVariants != null) _context.ProductVariants.RemoveRange(product.ProductVariants);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Đã xóa sản phẩm.";
            }
            return RedirectToAction(nameof(Index));
        }

        // --- ACTION TEST ĐỂ DEBUG ---
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
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/images/products/" + fileName;
        }
    }
}