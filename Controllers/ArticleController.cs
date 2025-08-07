using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using OkkeianBlog.Helpers;
using OkkeianBlog.Services; // ✅ 追加
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using OkkeianBlog.Models.ViewModels;





namespace OkkeianBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RssService _rssService; // ✅ 追加

        public ArticleController(ApplicationDbContext context, RssService rssService)
{
    _context = context;
    _rssService = rssService;  // ここで _rssService に値を設定
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
    var viewModel = new ArticleIndexViewModel
    {
        Articles = articles,
        RssItems = modelRssItems
    };

    return View(viewModel); // ビューにデータを渡す
}


     
        // アーカイブページ
      public IActionResult Archive(int year, int month)
{
    var articles = _context.Articles
        .Where(a => a.PublishDate.Year == year && a.PublishDate.Month == month)
        .OrderByDescending(a => a.PublishDate)
        .Select(a => new ArticleSummaryViewModel
        {
            Id = a.Id,
            Title = a.Title,
            ThumbnailPath = a.ThumbnailPath, // サムネイル画像のパス
            CommentCount = a.Comments.Count(), // コメント数
            PublishDate = a.PublishDate
        })
        .ToList();

        // RSSアイテムを取得
    var rssItems = _rssService.GetLatestItems(); // ← 実装してあるならOK

    ViewData["Year"] = year;
    ViewData["Month"] = month;
    ViewData["RssItems"] = rssItems;

    return View(articles);  // ArticleSummaryViewModel のリストを渡す
}


    }
}