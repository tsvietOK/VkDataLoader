using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VkDataLoader.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace VkDataLoader.UWP
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private string selectedFolderPath;
        private bool isVkFolder;
        private Symbol folderStatus = Symbol.Folder;
        private bool isTipOpen;
        private bool isParseLinksButtonEnabled;

        public MainPageViewModel()
        {
            SelectFolderCommand = new RelayCommand(async () =>
            {
                FolderPicker picker = new FolderPicker();
                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SelectedFolderPath = folder.Path;
                    IsVkFolder = await DoesFileExistAsync(folder, "index.html");
                }
            });

            ParseLinksCommand = new RelayCommand(() =>
            {
                DataProcessor = new VkDataProcessor(SelectedFolderPath);
                DataProcessor.ParseItems("images");
            });
        }

        public VkDataProcessor DataProcessor { get; private set; }

        //public List<VkDataItem> VkDataItems => DataProcessor.VkDataItems;

        public RelayCommand SelectFolderCommand { get; set; }

        public RelayCommand ParseLinksCommand { get; set; }

        public string SelectedFolderPath
        {
            get => selectedFolderPath; set
            {
                if (selectedFolderPath != value)
                {
                    selectedFolderPath = value;
                    NotifyPropertyChanged(nameof(SelectedFolderPath));
                }
            }
        }

        public bool IsVkFolder
        {
            get => isVkFolder; set
            {
                if (isVkFolder != value)
                {
                    isVkFolder = value;
                    NotifyPropertyChanged(nameof(IsVkFolder));
                }
                if (isVkFolder)
                {
                    FolderStatus = Symbol.Accept;
                    IsParseLinksButtonEnabled = true;
                }
                else
                {
                    FolderStatus = Symbol.Cancel;
                    IsTipOpen = true;
                }
            }
        }

        public Symbol FolderStatus
        {
            get => folderStatus; set
            {
                if (folderStatus != value)
                {
                    folderStatus = value;
                    NotifyPropertyChanged(nameof(FolderStatus));
                }
            }
        }

        public bool IsTipOpen
        {
            get => isTipOpen; set
            {
                if (isTipOpen != value)
                {
                    isTipOpen = value;
                    NotifyPropertyChanged(nameof(IsTipOpen));
                }
            }
        }

        public bool IsParseLinksButtonEnabled
        {
            get => isParseLinksButtonEnabled; set
            {
                if (isParseLinksButtonEnabled != value)
                {
                    isParseLinksButtonEnabled = value;
                    NotifyPropertyChanged(nameof(IsParseLinksButtonEnabled));
                }

            }
        }

        static async Task<bool> DoesFileExistAsync(StorageFolder folder, string fileName)
        {
            try
            {
                await folder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
