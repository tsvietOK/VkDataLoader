using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

    internal class MainPageViewModel : ObservableObject
    {
        private string selectedFolderPath;
        private bool isVkFolder;
        private Symbol folderStatus;
        private bool isTipOpen;
        private bool isParseLinksButtonEnabled;
        private VkDataProcessorFactory processorFactory;
        private VkDataProcessor dataProcessor;
        private bool isSelectFolderButtonEnabled = true;
        private bool isParserInProgress;
        private bool isImagesCheckBoxEnabled;
        private bool isDocumentsCheckBoxEnabled;
        private bool isImagesCheckBoxChecked;
        private bool isDocumentsCheckBoxChecked;

        public MainPageViewModel()
        {
            SelectFolderCommand = new RelayCommand(async () =>
            {
                FolderPicker picker = new();
                // workaround for Invalid window handle https://stackoverflow.com/questions/57161258/invalid-window-handle-error-when-using-fileopenpicker-from-c-sharp-net-framwo
                picker.As<IInitializeWithWindow>().Initialize(Process.GetCurrentProcess().MainWindowHandle);

                picker.FileTypeFilter.Add("*");
                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SelectedFolderPath = folder.Path;
                    processorFactory = new VkDataProcessorFactory(SelectedFolderPath);
                    IsVkFolder = processorFactory.IsVkFolder;
                    DataProcessor = processorFactory.GetVkDataProcessor();

                    IsImagesCheckBoxChecked = DataProcessor.Parser.IsImagesEnabled;
                    IsImagesCheckBoxEnabled = DataProcessor.Parser.IsImagesSupported;

                    IsDocumentsCheckBoxChecked = DataProcessor.Parser.IsDocumentsEnabled;
                    IsDocumentsCheckBoxEnabled = DataProcessor.Parser.IsDocumentsSupported;
                }
            });

            ParseLinksCommand = new RelayCommand(async () =>
            {
                if (DataProcessor is null)
                {
                    return;
                }

                IsParserInProgress = true;
                List<string> itemsToLoad = new();
                if (IsImagesCheckBoxChecked)
                {
                    itemsToLoad.Add("images");
                }
                if (IsDocumentsCheckBoxChecked)
                {
                    itemsToLoad.Add("documents");
                }
                await DataProcessor.ParseItems(itemsToLoad);
                IsParserInProgress = false;
            });
        }

        public VkDataProcessor DataProcessor
        {
            get => dataProcessor;
            set => SetProperty(ref dataProcessor, value);
        }

        public RelayCommand SelectFolderCommand { get; set; }

        public RelayCommand ParseLinksCommand { get; set; }

        public string SelectedFolderPath
        {
            get => selectedFolderPath;
            set => SetProperty(ref selectedFolderPath, value);
        }

        public bool IsVkFolder
        {
            get => isVkFolder;
            set
            {
                SetProperty(ref isVkFolder, value);

                if (isVkFolder)
                {
                    FolderStatus = Symbol.Accept;
                    IsParseLinksButtonEnabled = true;
                    IsSelectFolderButtonEnabled = false;
                    ChangeAllCheckBoxesIsEnabled(true);
                    CheckAtLeastOneCheckBoxChecked();
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
            get => folderStatus;
            set => SetProperty(ref folderStatus, value);
        }

        public bool IsTipOpen
        {
            get => isTipOpen;
            set => SetProperty(ref isTipOpen, value);
        }

        public bool IsImagesCheckBoxEnabled
        {
            get => isImagesCheckBoxEnabled;
            set => SetProperty(ref isImagesCheckBoxEnabled, value);
        }

        public bool IsImagesCheckBoxChecked
        {
            get => isImagesCheckBoxChecked;
            set
            {
                SetProperty(ref isImagesCheckBoxChecked, value);
                CheckAtLeastOneCheckBoxChecked();
                DataProcessor.Parser.IsImagesEnabled = value;
            }
        }

        public bool IsDocumentsCheckBoxEnabled
        {
            get => isDocumentsCheckBoxEnabled;
            set => SetProperty(ref isDocumentsCheckBoxEnabled, value);
        }

        public bool IsDocumentsCheckBoxChecked
        {
            get => isDocumentsCheckBoxChecked;
            set
            {
                SetProperty(ref isDocumentsCheckBoxChecked, value);
                CheckAtLeastOneCheckBoxChecked();
                DataProcessor.Parser.IsDocumentsEnabled = value;
            }
        }

        public bool IsParseLinksButtonEnabled
        {
            get => isParseLinksButtonEnabled;
            set => SetProperty(ref isParseLinksButtonEnabled, value);
        }

        public bool IsSelectFolderButtonEnabled
        {
            get => isSelectFolderButtonEnabled;
            set => SetProperty(ref isSelectFolderButtonEnabled, value);
        }

        public bool IsParserInProgress
        {
            get => isParserInProgress;
            set
            {
                SetProperty(ref isParserInProgress, value);
                IsParseLinksButtonEnabled = !isParserInProgress;
                ChangeAllCheckBoxesIsEnabled(!isParserInProgress);
            }
        }

        public void ChangeAllCheckBoxesIsEnabled(bool enabled)
        {
            IsImagesCheckBoxEnabled = enabled;
            // not supported
            //IsDocumentsCheckBoxEnabled = state;
        }

        public void CheckAtLeastOneCheckBoxChecked()
        {
            IsParseLinksButtonEnabled = IsImagesCheckBoxChecked || IsDocumentsCheckBoxChecked;
        }
    }
}
