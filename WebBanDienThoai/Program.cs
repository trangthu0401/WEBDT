using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebBanDienThoai.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. L?y chu?i k?t n?i t? appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. ??ng ký DbContext (Service)
// ??m b?o b?n ?ã cài NuGet package: Microsoft.EntityFrameworkCore.SqlServer
builder.Services.AddDbContext<DemoWebBanDienThoaiDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. ??ng ký các service khác (ví d?: Controllers, Views)
builder.Services.AddControllersWithViews();

// ??ng k� d?ch v? Authentication (x�c th?c) b?ng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // ???ng d?n ??n trang ??ng nh?p
        options.AccessDeniedPath = "/Home/AccessDenied"; // (T�y ch?n) Trang t? ch?i
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });


// --- 3. X�y d?ng ?ng d?ng (app) ---
var app = builder.Build();

// 4. C?u hình HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho ph�p d�ng file CSS, JS, Images...

app.UseRouting(); // B?t t�nh n?ng ??nh tuy?n (Routing)

app.UseAuthentication(); // Nếu dùng Identity
app.UseAuthorization();

DbInitializer.Initialize(app);
// 5. C?u hình route (???ng d?n) m?c ??nh
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();