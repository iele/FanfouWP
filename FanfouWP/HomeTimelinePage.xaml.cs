using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FanfouWP
{
    public partial class HomeTimelinePage : PhoneApplicationPage
    {
        public HomeTimelinePage()
        {
            InitializeComponent();
            this.Loaded += HomeTimelinePage_Loaded;
        }

        void HomeTimelinePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.HomeTimelineListBox.ItemsSource = FanfouWP.API.FanfouAPI.Instance.HomeTimeLineStatus;  
        }

        private void HomeTimelineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.HomeTimelineListBox.SelectedItem != null)
            {
                var item = this.HomeTimelineListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }
    }
}