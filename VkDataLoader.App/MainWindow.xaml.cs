using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using VkDataLoader.App;
using VkLoader.App.Extensions;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VkLoader.App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();

        AppWindow appWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            // https://nicksnettravels.builttoroam.com/winappsdk-windowing/
            appWindow = this.GetAppWindowForWinUI();
            appWindow.Resize(new SizeInt32(900, 600));
        }
    }
}
