﻿using FanfouWP.API.Event;
using FanfouWP.Storage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FanfouWP
{
    public partial class FriendshipPage : PhoneApplicationPage
    {
        private API.Items.User user;
        public FriendshipPage()
        {
            InitializeComponent();
            if (PhoneApplicationService.Current.State.ContainsKey("FriendshipPage"))
            {
                user = PhoneApplicationService.Current.State["FriendshipPage"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("FriendshipPage");
            }
            this.Loaded += UserPage_Loaded;

            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateSuccess += Instance_FriendshipsCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateFailed += Instance_FriendshipsCreateFailed;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroySuccess += Instance_FriendshipsDestroySuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroyFailed += Instance_FriendshipsDestroyFailed;
        }


        void Instance_FriendshipsDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("取消好友失败:( " + e.error.error); });
        }

        void Instance_FriendshipsDestroySuccess(object sender, EventArgs e)
        {
            user = sender as FanfouWP.API.Items.User;
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            checkMenu();
            Dispatcher.BeginInvoke(() => { toast.NewToast("取消好友成功.. "); });
        }

        void Instance_FriendshipsCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("创建好友失败:( " + e.error.error); });
        }

        void Instance_FriendshipsCreateSuccess(object sender, EventArgs e)
        {
            user = sender as FanfouWP.API.Items.User;
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            checkMenu();
            Dispatcher.BeginInvoke(() => { toast.NewToast("创建好友成功:)"); });
        }

        void UserPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = user;
            checkMenu();
        }

        protected void checkMenu()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (this.user.id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                {
                    (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
                    return;
                }
                if (user.following)
                    (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "解除好友";
                else
                    (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "添加好友";

                if (this.user.id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
                {
                    (this.ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).IsEnabled = false;
                    return;
                }

            });
        }

        private void FriendMenu_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            if (user.following)
            {
                FanfouWP.API.FanfouAPI.Instance.FriendshipDestroy(user.id);
            }
            else
                FanfouWP.API.FanfouAPI.Instance.FriendshipCreate(user.id);
        }
        void Instance_FriendshipsAcceptFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("接受好友请求失败:(" + e.error.error);
            });
        }

        void Instance_FriendshipsAcceptSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("已成功接受好友请求:)");
            });
        }

        void Instance_FriendshipsDenyFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("拒绝好友请求失败:(" + e.error.error);
            });
        }

        void Instance_FriendshipsDenySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("已成功拒绝好友请求...");
            });
        }
        private void DenyButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            FanfouWP.API.FanfouAPI.Instance.FriendshipDeny(user.id);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            FanfouWP.API.FanfouAPI.Instance.FriendshipAccept(user.id);
        }

    }
}