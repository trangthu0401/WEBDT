// Thêm các using cần thiết
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
                    Variants = product.ProductVariants != null ? product.ProductVariants.ToList() : new List<ProductVariant>(),
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateForm")] ProductVariantCreateViewModel viewModel)
        {
            if (viewModel.ProductId <= 0)
            {
                TempData["StatusMessage"] = "Lỗi: Sản phẩm không hợp lệ.";
                return RedirectToAction("Index", "Product");
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
                var detail = errors.Count > 0 ? " (" + string.Join(" ", errors) + ")" : string.Empty;
                SetVariantFeedback(TempData, "error", "Vui lòng điền đủ các trường bắt buộc." + detail);
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            if (viewModel.Price <= 0)
            {
                SetVariantFeedback(TempData, "error", "Lỗi thêm biến thể: Giá phải lớn hơn 0.");
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            if (viewModel.Stock < 0)
            {
                SetVariantFeedback(TempData, "error", "Lỗi thêm biến thể: Tồn kho không được âm.");
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
                    RAM = string.IsNullOrEmpty(ramValue) ? "-" : ramValue.Replace("GB", "").Trim(),
                    Price = viewModel.Price,
                    DiscountPrice = viewModel.DiscountPrice,
                    Stock = viewModel.Stock,
                    ImageUrl = variantImagePath,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.ProductVariants.Add(newVariant);
                await _context.SaveChangesAsync();

                SetVariantFeedback(TempData, "success", "Thêm biến thể thành công!");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"Inner: {dbEx.InnerException?.Message}");
                SetVariantFeedback(TempData, "error", "Lỗi thêm biến thể: " + (dbEx.InnerException?.Message ?? dbEx.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm biến thể: {ex.Message}");
                SetVariantFeedback(TempData, "error", "Lỗi thêm biến thể: " + ex.Message);
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
                SetVariantFeedback(TempData, "error", "Giá hoặc tồn kho không hợp lệ.");
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
                var detail = errors.Count > 0 ? " (" + string.Join(" ", errors) + ")" : string.Empty;
                SetVariantFeedback(TempData, "error", "Vui lòng điền đủ các trường bắt buộc." + detail);
                return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
            }

            try
            {
                var variantToUpdate = await _context.ProductVariants.FindAsync(viewModel.VariantId);

                if (variantToUpdate == null)
                {
                    SetVariantFeedback(TempData, "error", "Không tìm thấy biến thể để cập nhật.");
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

                SetVariantFeedback(TempData, "success", "Cập nhật biến thể thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sửa biến thể: {ex.Message}");
                SetVariantFeedback(TempData, "error", "Lỗi cập nhật biến thể: " + ex.Message);
            }

            return RedirectToAction(nameof(Index), new { productId = viewModel.ProductId });
        }

        /// <summary>Bật/tắt hiển thị một biến thể (chỉ ảnh hưởng biến thể đó; ẩn cả sản phẩm thì xử lý ở Product/Edit).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVariantActive(int variantId, int productId, bool setActive)
        {
            if (productId <= 0 || variantId <= 0)
            {
                SetVariantFeedback(TempData, "error", "Thông tin không hợp lệ.");
                return RedirectToAction("Index", "Product");
            }

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == variantId && v.ProductId == productId);

            if (variant == null)
            {
                SetVariantFeedback(TempData, "error", "Không tìm thấy biến thể.");
                return RedirectToAction(nameof(Index), new { productId });
            }

            try
            {
                variant.IsActive = setActive;
                variant.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                SetVariantFeedback(TempData, "success", setActive
                    ? "Biến thể đã chuyển sang Đang bán."
                    : "Biến thể đã được ẩn.");
            }
            catch (Exception ex)
            {
                SetVariantFeedback(TempData, "error", "Không thể cập nhật trạng thái: " + ex.Message);
            }

            return RedirectToAction(nameof(Index), new { productId });
        }

        // --- 4. XÓA BIẾN THỂ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int variantId, int productId)
        {
            var variantToDelete = await _context.ProductVariants.FindAsync(variantId);

            if (variantToDelete == null)
            {
                SetVariantFeedback(TempData, "error", "Không tìm thấy biến thể để xóa.");
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

                SetVariantFeedback(TempData, "success", "Đã xóa biến thể thành công.");
            }
            catch (DbUpdateException dbEx)
            {
                var baseException = dbEx.GetBaseException() as SqlException;

                if (baseException != null && baseException.Number == 547)
                {
                    string errorMessage = baseException.Message;

                    if (errorMessage.Contains("OrderDetails"))
                    {
                        SetVariantFeedback(TempData, "error", "Không thể xóa: biến thể đã có trong đơn hàng.");
                    }
                    else if (errorMessage.Contains("CartItems"))
                    {
                        SetVariantFeedback(TempData, "error", "Không thể xóa: biến thể đang có trong giỏ hàng.");
                    }
                    else if (errorMessage.Contains("ReviewDetails"))
                    {
                        SetVariantFeedback(TempData, "error", "Không thể xóa: biến thể đã có đánh giá.");
                    }
                    else if (errorMessage.Contains("FavoriteDetails"))
                    {
                        SetVariantFeedback(TempData, "error", "Không thể xóa: biến thể đang trong danh sách yêu thích.");
                    }
                    else
                    {
                        SetVariantFeedback(TempData, "error", "Không thể xóa do ràng buộc dữ liệu.");
                    }
                }
                else
                {
                    Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                    SetVariantFeedback(TempData, "error", "Lỗi database: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa: {ex.Message}");
                SetVariantFeedback(TempData, "error", "Lỗi xóa biến thể: " + ex.Message);
            }

            return RedirectToAction(nameof(Index), new { productId = productId });
        }

        private static void SetVariantFeedback(ITempDataDictionary tempData, string kind, string message)
        {
            tempData["PvFeedbackKind"] = kind;
            tempData["PvFeedbackMessage"] = message;
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
