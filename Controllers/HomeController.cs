using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Linq;

namespace OkkeianBlog.Controllers
{
   public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // すべてのアクションの前に実行される
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // 人気記事（コメント数が多い順）を最大15件取得
        var popularArticles = _context.Articles
            .OrderByDescending(a => a.Comments.Count)
            .Select(a => new ArticleSummaryViewModel
            {
                Id = a.Id,
                Title = a.Title,
                CommentCount = a.Comments.Count() // コメント数を設定
            })
            .Take(15)
            .ToList();

        ViewData["PopularArticles"] = popularArticles;
    }

    public IActionResult Index()
    {
        // 最新記事を取得（最新順）
        var articles = _context.Articles
            .OrderByDescending(a => a.PublishDate)
            .Select(a => new ArticleSummaryViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content, // 中身の一部をセット
                PublishDate = a.PublishDate,
                Thumbnail = a.Thumbnail,  // サムネイル画像（仮にArticleモデルに追加された場合）
                ThumbnailPath = a.ThumbnailPath,
                CommentCount = a.Comments.Count()
            })
            .ToList();

        // Viewにデータを渡す
        return View(articles);
    }

    // 記事詳細表示
    public IActionResult ArticleDetails(int id)
    {
        var article = _context.Articles
            .Include(a => a.Comments)
            .FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    [HttpPost]
    public IActionResult PostComment(int articleId, string author, string content)
    {
        var comment = new Comment
        {
            ArticleId = articleId,
            Author = author,
            Content = content,
            PostDate = DateTime.Now
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        return RedirectToAction("ArticleDetails", new { id = articleId });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}