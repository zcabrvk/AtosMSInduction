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
using System.IO;

namespace AtosInduction
{
    public partial class LoginScreen : PhoneApplicationPage
    {
        private bool keepLogged = false;

        public LoginScreen()
        {
            InitializeComponent();
        }

        //empty navigation stack
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private async Task Login()
        {
            if(!App.loggedin)
            {
                bool successful = false;
                if (await LoginProcess.isThereTokenFile())
                {
                    try
                    {
                        await LoginProcess.getAccessToken();
                        successful = true;
                    } catch (Exception)
                    {
                        successful = false;
                    }
                }
                if (!successful)
                {
                    try
                    {
                        await LoginProcess.RequireAccess(this.keepLogged);
                    } catch(Exception)
                    {
                        App.loggedin = false; //Access failed
                    }
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Login();
            if(App.loggedin)
                NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            keepLogged = !keepLogged;
            if(keepLogged == false)
            {
                LoginProcess.deleteTokenFile();
            }
        }
    }
}