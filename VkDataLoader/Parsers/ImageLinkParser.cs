using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkDataLoader.Enums;
using VkDataLoader.Models;

namespace VkDataLoader.Parsers
{
    internal class ImageLinkParser : ILinkParser
    {
        public void GetLinksFromHtml(ObservableCollection<VkDataItem> vkDataItems, string html)
        {
            HtmlDocument htmlSnippet = new();
            htmlSnippet.LoadHtml(html);

            //List<VkDataItem> hrefTags = new();
            string xpathExpression = $"//div[@class='attachment']/a[@class='attachment__link' and contains(@href,'userapi')]";
            var nodes = htmlSnippet.DocumentNode.SelectNodes(xpathExpression);
            if (nodes?.Count > 0)
            {
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
}
