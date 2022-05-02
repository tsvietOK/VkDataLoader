using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VkDataLoader.Models;

namespace VkDataLoader
{
    internal interface ILinkParser
    {
        void GetLinksFromHtml(ObservableCollection<VkDataItem> vkDataItems, string html);

        ObservableCollection<string> GetHtmlFilesList(string vkFolderPath);
    }
}
