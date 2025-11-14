// Thư mục: /Data/DbInitializer.cs
using Microsoft.EntityFrameworkCore;
using WebBanDienThoai.Models;
using BCrypt.Net;
using System.Linq;

namespace WebBanDienThoai.Data
{
    public static class DbInitializer
    {
        // Hàm này sẽ được gọi TỰ ĐỘNG khi project khởi động
        public static void Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<DemoWebBanDienThoaiDbContext>();

                if (context == null)
                {
                    // Nếu lỗi ở đây, kiểm tra lại Program.cs và DbContext
                    return;
                }

                int patchedCount = 0;

                // 1. TÌM VÀ SỬA ADMIN
                var adminAcc = context.Accounts.FirstOrDefault(a => a.Email == "admin@shop.com");
                if (adminAcc != null && adminAcc.Password == "admin123")
                {
                    // Dùng chính thư viện của project để mã hóa
                    adminAcc.Password = BCrypt.Net.BCrypt.HashPassword("admin123");
                    patchedCount++;
                }

                // 2. TÌM VÀ SỬA CUSTOMER 1
                var customerAcc1 = context.Accounts.FirstOrDefault(a => a.Email == "nguyenvana@gmail.com");
                if (customerAcc1 != null && customerAcc1.Password == "user123")
                {
                    customerAcc1.Password = BCrypt.Net.BCrypt.HashPassword("user123");
                    patchedCount++;
                }

                // 3. TÌM VÀ SỬA CUSTOMER 2
                var customerAcc2 = context.Accounts.FirstOrDefault(a => a.Email == "tranthib@gmail.com");
                if (customerAcc2 != null && customerAcc2.Password == "user456")
                {
                    customerAcc2.Password = BCrypt.Net.BCrypt.HashPassword("user456");
                    patchedCount++;
                }

                // 4. LƯU THAY ĐỔI NẾU CÓ
                if (patchedCount > 0)
                {
                    context.SaveChanges();
                }
            }
        }
    }
}