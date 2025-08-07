using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using OkkeianBlog.Models; // RssItem を使用
using System.Xml.Linq;

using System;

namespace OkkeianBlog.Services
{
    public class RssService
    {
        private readonly HttpClient _httpClient;
        private readonly RssParser _rssParser;

        public RssService(HttpClient httpClient, RssParser rssParser)
        {
            _httpClient = httpClient;
            _rssParser = rssParser;
        }

        public async Task<List<RssItemModel>> GetRssFeedsAsync(IEnumerable<string> rssUrls)
        {
            var allRssItems = new List<RssItemModel>();

            foreach (var rssUrl in rssUrls)
            {
                try
                {
                    // RSSデータを取得
                    var response = await _httpClient.GetAsync(rssUrl);
                    response.EnsureSuccessStatusCode();
                    
                    var rssContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Fetched RSS content from {rssUrl}:\n{rssContent.Substring(0, 500)}..."); // 先頭500文字だけ出力

                    // RSSフィードを解析
                    var rssItems = await _rssParser.ParseRssAsync(rssUrl);
                    allRssItems.AddRange(rssItems);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching RSS feed from {rssUrl}: {ex}");
                }
            }

            return allRssItems;
        }
        public List<RssItemModel> GetLatestItems()
        {
            var feedUrl = "http://blog.esuteru.com/index.rdf"; // ← あなたの対象RSS URLに置き換えてね
            var rssItems = new List<RssItemModel>();

            try
            {
                var doc = XDocument.Load(feedUrl);

                // RSS 1.0 の <item> を対象に抽出
                var items = doc.Descendants("{http://purl.org/rss/1.0/}item");

                foreach (var item in items)
                {
                    string title = item.Element("{http://purl.org/rss/1.0/}title")?.Value ?? "タイトルなし";
                    string link = item.Element("{http://purl.org/rss/1.0/}link")?.Value ?? "#";

                    rssItems.Add(new RssItemModel
                    {
                        Title = title,
                        Link = link
                    });
                }
            }
            catch
            {
                // ログを入れたりエラーハンドリングもここでやる
                rssItems.Add(new RssItemModel
                {
                    Title = "RSSの読み込みに失敗しました",
                    Link = "#"
                });
            }

            return rssItems;
        }
    }



}
