using HtmlAgilityPack;
using System.Collections.ObjectModel;
using VkDataLoader.Enums;
using VkDataLoader.Models;

namespace VkDataLoader.Parsers
{
    internal class ImageLinkParser : ILinkParser
    {
        public ImageLinkParser()
        {
        }

        public ObservableCollection<string> GetHtmlFilesList(string vkFolderPath)
        {
            string messageFolderPath = Path.Combine(vkFolderPath, "messages");
            return new ObservableCollection<string>(Directory.EnumerateFiles(messageFolderPath, "*.html", SearchOption.AllDirectories).ToList());
        }

        public void GetLinksFromHtml(ObservableCollection<VkDataItem> vkDataItems, string html)
        {
            HtmlDocument htmlSnippet = new();
            htmlSnippet.LoadHtml(html);

            string xpathExpression = $"//div[@class='attachment']/a[@class='attachment__link' and contains(@href,'userapi')]";
            var nodes = htmlSnippet.DocumentNode.SelectNodes(xpathExpression);
            if (nodes is null)
            {
                return;
            }

            foreach (HtmlNode link in nodes)
            {
                var href = link.Attributes["href"].Value;
                var dataItem = new VkDataItem
                {
                    Url = href,
                    DataType = VkDataType.Image
                };

                vkDataItems.Add(dataItem);
            }
        }
    }
}
