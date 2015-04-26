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
        private readonly Database database = PhoneApplicationService.Current.State["database"] as Database;
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
                if (await database.areUserDetailsLogged())
                {
                    try
                    {
                        await database.loginFromStoredDetails();
                        successful = true;
                        App.loggedin = true;
                    } catch (Exception)
                    {
                        App.loggedin = false;
                        successful = false;
                    }
                }
                if (!successful)
                {
                    try
                    {
                        await database.performLoginProcess();
                        if (keepLogged)
                            await Task.Run(async () => { await database.storeLoginDetails(); });
                        App.loggedin = true;
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
            if (App.loggedin)
            {
                NavigationService.Navigate(new Uri("/PivotMainPage.xaml", UriKind.Relative));
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            keepLogged = !keepLogged;
            if(keepLogged == false)
            {
                database.deleteLoginDetails();
            }
        }
    }
}