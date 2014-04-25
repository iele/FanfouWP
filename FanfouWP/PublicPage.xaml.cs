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
using FanfouWP.API;
using FanfouWP.Storage;

namespace FanfouWP
{
    public partial class PublicPage : PhoneApplicationPage
    {
        private dynamic list;
        public PublicPage()
        {
            InitializeComponent();

            this.Loaded += PublicPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["PublicPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("PublicPage_list"))
                this.list = State["PublicPage_list"];

            base.OnNavigatedTo(e);
        }

        void PublicPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.PublicTimelineSuccess += Instance_PublicTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.PublicTimelineFailed += Instance_PublicTimelineFailed;

            if (list != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.PublicStatusListBox.ItemsSource = list;
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });

                return;
            }
            FanfouWP.API.FanfouAPI.Instance.StatusPublicTimeline(SettingManager.GetInstance().defaultCount2 * 10 + 20 * 10 + 20 * 10 + 20);
        }

        void Instance_PublicTimelineFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("时间线获取失败:( " + e.error.error);
            });
        }

        void Instance_PublicTimelineSuccess(object sender, ModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                list = FanfouWP.API.FanfouAPI.Instance.PublicTimeLineStatus;
                this.PublicStatusListBox.ItemsSource = list;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }



        private void PublicStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PublicStatusListBox.SelectedItem != null)
            {
                var item = this.PublicStatusListBox.SelectedItem;
                this.PublicStatusListBox.SelectedIndex = -1;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            FanfouWP.API.FanfouAPI.Instance.StatusPublicTimeline(SettingManager.GetInstance().defaultCount2 * 10 + 20 * 10 + 20);
        }
    }
}