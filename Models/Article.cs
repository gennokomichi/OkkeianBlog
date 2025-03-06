using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OkkeianBlog.Models
{
    public class Article
    {
        [Key]  // 主キー（自動生成される）
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }  // 記事タイトル

        [Required]
        public string? Content { get; set; }  // 記事本文

        [Required]
        [MaxLength(50)]
        public string? Author { get; set; }  // 投稿者

        // 実際に記事が公開された日時（自動設定）
        public DateTime? PublishedAt { get; set; } = DateTime.Now;

        // 公開予定日時（ユーザー設定）
        [Required]
        public DateTime PublishDate { get; set; }  // 公開日

        public string? Thumbnail { get; set; }  // サムネイル画像のURLを保持

        public string? ThumbnailPath { get; set; }

       
        public List<Comment> Comments { get; set; } = new List<Comment>();  // コメントのリスト
    }
}
