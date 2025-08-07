using Microsoft.AspNetCore.Mvc;
using OkkeianBlog.Services; // RssService の名前空間
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class RssFeedViewComponent : ViewComponent
{
    private readonly RssService _rssService;

    public RssFeedViewComponent(RssService rssService)
    {
        _rssService = rssService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var rssUrls = new List<string>
        {
            "http://blog.esuteru.com/index.rdf", // 複数のRSSフィードURL
        };

        var allRssItems = await _rssService.GetRssFeedsAsync(rssUrls); // 複数のRSSフィードを取得

        // 修正: rssItems ではなく allRssItems を使用
        Console.WriteLine($"RSS Items Count: {allRssItems.Count}");

        return View(allRssItems); // RSSアイテムをビューに渡す
    }
}
