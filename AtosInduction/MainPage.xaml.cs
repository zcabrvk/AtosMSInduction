using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AtosInduction.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;

namespace AtosInduction
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage() {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!await checkInternetConnection())
                throw new Exception("No Internet Connection"); //unhandled excpetion closes the app!
            navigateToNextPage();
        }

        private async void navigateToNextPage()
        {
            bool skipLogin = await LoginProcess.isThereTokenFile();
            if (!skipLogin)
            {
                NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.Relative));
            }
            else
            {
                try
                {
                    await LoginProcess.getAccessToken();
                    NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
                }
                catch(Exception)
                {
                    NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.Relative));
                }
            }
        }

        private async Task<bool> checkInternetConnection()
        {
            bool isConnectionAvailable = await Task.Run(() => (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None));

            if (isConnectionAvailable == false)
            {
                MessageBox.Show("You need Internet connection in order to use this app.", "You are not connected.", MessageBoxButton.OK);
                return false;
            }
            return true;
        }
    }
}