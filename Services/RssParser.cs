using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OkkeianBlog.Models;

namespace OkkeianBlog.Services
{
    public class RssParser
    {
        private readonly HttpClient _httpClient;

        public RssParser(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // ✅ RSS取得時に User-Agent を設定
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        // 非同期でRSSを取得して解析
        public async Task<List<RssItemModel>> ParseRssAsync(string rssUrl)
        {
            var rssItems = new List<RssItemModel>();

            try
            {
                var response = await _httpClient.GetStringAsync(rssUrl);
                Console.WriteLine("Response XML: " + response);

                // XMLを文字列で取得後、ParseRssに渡してパース
                rssItems = ParseRss(response);

                Console.WriteLine($"Fetched {rssItems.Count} items from {rssUrl}:");
                foreach (var item in rssItems)
                {
                    Console.WriteLine($"Title: {item.Title}, Link: {item.Link},");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing RSS feed from {rssUrl}: {ex.Message}");
            }

            return rssItems;
        }

        // XML文字列を手動で解析するメソッド（XDocumentベース）
        public List<RssItemModel> ParseRss(string xmlContent)
{
    var items = new List<RssItemModel>();

    try
    {
        var xdoc = XDocument.Parse(xmlContent);

        // RSS 1.0のネームスペース
        XNamespace ns = "http://purl.org/rss/1.0/";
        XNamespace contentNs = "http://purl.org/rss/1.0/modules/content/";
        XNamespace dcNs = "http://purl.org/dc/elements/1.1/";

        // RSSアイテムを取得
        var rssItems = xdoc.Descendants(ns + "item")
            .Select(item => new RssItemModel
            {
                Title = item.Element(ns + "title")?.Value,  // titleの取得
                Link = item.Element(ns + "link")?.Value,    // linkの取得
              //  Description = item.Element(ns + "description")?.Value ?? "No description available",  // 空の場合の処理
              //  ContentEncoded = item.Element(contentNs + "encoded")?.Value  // content:encodedを取得
            }).ToList();

        items.AddRange(rssItems);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while parsing RSS XML: " + ex.Message);
    }

    return items;
}

    }
}
