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
        public static IReadOnlyList<Tab> tabs; //set by PivotMainPage when navigating to this page

        public WebBrowser()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarMenuItem item;
            foreach (Tab tab in WebBrowser.tabs) //Add a menu item for each tab 
            {
                item = new ApplicationBarMenuItem();
                item.Text = (string)tab.content;
                item.Click += new EventHandler(Click);
                ApplicationBar.MenuItems.Add(item);
            }
        }

        private void Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem pressed = sender as ApplicationBarMenuItem;
            int i = 0;
            foreach (ApplicationBarMenuItem item in ApplicationBar.MenuItems)
            {
                if (pressed.Text.CompareTo(item.Text) == 0)
                {
                    this.Browser.Navigate(new Uri(WebBrowser.tabs.ElementAt(i).url));
                }
                i++;
            }
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