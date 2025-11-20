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

        // === INDEX (ĐÃ SỬA LỖI) ===
        public async Task<IActionResult> Index(int? id, string sortOrder, string searchString,
                                                 decimal? minPrice, decimal? maxPrice, string ram, string storage)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            try
            {
                // 1. Query cơ bản lấy Product
                var productsQuery = _context.Products
                    .AsNoTracking()
                    .Include(p => p.Brand)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive);

                // 2. Áp dụng Filter cơ bản trên SQL
                if (id.HasValue && id > 0)
                    productsQuery = productsQuery.Where(p => p.BrandId == id);

                if (!string.IsNullOrEmpty(searchString))
                    productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));

                // 3. Thực thi Query
                var rawProducts = await productsQuery.ToListAsync();

                // 4. Xử lý Logic Map sang ViewModel
                var processedList = rawProducts.Select(p =>
                {
                    var activeVariants = p.ProductVariants?.Where(v => v.IsActive).ToList() ?? new List<ProductVariant>();

                    var minPriceVariant = activeVariants.OrderBy(v => v.DiscountPrice ?? v.Price).FirstOrDefault();
                    var minPriceValue = minPriceVariant != null ? (minPriceVariant.DiscountPrice ?? minPriceVariant.Price) : 0;

                    // Tính số lượng đã bán (trong thực tế sẽ lấy từ bảng OrderDetails)
                    // Tạm thời dùng random để demo
                    var soldCount = new System.Random().Next(50, 500);

                    return new ProductListViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        MainImage = p.MainImage,
                        BrandName = p.Brand?.BrandName ?? "Unknown",
                        IsActive = p.IsActive,
                        CreatedDate = p.CreatedDate,
                        Price = minPriceValue,
                        Stock = activeVariants.Sum(v => v.Stock),
                        FirstVariantId = minPriceVariant?.VariantId ?? 0,
                        SoldCount = soldCount
                    };
                });

                // 5. Áp dụng Bộ lọc nâng cao
                if (minPrice.HasValue)
                    processedList = processedList.Where(p => p.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    processedList = processedList.Where(p => p.Price <= maxPrice.Value);

                // 6. Sắp xếp sử dụng switch expression
                processedList = sortOrder switch
                {
                    "price_desc" => processedList.OrderByDescending(p => p.Price),
                    "price_asc" => processedList.OrderBy(p => p.Price),
                    _ => processedList.OrderByDescending(p => p.CreatedDate)
                };

                // 7. Xử lý Yêu thích (Favorites)
                int customerId = User.Identity?.IsAuthenticated == true &&
                               int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idValue) ? idValue : 0;

                var favIds = new HashSet<int>();
                if (customerId > 0)
                {
                    favIds = (await _context.FavoriteDetails
                        .Where(fd => fd.Favorite != null && fd.Favorite.CustomerID == customerId)
                        .Select(fd => fd.VariantId)
                        .ToListAsync()).ToHashSet();
                }

                // Map lại IsFavorited
                var finalProducts = processedList.Select(p => {
                    p.IsFavorited = favIds.Contains(p.FirstVariantId);
                    return p;
                }).ToList();

                // 8. Lấy Top bán chạy (5 sản phẩm bán chạy nhất)
                var topSellingProducts = finalProducts
                    .OrderByDescending(p => p.SoldCount)
                    .Take(5)
                    .ToList();

                var dsHang = await GetBrandsAsync();
                var totalCount = finalProducts.Count;

                var viewModel = new ManageProductsViewModel
                {
                    Products = finalProducts,
                    TopSellingProducts = topSellingProducts,
                    BrandCounts = dsHang,
                    TotalProductCount = totalCount,
                    BrandId = id,
                    CurrentSearch = searchString ?? string.Empty,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SelectedRam = ram,
                    SelectedStorage = storage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Index Home.");
                return RedirectToAction("Error");
            }
        }

        // === PRODUCT DETAIL ===
        public async Task<IActionResult> ProductDetail(int id)
        {
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
            if (string.IsNullOrEmpty(term) || term.Length < 2) return Json(new List<object>());

            var data = await _context.Products
                .AsNoTracking()
                .Where(p => p.Name.Contains(term) && p.IsActive)
                .Take(5)
                .Select(p => new {
                    productId = p.ProductId,
                    name = p.Name,
                    mainImage = p.MainImage,
                    price = p.ProductVariants.Count > 0 ? p.ProductVariants.Min(v => v.DiscountPrice ?? v.Price) : 0
                })
                .ToListAsync();

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