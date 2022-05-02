using HtmlAgilityPack;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
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
        private string vkFolderPath;
        private ObservableCollection<VkDataItem> vkDataItems = new();
        private ObservableCollection<string> htmlFilesList = new();
        private int htmlFilesCount;
        private int currentHtmlFileNumber;
        private bool isImagesEnabled;
        private bool isDocumentsEnabled;
        private bool isImagesSupported = true;
        private bool isDocumentsSupported;

        public LinksParser(string vkFolderPath)
        {
            this.vkFolderPath = vkFolderPath;
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

        public ObservableCollection<string> HtmlFilesList
        {
            get => htmlFilesList;
            private set
            {
                SetProperty(ref htmlFilesList, value);
                HtmlFilesCount = htmlFilesList.Count;
            }
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

                HtmlFilesList = parser.GetHtmlFilesList(vkFolderPath);
                for (int i = 0; i < HtmlFilesList.Count; i++)
                {
                    CurrentHtmlFileNumber = i;
                    string file = HtmlFilesList[i];
                    using var reader = File.OpenText(file);
                    var text = await reader.ReadToEndAsync();
                    parser.GetLinksFromHtml(VkDataItems, text);
                }
            }
        }

        public void SetFolderPath(string path)
        {
            vkFolderPath = path;
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