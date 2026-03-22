// Thêm các using cần thiết
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class ProductVariantController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductVariantController(DemoWebBanDienThoaiDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- 1. DANH SÁCH BIẾN THỂ (Trang chi tiết) ---
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
            {
                TempData["StatusMessage"] = "Lỗi: Sản phẩm không hợp lệ.";
                return RedirectToAction("Index", "Product");
            }

            try
            {
                var product = await _context.Products
                                        .Include(p => p.Brand)
                                        .Include(p => p.ProductVariants)
                                        .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    TempData["StatusMessage"] = "Lỗi: Không tìm thấy sản phẩm.";
                    return RedirectToAction("Index", "Product");
                }

                var viewModel = new ProductVariantIndexViewModel
                {
                    Product = product,
                    Variants = product.ProductVariants.ToList(),
                    CreateForm = new ProductVariantCreateViewModel { ProductId = productId }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy chi tiết sản phẩm: {ex.Message}");
                TempData["StatusMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index", "Product");
            }
        }

        // --- 2. THÊM BIẾN THỂ MỚI ---
        // --- 2. THÊM BIẾN THỂ MỚI ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantCreateViewModel viewModel)
        {
            // ← SỬA: Dùng viewModel.ProductId thay vì productId
            if (viewModel.ProductId <= 0)
            {
                TempData["StatusMessage"] = "Lỗi: Sản phẩm không hợp lệ.";
                return RedirectToAction("Index", "Product");
            }

            // ← KIỂM TRA MODELSTATE CHI TIẾT
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                TempData["StatusMessage"] = "Lỗi: " + string.Join(" | ", errors);
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            // ← KIỂM TRA LOGIC THÊM
            if (viewModel.Price <= 0)
            {
                TempData["StatusMessage"] = "Lỗi: Giá phải lớn hơn 0.";
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            if (viewModel.Stock < 0)
            {
                TempData["StatusMessage"] = "Lỗi: Tồn kho không được âm.";
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            try
            {
                string? variantImagePath = null;
                if (viewModel.ImageFile != null)
                {
                    variantImagePath = await UploadFile(viewModel.ImageFile);
                }

                string storageValue = viewModel.Storage?.Trim() ?? string.Empty;
                string ramValue = viewModel.Ram?.Trim() ?? string.Empty;

                var newVariant = new ProductVariant
                {
                    ProductId = viewModel.ProductId,
                    Color = viewModel.Color?.Trim() ?? string.Empty,
                    Storage = storageValue.Replace("GB", "").Trim(),
                    RAM = ramValue.Replace("GB", "").Trim(),
                    Price = viewModel.Price,
                    DiscountPrice = viewModel.DiscountPrice,
                    Stock = viewModel.Stock,
                    ImageUrl = variantImagePath,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.ProductVariants.Add(newVariant);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "✅ Thêm biến thể mới thành công!";
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"Inner: {dbEx.InnerException?.Message}");
                TempData["StatusMessage"] = $"❌ Lỗi database: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm biến thể: {ex.Message}");
                TempData["StatusMessage"] = $"❌ Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
        }
        // --- 3. SỬA BIẾN THỂ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVariantEditViewModel viewModel)
        {
            if (viewModel.Price < 0 || viewModel.Stock < 0)
            {
                TempData["StatusMessage"] = "Lỗi: Giá hoặc Tồn kho không thể âm.";
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                TempData["StatusMessage"] = "Lỗi: " + string.Join(" | ", errors);
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            try
            {
                var variantToUpdate = await _context.ProductVariants.FindAsync(viewModel.VariantId);

                if (variantToUpdate == null)
                {
                    TempData["StatusMessage"] = "❌ Lỗi: Không tìm thấy biến thể.";
                    return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
                }

                variantToUpdate.Color = viewModel.Color?.Trim() ?? string.Empty;
                variantToUpdate.Storage = viewModel.Storage?.Trim() ?? string.Empty;
                variantToUpdate.RAM = viewModel.Ram?.Trim() ?? string.Empty;
                variantToUpdate.Price = viewModel.Price;
                variantToUpdate.DiscountPrice = viewModel.DiscountPrice;
                variantToUpdate.Stock = viewModel.Stock;
                variantToUpdate.UpdatedDate = DateTime.Now;

                _context.ProductVariants.Update(variantToUpdate);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "✅ Cập nhật biến thể thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sửa biến thể: {ex.Message}");
                TempData["StatusMessage"] = $"❌ Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
        }

        // --- 4. XÓA BIẾN THỂ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int variantId, int productId)
        {
            var variantToDelete = await _context.ProductVariants.FindAsync(variantId);

            if (variantToDelete == null)
            {
                TempData["StatusMessage"] = "❌ Lỗi: Không tìm thấy biến thể để xóa.";
                if (productId > 0)
                    return RedirectToAction(nameof(Index), new { productId = productId });
                return RedirectToAction("Index", "Product");
            }

            if (productId == 0)
            {
                productId = variantToDelete.ProductId;
            }

            try
            {
                _context.ProductVariants.Remove(variantToDelete);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "✅ Đã xóa biến thể thành công.";
            }
            catch (DbUpdateException dbEx)
            {
                var baseException = dbEx.GetBaseException() as SqlException;

                if (baseException != null && baseException.Number == 547)
                {
                    string errorMessage = baseException.Message;

                    if (errorMessage.Contains("OrderDetails"))
                    {
                        TempData["StatusMessage"] = "❌ Không thể xóa. Biến thể này đã tồn tại trong 'Chi tiết Đơn hàng'.";
                    }
                    else if (errorMessage.Contains("CartItems"))
                    {
                        TempData["StatusMessage"] = "❌ Không thể xóa. Biến thể này trong 'Giỏ hàng' khách.";
                    }
                    else if (errorMessage.Contains("ReviewDetails"))
                    {
                        TempData["StatusMessage"] = "❌ Không thể xóa. Biến thể này đã được 'Đánh giá'.";
                    }
                    else if (errorMessage.Contains("FavoriteDetails"))
                    {
                        TempData["StatusMessage"] = "❌ Không thể xóa. Biến thể trong 'Danh sách Yêu thích'.";
                    }
                    else
                    {
                        TempData["StatusMessage"] = "❌ Không thể xóa do ràng buộc dữ liệu.";
                    }
                }
                else
                {
                    Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                    TempData["StatusMessage"] = $"❌ Lỗi database: {dbEx.InnerException?.Message}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa: {ex.Message}");
                TempData["StatusMessage"] = $"❌ Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { productId = productId });
        }

        // --- 5. HÀM HỖ TRỢ ---
        private async Task<string?> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return "/images/products/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi upload file: {ex.Message}");
                return null;
            }
        }
    }
}