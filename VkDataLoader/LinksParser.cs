using HtmlAgilityPack;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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
    public class LinksParser : ObservableObject
    {
        private readonly string vkFolderPath;
        private ObservableCollection<VkDataItem> vkDataItems = new();
        private ObservableCollection<string> htmlFilesList = new();
        private int htmlFilesCount;
        private int currentHtmlFileNumber;

        public LinksParser(string vkFolderPath)
        {
            this.vkFolderPath = vkFolderPath;
        }

        public ObservableCollection<VkDataItem> VkDataItems
        {
            get => vkDataItems; private set
            {
                SetProperty(ref vkDataItems, value);
                //string jsonVkItems = JsonConvert.SerializeObject(vkDataItems);
            }
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

        public ObservableCollection<string> HtmlFilesList
        {
            get => htmlFilesList;
            private set
            {
                SetProperty(ref htmlFilesList, value);
                HtmlFilesCount = htmlFilesList.Count;
            }
        }

        public async Task ParseAsync(string itemsToLoad)
        {
            ILinkParser? parser = itemsToLoad switch
            {
                "images" => new ImageLinkParser(),
                //"documents" => new DocumentLinkParser(),
                _ => new ImageLinkParser(),
            };

            HtmlFilesList = parser.GetHtmlFilesList(vkFolderPath);
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
    }
}