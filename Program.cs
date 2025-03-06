using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data; // ApplicationDbContextの名前空間
using OkkeianBlog.Models; // 追加
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // 追加

var builder = WebApplication.CreateBuilder(args);

// MySQL接続設定
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23)) // MySQLのバージョンを指定
    ));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// `UseEndpoints` を削除し、`MapControllerRoute` のみを使用
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
