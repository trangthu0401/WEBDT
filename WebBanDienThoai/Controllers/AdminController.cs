// Thêm các using cần thiết cho Controller, DbContext, Models, ViewModels
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanDienThoai.Models;        // Namespace Models của bạn
using WebBanDienThoai.Models.ViewModels; // Namespace ViewModels của bạn
using Microsoft.AspNetCore.Hosting; // Cần thiết cho việc lấy đường dẫn wwwroot
using System.IO;                    // Cần thiết cho việc xử lý file (Path, FileStream)
using Microsoft.AspNetCore.Mvc.Rendering; // Cần thiết cho SelectListItem (dropdown)
using Microsoft.AspNetCore.Http; // Cần thiết cho IFormFile

namespace WebBanDienThoai.Controllers
{
    public class AdminController : Controller
    {
        private readonly DemoWebBanDienThoaiContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // --- 1. CONSTRUCTOR (Hàm khởi tạo) ---
        // Nhận DbContext và Môi trường Web để làm việc với CSDL và file
        public AdminController(DemoWebBanDienThoaiContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- 2. TRANG TỔNG QUAN (DASHBOARD) ---
        // GET: /Admin/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardViewModel = new DashboardViewModel
                {
                    TotalRevenue = await GetTotalRevenue(),
                    ProductCount = await GetProductCount(),
                    UserCount = await GetUserCount(),
                    OrderCount = await GetOrderCount(),
                    TopSellingProducts = await GetTopSellingProducts(5)
                };
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải dữ liệu Dashboard: {ex.Message}");
                return View(new DashboardViewModel { TopSellingProducts = new List<BestSellingProductViewModel>() });
            }
        }

        // --- 3. QUẢN LÝ SẢN PHẨM (Trang chính) ---

        // GET: /Admin/ManageProducts
        // Lấy danh sách sản phẩm, tính toán giá/tồn kho để hiển thị
        public async Task<IActionResult> ManageProducts(int? brandId)
        {
            try
            {
                var productsQuery = _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.ProductVariants)
                                            .AsQueryable();

                if (brandId.HasValue && brandId.Value > 0)
                {
                    productsQuery = productsQuery.Where(p => p.BrandId == brandId.Value);
                }

                var productList = await productsQuery
                    .OrderBy(p => p.Name)
                    .Select(p => new ProductListViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name ?? "N/A",
                        MainImage = p.MainImage,
                        BrandId = p.BrandId,
                        BrandName = p.Brand != null ? (p.Brand.BrandName ?? "N/A") : "N/A",
                        CreatedDate = p.CreatedDate ?? DateTime.MinValue,
                        IsActive = p.IsActive ?? false,

                        // Lấy giá/tồn kho của BIẾN THỂ ĐẦU TIÊN (Dùng cho nút Sửa nhanh)
                        FirstVariantPrice = (p.ProductVariants != null && p.ProductVariants.Any())
                                            ? p.ProductVariants.OrderBy(v => v.VariantId).First().DiscountPrice.GetValueOrDefault(p.ProductVariants.OrderBy(v => v.VariantId).First().Price.GetValueOrDefault(0m))
                                            : 0m,
                        FirstVariantStock = (p.ProductVariants != null && p.ProductVariants.Any())
                                            ? p.ProductVariants.OrderBy(v => v.VariantId).First().Stock.GetValueOrDefault(0)
                                            : 0,

                        // Lấy giá THẤP NHẤT (Dùng để hiển thị)
                        LowestPrice = (p.ProductVariants != null && p.ProductVariants.Any())
                            ? p.ProductVariants.Min(v => v.DiscountPrice.GetValueOrDefault(v.Price.GetValueOrDefault(0m)))
                            : 0m,

                        // Lấy TỔNG tồn kho (Dùng để hiển thị)
                        TotalStock = (p.ProductVariants != null && p.ProductVariants.Any())
                            ? p.ProductVariants.Sum(v => v.Stock.GetValueOrDefault(0))
                            : 0
                    })
                    .ToListAsync();

                // Lấy danh sách hãng để làm tab lọc
                var brandCounts = await _context.Brands
                    .Select(b => new BrandCount
                    {
                        brandId = b.BrandId,
                        BrandName = b.BrandName ?? "N/A",
                        IsActive = brandId.HasValue && b.BrandId == brandId.Value,
                        Count = _context.Products.Count(p => p.BrandId == b.BrandId)
                    })
                    .OrderBy(b => b.BrandName)
                    .ToListAsync();

                var totalProductCount = await _context.Products.CountAsync();

                // Đóng gói vào ViewModel
                var viewModel = new ManageProductsViewModel
                {
                    Products = productList,
                    BrandCounts = brandCounts,
                    TotalProductCount = totalProductCount
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải danh sách sản phẩm: {ex.Message}");
                return View("Error");
            }
        }

        // POST: /Admin/QuickEditProduct
        // Xử lý pop-up Sửa Nhanh từ trang ManageProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickEditProduct(
            int productId,
            string name,
            int brandId,
            bool isActive,      // <-- Đã kiểm tra
            decimal lowestPrice,  // <-- Sửa tên tham số (từ newPrice)
            int totalStock         // <-- Sửa tên tham số (từ newStock)
        )
        {
            try
            {
                var productToUpdate = await _context.Products
                                                .Include(p => p.ProductVariants) // Phải Include
                                                .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (productToUpdate == null)
                {
                    TempData["StatusMessage"] = "Lỗi: Không tìm thấy sản phẩm.";
                    return RedirectToAction(nameof(ManageProducts));
                }

                // Cập nhật thông tin chung
                productToUpdate.Name = name;
                productToUpdate.BrandId = brandId;
                productToUpdate.IsActive = isActive; // Lưu trạng thái

                // Lấy biến thể đầu tiên để cập nhật
                var firstVariant = productToUpdate.ProductVariants?.OrderBy(v => v.VariantId).FirstOrDefault();

                if (firstVariant != null)
                {
                    // Dùng tên tham số đã sửa
                    firstVariant.Price = lowestPrice;
                    firstVariant.DiscountPrice = lowestPrice;
                    firstVariant.Stock = totalStock;

                    _context.ProductVariants.Update(firstVariant);
                }

                _context.Products.Update(productToUpdate);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "Cập nhật sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sửa sản phẩm ID {productId}: {ex.Message}");
                TempData["StatusMessage"] = "Lỗi khi lưu sản phẩm: " + ex.Message;
            }

            return RedirectToAction(nameof(ManageProducts));
        }

        // POST: /Admin/DeleteProduct
        // Xử lý nút Xóa (Xóa toàn bộ sản phẩm và các biến thể con)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var productToDelete = await _context.Products
                                                .Include(p => p.ProductVariants)
                                                .FirstOrDefaultAsync(p => p.ProductId == id);

                if (productToDelete == null)
                {
                    return NotFound();
                }

                // Xóa các biến thể (dòng con) trước
                if (productToDelete.ProductVariants != null && productToDelete.ProductVariants.Any())
                {
                    _context.ProductVariants.RemoveRange(productToDelete.ProductVariants);
                }

                // Xóa sản phẩm (dòng cha)
                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "Đã xóa sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sản phẩm ID {id}: {ex.Message}");
                TempData["StatusMessage"] = "Lỗi khi xóa sản phẩm.";
            }

            return RedirectToAction(nameof(ManageProducts));
        }


        // --- 4. THÊM SẢN PHẨM MỚI ---

        // GET: /Admin/CreateProduct
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> CreateProduct()
        {
            var viewModel = new CreateProductViewModel
            {
                BrandList = await _context.Brands
                                         .OrderBy(b => b.BrandName)
                                         .Select(b => new SelectListItem
                                         {
                                             Value = b.BrandId.ToString(),
                                             Text = b.BrandName
                                         })
                                         .ToListAsync(),
                Product = new Product() // Khởi tạo Product rỗng
            };

            return View(viewModel);
        }

        // POST: /Admin/CreateProduct
        // Nhận dữ liệu từ form Thêm sản phẩm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý ảnh chính
                    string? mainImagePath = null;
                    if (viewModel.MainImageFile != null)
                    {
                        mainImagePath = await UploadFile(viewModel.MainImageFile);
                    }

                    // Xử lý ảnh biến thể
                    string? variantImagePath = null;
                    if (viewModel.VariantImageFile != null)
                    {
                        variantImagePath = await UploadFile(viewModel.VariantImageFile);
                    }

                    // 1. Lưu Product
                    Product newProduct = viewModel.Product!;
                    newProduct.CreatedDate = DateTime.Now;
                    newProduct.IsActive = true;
                    newProduct.MainImage = mainImagePath;

                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync(); // Lưu để lấy ProductId

                    // 2. Lưu Biến thể đầu tiên
                    ProductVariant newVariant = new ProductVariant
                    {
                        ProductId = newProduct.ProductId, // Gán ID vừa tạo
                        Color = viewModel.VariantColor ?? string.Empty,
                        Storage = viewModel.VariantStorage ?? string.Empty,
                        Ram = viewModel.VariantRam ?? string.Empty,
                        Price = viewModel.VariantPrice,
                        Stock = viewModel.VariantStock,
                        ImageUrl = variantImagePath ?? mainImagePath, // Ưu tiên ảnh variant
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.ProductVariants.Add(newVariant);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Thêm sản phẩm mới thành công.";
                    return RedirectToAction(nameof(ManageProducts));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi tạo sản phẩm: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra, không thể lưu sản phẩm.");
                }
            }

            // Nếu lỗi, tải lại BrandList cho dropdown
            viewModel.BrandList = await _context.Brands
                                        .OrderBy(b => b.BrandName)
                                        .Select(b => new SelectListItem
                                        {
                                            Value = b.BrandId.ToString(),
                                            Text = b.BrandName
                                        })
                                        .ToListAsync();

            return View(viewModel); // Trả về view với lỗi
        }


        // --- 5. CHI TIẾT SẢN PHẨM & QUẢN LÝ BIẾN THỂ ---

        // GET: /Admin/ProductDetails/5
        // Hiển thị trang chi tiết (nơi chứa bảng các biến thể)
        public async Task<IActionResult> ProductDetails(int id)
        {
            try
            {
                var product = await _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.ProductVariants) // Tải kèm các biến thể
                                            .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    return NotFound();
                }

                var viewModel = new ProductDetailViewModel
                {
                    Product = product,
                    Variants = product.ProductVariants.ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy chi tiết sản phẩm: {ex.Message}");
                return View("Error");
            }
        }

        // POST: /Admin/AddVariant
        // Xử lý pop-up Thêm Biến Thể Mới (từ trang ProductDetails)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariant(AddVariantViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string? variantImagePath = null;
                    if (viewModel.ImageFile != null)
                    {
                        variantImagePath = await UploadFile(viewModel.ImageFile);
                    }

                    string storageValue = viewModel.Storage ?? string.Empty;
                    string ramValue = viewModel.Ram ?? string.Empty;

                    var newVariant = new ProductVariant
                    {
                        ProductId = viewModel.ProductId,
                        Color = viewModel.Color ?? string.Empty,
                        Storage = storageValue.Replace("GB", "").Trim(),
                        Ram = ramValue.Replace("GB", "").Trim(),
                        Price = viewModel.Price,
                        DiscountPrice = viewModel.DiscountPrice,
                        Stock = viewModel.Stock,
                        ImageUrl = variantImagePath,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.ProductVariants.Add(newVariant);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Thêm biến thể mới thành công!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi thêm biến thể: {ex.Message}");
                    TempData["StatusMessage"] = $"Lỗi khi thêm biến thể: {ex.Message}";
                }
            }
            // Quay lại trang chi tiết sản phẩm dù thành công hay thất bại
            return RedirectToAction(nameof(ProductDetails), new { id = viewModel.ProductId });
        }

        // POST: /Admin/EditVariant
        // Xử lý pop-up Sửa Biến Thể (từ trang ProductDetails)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVariant(
            int ProductId,
            int VariantId,
            string Color,
            string Storage,
            string Ram,
            decimal Price,
            decimal? DiscountPrice,
            int Stock)
        {
            if (Stock < 0 || Price < 0)
            {
                TempData["StatusMessage"] = "Lỗi: Giá hoặc Tồn kho không thể là số âm.";
                return RedirectToAction(nameof(ProductDetails), new { id = ProductId });
            }

            try
            {
                var variantToUpdate = await _context.ProductVariants.FindAsync(VariantId);

                if (variantToUpdate == null)
                {
                    TempData["StatusMessage"] = "Lỗi: Không tìm thấy biến thể.";
                    return RedirectToAction(nameof(ProductDetails), new { id = ProductId });
                }

                // Cập nhật các trường
                variantToUpdate.Color = Color;
                variantToUpdate.Storage = Storage.EndsWith("GB") ? Storage : Storage + "GB";
                variantToUpdate.Ram = string.IsNullOrEmpty(Ram) ? null : (Ram.EndsWith("GB") ? Ram : Ram + "GB");
                variantToUpdate.Price = Price;
                variantToUpdate.DiscountPrice = DiscountPrice;
                variantToUpdate.Stock = Stock;

                _context.ProductVariants.Update(variantToUpdate);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "Cập nhật biến thể (ID: " + VariantId + ") thành công.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sửa biến thể ID {VariantId}: {ex.Message}");
                TempData["StatusMessage"] = "Lỗi khi cập nhật biến thể: " + ex.Message;
            }

            return RedirectToAction(nameof(ProductDetails), new { id = ProductId });
        }

        // POST: /Admin/DeleteVariant
        // Xử lý pop-up Xóa Biến Thể (từ trang ProductDetails)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariant(int id) // 'id' này là VariantId
        {
            int productId = 0; // Biến để giữ ProductId để chuyển hướng
            try
            {
                var variantToDelete = await _context.ProductVariants.FindAsync(id);

                if (variantToDelete == null)
                {
                    TempData["StatusMessage"] = "Lỗi: Không tìm thấy biến thể để xóa.";
                    return RedirectToAction(nameof(ManageProducts));
                }

                productId = variantToDelete.ProductId; // Lấy ProductId TRƯỚC KHI XÓA

                _context.ProductVariants.Remove(variantToDelete);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "Đã xóa biến thể thành công.";
                return RedirectToAction(nameof(ProductDetails), new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa biến thể ID {id}: {ex.Message}");
                TempData["StatusMessage"] = "Lỗi khi xóa biến thể: " + ex.Message;

                if (productId > 0)
                {
                    return RedirectToAction(nameof(ProductDetails), new { id = productId });
                }
                return RedirectToAction(nameof(ManageProducts));
            }
        }


        // --- 6. CÁC TRANG KHÁC (Chưa làm) ---

        // GET: /Admin/EditProduct/5
        // (Placeholder) Trang sửa chi tiết toàn bộ sản phẩm (khác với Sửa Nhanh)
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            // Cần tạo ViewModel và View riêng cho trang này
            return View(/* viewModel */);
        }

        public IActionResult ManageOrders() { return View(); }
        public IActionResult ManageUsers() { return View(); }
        public IActionResult Statistics() { return View(); }


        // --- 7. HÀM HỖ TRỢ (PRIVATE) ---

        // Hàm helper tải file lên (dùng cho CreateProduct và AddVariant)
        private async Task<string?> UploadFile(IFormFile file)
        {
            // Đường dẫn thư mục lưu trữ (ví dụ: wwwroot/images/products)
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Tạo tên file duy nhất
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadDir, uniqueFileName);

            // Lưu file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn TƯƠNG ĐỐI để lưu vào CSDL
            return "/images/products/" + uniqueFileName;
        }

        // Các hàm helper cho Dashboard
        private async Task<decimal> GetTotalRevenue()
        {
            return await _context.Orders.SumAsync(o => o.TotalAmount) ?? 0m;
        }

        private async Task<int> GetProductCount()
        {
            return await _context.Products.CountAsync();
        }

        private async Task<int> GetUserCount()
        {
            return await _context.Customers.CountAsync();
        }

        private async Task<int> GetOrderCount()
        {
            return await _context.Orders.CountAsync();
        }

        // Hàm helper lấy Top 5 SP bán chạy
        private async Task<List<BestSellingProductViewModel>> GetTopSellingProducts(int topN = 5)
        {
            var topVariantsInfo = await _context.OrderDetails
                .Where(od => od.VariantId.HasValue)
                .GroupBy(od => od.VariantId)
                .Select(g => new {
                    VariantId = g.Key!.Value,
                    TotalQuantity = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topN)
                .ToListAsync();

            var variantIds = topVariantsInfo.Select(v => v.VariantId).ToList();

            var variants = await _context.ProductVariants
                                    .Include(v => v.Product)
                                        .ThenInclude(p => p!.Brand)
                                    .Where(v => variantIds.Contains(v.VariantId))
                                    .ToListAsync();

            var result = new List<BestSellingProductViewModel>();
            foreach (var topInfo in topVariantsInfo)
            {
                var variant = variants.FirstOrDefault(v => v.VariantId == topInfo.VariantId);
                if (variant != null && variant.Product != null)
                {
                    result.Add(new BestSellingProductViewModel
                    {
                        ProductName = (variant.Product.Name ?? "N/A") +
                                        (!string.IsNullOrEmpty(variant.Color) ? $" ({variant.Color}" : "") +
                                        (!string.IsNullOrEmpty(variant.Storage) ? $" - {variant.Storage})" : (!string.IsNullOrEmpty(variant.Color) ? ")" : "")),
                        ImageUrl = variant.ImageUrl ?? variant.Product.MainImage,
                        QuantitySold = topInfo.TotalQuantity,
                        Price = variant.DiscountPrice ?? variant.Price ?? 0m,
                        Stock = variant.Stock ?? 0
                    });
                }
            }
            return result.OrderByDescending(r => r.QuantitySold).ToList();
        }

    } // <-- Đóng class AdminController
} // <-- Đóng namespace