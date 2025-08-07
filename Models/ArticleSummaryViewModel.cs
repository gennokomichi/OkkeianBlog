namespace OkkeianBlog.Models
{
    public class ArticleSummaryViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }

         public string? Excerpt { get; set; }
        public DateTime PublishDate { get; set; }
        public string? Thumbnail { get; set; }  // サムネイル画像のURLなど
        public string? ThumbnailPath { get; set; }
        public int CommentCount { get; set; }
        public string? Content { get; set; }  // ここに本文の一部を追加
   
    }
}
