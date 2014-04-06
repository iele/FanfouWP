﻿using System;
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
        public TagPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("TagPage"))
            {
                user = (PhoneApplicationService.Current.State["TagPage"] as FanfouWP.API.Items.User);
                PhoneApplicationService.Current.State.Remove("TagPage");
            }

            this.Loaded += TagPage_Loaded;
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
                this.toast.NewToast("用户列表获取失败:( "  + e.error.error);
            });
        }

        void Instance_TaggedSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.ListBox.ItemsSource = e.UserStatus;
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
                this.TagPicker.ItemsSource = e.Result;
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