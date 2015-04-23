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
        private static List<Tab> currentTabList;

        public PivotMainPage()
        {
            InitializeComponent();
            setName();
        }

        private async void setName()
        {
            string key;
            try
            {
                key = await LoginProcess.getAccessToken();
            }
            catch (Exception)
            {
                key = "";
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/plus/v1/people/me?fields=displayName");
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + key;
            string textToDisplay;

            using (HttpWebResponse response = await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK) //successful query!
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string str = streamReader.ReadToEnd();
                        string name;
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(str).TryGetValue("displayName", out name);
                        textToDisplay = "Welcome " + name;
                    }
                }
                else
                    textToDisplay = "";
            }

            this.Name.Text = textToDisplay;
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

            if (((sender as ListBox).Name as string).CompareTo("AtosTabs") == 0)
                currentTabList = (this.Resources["atostabs"] as ObservableCollection<Tab>).ToList<Tab>();
            else
                currentTabList = (this.Resources["mstabs"] as ObservableCollection<Tab>).ToList<Tab>();

            (sender as ListBox).SelectedIndex = -1; //deselect item
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlurryWP8SDK.Api.LogEvent((this.Pivot.SelectedItem as PivotItem).Header.ToString());
            System.Diagnostics.Debug.WriteLine((this.Pivot.SelectedItem as PivotItem).Header.ToString());
        }

        private void Logout(object sender, EventArgs e)
        {
            Task.Run(async () => {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/revoke?token=" + await LoginProcess.getAccessToken());
                await request.GetResponseAsync();
                if (await LoginProcess.isThereTokenFile())
                    LoginProcess.deleteTokenFile();
                });
            NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}