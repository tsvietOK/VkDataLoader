using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VkDataLoader.Models;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT;

namespace VkDataLoader.App
{
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }

    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private string selectedFolderPath;
        private bool isVkFolder;
        private Symbol folderStatus;
        private bool isTipOpen;
        private bool isParseLinksButtonEnabled;
        private VkDataProcessor dataProcessor;
        private bool isSelectFolderButtonEnabled = true;
        private bool isParserInProgress;

        public MainPageViewModel()
        {
            SelectFolderCommand = new RelayCommand(async () =>
            {
                FolderPicker picker = new();
                // workaround for Invalid window handle https://stackoverflow.com/questions/57161258/invalid-window-handle-error-when-using-fileopenpicker-from-c-sharp-net-framwo
                picker.As<IInitializeWithWindow>().Initialize(Process.GetCurrentProcess().MainWindowHandle);

                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SelectedFolderPath = folder.Path;
                    IsVkFolder = await DoesFileExistAsync(folder, "index.html");
                }
            });

            ParseLinksCommand = new RelayCommand(async () =>
            {
                IsParserInProgress = true;
                await DataProcessor.ParseItems("images");
                IsParserInProgress = false;
            });
        }

        public VkDataProcessor DataProcessor
        {
            get => dataProcessor; set
            {
                dataProcessor = value;
                NotifyPropertyChanged(nameof(DataProcessor));
            }
        }

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
                    IsSelectFolderButtonEnabled = false;
                    DataProcessor = new VkDataProcessor(SelectedFolderPath);
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

        public bool IsSelectFolderButtonEnabled
        {
            get => isSelectFolderButtonEnabled; set
            {
                if (isSelectFolderButtonEnabled != value)
                {
                    isSelectFolderButtonEnabled = value;
                    NotifyPropertyChanged(nameof(IsSelectFolderButtonEnabled));
                }
            }
        }

        public bool IsParserInProgress
        {
            get => isParserInProgress; set
            {
                if (isParserInProgress != value)
                {
                    isParserInProgress = value;
                    NotifyPropertyChanged(nameof(IsParserInProgress));

                    IsParseLinksButtonEnabled = !isParserInProgress;
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
