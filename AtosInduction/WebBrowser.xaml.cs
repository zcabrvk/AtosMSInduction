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
    public partial class WebBrowser : PhoneApplicationPage
    {
        public WebBrowser()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url;

            try
            {
                if (NavigationContext.QueryString.TryGetValue("url", out url))
                    this.Browser.Navigate(new Uri(url));
            }
            catch (Exception)
            {
                NavigationService.GoBack();
            }
        }
    }
}