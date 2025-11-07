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

var app = builder.Build();

// 4. C?u hình HTTP request pipeline
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

// 5. C?u hình route (???ng d?n) m?c ??nh
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();