using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VkDataLoader.Enums;
using VkDataLoader.Loaders;
using VkDataLoader.Models;

namespace VkDataLoader
{
    public class LinksLoader : ObservableObject
    {
        private const int MILLISECONDS_DELAY = 500;
        private readonly VkDataProcessorFactory processorFactory;
        private HttpClient httpClient;
        private int currentDownloadedItem;

        public LinksLoader(VkDataProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public int CurrentDownloadedItem
        {
            get => currentDownloadedItem;
            set => SetProperty(ref currentDownloadedItem, value);
        }

        public async Task LoadAsync(ObservableCollection<VkDataItem> vkDataItems)
        {
            httpClient = new HttpClient();
            for (int i = 0; i < vkDataItems.Count; i++)
            {
                VkDataItem? item = vkDataItems[i];
                if (item.DownloadStatus == VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_OK)
                {
                    continue;
                }

                ILoader loader = item.DataType switch
                {
                    VkDataType.VK_DATA_IMAGE => new ImageLoader(),
                    VkDataType.VK_DATA_DOCUMENT => throw new NotImplementedException(),
                    _ => throw new NotImplementedException(),
                };

                bool downloadResult = await loader.TryLoadAsync(httpClient, item.Url, i, processorFactory.ApplicationFolderPath);
                if (downloadResult)
                {
                    item.DownloadStatus = VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_OK;
                }
                else
                {
                    item.DownloadStatus = VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_ERROR;
                }

                processorFactory.SaveConfiguration();
                CurrentDownloadedItem = i;
                await Task.Delay(MILLISECONDS_DELAY);
            }

            httpClient.Dispose();
        }
    }
}
