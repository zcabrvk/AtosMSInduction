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
        public HomeScreen()
        {
            InitializeComponent();
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

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MSInduction.xaml", UriKind.Relative));
        }
    }
}