using Microsoft.EntityFrameworkCore;
using WebBanDienThoai.Models; // Namespace chứa DbContext của bạn

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<DemoWebBanDienThoaiContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm dịch vụ Controller và View
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- PHẦN CẤU HÌNH BỊ THIẾU NẰM Ở ĐÂY ---

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

// DÒNG QUAN TRỌNG: Cho phép tải file tĩnh (CSS, JS, Ảnh)
app.UseStaticFiles();

// DÒNG QUAN TRỌNG: Kích hoạt hệ thống định tuyến (Routing)
app.UseRouting();

// (Tùy chọn) Bật tính năng xác thực/phân quyền (nếu có đăng nhập)
app.UseAuthorization();

// --- KẾT THÚC PHẦN BỊ THIẾU ---


// Ánh xạ route mặc định của bạn (đặt sau UseRouting)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();