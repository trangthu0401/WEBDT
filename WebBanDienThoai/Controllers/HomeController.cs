using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.modelView;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanDienThoai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DemoWebBanDienThoaiContext _context;

        public HomeController(ILogger<HomeController> logger, DemoWebBanDienThoaiContext context)
        {
            _logger = logger;
            _context = context;
        }

        // === SỬA: Thêm 'string sortOrder' ===
        public async Task<IActionResult> Index(int? id, string sortOrder)
        {
            // Truyền sortOrder sang View để biết nút nào đang active
            ViewData["CurrentSort"] = sortOrder;

            try
            {
                int customerId = 1;
                var favoritedVariantIds = new HashSet<int?>();
                var customerFavorite = await _context.Favorites
                                      .Include(f => f.FavoriteDetails)
                                      .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite != null)
                {
                    favoritedVariantIds = customerFavorite.FavoriteDetails.Select(fd => fd.VariantId).ToHashSet();
                }

                var productsQuery = _context.Products
                    .AsNoTracking()
                    .Where(p => p.IsActive == true && p.ProductVariants.Any(v => v.IsActive == true));

                if (id != null && id > 0)
                {
                    productsQuery = productsQuery.Where(p => p.BrandId == id);
                }

                // === SỬA: Tách .Select() ra để sắp xếp (Sort) ===
                var viewModelQuery = productsQuery
                    .Include(p => p.Brand)
                    .Select(p => new ProductListViewModel
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        MainImage = p.MainImage,
                        BrandName = p.Brand.BrandName,
                        IsActive = p.IsActive ?? false,
                        CreatedDate = p.CreatedDate ?? DateTime.MinValue,
                        Price = p.ProductVariants
                                 .Where(v => v.IsActive == true)
                                 .Select(v => v.DiscountPrice ?? v.Price)
                                 .Min() ?? 0M,
                        Stock = p.ProductVariants
                                 .Where(v => v.IsActive == true)
                                 .Sum(v => v.Stock ?? 0),
                        IsFavorited = p.ProductVariants.Any(v => favoritedVariantIds.Contains(v.VariantId))
                    });

                // === THÊM LOGIC SẮP XẾP MỚI ===
                switch (sortOrder)
                {
                    case "price_desc": // Giá cao -> thấp
                        viewModelQuery = viewModelQuery.OrderByDescending(p => p.Price);
                        break;
                    case "price_asc": // Giá thấp -> cao
                        viewModelQuery = viewModelQuery.OrderBy(p => p.Price);
                        break;
                    default: // Mặc định: Mới nhất
                        viewModelQuery = viewModelQuery.OrderByDescending(p => p.CreatedDate);
                        break;
                }

                var dsSanPham = await viewModelQuery.ToListAsync(); // Chuyển sang ToListAsync

                var dsHang = await _context.Brands // Dùng async
                    .AsNoTracking()
                    .Where(b => b.Products.Any(p => p.IsActive == true))
                    .Select(b => new BrandCount
                    {
                        brandId = b.BrandId,
                        BrandName = b.BrandName,
                        Count = b.Products.Count(p => p.IsActive == true)
                    })
                    .ToListAsync();

                var totalProductCount = await _context.Products // Dùng async
                    .CountAsync(p => p.IsActive == true && p.ProductVariants.Any(v => v.IsActive == true));

                var viewModel = new ManageProductsViewModel
                {
                    Products = dsSanPham,
                    BrandCounts = dsHang,
                    TotalProductCount = totalProductCount,
                    BrandId = id
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu cho trang Home Index.");
                return RedirectToAction("Error");
            }
        }

        // === SỬA: Thêm 'string sortOrder' ===
        public async Task<IActionResult> Favorites(string sortOrder)
        {
            // Truyền sortOrder sang View
            ViewData["CurrentSort"] = sortOrder;

            try
            {
                int customerId = 1;

                // Tách truy vấn ra
                var favoritesQuery = _context.FavoriteDetails
                    .AsNoTracking()
                    .Where(fd => fd.Favorite.CustomerId == customerId && fd.Variant.IsActive == true)
                    .Include(fd => fd.Variant.Product.Brand)
                    .Select(fd => fd.Variant)
                    .Select(v => new ProductListViewModel
                    {
                        ProductId = v.ProductId,
                        Name = v.Product.Name + " (" + v.Color + ", " + v.Storage + ")",
                        MainImage = v.ImageUrl ?? v.Product.MainImage,
                        BrandName = v.Product.Brand.BrandName,
                        IsActive = v.IsActive ?? false,
                        CreatedDate = v.CreatedDate ?? DateTime.MinValue,
                        Price = (v.DiscountPrice ?? v.Price) ?? 0M,
                        Stock = v.Stock ?? 0
                    });

                // === THÊM LOGIC SẮP XẾP MỚI ===
                switch (sortOrder)
                {
                    case "price_desc": // Giá cao -> thấp
                        favoritesQuery = favoritesQuery.OrderByDescending(p => p.Price);
                        break;
                    case "price_asc": // Giá thấp -> cao
                        favoritesQuery = favoritesQuery.OrderBy(p => p.Price);
                        break;
                    default: // Mặc định: Mới nhất
                        favoritesQuery = favoritesQuery.OrderByDescending(p => p.CreatedDate);
                        break;
                }

                var favoriteProducts = await favoritesQuery.ToListAsync();

                var viewModel = new FavoritesViewModel
                {
                    FavoriteProducts = favoriteProducts
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu cho trang Favorites.");
                return RedirectToAction("Error");
            }
        }

        // === ACTION (POST) LƯU YÊU THÍCH (Không đổi) ===
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            try
            {
                int customerId = 1;

                var customerFavorite = await _context.Favorites
                                    .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite == null)
                {
                    customerFavorite = new Favorite { CustomerId = customerId };
                    _context.Favorites.Add(customerFavorite);
                    await _context.SaveChangesAsync();
                }

                var variantToAdd = await _context.ProductVariants
                    .Where(v => v.ProductId == id && v.IsActive == true)
                    .Select(v => v.VariantId)
                    .FirstOrDefaultAsync();

                if (variantToAdd == 0)
                {
                    return Json(new { success = false, message = "Sản phẩm không có biến thể hợp lệ." });
                }

                bool alreadyExists = await _context.FavoriteDetails
                    .AnyAsync(fd => fd.FavoriteId == customerFavorite.FavoriteId && fd.VariantId == variantToAdd);

                if (!alreadyExists)
                {
                    _context.FavoriteDetails.Add(new FavoriteDetail
                    {
                        FavoriteId = customerFavorite.FavoriteId,
                        VariantId = variantToAdd
                    });
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Đã thêm vào yêu thích!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào Favorites.");
                return Json(new { success = false, message = "Lỗi máy chủ." });
            }
        }

        // === ACTION (POST) XÓA KHỎI YÊU THÍCH (Không đổi) ===
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            try
            {
                int customerId = 1;

                var customerFavorite = await _context.Favorites
                                      .Include(f => f.FavoriteDetails)
                                      .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite == null)
                {
                    return Json(new { success = true });
                }

                var variantIdsToRemove = await _context.ProductVariants
                    .Where(v => v.ProductId == id)
                    .Select(v => v.VariantId)
                    .ToListAsync();

                if (!variantIdsToRemove.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
                }

                var detailToRemove = customerFavorite.FavoriteDetails
                    .Where(fd => fd.VariantId != null && variantIdsToRemove.Contains(fd.VariantId.Value))
                    .ToList();

                if (detailToRemove.Any())
                {
                    _context.FavoriteDetails.RemoveRange(detailToRemove);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Đã xóa khỏi yêu thích." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khỏi Favorites.");
                return Json(new { success = false, message = "Lỗi máy chủ." });
            }
        }


        // ... (Các Action khác) ...
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}