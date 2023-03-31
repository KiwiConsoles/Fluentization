using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Linq;
using System.ServiceProcess;
using System_Configuration.CommandBars;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace System_Configuration.Content.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Services : Page
    {
        private ServiceController[] _services;
        public Services()
        {
            this.InitializeComponent();
            _services = ServiceController.GetServices();
            CommandBars.Services.HideServices.Click += FilterServices_Click;
            GetServices();
            //new Thread(new ThreadStart(GetServices)).Start();
        }

        public void GetServices()
        {          
            foreach (ServiceController service in ServiceController.GetServices())
            {
                ServicesList.Items.Add(service.DisplayName + ", " + service.Status);
            }
        }

        private void FilterServices_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Clear the list view
            ServicesList.Items.Clear();

            // Filter services based on the toggle button state
            ServiceController[] filteredServices;
            if (CommandBars.Services.HideServices.IsChecked == true)
            {
                // Show non-system services only
                filteredServices = _services.Where(service =>
                    (service.ServiceType & ServiceType.Win32OwnProcess) == ServiceType.Win32OwnProcess &&
                    (service.ServiceType & ServiceType.InteractiveProcess) != ServiceType.InteractiveProcess &&
                    !service.ServiceName.StartsWith("S")) // Filter out services that start with "S"
                    .ToArray();
            }
            else
            {
                // Show all services
                filteredServices = _services;
            }

            // Add filtered services to the list view
            foreach (ServiceController service in filteredServices)
            {
                AddServiceToListView(service);
            }
        }

        private void AddServiceToListView(ServiceController service)
        {
            ServicesList.Items.Add(service.DisplayName);
        }


        private void EnableAll_Click(object sender, RoutedEventArgs e)
        {
            ServicesList.SelectAll();
        }

        private void DisableAll_Click(object sender, RoutedEventArgs e)
        {
            ServicesList.SelectedItem = null;
        }


    }
}
