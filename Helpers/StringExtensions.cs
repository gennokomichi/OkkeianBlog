using System.Text.RegularExpressions;
using System.Text;  // ← これを追加！

namespace OkkeianBlog.Helpers
{
   public static class StringExtensions
{
    public static string ToExcerpt(this string content, int length)
    {
        if (string.IsNullOrEmpty(content)) return "内容なし";

        // HTMLタグを除去
        var plainText = Regex.Replace(content, "<.*?>", string.Empty);
        plainText = Regex.Replace(plainText, "&nbsp;", " ");

        // 全角文字と半角文字の区別
        int count = 0;
        StringBuilder excerptBuilder = new StringBuilder();

        foreach (char c in plainText)
        {
            count += (c > 127) ? 2 : 1; // 全角なら+2、半角なら+1
            if (count > length * 2) break; // 半角文字は最大200文字
            excerptBuilder.Append(c);
        }

        return excerptBuilder.ToString() + "...";
    }
}

}