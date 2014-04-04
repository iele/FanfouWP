using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.API.Items;
using FanfouWP.API.Event;

namespace FanfouWP
{
    public partial class PublicPage : PhoneApplicationPage
    {
        public PublicPage()
        {
            InitializeComponent();

            this.Loaded += PublicPage_Loaded;
        }

        void PublicPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.StatusPublicTimeline();
            FanfouWP.API.FanfouAPI.Instance.PublicTimelineSuccess += Instance_PublicTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.PublicTimelineFailed += Instance_PublicTimelineFailed;
        }

        void Instance_PublicTimelineFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void Instance_PublicTimelineSuccess(object sender, ModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.PublicStatusListBox.ItemsSource = FanfouWP.API.FanfouAPI.Instance.PublicTimeLineStatus;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

     

        private void PublicStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PublicStatusListBox.SelectedItem != null)
            {
                var item = this.PublicStatusListBox.SelectedItem;

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