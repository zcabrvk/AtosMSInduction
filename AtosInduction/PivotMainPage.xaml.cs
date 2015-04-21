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

namespace AtosInduction
{
    public class Tab
    {
        public string content { get; private set; }
        public string url { get; private set; }

        public Tab(string content, string url)
        {
            this.content = content;
            this.url = url;
        }
    }

    class Name
    {
        [JsonProperty("displayName")]
        public readonly string name;

        public Name(string name)
        {
            this.name = name;
        }
    }

    public partial class PivotMainPage : PhoneApplicationPage
    {
        private List<Tab> Atos = new List<Tab>();
        private List<Tab> MS = new List<Tab>();

        public PivotMainPage()
        {
            InitializeComponent();

            //add Atos list items with urls
            addAtosListItem("Home", "http://atos.net");
            addAtosListItem("Who Are Atos?", "http://atos.net/en-us/home/we-are.html");
            addAtosListItem("Where do I fit?", "http://atos.net/en-us/home/we-do/application-management.html");
            addAtosListItem("Atos customers", "http://atos.net/en-us/home/we-are/our-customers.html");
            addAtosListItem("Company structure", "http://atos.net/en-us/home/we-are/company-profile.html");
            addAtosListItem("Operational targets", "http://atos.net/en-us/home/we-are/company-profile/corporate-values.html");
            addAtosListItem("Where next?", "http://atos.net/en-us/home/we-are/insights-innovation.html");

            //add MS list items with urls
            addMSListItem("Home", "http://atos.net/en-us/home/your-business/utilities/managed-services-for-utilities.html");
            addMSListItem("Structure", "http://atos.net/en-us/home/your-business/manufacturing.html");
            addMSListItem("Operational Targets", "http://atos.net/en-us/home/your-business/telecommunications/telecom-managed-operations.html");
            addMSListItem("Essentials", "http://atos.net/en-us/home/we-do/business-integration-solutions.html");
            addMSListItem("Departement Induction", "http://atos.net/en-us/home/we-do/project-services.html");
            addMSListItem("Where Next?", "http://atos.net/en-us/home/olympic-games.html");

            this.AtosTabs.ItemsSource = Atos;
            this.AtosTabs.DisplayMemberPath = "content";
            this.AtosTabs.SelectedValuePath = "url";
            this.MSTabs.ItemsSource = MS;
            this.MSTabs.DisplayMemberPath = "content";
            this.MSTabs.SelectedValuePath = "url";

            setName();
        }

        private async void setName()
        {
            string key;
            try
            {
                key = await LoginScreen.getAccessToken();
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
                        textToDisplay = "Welcome " + JsonConvert.DeserializeObject<Name>(str).name;
                    }
                }
                else
                    textToDisplay = "";
            }

            this.Name.Text = textToDisplay;
        }

        private void addAtosListItem(string content, string url)
        {
            Tab item = new Tab(content, url);
            this.Atos.Add(item);
        }

        private void addMSListItem(string content, string url)
        {
            Tab item = new Tab(content, url);
            this.MS.Add(item);
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
            NavigationService.Navigate(new Uri("/WebBrowser.xaml?url=" + ((sender as ListBox).SelectedValue as string), UriKind.RelativeOrAbsolute));

            if (((sender as ListBox).Name as string).CompareTo("AtosTabs") == 0)
                WebBrowser.tabs = this.Atos.AsReadOnly();
            else
                WebBrowser.tabs = this.MS.AsReadOnly();

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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/revoke?token=" + await LoginScreen.getAccessToken());
                await request.GetResponseAsync();
                if (await LoginScreen.isThereTokenFile())
                    LoginScreen.deleteTokenFile();
                });
            NavigationService.Navigate(new Uri("/LoginScreen.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}