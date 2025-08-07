using System.Collections.Generic;
using OkkeianBlog.Services;

namespace OkkeianBlog.Models.ViewModels
{
    public class ArticleIndexViewModel
    {
        public List<ArticleSummaryViewModel> Articles { get; set; }
        public List<RssItemModel> RssItems { get; set; }

    public ArticleIndexViewModel()
    {
        Articles = new List<ArticleSummaryViewModel>(); // 初期化
        RssItems = new List<RssItemModel>(); // 初期化
    }
}

}
