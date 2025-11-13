using Microsoft.EntityFrameworkCore;
using WebBanDienThoai.Models; // Namespace chứa DbContext của bạn
using Microsoft.AspNetCore.Authentication.Cookies; // <-- THÊM DÒNG NÀY

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<DemoWebBanDienThoaiContext>(options =>
    options.UseSqlServer(connectionString));

// === THÊM CẤU HÌNH DỊCH VỤ XÁC THỰC COOKIE ===
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Account/Logout"; // Đường dẫn đăng xuất
        options.AccessDeniedPath = "/Home/Error"; // Trang khi bị cấm truy cập
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Thời gian cookie tồn tại
        options.SlidingExpiration = true; // Gia hạn cookie nếu truy cập
    });

// Thêm dịch vụ để có thể truy cập HttpContext (ví dụ: trong Controller)
builder.Services.AddHttpContextAccessor();
// === KẾT THÚC PHẦN THÊM MỚI ===

// Thêm dịch vụ Controller và View (Dòng này đã có)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình đường ống (pipeline) cho HTTP request
// Bật trang báo lỗi chi tiết khi đang phát triển (Development)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Bật trang báo lỗi thân thiện khi đã phát hành (Production)
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Tự động chuyển http sang https
app.UseHttpsRedirection();

// Cho phép tải file tĩnh (CSS, JS, Ảnh)
app.UseStaticFiles();

// Kích hoạt hệ thống định tuyến (Routing)
app.UseRouting();

// === THÊM MIDDLEWARE XÁC THỰC ===
// (Phải nằm SAU UseRouting và TRƯỚC UseAuthorization)
app.UseAuthentication();

// Bật tính năng xác thực/phân quyền (Dòng này đã có)
app.UseAuthorization();
// === KẾT THÚC PHẦN THÊM MỚI ===


// Ánh xạ route mặc định của bạn (đặt sau UseRouting)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();