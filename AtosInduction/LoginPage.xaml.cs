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
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IDictionary<string, string> parameters = this.NavigationContext.QueryString;

            string authEndpoint = parameters["authEndpoint"];
            string clientId = parameters["clientId"];
            string scope = parameters["scope"];

            string uri = string.Format("{0}?response_type=code&client_id={1}&redirect_uri={2}&scope={3}&approval_prompt=auto",
                authEndpoint,
                clientId,
                "urn:ietf:wg:oauth:2.0:oob",
                scope);

            webBrowser.Navigate(new Uri(uri, UriKind.Absolute));
        }

        private async void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            string title = (string)webBrowser.InvokeScript("eval", "document.title.toString()");

            if (title.StartsWith("Success"))
            {
                string authorizationCode = title.Substring(title.IndexOf('=') + 1);
                PhoneApplicationService.Current.State["AtosInduction.AuthorizationCode"] = authorizationCode;
                App.loggedin = true;
                await webBrowser.ClearInternetCacheAsync();
                NavigationService.GoBack();
            }
            else if(title.StartsWith("Denied")) //Consent refused
            {
                NavigationService.GoBack();
            }
        }
    }
}