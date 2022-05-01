using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VkDataLoader.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //this.DataContext = new MainPageViewModel();
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(900, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            
        }

        private MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(@"C:\");
            }
            catch
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "File system permissions";
                dialog.Content = "It seems you have not granted permission for this app to access the file system broadly. " +
                    "Without this permission, the app will only be able to access a very limited set of filesystem locations. " +
                    "You can grant this permission in the Settings app, if you wish. You can do this now or later. " +
                    "If you change the setting while this app is running, it will terminate the app so that the " +
                    "setting can be applied. Do you want to do this now?";
                dialog.PrimaryButtonText = "OK";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.PrimaryButtonCommand = new RelayCommand(async () =>
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
                });
                await dialog.ShowAsync();
            }
        }
    }
}
