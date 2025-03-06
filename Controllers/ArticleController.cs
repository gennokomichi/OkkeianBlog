using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using System.Threading.Tasks;

namespace OkkeianBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 記事一覧ページ（GET）: 一般ユーザーが記事を閲覧する
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles
            .OrderByDescending(a => a.PublishDate) // 最新順に並べ替え
                .Select(a => new ArticleSummaryViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Thumbnail = a.Thumbnail, // ここを Thumbnail に変更
                     PublishDate = a.PublishDate  // PublishDate が設定されているか確認
                })
                .ToListAsync();

            return View(articles); // ビューに渡す
        }
    }
}
