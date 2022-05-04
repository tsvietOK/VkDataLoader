using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
        private Symbol folderStatus;
        private bool isTipOpen;
        private bool isParseLinksButtonEnabled;
        private VkDataProcessorFactory processorFactory;
        private VkDataProcessor dataProcessor;
        private bool isSelectFolderButtonEnabled = true;
        private bool isImagesCheckBoxEnabled;
        private bool isDocumentsCheckBoxEnabled;
        private bool isImagesCheckBoxChecked;
        private bool isDocumentsCheckBoxChecked;
        private Symbol parseStatus;
        private bool isDownloadButtonEnabled;
        private Symbol downloadStatusSymbol;
        private bool isCheckVkConnectionButtonEnabled;
        private Symbol checkVkConnectionSymbol;
        private bool isConnectionAvailable;
        private string connectionStatusString = "Status: Needs to be checked";
        private bool isOpenDataLocationButtonEnabled;

        public MainPageViewModel()
        {
            SelectFolderCommand = new RelayCommand(async () => await SelectFolder());
            ParseLinksCommand = new RelayCommand(async () => await ParseLinks());
            DownloadCommand = new RelayCommand(async () => await Download());
            CheckVkConnectionCommand = new RelayCommand(async () => await CheckVkConnection());
            OpenDataLocationCommand = new RelayCommand(() => OpenDataLocation());
        }

        public VkDataProcessor DataProcessor
        {
            get => dataProcessor;
            set => SetProperty(ref dataProcessor, value);
        }

        public RelayCommand SelectFolderCommand { get; set; }

        public RelayCommand ParseLinksCommand { get; set; }

        public RelayCommand DownloadCommand { get; set; }

        public RelayCommand CheckVkConnectionCommand { get; set; }

        public RelayCommand OpenDataLocationCommand { get; set; }

        public string SelectedFolderPath
        {
            get => selectedFolderPath;
            set => SetProperty(ref selectedFolderPath, value);
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

        public Symbol ParseStatusSymbol
        {
            get => parseStatus;
            set => SetProperty(ref parseStatus, value);
        }

        public bool IsCheckVkConnectionButtonEnabled
        {
            get => isCheckVkConnectionButtonEnabled;
            set => SetProperty(ref isCheckVkConnectionButtonEnabled, value);
        }

        public Symbol CheckVkConnectionSymbol
        {
            get => checkVkConnectionSymbol;
            set => SetProperty(ref checkVkConnectionSymbol, value);
        }

        public bool IsConnectionAvailable
        {
            get => isConnectionAvailable;
            set
            {
                SetProperty(ref isConnectionAvailable, value);
                ConnectionStatusString = value ? "Status: Available" : "Status: Unavailable";
            }
        }

        public string ConnectionStatusString
        {
            get => connectionStatusString;
            set => SetProperty(ref connectionStatusString, value);
        }

        public bool IsDownloadButtonEnabled
        {
            get => isDownloadButtonEnabled;
            set => SetProperty(ref isDownloadButtonEnabled, value);
        }

        public Symbol DownloadStatusSymbol
        {
            get => downloadStatusSymbol;
            set => SetProperty(ref downloadStatusSymbol, value);
        }

        public bool IsOpenDataLocationButtonEnabled
        {
            get => isOpenDataLocationButtonEnabled;
            set => SetProperty(ref isOpenDataLocationButtonEnabled, value);
        }

        private async Task SelectFolder()
        {
            FolderPicker picker = new();
            // workaround for Invalid window handle https://stackoverflow.com/questions/57161258/invalid-window-handle-error-when-using-fileopenpicker-from-c-sharp-net-framwo
            picker.As<IInitializeWithWindow>().Initialize(Process.GetCurrentProcess().MainWindowHandle);

            picker.FileTypeFilter.Add("*");
            var folder = await picker.PickSingleFolderAsync();
            if (folder == null)
            {
                return;
            }

            SelectedFolderPath = folder.Path;
            processorFactory = new VkDataProcessorFactory(SelectedFolderPath);

            DataProcessor = processorFactory.GetVkDataProcessor();
            if (processorFactory.IsVkFolder)
            {
                IsOpenDataLocationButtonEnabled = true;
                FolderStatus = Symbol.Accept;
                IsSelectFolderButtonEnabled = false;

                IsImagesCheckBoxChecked = DataProcessor.Parser.IsImagesEnabled;
                IsDocumentsCheckBoxChecked = DataProcessor.Parser.IsDocumentsEnabled;

                if (processorFactory.IsConfigurationLoaded)
                {
                    if (DataProcessor.Parser.IsParseSuccessful)
                    {
                        ParseStatusSymbol = Symbol.Accept;
                        IsParseLinksButtonEnabled = false;
                        ChangeAllCheckBoxesIsEnabled(false);
                        IsCheckVkConnectionButtonEnabled = true;
                        DataProcessor.Loader.LinksCount = DataProcessor.Parser.LinksCount;
                    }
                    else
                    {
                        ParseStatusSymbol = Symbol.Cancel;
                        IsParseLinksButtonEnabled = IsImagesCheckBoxChecked || IsDocumentsCheckBoxChecked;
                    }
                }
                else
                {
                    IsImagesCheckBoxEnabled = DataProcessor.Parser.IsImagesSupported && !DataProcessor.Parser.IsParseSuccessful;
                    IsDocumentsCheckBoxEnabled = DataProcessor.Parser.IsDocumentsSupported && !DataProcessor.Parser.IsParseSuccessful;
                }
            }
            else
            {
                FolderStatus = Symbol.Cancel;
                IsTipOpen = true;
            }
        }

        private async Task ParseLinks()
        {
            if (DataProcessor is null)
            {
                return;
            }

            ChangeAllCheckBoxesIsEnabled(false);
            IsParseLinksButtonEnabled = false;

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

            ParseStatusSymbol = Symbol.Accept;
            IsCheckVkConnectionButtonEnabled = true;
            DataProcessor.Loader.LinksCount = DataProcessor.Parser.LinksCount;
        }

        private async Task CheckVkConnection()
        {
            IsConnectionAvailable = await VkConnectionChecker.IsConnectionAvailableAsync();
            if (IsConnectionAvailable)
            {
                IsCheckVkConnectionButtonEnabled = false;
                IsDownloadButtonEnabled = true;
                CheckVkConnectionSymbol = Symbol.Accept;
            }
            else
            {
                CheckVkConnectionSymbol = Symbol.Cancel;
            }
        }

        private async Task Download()
        {
            DownloadStatusSymbol = Symbol.Download;
            IsDownloadButtonEnabled = false;

            await DataProcessor.LoadParsedItems();

            DownloadStatusSymbol = Symbol.Accept;
        }

        private void OpenDataLocation()
        {
            DataProcessor.OpenFolder();
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
