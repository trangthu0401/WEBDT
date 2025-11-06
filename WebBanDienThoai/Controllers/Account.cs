using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanDienThoai.Data; // Đảm bảo có


namespace WebBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        private readonly DemoWebBanDienThoaiDbContext _context;

        public AccountController(DemoWebBanDienThoaiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int accountId, int customerId)
    {
        var account = await _context.Accounts.FindAsync(accountId);

        if (account == null)
        {
            return NotFound();
        }

        // Đảo ngược trạng thái: 
        // true -> false, false -> true
        account.IsActive = !account.IsActive;

        try
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            if(account.IsActive == false)
                TempData["SuccessMessage"] = "Đã khóa tài khoản";
            else TempData["SuccessMessage"] = "Đã mở tài khoản";
            }
        catch (DbUpdateException /* e */)
        {
            // Xử lý lỗi nếu cần
            TempData["ErrorMessage"] = "Không thể cập nhật trạng thái tài khoản.";
        }

        // Quay lại đúng trang chi tiết khách hàng mà bạn vừa xem

        return RedirectToAction("Details", "Customer", new { id = customerId });
    }
}
}
