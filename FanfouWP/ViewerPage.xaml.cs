using FanfouWP.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FanfouWP
{
    public partial class ViewerPage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.User user;
        private dynamic list;

        private ToastUtil toast = new ToastUtil();

        public ViewerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("ViewerPage"))
            {
                this.user = PhoneApplicationService.Current.State["ViewerPage"] as FanfouWP.API.Items.User;
            }

            this.Loaded += ViewerPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["ViewerPage_user"] = this.user;
                State["ViewerPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("ViewerPage_user"))
                this.user = State["ViewerPage_user"] as FanfouWP.API.Items.User;
            if (State.ContainsKey("ViewerPage_list"))
                this.list = State["ViewerPage_list"];

            base.OnNavigatedTo(e);
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

            if (list != null)
            {
                Dispatcher.BeginInvoke(() => this.loading.Visibility = Visibility.Collapsed);
                this.images.ItemsSource = list;
                return;
            }
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
                list = e.UserStatus;
                this.images.ItemsSource = list;
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