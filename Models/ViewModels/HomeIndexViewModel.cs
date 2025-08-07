using System.Collections.Generic;
using OkkeianBlog.Models; // RssItemModel の名前空間をインポート

namespace OkkeianBlog.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<ArticleSummaryViewModel> Articles { get; set; }
        public List<RssItemModel> RssItems { get; set; }  // 型を RssItemModel に修正

        public HomeIndexViewModel()
        {
            Articles = new List<ArticleSummaryViewModel>(); // 初期化
            RssItems = new List<RssItemModel>(); // 初期化
        }
    }
}