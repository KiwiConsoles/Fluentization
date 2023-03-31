using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Threading;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace System_Configuration.Content.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupApps : Page
    {
        public StartupApps()
        {
            this.InitializeComponent();
        }

        private void Start()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ms-settings:startupapps",
                        UseShellExecute = true,
                    }
                };
                MainWindow.PWindow.Minimize();
                process.Start();
            }
            catch { }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(Start)).Start();
            //await Task.Run(() => Start());
        }
    }
}
