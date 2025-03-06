using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OkkeianBlog.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Author { get; set; }  // コメントの投稿者名

        [Required]
        public string? Content { get; set; } // コメント内容

        public DateTime PostDate { get; set; }  // 投稿日時

        // 記事への外部キー
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }
    }
}
