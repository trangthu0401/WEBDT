using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    public class HomeController(ILogger<HomeController> logger, DemoWebBanDienThoaiDbContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly DemoWebBanDienThoaiDbContext _context = context;

        // HÀM LẤY DANH SÁCH THƯƠNG HIỆU
        private async Task<List<BrandCount>> GetBrandsAsync()
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .Include(b => b.Products)
                .ToListAsync();

            return brands.Select(b => new BrandCount
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                Count = b.Products?.Count(p => p.IsActive) ?? 0
            })
            .Where(b => b.Count > 0)
            .ToList();
        }

        // === INDEX (ĐÃ SỬA LẠI PHẦN LẤY SỐ LƯỢNG BÁN) ===
        public async Task<IActionResult> Index(int? id, string sortOrder, string searchString,
                                          decimal? minPrice, decimal? maxPrice, string ram, string storage, List<int> brandIds)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            // 1. Phải lấy lại danh sách Brand để popup lọc có dữ liệu
            var dsHang = await GetBrandsAsync();

            try
            {
                // 2. Query cơ bản
                var productsQuery = _context.Products
                    .AsNoTracking()
                    .Include(p => p.Brand)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive == true)
                    .AsQueryable();

                // 3. Lọc theo search
                if (!string.IsNullOrEmpty(searchString))
                    productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));

                // 4. Lọc theo THƯƠNG HIỆU (Dùng brandIds để hỗ trợ chọn nhiều hãng)
                if (brandIds != null && brandIds.Any())
                {
                    productsQuery = productsQuery.Where(p => brandIds.Contains(p.BrandId));
                }
                else if (id.HasValue && id > 0)
                {
                    productsQuery = productsQuery.Where(p => p.BrandId == id.Value);
                    brandIds = new List<int> { id.Value };
                }

                var rawProducts = await productsQuery.ToListAsync();

                // 5. Lấy số lượng bán thực tế từ OrderDetails
                var productSales = await _context.OrderDetails
                    .AsNoTracking()
                    .Include(od => od.ProductVariant)
                    .GroupBy(od => od.ProductVariant.ProductId)
                    .Select(g => new { ProductId = g.Key, SoldCount = g.Sum(od => od.Quantity ?? 0) })
                    .ToDictionaryAsync(x => x.ProductId, x => x.SoldCount);

                // 6. Mapping ViewModel & Lọc cấu hình (SỬA LỖI ĐỎ TẠI ĐÂY)
                var processedList = rawProducts.Select(p =>
                {
                    var activeVariants = p.ProductVariants?.Where(v => v.IsActive == true).ToList() ?? new List<ProductVariant>();

                    // Lọc theo RAM/Storage nếu người dùng chọn trong popup
                    if (!string.IsNullOrEmpty(ram)) activeVariants = activeVariants.Where(v => v.RAM == ram).ToList();
                    if (!string.IsNullOrEmpty(storage)) activeVariants = activeVariants.Where(v => v.Storage == storage).ToList();

                    var bestVariant = activeVariants.OrderBy(v => v.DiscountPrice ?? v.Price).FirstOrDefault();
                    if (bestVariant == null) return null;

                    var soldCount = productSales.ContainsKey(p.ProductId) ? productSales[p.ProductId] : 0;

                    return new ProductListViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        MainImage = p.MainImage,
                        BrandName = p.Brand?.BrandName ?? "Unknown",
                        // KHÔNG dùng ?? cho các trường không null để hết lỗi đỏ
                        IsActive = p.IsActive  ,
                        CreatedDate = p.CreatedDate ,
                        Price = bestVariant.DiscountPrice ?? bestVariant.Price ,
                        Stock = activeVariants.Sum(v => v.Stock),
                        FirstVariantId = bestVariant.VariantId,
                        SoldCount = soldCount
                    };
                }).Where(x => x != null).Cast<ProductListViewModel>().AsQueryable();

                // 7. Lọc khoảng giá
                if (minPrice.HasValue) processedList = processedList.Where(p => p.Price >= minPrice.Value);
                if (maxPrice.HasValue) processedList = processedList.Where(p => p.Price <= maxPrice.Value);

                // 8. Sắp xếp
                var finalProducts = sortOrder switch
                {
                    "price_desc" => processedList.OrderByDescending(p => p.Price).ToList(),
                    "price_asc" => processedList.OrderBy(p => p.Price).ToList(),
                    _ => processedList.OrderByDescending(p => p.CreatedDate).ToList()
                };

                // 9. LẤY TOP BÁN CHẠY (Chỉ lấy sản phẩm có SoldCount > 0 để không bị hiện số 0)
                var topSelling = finalProducts
                    .Where(p => p.SoldCount > 0)
                    .OrderByDescending(p => p.SoldCount)
                    .Take(5)
                    .ToList();

                // 10. Lấy Options cho Dropdown RAM/ROM từ DB
                var allV = await _context.ProductVariants.Where(v => v.IsActive == true).ToListAsync();

                var viewModel = new ManageProductsViewModel
                {
                    Products = finalProducts,
                    TopSellingProducts = topSelling, // Danh sách sạch, không có số 0
                    BrandCounts = dsHang,
                    SelectedBrandIds = brandIds ?? new List<int>(),
                    RamOptions = allV.Select(v => v.RAM).Distinct().Where(s => !string.IsNullOrEmpty(s)).OrderBy(s => s).ToList(),
                    StorageOptions = allV.Select(v => v.Storage).Distinct().Where(s => !string.IsNullOrEmpty(s)).OrderBy(s => s).ToList(),
                    TotalProductCount = finalProducts.Count,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SelectedRam = ram,
                    SelectedStorage = storage,
                    BrandId = id
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Index Home.");
                return View("Error");
            }
        }
        // === PRODUCT DETAIL ===
        public async Task<IActionResult> ProductDetail(int id)
        {
            ViewBag.BrandCounts = await GetBrandsAsync();

            try
            {
                var productDetail = await _context.ProductVariants
                    .AsNoTracking()
                    .Include(v => v.Product)
                    .ThenInclude(p => p.Brand)
                    .FirstOrDefaultAsync(v => v.VariantId == id);

                if (productDetail == null) return NotFound();

                // Lấy danh sách các biến thể khác cùng ProductId
                var allVariants = await _context.ProductVariants
                    .AsNoTracking()
                    .Where(v => v.ProductId == productDetail.ProductId && v.IsActive)
                    .ToListAsync();

                // Lấy sản phẩm liên quan
                var brandId = productDetail.Product?.BrandId ?? 0;

                var relatedProducts = new List<ProductVariant>();
                if (brandId > 0)
                {
                    relatedProducts = await _context.ProductVariants
                        .AsNoTracking()
                        .Include(v => v.Product)
                            .ThenInclude(p => p.Brand)
                        .Where(v => v.Product != null &&
                                   v.Product.BrandId == brandId &&
                                   v.ProductId != productDetail.ProductId &&
                                   v.IsActive)
                        .Take(4)
                        .ToListAsync();
                }

                var dsHang = await GetBrandsAsync();

                var viewModel = new ProductDetailViewModel
                {
                    ProductDetail = productDetail,
                    AllVariants = allVariants,
                    RelatedProducts = relatedProducts,
                    BrandCounts = dsHang
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ProductDetail.");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Favorites(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewBag.BrandCounts = await GetBrandsAsync();

            try
            {
                int customerId = User.Identity?.IsAuthenticated == true &&
                               int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idValue) ? idValue : 0;
                if (customerId == 0) return RedirectToAction("Login", "Account");

                var favoritesQuery = _context.FavoriteDetails
                    .AsNoTracking()
                    .Include(fd => fd.Favorite)
                    .Include(fd => fd.ProductVariant)
                       .ThenInclude(pv => pv.Product)
                           .ThenInclude(p => p.Brand)
                    .Where(fd => fd.Favorite != null && fd.Favorite.CustomerID == customerId &&
                                fd.ProductVariant != null && fd.ProductVariant.IsActive)
                    .Select(fd => fd.ProductVariant);

                var rawFavs = await favoritesQuery.ToListAsync();

                var favList = rawFavs.Select(v => new ProductListViewModel
                {
                    ProductId = v.ProductId,
                    Name = (v.Product?.Name ?? "???") + $" ({v.Color}, {v.Storage})",
                    MainImage = v.ImageUrl ?? v.Product?.MainImage,
                    BrandName = v.Product?.Brand?.BrandName ?? "",
                    IsActive = v.IsActive,
                    CreatedDate = v.CreatedDate,
                    Price = v.DiscountPrice ?? v.Price,
                    Stock = v.Stock,
                    FirstVariantId = v.VariantId,
                    IsFavorited = true
                });

                // Sử dụng switch expression cho sort
                favList = sortOrder switch
                {
                    "price_desc" => favList.OrderByDescending(p => p.Price),
                    "price_asc" => favList.OrderBy(p => p.Price),
                    _ => favList.OrderByDescending(p => p.CreatedDate)
                };

                var viewModel = new FavoritesViewModel
                {
                    FavoriteProducts = favList.ToList(),
                    BrandCounts = await GetBrandsAsync()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Favorites.");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            try
            {
                int customerId = User.Identity?.IsAuthenticated == true &&
                               int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idValue) ? idValue : 0;
                if (customerId == 0) return Json(new { success = false, message = "Vui lòng đăng nhập." });

                var customerFavorite = await _context.Favorites.FirstOrDefaultAsync(f => f.CustomerID == customerId);
                if (customerFavorite == null)
                {
                    customerFavorite = new Favorite { CustomerID = customerId };
                    _context.Favorites.Add(customerFavorite);
                    await _context.SaveChangesAsync();
                }

                var variantToAdd = await _context.ProductVariants
                                    .Where(v => v.ProductId == id && v.IsActive)
                                    .Select(v => v.VariantId)
                                    .FirstOrDefaultAsync();

                if (variantToAdd == 0) return Json(new { success = false, message = "Sản phẩm tạm hết hàng." });

                bool exists = await _context.FavoriteDetails.AnyAsync(fd =>
                    fd.FavoriteId == customerFavorite.FavoriteId && fd.VariantId == variantToAdd);

                if (!exists)
                {
                    _context.FavoriteDetails.Add(new FavoriteDetail
                    {
                        FavoriteId = customerFavorite.FavoriteId,
                        VariantId = variantToAdd
                    });
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Lỗi server" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            try
            {
                int customerId = User.Identity?.IsAuthenticated == true &&
                               int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idValue) ? idValue : 0;
                var fav = await _context.Favorites.Include(f => f.FavoriteDetails).FirstOrDefaultAsync(f => f.CustomerID == customerId);
                if (fav == null) return Json(new { success = true });

                var variantIds = await _context.ProductVariants.Where(v => v.ProductId == id).Select(v => v.VariantId).ToListAsync();

                var toRemove = fav.FavoriteDetails.Where(fd => variantIds.Contains(fd.VariantId)).ToList();
                if (toRemove.Count > 0)
                {
                    _context.FavoriteDetails.RemoveRange(toRemove);
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }



        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
                return Json(new List<object>());

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive && p.Name.Contains(term))
                .OrderBy(p => p.Name)
                .Take(5) // Lấy tối đa 5 sản phẩm
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.MainImage,
                    Variants = p.ProductVariants
                                .Where(v => v.IsActive)
                                .Select(v => new { v.Price, v.DiscountPrice })
                                .ToList()
                })
                .ToListAsync();


            var data = products.Select(p => new
            {
                productId = p.ProductId,
                name = p.Name,
                mainImage = p.MainImage,

                price = p.Variants.Any()
                        ? p.Variants.Min(v => v.DiscountPrice ?? v.Price)
                        : 0
            });

            return Json(data);
        }

        [HttpPost]
        public IActionResult GetAiResponse()
        {
            return Ok(new { success = false, message = "AI chưa cấu hình key." });
        }

        public IActionResult Privacy() => View();
        public IActionResult Home() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}