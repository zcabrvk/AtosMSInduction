using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;

namespace AtosInduction
{
    public partial class PivotMainPage : PhoneApplicationPage
    {
        private readonly Database database = PhoneApplicationService.Current.State["database"] as Database;

        public PivotMainPage()
        {
            InitializeComponent();
            setName();
        }

        private async void setName()
        {
            string name = await database.getUserFullName();

            if (string.IsNullOrEmpty(name) || !name.All((char c) => { return (char.IsSeparator(c) || char.IsLetter(c)); })) //validates string
                this.UserName.Visibility = System.Windows.Visibility.Collapsed; //query failed, hide ui element
            else
                this.UserName.Text = "Welcome " + name;
        }

        //If the user press the back button exit the app (empty Navigation stack)
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void OpenPage(object sender, SelectionChangedEventArgs args)
        {
            if ((sender as ListBox).SelectedIndex != -1)
            {
                PhoneApplicationService.Current.State["tabs"] = ((sender as ListBox).ItemsSource as ObservableCollection<Tab>).ToList<Tab>();
                NavigationService.Navigate(new Uri("/WebBrowser.xaml?url=" + (sender as ListBox).SelectedValue, UriKind.RelativeOrAbsolute));
                (sender as ListBox).SelectedIndex = -1; //deselect item
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlurryWP8SDK.Api.LogEvent((this.Pivot.SelectedItem as PivotItem).Header.ToString());
            System.Diagnostics.Debug.WriteLine((this.Pivot.SelectedItem as PivotItem).Header.ToString());
        }

        private void Logout(object sender, EventArgs e)
        {
            Task.Run(async () => { await database.forceLogout();  });
            App.loggedin = false;
            NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}