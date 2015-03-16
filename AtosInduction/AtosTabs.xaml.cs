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
    public partial class AtosTabs : PhoneApplicationPage
    {
        // Constructor
        public AtosTabs()
        {
            InitializeComponent();

            BuildLocalizedApplicationBar();
        }

        private void Panorama_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarMenuItem item;
            foreach (PanoramaItem tab in this.AtosPanorama.Items) //Add a menu item for each tab in the panorama page
            {
                item = new ApplicationBarMenuItem();
                item.Text = (string)tab.Header;
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
                    this.AtosPanorama.DefaultItem = this.AtosPanorama.Items[i];
                }
                i++;
            }
        }
    }
}