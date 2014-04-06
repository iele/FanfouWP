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
    public partial class ViewerPage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.User user;
        public ViewerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("ViewerPage"))
            {
                this.user = PhoneApplicationService.Current.State["ViewerPage"] as FanfouWP.API.Items.User;

                PhoneApplicationService.Current.State.Remove("ViewerPage");
            }

            this.Loaded += ViewerPage_Loaded;
        }

        void ViewerPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = Visibility.Visible;

                this.title.Text = this.user.screen_name + "的照片流";
            });

            FanfouWP.API.FanfouAPI.Instance.PhotosUserTimelineSuccess += Instance_PhotosUserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.PhotosUserTimelineFailed += Instance_PhotosUserTimelineFailed;
            FanfouWP.API.FanfouAPI.Instance.PhotosUserTimeline(this.user.id);
        }

        void Instance_PhotosUserTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = Visibility.Collapsed;
                this.toast.NewToast("照片流获取失败;( " + e.error.error);
            });
        }

        void Instance_PhotosUserTimelineSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.Status> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = Visibility.Collapsed;
                this.images.ItemsSource = e.UserStatus;
            });
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (images.SelectedItem != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", images.SelectedItem);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
            this.images.SelectedIndex = -1;
        }
    }
}