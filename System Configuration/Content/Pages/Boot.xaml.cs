using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Management;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace System_Configuration.Content.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Boot : Page
    {
        public Boot()
        {
            this.InitializeComponent();
            GetBoot();
        }

        public void GetBoot()
        {
            try
            {
                ConnectionOptions connectionOptions = new ConnectionOptions();
                connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
                connectionOptions.EnablePrivileges = true;

                // The ManagementScope is used to access the WMI info as Administrator
                ManagementScope managementScope = new ManagementScope(@"root\WMI", connectionOptions);

                // {9dea862c-5cdd-4e70-acc1-f32b344d4795} is the GUID of the System BcdStore
                ManagementObject privateLateBoundObject = new ManagementObject(managementScope, new ManagementPath("root\\WMI:BcdObject.Id=\"{9dea862c-5cdd-4e70-acc1-f32b344d4795}\",StoreFilePath=\"\""), null);

                ManagementBaseObject inParams = null;
                inParams = privateLateBoundObject.GetMethodParameters("GetElement");

                // 0x24000001 is a BCD constant: BcdBootMgrObjectList_DisplayOrder
                inParams["Type"] = ((UInt32)0x24000001);
                ManagementBaseObject outParams = privateLateBoundObject.InvokeMethod("GetElement", inParams, null);
                ManagementBaseObject mboOut = ((ManagementBaseObject)(outParams.Properties["Element"].Value));

                string[] osIdList = (string[])mboOut.GetPropertyValue("Ids");

                FindName("MainContent");

                // Each osGuid is the GUID of one Boot Manager in the BcdStore
                foreach (string osGuid in osIdList)
                {
                    ManagementObject currentManObj = new ManagementObject(managementScope, new ManagementPath("root\\WMI:BcdObject.Id=\"" + osGuid + "\",StoreFilePath=\"\""), null);
                    BootList.Items.Add(currentManObj.GetPropertyValue("Id"));
                }

            }
            catch (Exception ex)
            {
                //Load error screen
                FindName("ErrorContent");
                Loaddetails.Text = ex.ToString();
                //this.FindName("MainContent");
                UnloadObject(MainContent);
                //RootGrid.Visibility = Visibility.Collapsed;
                //IsEnabled = false;

                //BootList.Items.Add("Couldn't load boot configuration");
            }
        }

        private void BootList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            ContextMenu.ShowAt(listView, e.GetPosition(listView));
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int RandomNumber = random.Next(0, 100);
            BootList.Items.Add(RandomNumber.ToString());
            await Task.Delay(100);
            BootList.SelectedIndex = BootList.Items.Count - 1;

        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            BootList.Items.Remove(BootList.SelectedItem);
            await Task.Delay(100);
            BootList.SelectedIndex = BootList.Items.Count - 1;
        }
    }
}
