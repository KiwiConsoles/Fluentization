using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System_Configuration.Content.Pages;
using Windows.ApplicationModel;
using WinRT;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace System_Configuration
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public static WindowEx PWindow { get; set; }
        Shared.WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;

        public MainWindow()
        {
            this.InitializeComponent();
            Title = Package.Current.DisplayName;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitlebarDragRegion);
            TrySetMicaBackdrop();
            PWindow = this;
        }

        bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new Shared.WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();
                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
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
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
            //Even though the background should automatically change, it sometimes breaks
            //so this force changes the background
            MBackground.Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["ContentBackground"];
            if (App.Current.RequestedTheme == ApplicationTheme.Light)
            {

            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }

        private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var Content = ContentFrame;
            var Command = CommandBarFrame;
            var Animation = new DrillInNavigationTransitionInfo();

            if (args.IsSettingsSelected == false)
            {
                if (ContentGrid.RowDefinitions[0].Height == new GridLength(0))
                {
                    ContentGrid.RowDefinitions[0].Height = new GridLength(51);
                    MBackground.Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["ContentBackground"];
                }
            }
            if (args.IsSettingsSelected)
            {
                ContentGrid.RowDefinitions[0].Height = new GridLength(0);
                Content.Navigate(typeof(Settings), null, Animation);
                MBackground.Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["GenericFrameBackground"];
            }
            else
            {
                NavigationViewItem Selection = args.SelectedItem as NavigationViewItem;
                switch (Selection.Tag.ToString())
                {
                    case "General":
                        Command.Navigate(typeof(CommandBars.General), null, Animation);
                        Content.Navigate(typeof(General), null, Animation);
                        break;
                    case "Boot":
                        Command.Navigate(typeof(CommandBars.Boot), null, Animation);
                        Content.Navigate(typeof(Boot), null, Animation);
                        break;
                    case "Services":
                        Command.Navigate(typeof(CommandBars.Services), null, Animation);
                        Content.Navigate(typeof(Services), null, Animation);
                        break;
                    case "StartupApps":
                        Command.Navigate(typeof(CommandBars.StartupApps), null, Animation);
                        Content.Navigate(typeof(StartupApps), null, Animation);
                        break;
                    case "Tools":
                        Command.Navigate(typeof(CommandBars.Tools), null, Animation);
                        Content.Navigate(typeof(Tools), null, Animation);
                        break;
                }
            }
        }
    }
}