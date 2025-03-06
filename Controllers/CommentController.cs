using Microsoft.AspNetCore.Mvc;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using System.Linq;

namespace OkkeianBlog.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 記事ごとのコメント取得
        [HttpGet("{articleId}")]
        public IActionResult GetComments(int articleId)
        {
            var comments = _context.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.PostDate) // 最新のコメントを上に
                .ToList();
            return Ok(comments);
        }

        // コメント投稿
        [HttpPost]
        public IActionResult PostComment([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
                return BadRequest("コメントを入力してください");

            // NGワードフィルター
            string[] ngWords = { "悪口", "禁止ワード" };
            foreach (var word in ngWords)
            {
                if (comment.Content.Contains(word))
                    return BadRequest("このコメントは投稿できません");
            }

            comment.PostDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Ok(comment);
        }
    }
}
