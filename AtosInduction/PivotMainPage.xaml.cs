using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AtosInduction
{
    class Tab
    {
        public string content { get; private set; }
        public string url { get; private set; }

        public Tab(string content, string url)
        {
            this.content = content;
            this.url = url;
        }
    }

    public partial class PivotMainPage : PhoneApplicationPage
    {
        List<Tab> Atos = new List<Tab>();
        List<Tab> MS = new List<Tab>();

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
        }

        //Google login
        private async void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.loggedin)
            {
                OAuthAuthorization authorization = new OAuthAuthorization(
                "https://accounts.google.com/o/oauth2/auth",
                "https://accounts.google.com/o/oauth2/token");
                TokenPair tokenPair = await authorization.Authorize(
                    "541911829027-6pa6sv71joloj44clqolbc0lojbsj4gj.apps.googleusercontent.com",
                    "8ZiX2Sk1JHItld3KrZH7boD6",
                    new string[] { GoogleScopes.UserinfoEmail });

                // Request a new access token using the refresh token (when the access token was expired)
                TokenPair refreshTokenPair = await authorization.RefreshAccessToken(
                    "541911829027-6pa6sv71joloj44clqolbc0lojbsj4gj.apps.googleusercontent.com",
                    "8ZiX2Sk1JHItld3KrZH7boD6",
                    tokenPair.RefreshToken);
            }
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

        //If the user press the back button exit the app
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private void OpenPage(object sender, SelectionChangedEventArgs args)
        {
            NavigationService.Navigate(new Uri("/WebBrowser.xaml?url=" + ((sender as ListBox).SelectedValue as string), UriKind.RelativeOrAbsolute));

            (sender as ListBox).SelectedIndex = -1; //deselect item
        }

        private void PivotItem_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlurryWP8SDK.Api.LogEvent((this.Pivot.SelectedItem as PivotItem).Header.ToString());
            System.Diagnostics.Debug.WriteLine((this.Pivot.SelectedItem as PivotItem).Header.ToString());
        }

        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}