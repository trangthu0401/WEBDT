using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.modelView;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks; // Đảm bảo bạn có dòng này

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

        // === SỬA (NÂNG CẤP LÊN ASYNC) ===
        public async Task<IActionResult> Index(int? id) // id là BrandId
        {
            try
            {
                // === THÊM LOGIC LẤY DS YÊU THÍCH CỦA USER ===
                // TODO: Thay thế '1' bằng ID của khách hàng đã đăng nhập
                int customerId = 1;
                var favoritedVariantIds = new HashSet<int?>();
                // Dùng 'CustomerId' (chữ 'd' thường) như file Favorite.cs của bạn
                var customerFavorite = await _context.Favorites
                                      .Include(f => f.FavoriteDetails)
                                      .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite != null)
                {
                    favoritedVariantIds = customerFavorite.FavoriteDetails.Select(fd => fd.VariantId).ToHashSet();
                }
                // === KẾT THÚC LOGIC MỚI ===


                // 1. Bắt đầu truy vấn, KHÔNG GỌI .Include() vội
                var productsQuery = _context.Products
                    .AsNoTracking()
                    .Where(p => p.IsActive == true && p.ProductVariants.Any(v => v.IsActive == true));

                // 2. Thêm logic lọc (FILTER)
                if (id != null && id > 0)
                {
                    productsQuery = productsQuery.Where(p => p.BrandId == id);
                }

                // 3. Thực thi: GỌI .Include() SAU CÙNG, TRƯỚC .Select()
                var dsSanPham = productsQuery
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

                        // === THÊM DÒNG NÀY ===
                        // (Kiểm tra xem CÓ BẤT KỲ biến thể nào của SP này nằm trong ds yêu thích không)
                        IsFavorited = p.ProductVariants.Any(v => favoritedVariantIds.Contains(v.VariantId))
                    })
                    .ToList();

                var dsHang = _context.Brands
                    .AsNoTracking()
                    .Where(b => b.Products.Any(p => p.IsActive == true))
                    .Select(b => new BrandCount
                    {
                        brandId = b.BrandId,
                        BrandName = b.BrandName,
                        Count = b.Products.Count(p => p.IsActive == true)
                    })
                    .ToList();

                var totalProductCount = _context.Products
                    .Count(p => p.IsActive == true && p.ProductVariants.Any(v => v.IsActive == true));

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

        // === ACTION (GET) HIỂN THỊ TRANG YÊU THÍCH (ĐÃ SỬA LỖI) ===
        public async Task<IActionResult> Favorites()
        {
            try
            {
                int customerId = 1;

                var favoriteProducts = await _context.FavoriteDetails
                    .AsNoTracking()
                    // === SỬA LỖI Ở ĐÂY: Dùng 'CustomerId' (chữ 'd' thường) ===
                    .Where(fd => fd.Favorite.CustomerId == customerId && fd.Variant.IsActive == true)
                    .Include(fd => fd.Variant.Product.Brand)
                    .Select(fd => fd.Variant)
                    .Select(v => new ProductListViewModel
                    {
                        // === SỬA LỖI Ở ĐÂY: Thêm ProductId để JavaScript biết xóa SP nào ===
                        ProductId = v.ProductId, // Quan trọng cho nút Xóa
                        Name = v.Product.Name + " (" + v.Color + ", " + v.Storage + ")",
                        MainImage = v.ImageUrl ?? v.Product.MainImage,
                        BrandName = v.Product.Brand.BrandName,
                        IsActive = v.IsActive ?? false,
                        CreatedDate = v.CreatedDate ?? DateTime.MinValue,

                        // === SỬA LỖI Ở ĐÂY: Thêm ?? 0M ===
                        Price = (v.DiscountPrice ?? v.Price) ?? 0M,

                        Stock = v.Stock ?? 0
                    })
                    .ToListAsync();

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

        // === ACTION (POST) LƯU YÊU THÍCH (ĐÃ SỬA LỖI) ===
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            try
            {
                int customerId = 1;

                // === SỬA LỖI Ở ĐÂY: Dùng 'CustomerId' (chữ 'd' thường) ===
                var customerFavorite = await _context.Favorites
                                    .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite == null)
                {
                    // === SỬA LỖI Ở ĐÂY: Dùng 'CustomerId' (chữ 'd' thường) ===
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

        // === ACTION (POST) MỚI: ĐỂ XÓA KHỎI YÊU THÍCH ===
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int id)
        {
            try
            {
                int customerId = 1;

                var customerFavorite = await _context.Favorites
                                      .Include(f => f.FavoriteDetails) // Tải chi tiết
                                      .FirstOrDefaultAsync(f => f.CustomerId == customerId);

                if (customerFavorite == null)
                {
                    // Không có gì để xóa
                    return Json(new { success = true });
                }

                // Tìm (các) biến thể của sản phẩm này (ProductId)
                var variantIdsToRemove = await _context.ProductVariants
                    .Where(v => v.ProductId == id)
                    .Select(v => v.VariantId)
                    .ToListAsync();

                if (!variantIdsToRemove.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
                }

                // Tìm chi tiết yêu thích khớp với các biến thể
                var detailToRemove = customerFavorite.FavoriteDetails
                    .Where(fd => fd.VariantId != null && variantIdsToRemove.Contains(fd.VariantId.Value))
                    .ToList(); // Lấy tất cả chi tiết khớp

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