using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OkkeianBlog.Data;
using OkkeianBlog.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

/*─────────────────────────────
  1.  DB (MySQL) 設定
─────────────────────────────*/
/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23))));
*/
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
// MySQL のバージョンを明示的に指定する場合は、以下のように書き換え
/*─────────────────────────────
  2.  DI でサービス登録
─────────────────────────────*/
builder.Services.AddHttpClient();
builder.Services.AddScoped<RssService>();
builder.Services.AddSingleton<RssParser>();

/*─────────────────────────────
  3.  Cookie 認証を追加
     ── 自分だけ管理画面へ
─────────────────────────────*/
builder.Services
    .AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", opts =>
    {
        opts.LoginPath        = "/Account/Login";   // 未ログイン時の遷移先
        opts.AccessDeniedPath = "/Account/Denied";  // 権限不足時
        // 必要なら opts.ExpireTimeSpan で有効期限を調整
    });

builder.Services.AddAuthorization();

/*─────────────────────────────
  4.  MVC
─────────────────────────────*/
builder.Services.AddControllersWithViews();

var app = builder.Build();

/*─────────────────────────────
  5.  ミドルウェア
─────────────────────────────*/
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // ← 認証ミドルウェア
app.UseAuthorization();    // ← 認可ミドルウェア

/*─────────────────────────────
  6.  ルーティング
─────────────────────────────*/
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
