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
            string xpathExpression = $"//div[@class='attachment']/a[@class='attachment__link']";
            if (htmlSnippet.DocumentNode.SelectNodes(xpathExpression) != null)
            {
                foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes(xpathExpression))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    var href = att.Value;
                    if (href.Contains("userapi"))
                    {
                        var dataItem = new VkDataItem
                        {
                            Url = att.Value,
                            DataType = VkDataType.Image
                        };

                        vkDataItems.Add(dataItem);
                    }
                }
            }
        }
    }
}
