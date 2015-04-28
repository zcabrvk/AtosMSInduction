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
        private readonly IEnumerator<Tab> tabs = (IEnumerator<Tab>)PhoneApplicationService.Current.State["tabs"];

        public WebBrowser()
        {
            InitializeComponent();

            ApplicationBarMenuItem item;
            while (tabs.MoveNext())  //Add a menu item for each tab 
            {
                Tab tab = tabs.Current;
                item = new ApplicationBarMenuItem();
                item.Text = tab.content;
                item.Click += new EventHandler(Click);
                ApplicationBar.MenuItems.Add(item);
            }
            tabs.Reset();
        }

        private void Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem pressed = sender as ApplicationBarMenuItem;
            foreach (ApplicationBarMenuItem item in ApplicationBar.MenuItems)
            {
                tabs.MoveNext();
                if (pressed.Text.CompareTo(item.Text) == 0)
                {
                    this.Browser.Navigate(new Uri(tabs.Current.url));
                }
            }
            tabs.Reset();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url;
            try
            {
                NavigationContext.QueryString.TryGetValue("url", out url);
                this.Browser.Navigate(new Uri(url));
            }
            catch (Exception)
            {
                NavigationService.GoBack();
            }
        }
    }
}