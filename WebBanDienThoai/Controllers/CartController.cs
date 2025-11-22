using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanDienThoai.Data;
using WebBanDienThoai.Models;
using WebBanDienThoai.Models.ViewModels;

namespace WebBanDienThoai.Controllers
{
    public class CartController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public CartController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // HELPER: LẤY CUSTOMER ID CHUẨN TỪ COOKIE
        // ============================================================
        private int GetCurrentCustomerId()
        {
            // Lấy giá trị từ Claim mà ta đã lưu ở AccountController/Login
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int idFromCookie))
                return 0;

            // Vì AccountController đã lưu CustomerID vào đây rồi, nên ta return luôn
            // KHÔNG query lại Database để tránh nhầm lẫn giữa AccountID và CustomerID
            return idFromCookie;
        }

        // ============================================================
        // 1. XEM GIỎ HÀNG
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerId = GetCurrentCustomerId();
            var viewModel = new CartViewModel();

            if (customerId > 0)
            {
                var cartItems = await _context.CartItems
                    .Include(c => c.ProductVariant).ThenInclude(v => v.Product)
                    .Where(c => c.CustomerID == customerId)
                    .ToListAsync();

                viewModel.Items = cartItems.Select(c => new CartItemViewModel
                {
                    CartItemId = c.CartItemId,
                    VariantId = c.VariantId,
                    ProductName = c.ProductVariant.Product.Name,
                    VariantName = $"{c.ProductVariant.Color} - {c.ProductVariant.Storage}",

                    // Ưu tiên ảnh biến thể
                    ImageUrl = !string.IsNullOrEmpty(c.ProductVariant.ImageUrl)
                               ? c.ProductVariant.ImageUrl
                               : c.ProductVariant.Product.MainImage,

                    // Logic giá bán (Ưu tiên giá giảm)
                    Price = (c.ProductVariant.DiscountPrice.HasValue && c.ProductVariant.DiscountPrice > 0)
                            ? c.ProductVariant.DiscountPrice.Value
                            : c.ProductVariant.Price,

                    // Logic giá gốc (để gạch ngang)
                    OldPrice = (c.ProductVariant.DiscountPrice.HasValue && c.ProductVariant.DiscountPrice > 0)
                               ? c.ProductVariant.Price
                               : null,

                    Quantity = c.Quantity,
                    MaxStock = c.ProductVariant.Stock,
                    IsSelected = true
                }).ToList();
            }

            return View(viewModel);
        }

        // ============================================================
        // 2. THÊM VÀO GIỎ (AJAX)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int quantity)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == 0)
                return Json(new { success = false, message = "Vui lòng đăng nhập để mua hàng." });

            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            if (variant.Stock < quantity)
                return Json(new { success = false, message = $"Kho chỉ còn {variant.Stock} sản phẩm." });

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CustomerID == customerId && c.VariantId == variantId);

            if (cartItem != null)
            {
                if (cartItem.Quantity + quantity > variant.Stock)
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho." });
                }
                cartItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    CustomerID = customerId,
                    VariantId = variantId,
                    Quantity = quantity
                };
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!" });
        }

        // ============================================================
        // 3. CẬP NHẬT SỐ LƯỢNG (AJAX)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null) return Json(new { success = false });

            var variant = await _context.ProductVariants.FindAsync(item.VariantId);

            if (quantity > variant.Stock)
                return Json(new { success = false, message = $"Kho chỉ còn {variant.Stock} sp." });

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ============================================================
        // 4. XÓA SẢN PHẨM (AJAX)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        // ============================================================
        // 5. LẤY TỔNG SỐ LƯỢNG (Badge Header)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == 0) return Json(new { count = 0 });
            var count = await _context.CartItems.Where(c => c.CustomerID == customerId).SumAsync(c => c.Quantity);
            return Json(new { count = count });
        }
    }
}