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

namespace AtosInduction
{
    public partial class HomeScreen : PhoneApplicationPage
    {
        private List<string> buttons = new List<String>();
        private List<string> pages = new List<String>();

        public HomeScreen()
        {
            InitializeComponent();

            //add menu elements specifying text, subtitle and page they navigate to
            addMenuItem("About Atos", "Learn more about the Company", "/AtosTabs.xaml");
            addMenuItem("Managed Services", "Learn More About Our Managed Services", "/MSInduction.xaml");
            //addMenuItem("How to Use this App", "A brief on how to use this App", "/MSInduction.xaml"); to be implemented???
        }

        private void addMenuItem(string titleText, string subTitleText, string page)
        {
            TextBlock title = new TextBlock();
            TextBlock subTitle = new TextBlock();
            StackPanel container = new StackPanel();

            title.Text = titleText;
            subTitle.Text = subTitleText;

            title.TextWrapping = TextWrapping.Wrap;
            subTitle.TextWrapping = TextWrapping.Wrap;

            Thickness margin = subTitle.Margin;
            margin.Left = 12;
            margin.Top = -6;
            margin.Right = 12;
            subTitle.Margin = margin;

            title.Style = (Style)App.Current.Resources["PhoneTextExtraLargeStyle"];
            subTitle.Style = (Style)App.Current.Resources["PhoneTextSubtleStyle"];

            container.Children.Add(title);
            container.Children.Add(subTitle);

            margin = container.Margin;
            margin.Top = 10;
            container.Margin = margin;
            container.Name = titleText;
            this.buttons.Add(titleText);
            this.pages.Add(page);
            container.Tap += Clicked;

            this.ContentPanel.Children.Add(container);
        }

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

        //If the user press the back button exit the app
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private void Clicked(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel selected = sender as StackPanel;
            string uri = this.pages[this.buttons.IndexOf(selected.Name)];

            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}