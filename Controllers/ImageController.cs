using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace OkkeianBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost("upload")]
public async Task<IActionResult> UploadImage(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest("No file uploaded.");
    }

    // 許可する画像ファイルの拡張子
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var fileExtension = Path.GetExtension(file.FileName).ToLower();

    if (!allowedExtensions.Contains(fileExtension))
    {
        return BadRequest("Invalid file type. Only image files are allowed.");
    }

    // 最大ファイルサイズ（例：5MB）
    var maxFileSize = 5 * 1024 * 1024;

    if (file.Length > maxFileSize)
    {
        return BadRequest("File size exceeds the maximum limit of 5MB.");
    }

    // 保存先のパスを指定（wwwroot/uploads）
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    // ディレクトリが存在しない場合は作成
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }

    // 年別・月別ディレクトリ作成（オプション）
    var subDirectory = Path.Combine(uploadPath, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
    if (!Directory.Exists(subDirectory))
    {
        Directory.CreateDirectory(subDirectory);
    }

    // ファイル名をランダムに生成して重複を防ぐ
    var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
    var filePath = Path.Combine(subDirectory, fileName);

    try
    {
        // ファイルをサーバーに保存
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }

    // 保存した画像のURLを返す（Quillエディタで使うURL）
    var imageUrl = $"/uploads/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{fileName}";
    return Ok(new { url = imageUrl, fileName = fileName, filePath = filePath });
}

    }
}
