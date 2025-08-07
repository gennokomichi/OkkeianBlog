namespace OkkeianBlog.Models
{
public class RssItemModel
{
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Description { get; set; }  // description フィールド
    public string? ContentEncoded { get; set; }  // content:encoded フィールド
}


}
