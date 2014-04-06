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
    public partial class TrendsPage : PhoneApplicationPage
    {
        public TrendsPage()
        {
            InitializeComponent();

            this.Loaded += TrendsPage_Loaded;
        }

        void TrendsPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.TrendsListSuccess += Instance_TrendsListSuccess;
            FanfouWP.API.FanfouAPI.Instance.TrendsListFailed += Instance_TrendsListFailed;
            FanfouWP.API.FanfouAPI.Instance.TrendsList();
        }

        void Instance_TrendsListFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = Visibility.Collapsed;
                this.toast.NewToast("热门话题获取失败;( " + e.error.error);
            });
        }

        void Instance_TrendsListSuccess(object sender, API.Event.TrendsListEventArgs e)
        {
            Dispatcher.BeginInvoke(()=>{
                this.TrendsListBox.ItemsSource = e.trendsList.trends;
                this.loading.Visibility = Visibility.Collapsed;
            });
        }

        private void TrendsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TrendsListBox.SelectedItem != null)
            {
                var item = this.TrendsListBox.SelectedItem;
                this.TrendsListBox.SelectedItem = null;
                if (PhoneApplicationService.Current.State.ContainsKey("SearchPage"))
                {
                    PhoneApplicationService.Current.State.Remove("SearchPage");
                }
                PhoneApplicationService.Current.State.Add("SearchPage", item);
                NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
            }
        }
    }
}