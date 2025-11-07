using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.modelView;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// === THÊM 4 USING NÀY CHO AI CHATBOT ===
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

        // === ACTION INDEX (Code của bạn, giữ nguyên) ===
        public async Task<IActionResult> Index(int? id, string sortOrder)
        {
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

                switch (sortOrder)
                {
                    case "price_desc":
                        viewModelQuery = viewModelQuery.OrderByDescending(p => p.Price);
                        break;
                    case "price_asc":
                        viewModelQuery = viewModelQuery.OrderBy(p => p.Price);
                        break;
                    default:
                        viewModelQuery = viewModelQuery.OrderByDescending(p => p.CreatedDate);
                        break;
                }

                var dsSanPham = await viewModelQuery.ToListAsync();

                var dsHang = await _context.Brands
                    .AsNoTracking()
                    .Where(b => b.Products.Any(p => p.IsActive == true))
                    .Select(b => new BrandCount
                    {
                        brandId = b.BrandId,
                        BrandName = b.BrandName,
                        Count = b.Products.Count(p => p.IsActive == true)
                    })
                    .ToListAsync();

                var totalProductCount = await _context.Products
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

        // === ACTION FAVORITES (Code của bạn, giữ nguyên) ===
        public async Task<IActionResult> Favorites(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;

            try
            {
                int customerId = 1;

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

                switch (sortOrder)
                {
                    case "price_desc":
                        favoritesQuery = favoritesQuery.OrderByDescending(p => p.Price);
                        break;
                    case "price_asc":
                        favoritesQuery = favoritesQuery.OrderBy(p => p.Price);
                        break;
                    default:
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

        // === ACTION ADDTOFAVORITES (Code của bạn, giữ nguyên) ===
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

        // === ACTION REMOVEFROMFAVORITES (Code của bạn, giữ nguyên) ===
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


        // === ACTION MỚI ĐỂ GỌI AI ===
        [HttpPost]
        public async Task<IActionResult> GetAiResponse([FromBody] ChatRequest request)
        {
            try
            {
                // === TÔI ĐÃ DÁN KEY CỦA BẠN VÀO ĐÂY ===
                var apiKey = "AIzaSyBnUmG9b_K-Q1fTQ0i6loBKulGRNuXU0fg";

                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Thông tin "dạy" cho AI
                    var systemInstruction = "Bạn là Rabit AI, trợ lý tư vấn điện thoại của Rabit Store. Hãy trả lời ngắn gọn, thân thiện.";

                    // Tạo nội dung gửi đi
                    var payload = new
                    {
                        contents = new[]
                        {
                            new {
                                role = "user",
                                parts = new[] { new { text = systemInstruction } }
                            },
                            new {
                                role = "model",
                                parts = new[] { new { text = "Chào bạn! Tôi có thể giúp gì?" } }
                            },
                            new {
                                role = "user",
                                parts = new[] { new { text = request.Message } }
                            }
                        }
                    };

                    var jsonPayload = JsonSerializer.Serialize(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Gửi request đến Google
                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        // Phân tích JSON trả về để lấy text
                        using (var jsonDoc = JsonDocument.Parse(responseBody))
                        {
                            var botText = jsonDoc.RootElement
                                            .GetProperty("candidates")[0]
                                            .GetProperty("content")
                                            .GetProperty("parts")[0]
                                            .GetProperty("text")
                                            .GetString();

                            return Ok(new { success = true, message = botText });
                        }
                    }
                    else
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Lỗi API Gemini: {errorBody}");
                        return Ok(new { success = false, message = "Xin lỗi, AI đang bận. Bạn thử lại sau nhé." });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi gọi GetAiResponse.");
                return Ok(new { success = false, message = "Lỗi kết nối đến máy chủ AI." });
            }
        }

        // (Class nhỏ để nhận request từ JavaScript)
        public class ChatRequest
        {
            public string Message { get; set; }
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