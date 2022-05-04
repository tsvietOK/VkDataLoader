using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using VkDataLoader.App;
using VkLoader.App.Extensions;
using Windows.Graphics;
using WinRT;

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

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        MicaController m_micaController;
        SystemBackdropConfiguration m_configurationSource;

        DesktopAcrylicController m_acrylicController;

        AppWindow appWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            // https://nicksnettravels.builttoroam.com/winappsdk-windowing/
            appWindow = this.GetAppWindowForWinUI();
            appWindow.Resize(new SizeInt32(900, 600));

            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            SetBackdrop(BackdropType.Mica);
        }

        public void SetBackdrop(BackdropType type)
        {
            BackdropType m_currentBackdrop = BackdropType.DefaultColor;

            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            if (m_acrylicController != null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }
            Activated -= Window_Activated;
            Closed -= Window_Closed;
            m_configurationSource = null;

            if (type == BackdropType.Mica)
            {
                if (TrySetMicaBackdrop())
                {
                    m_currentBackdrop = type;
                }
                else
                {
                    // Mica isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                }
            }
            if (type == BackdropType.DesktopAcrylic)
            {
                if (TrySetAcrylicBackdrop())
                {
                    m_currentBackdrop = type;
                }
            }
        }

        private bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new SystemBackdropConfiguration();
                Activated += Window_Activated;
                Closed += Window_Closed;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                switch (((FrameworkElement)Content).ActualTheme)
                {
                    case ElementTheme.Dark:
                        m_configurationSource.Theme = SystemBackdropTheme.Dark;
                        break;
                    case ElementTheme.Light:
                        m_configurationSource.Theme = SystemBackdropTheme.Light;
                        break;
                    case ElementTheme.Default:
                        m_configurationSource.Theme = SystemBackdropTheme.Default;
                        break;
                }

                m_micaController = new MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private bool TrySetAcrylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new SystemBackdropConfiguration();
                Activated += Window_Activated;
                Closed += Window_Closed;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                switch (((FrameworkElement)Content).ActualTheme)
                {
                    case ElementTheme.Dark:
                        m_configurationSource.Theme = SystemBackdropTheme.Dark;
                        break;
                    case ElementTheme.Light:
                        m_configurationSource.Theme = SystemBackdropTheme.Light;
                        break;
                    case ElementTheme.Default:
                        m_configurationSource.Theme = SystemBackdropTheme.Default;
                        break;
                }

                m_acrylicController = new DesktopAcrylicController();

                // Enable the system backdrop.
                m_acrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
                m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Acrylic is not supported on this system
        }


        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            Activated -= Window_Activated;
            m_configurationSource = null;
        }

    }
}
