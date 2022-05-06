using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VkDataLoader.Enums;
using VkDataLoader.Loaders;
using VkDataLoader.Models;

namespace VkDataLoader
{
    public class LinksLoader : ObservableObject
    {
        private const int MILLISECONDS_DELAY = 200;
        private readonly VkDataProcessorFactory processorFactory;
        private HttpClient httpClient;
        private int downloadedCount;
        private int skippedCount;
        private int errorCount;
        private int overallProgressCount;
        private int linksCount;

        public LinksLoader(VkDataProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public int OverallProgressCount
        {
            get => overallProgressCount;
            set => SetProperty(ref overallProgressCount, value);
        }

        public int DownloadedCount
        {
            get => downloadedCount;
            set => SetProperty(ref downloadedCount, value);
        }

        public int SkippedCount
        {
            get => skippedCount;
            set => SetProperty(ref skippedCount, value);
        }

        public int ErrorCount
        {
            get => errorCount;
            set => SetProperty(ref errorCount, value);
        }

        public int LinksCount
        {
            get => linksCount;
            set => SetProperty(ref linksCount, value);
        }

        public async Task LoadAsync(ObservableCollection<VkDataItem> vkDataItems)
        {
            httpClient = new HttpClient();
            for (int i = 0; i < vkDataItems.Count; i++)
            {

                VkDataItem? item = vkDataItems[i];
                if (item.DownloadStatus == VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_OK)
                {
                    SkippedCount++;
                    OverallProgressCount++;
                    continue;
                }

                ILoader loader = item.DataType switch
                {
                    VkDataType.VK_DATA_IMAGE => new ImageLoader(processorFactory.ApplicationFolderPath),
                    VkDataType.VK_DATA_DOCUMENT => throw new NotImplementedException(),
                    _ => throw new NotImplementedException(),
                };

                bool downloadResult = await loader.TryLoadAsync(httpClient, item.Url, i);
                if (downloadResult)
                {
                    item.DownloadStatus = VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_OK;
                    DownloadedCount++;
                }
                else
                {
                    item.DownloadStatus = VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_ERROR;
                    ErrorCount++;
                }

                OverallProgressCount++;
                processorFactory.SaveConfiguration();
                await Task.Delay(MILLISECONDS_DELAY);
            }

            httpClient.Dispose();
        }
    }
}
