using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AtosInduction.Resources;
using Microsoft.Phone.Net.NetworkInformation;

namespace AtosInduction
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isConnectionAvailable = (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None);

            if (isConnectionAvailable == false)
            {
                MessageBox.Show("You need Internet conncetion in order to use this app.", "You are not connected.", MessageBoxButton.OK);
            }
            else
            {
                NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
            }
        }
    }
}