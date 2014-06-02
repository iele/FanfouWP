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
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class NoticePage : PhoneApplicationPage
    {
        private dynamic keyword_list;

        private Toast toast = new Toast();

        public NoticePage()
        {
            InitializeComponent();

            this.Loaded += SearchPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["NoticePage_keyword_list"] = this.keyword_list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("NoticePage_keyword_list"))
                this.keyword_list = State["NoticePage_keyword_list"];

            base.OnNavigatedTo(e);
        }

        void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineSuccess += Instance_SearchUserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineFailed += Instance_SearchUserTimelineFailed;

            Dispatcher.BeginInvoke(() =>
            {
                if (keyword_list != null)
                {
                    this.SearchStatusListBox.ItemsSource = this.keyword_list;
                }
                else
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline("饭窗公告", "fanwp");
                }

            });

        }

        void Instance_SearchUserTimelineFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("公告获取失败:( " + e.error.error);
            });
        }

        void Instance_SearchUserTimelineSuccess(object sender, UserTimelineEventArgs<Status> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.keyword_list = e.UserStatus;
                this.SearchStatusListBox.ItemsSource = keyword_list;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
             });
        }
    }
}