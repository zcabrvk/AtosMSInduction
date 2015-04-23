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
        private static List<Tab> currentTabList = (Application.Current.Resources["atostabs"] as ObservableCollection<Tab>).ToList<Tab>();

        public PivotMainPage()
        {
            InitializeComponent();
            setName();
        }

        private async void setName()
        {
            string name = await MainPage.database.getUserFullName();

            if (name.CompareTo("") == 0 || !name.All((char c) => { return (char.IsSeparator(c) || char.IsLetter(c)); })) //validates string
                this.UserName.Visibility = System.Windows.Visibility.Collapsed; //query failed, hide ui element
            else
                this.UserName.Text = "Welcome " + name + "!";
        }

        //If the user press the back button exit the app (empty Navigation stack)
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        public static IReadOnlyList<Tab> getTabsIterator()
        {
            return currentTabList.AsReadOnly();
        }

        private void OpenPage(object sender, SelectionChangedEventArgs args)
        {
            NavigationService.Navigate(new Uri("/WebBrowser.xaml?url=" + ((sender as ListBox).SelectedValue as string), UriKind.RelativeOrAbsolute));
            currentTabList = ((sender as ListBox).ItemsSource as ObservableCollection<Tab>).ToList<Tab>();
            (sender as ListBox).SelectedIndex = -1; //deselect item
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlurryWP8SDK.Api.LogEvent((this.Pivot.SelectedItem as PivotItem).Header.ToString());
            System.Diagnostics.Debug.WriteLine((this.Pivot.SelectedItem as PivotItem).Header.ToString());
        }

        private void Logout(object sender, EventArgs e)
        {
            Task.Run(async () => { await MainPage.database.forceLogout();  });
            NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}