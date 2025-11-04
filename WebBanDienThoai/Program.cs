using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using WebBanDienThoai.Models;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json
var configuration = builder.Configuration;

// Đăng ký ApplicationDbContext cho Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Identity (nếu dùng đăng nhập, phân quyền)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Đăng ký DbContext chính cho dữ liệu sản phẩm, đơn hàng...
builder.Services.AddDbContext<DemoWebBanDienThoaiContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Đăng ký MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Nếu dùng Identity
app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
