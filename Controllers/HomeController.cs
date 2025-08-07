using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using OkkeianBlog.Helpers;
using OkkeianBlog.Services; // ✅ 追加
using OkkeianBlog.Models.ViewModels;

using Microsoft.AspNetCore.Mvc.Filters;

using System.Linq;

namespace OkkeianBlog.Controllers
{
   public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    private readonly RssService _rssService; // ✅ 追加

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RssService rssService)
{
    _logger = logger;
    _context = context;
    _rssService = rssService;  // RssService を正しくセット
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
                ThumbnailPath = a.ThumbnailPath, // 追加
                CommentCount = a.Comments.Count() // コメント数を設定
            })
            .Take(15)
            .ToList();

        ViewData["PopularArticles"] = popularArticles;

        // アーカイブの取得（投稿日をもとに集計）
          var archives = _context.Articles
        .GroupBy(a => new { a.PublishDate.Year, a.PublishDate.Month })
        .Select(g => new ArchiveViewModel
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            ArticleCount = g.Count()
        })
        .OrderByDescending(a => a.Year).ThenByDescending(a => a.Month)
        .ToList();

    ViewData["Archives"] = archives;

    }

     public async Task<IActionResult> Index()
{
    // 記事情報の取得
    var articles = await _context.Articles
        .OrderByDescending(a => a.PublishDate) // 最新順に並べ替え
        .Select(a => new ArticleSummaryViewModel
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            Excerpt = !string.IsNullOrEmpty(a.Content) ? a.Content.ToExcerpt(100) : "内容なし", // null チェック
            ThumbnailPath = a.ThumbnailPath, // サムネイル画像のパス
            PublishDate = a.PublishDate  // 投稿日時
        })
        .ToListAsync();

    // ✅ RSS フィードの取得
    var rssUrls = new List<string>
    {
        "http://blog.esuteru.com/index.rdf", // 他のURLも追加可能
    };

    var serviceRssItems = await _rssService.GetRssFeedsAsync(rssUrls);  // Services.RssItem のリスト
    
    // Services.RssItem を Models.RssItem に変換
    var modelRssItems = serviceRssItems.Select(item => new RssItemModel
    {
        Title = item.Title,
        Link = item.Link
    }).ToList();

    // IndexViewModel にデータをまとめて渡す
    var viewModel = new HomeIndexViewModel
    {
        Articles = articles,
        RssItems = modelRssItems
    };

    return View(viewModel); // ビューにデータを渡す
}



public IActionResult Archive(int year, int month)
{
    var articles = _context.Articles
        .Where(a => a.PublishDate.Year == year && a.PublishDate.Month == month)
        .OrderByDescending(a => a.PublishDate)
        .Select(a => new ArticleSummaryViewModel
        {
            Id = a.Id,
            Title = a.Title,
            PublishDate = a.PublishDate
        })
        .ToList();

    ViewData["ArchiveTitle"] = $"{year}年 {month}月 の記事一覧";

    return View("Index", articles); // 既存のIndexビューを使い回す
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

         ViewData["RssItems"] = _rssService.GetLatestItems();  // ← ここ忘れずに！

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

    /*
    public IActionResult Privacy()
    {
        return View();
    }
    */
     public IActionResult About()
{
    ViewData["RssItems"] = _rssService.GetLatestItems();  // RSSアイテムを取得
    return View();
}


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}