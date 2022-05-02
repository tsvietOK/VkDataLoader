using System.Collections.ObjectModel;
using VkDataLoader.Models;

namespace VkDataLoader
{
    internal interface ILinkParser
    {
        void GetLinksFromHtml(ObservableCollection<VkDataItem> vkDataItems, string html);

        ObservableCollection<string> GetHtmlFilesList(string vkFolderPath);
    }
}
