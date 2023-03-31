using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace System_Configuration.CommandBars
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Services : Page
    {
        public static AppBarToggleButton HideServices { get; set; }

        public Services()
        {
            this.InitializeComponent();
            HideServices = Hide;
        }
    }
}
