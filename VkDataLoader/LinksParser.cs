using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VkDataLoader.Models;
using VkDataLoader.Parsers;

namespace VkDataLoader
{
    public class LinksParser : ObservableObject
    {
        private const int REFRESH_INTERVAL = 100;
        private string vkFolderPath;
        private ObservableCollection<VkDataItem> vkDataItems = new();
        private ObservableCollection<string> htmlFilesList = new();
        private int htmlFilesCount;
        private int currentHtmlFileNumber;
        private bool isImagesEnabled;
        private bool isDocumentsEnabled;
        private bool isImagesSupported = true;
        private bool isDocumentsSupported;
        private int linksCount;
        private bool isParseSuccessful;

        public LinksParser(string vkFolderPath)
        {
            VkFolderPath = vkFolderPath;
        }

        public ObservableCollection<VkDataItem> VkDataItems
        {
            get => vkDataItems; private set
            {
                SetProperty(ref vkDataItems, value);
            }
        }

        public bool IsImagesEnabled
        {
            get => isImagesEnabled;
            set => SetProperty(ref isImagesEnabled, value);
        }

        [JsonIgnore]
        public bool IsImagesSupported
        {
            get => isImagesSupported;
            set => SetProperty(ref isImagesSupported, value);
        }

        public bool IsDocumentsEnabled
        {
            get => isDocumentsEnabled;
            set => SetProperty(ref isDocumentsEnabled, value);
        }

        [JsonIgnore]
        public bool IsDocumentsSupported
        {
            get => isDocumentsSupported;
            set => SetProperty(ref isDocumentsSupported, value);
        }

        public int HtmlFilesCount
        {
            get => htmlFilesCount;
            set => SetProperty(ref htmlFilesCount, value);
        }

        public int CurrentHtmlFileNumber
        {
            get => currentHtmlFileNumber;
            set => SetProperty(ref currentHtmlFileNumber, value);
        }

        public int LinksCount
        {
            get => linksCount;
            set => SetProperty(ref linksCount, value);
        }

        public bool IsParseSuccessful
        {
            get => isParseSuccessful;
            set => SetProperty(ref isParseSuccessful, value);
        }

        public ObservableCollection<string> HtmlFilesList
        {
            get => htmlFilesList;
            private set
            {
                SetProperty(ref htmlFilesList, value);
                HtmlFilesCount = htmlFilesList.Count;
            }
        }

        public string VkFolderPath
        {
            get => vkFolderPath;
            set => vkFolderPath = value;
        }

        public async Task ParseAsync(List<string> itemsToLoad)
        {
            foreach (var item in itemsToLoad)
            {
                ILinkParser? parser = item switch
                {
                    "images" => new ImageLinkParser(),
                    "documents" => new DocumentLinkParser(),
                    _ => new ImageLinkParser(),
                };

                HtmlFilesList = parser.GetHtmlFilesList(VkFolderPath);
                Stopwatch stopwatch = Stopwatch.StartNew();
                var progressLock = new object();
                var tasks = new List<Task>();
                var parserLock = new object();
                for (int i = 0; i < HtmlFilesList.Count; i++)
                {
                    var task = Task.Run(async () =>
                    {
                        int j = i;
                        var file = HtmlFilesList[j];
                        using var reader = File.OpenText(file);
                        var text = await reader.ReadToEndAsync();
                        lock (parserLock)
                        {
                            parser.GetLinksFromHtml(VkDataItems, text);
                        }
                    });
                }
                int n = 0;
                while ((n = tasks.Count(t => t.Status == TaskStatus.Running)) > 0)
                {
                    if (stopwatch.ElapsedMilliseconds > REFRESH_INTERVAL)
                    {
                        CurrentHtmlFileNumber = tasks.Count - n;
                        LinksCount = VkDataItems.Count;
                        stopwatch.Restart();
                    }
                }
                CurrentHtmlFileNumber = HtmlFilesList.Count;
                LinksCount = VkDataItems.Count;
                stopwatch.Stop();
            }
            IsParseSuccessful = true;
        }

        public void SetFolderPath(string path)
        {
            VkFolderPath = path;
        }

        public void Reset()
        {
            HtmlFilesCount = 0;
            CurrentHtmlFileNumber = 0;
            HtmlFilesList?.Clear();
            VkDataItems?.Clear();
        }
    }
}