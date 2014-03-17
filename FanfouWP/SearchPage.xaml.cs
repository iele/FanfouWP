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
    public partial class SearchPage : PhoneApplicationPage
    {
        private string keyword = "";
        public SearchPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("SearchPage"))
            {
                keyword = (PhoneApplicationService.Current.State["SearchPage"] as FanfouWP.API.Items.Trends).query;
                PhoneApplicationService.Current.State.Remove("SearchPage");
            }
      
            this.Loaded += SearchPage_Loaded;
        }

        void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.SearchText.Text = keyword;
            });

            FanfouWP.API.FanfouAPI.Instance.SearchTimelineSuccess += Instance_SearchTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchTimelineFailed += Instance_SearchTimelineFailed;
        }

        void Instance_SearchTimelineFailed(object sender, FailedEventArgs e)
        {

        }

        void Instance_SearchTimelineSuccess(object sender, UserTimelineEventArgs<Status> e)
        {
            Dispatcher.BeginInvoke(() => {
                this.SearchStatusListBox.ItemsSource = e.UserStatus;            
            });
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (SearchText.Text.Count() != 0)
            {
                FanfouWP.API.FanfouAPI.Instance.SearchTimeline(this.SearchText.Text);
            }
        }

        private void SearchStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SearchStatusListBox.SelectedItem != null)
            {
                var item = this.SearchStatusListBox.SelectedItem;

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