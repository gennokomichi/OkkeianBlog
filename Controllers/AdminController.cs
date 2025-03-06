using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OkkeianBlog.Data;
using OkkeianBlog.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OkkeianBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 記事一覧
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles.ToListAsync();
            return View(articles);
        }

        // 記事作成ページ（GET）
        public IActionResult Create()
        {
            return View();
        }

        // 記事作成（POST）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article model, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                // 🔹 サムネイル画像の処理
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    var fileName = Path.GetFileName(uploadedFile.FileName);
                    var uploadDir = Path.Combine("wwwroot", "uploads");

                    // 🔹 アップロードフォルダがなければ作成
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    var filePath = Path.Combine(uploadDir, fileName);

                    try
                    {
                        // 🔹 ファイルを保存
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(stream);
                        }

                        // 🔹 DBに保存するパスを設定（`wwwroot` を省く）
                        model.ThumbnailPath = "/uploads/" + fileName;
                        model.Thumbnail = "/uploads/" + fileName;  // Thumbnailにもパスを設定
                    }
                    catch (Exception ex)
                    {
                        // 🔹 エラー処理（ログ出力など）
                        Console.WriteLine("サムネイル画像のアップロードに失敗しました: " + ex.Message);
                        ModelState.AddModelError("", "サムネイル画像のアップロードに失敗しました。");
                        return View(model);
                    }
                }
                else
                {
                    // 🔹 デフォルト画像を設定
                    model.ThumbnailPath = "/uploads/default-thumbnail.png";
                    model.Thumbnail = "/uploads/default-thumbnail.png";  // デフォルト画像をセット
                }

                // 🔹 投稿日時を設定
                model.PublishDate = DateTime.Now;
                model.PublishedAt = DateTime.Now;

                // 🔹 記事をデータベースに保存
                _context.Articles.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 記事編集ページ（GET）
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // 記事編集（POST）
       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Article model, IFormFile Thumbnail)
{
    if (id != model.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            // 既存のデータを取得
            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            // 🔹 新しいサムネイルがアップロードされた場合のみ変更
            if (Thumbnail != null && Thumbnail.Length > 0)
            {
                var fileName = Path.GetFileName(Thumbnail.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Thumbnail.CopyToAsync(stream);
                }

                // サムネイルのパスを更新
                existingArticle.ThumbnailPath = "/uploads/" + fileName;
            }

            // 🔹 変更があった内容を適用
            existingArticle.Title = model.Title;
            existingArticle.Content = model.Content;
            existingArticle.PublishedAt = DateTime.Now;

            // 🔹 データを更新
            _context.Update(existingArticle);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Articles.Any(e => e.Id == model.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }
    return View(model);
}


        // 記事削除ページ（GET）
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // 記事削除（POST）
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
