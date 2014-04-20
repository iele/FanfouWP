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
    public partial class TagPage : PhoneApplicationPage
    {
        private API.Items.User user;
        private dynamic picker;
        private int currentIndex;
        private dynamic list;
        public TagPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("TagPage"))
            {
                user = (PhoneApplicationService.Current.State["TagPage"] as FanfouWP.API.Items.User);
                if (user.id != FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                    this.title.Text = user.screen_name + "的标签";
            }

            this.Loaded += TagPage_Loaded;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["TagPage_user"] = this.user;
                State["TagPage_list"] = this.list;
                State["TagPage_picker"] = this.picker;
                State["TagPage_currentIndex"] = this.TagPicker.SelectedIndex;
            }

            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("TagPage_user"))
            {
                this.user = State["TagPage_user"] as FanfouWP.API.Items.User;
                if (user.id != FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                    this.title.Text = user.screen_name + "的标签";
            }
            if (State.ContainsKey("TagPage_list"))
            {
                this.list = State["TagPage_list"];
                this.ListBox.ItemsSource = list;
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
            if (State.ContainsKey("TagPage_picker"))
            {
                this.picker = State["TagPage_picker"];
                this.TagPicker.ItemsSource = picker;
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
            if (State.ContainsKey("TagPage_currentIndex"))
            {
                this.currentIndex = (int)State["TagPage_currentIndex"];
                this.TagPicker.SelectedIndex = currentIndex;
            }
            base.OnNavigatedTo(e);
        }
        void TagPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.TagListSuccess += Instance_TagListSuccess;
            FanfouWP.API.FanfouAPI.Instance.TagListFailed += Instance_TagListFailed;

            FanfouWP.API.FanfouAPI.Instance.TaggedSuccess += Instance_TaggedSuccess;
            FanfouWP.API.FanfouAPI.Instance.TaggedFailed += Instance_TaggedFailed;
            this.TagPicker.SelectionChanged += TagPicker_SelectionChanged;
            FanfouWP.API.FanfouAPI.Instance.TaggedList(user.id);
        }

        void Instance_TaggedFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("用户列表获取失败:( " + e.error.error);
            });
        }

        void Instance_TaggedSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                list = e.UserStatus;
                this.ListBox.ItemsSource = list;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void Instance_TagListFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("标签列表获取失败:( " + e.error.error);
            });
        }

        void Instance_TagListSuccess(object sender, API.Event.ListEventArgs<string> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                picker = e.Result;
                this.TagPicker.ItemsSource = picker;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        private void TagPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tag = ((sender as ListPicker).SelectedItem as string);
            if (tag != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                });
                FanfouWP.API.FanfouAPI.Instance.Tagged(tag);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.ListBox.SelectedItem as FanfouWP.API.Items.User;
            this.ListBox.SelectedIndex = -1;
            if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
            {
                PhoneApplicationService.Current.State.Remove("UserPage");
            }
            PhoneApplicationService.Current.State.Add("UserPage", item);
            App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
        }


    }
}