using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using VkDataLoader.Models;

namespace VkDataLoader.Parsers
{
    internal class DocumentLinkParser : ILinkParser
    {
        public ObservableCollection<string> GetHtmlFilesList(string vkFolderPath)
        {
            throw new NotImplementedException();
        }

        public void GetLinksFromHtml(ObservableCollection<VkDataItem> vkDataItems, string html)
        {
            throw new NotImplementedException();
        }
    }
}
