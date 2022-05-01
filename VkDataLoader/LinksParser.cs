using HtmlAgilityPack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VkDataLoader.Models;
using VkDataLoader.Parsers;
using Windows.Storage;
using Windows.Storage.Search;

namespace VkDataLoader
{
    public class LinksParser : INotifyPropertyChanged
    {
        private readonly string messageFolderPath;
        private ObservableCollection<VkDataItem> vkDataItems = new();
        private ObservableCollection<string> htmlFilesList = new();
        private int htmlFilesCount;
        private int currentHtmlFileNumber;

        public LinksParser(string messageFolderPath)
        {
            this.messageFolderPath = messageFolderPath;
        }

        public ObservableCollection<VkDataItem> VkDataItems
        {
            get => vkDataItems; private set
            {
                vkDataItems = value;
                NotifyPropertyChanged(nameof(VkDataItems));
                //string jsonVkItems = JsonConvert.SerializeObject(vkDataItems);
            }
        }

        public int HtmlFilesCount
        {
            get => htmlFilesCount; set
            {
                if (htmlFilesCount != value)
                {
                    htmlFilesCount = value;
                    NotifyPropertyChanged(nameof(HtmlFilesCount));
                }
            }
        }

        public int CurrentHtmlFileNumber
        {
            get => currentHtmlFileNumber; set
            {
                if (currentHtmlFileNumber != value)
                {
                    currentHtmlFileNumber = value;
                    NotifyPropertyChanged(nameof(CurrentHtmlFileNumber));
                }
            }
        }

        public ObservableCollection<string> HtmlFilesList
        {
            get => htmlFilesList; private set
            {
                htmlFilesList = value;
                NotifyPropertyChanged(nameof(HtmlFilesList));
                HtmlFilesCount = htmlFilesList.Count;
            }
        }
        public async Task ParseAsync(string itemsToLoad)
        {
            HtmlFilesList = new ObservableCollection<string>(Directory.EnumerateFiles(messageFolderPath, "*.html", SearchOption.AllDirectories).ToList());
            ILinkParser? parser = itemsToLoad switch
            {
                "images" => new ImageLinkParser(),
                //"documents" => new DocumentLinkParser(),
                _ => new ImageLinkParser(),
            };
            for (int i = 0; i < HtmlFilesList.Count; i++)
            {
                CurrentHtmlFileNumber = i;
                string file = HtmlFilesList[i];
                //Console.WriteLine($"Processing file:{file}");
                using var reader = File.OpenText(file);
                var text = await reader.ReadToEndAsync();
                parser.GetLinksFromHtml(VkDataItems, text);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}