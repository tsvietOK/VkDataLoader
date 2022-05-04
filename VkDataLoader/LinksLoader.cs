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
        private int downloadedCount;
        private int skippedCount;
        private int errorCount;
        private int overallProgressCount;

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

        public async Task LoadAsync(ObservableCollection<VkDataItem> vkDataItems)
        {
            httpClient = new HttpClient();
            for (int i = 0; i < vkDataItems.Count; i++)
            {
                OverallProgressCount = i;

                VkDataItem? item = vkDataItems[i];
                if (item.DownloadStatus == VkDataDownloadStatus.VK_DATA_DOWNLOAD_STATUS_OK)
                {
                    SkippedCount++;
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

                processorFactory.SaveConfiguration();
                await Task.Delay(MILLISECONDS_DELAY);
            }

            httpClient.Dispose();
        }
    }
}
