using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebBanDienThoai.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký DbContext
builder.Services.AddDbContext<DemoWebBanDienThoaiDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Đăng ký các service khác
builder.Services.AddControllersWithViews();

// === THÊM DÒNG NÀY - QUAN TRỌNG ===
builder.Services.AddAuthorization();

// Đăng ký dịch vụ Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// === CẤU HÌNH XỬ LÝ LỖI ===
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// === GỌI DBINITIALIZER VỚI TRY-CATCH ===
try
{
    Console.WriteLine("🔄 Đang chạy DbInitializer...");
    DbInitializer.Initialize(app);
    Console.WriteLine("✅ DbInitializer hoàn thành");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ DbInitializer bị lỗi nhưng ứng dụng vẫn chạy: {ex.Message}");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();