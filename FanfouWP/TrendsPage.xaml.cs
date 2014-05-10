using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class TrendsPage : PhoneApplicationPage
    {
        private dynamic list;

        private ToastUtil toast = new ToastUtil();

        public TrendsPage()
        {
            InitializeComponent();

            this.Loaded += TrendsPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["TrendsPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("TrendsPage_list"))
                this.list = State["TrendsPage_list"];

            base.OnNavigatedTo(e);
        }
        void TrendsPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.TrendsListSuccess += Instance_TrendsListSuccess;
            FanfouWP.API.FanfouAPI.Instance.TrendsListFailed += Instance_TrendsListFailed;

            if (list != null)
            {
                this.TrendsListBox.ItemsSource = list;
                return;
            }

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
            Dispatcher.BeginInvoke(() =>
            {
                list = e.trendsList.trends;
                this.TrendsListBox.ItemsSource = list;
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