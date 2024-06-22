using HtmlAgilityPack;

namespace SU24_VMO_API.Supporters.BankSupporter
{
    public class GetQRImage
    {
        public async Task<string?> CreateQRCodeBaseOnUrlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Tải nội dung HTML từ URL
                    var html = await httpClient.GetStringAsync(url);

                    // Tạo một HtmlDocument để load nội dung HTML
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    // Tìm tất cả các thẻ <img> với class "max-w-[80%] object-fill cursor-pointer"
                    var imgNodes = htmlDoc.DocumentNode.SelectNodes("//img[contains(@class, 'max-w-[80%] object-fill cursor-pointer')]");

                    if (imgNodes != null)
                    {
                        foreach (var imgNode in imgNodes)
                        {
                            // Lấy thuộc tính src của thẻ <img>
                            var src = imgNode.GetAttributeValue("src", "");
                            return src;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (HttpRequestException e)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
