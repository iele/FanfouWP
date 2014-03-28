using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.API;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using FanfouWP.Utils;
using System.Threading.Tasks;
using FanfouWP.API.Items;
using Microsoft.Phone.Tasks;

namespace FanfouWP
{
    public partial class TimelinePage : PhoneApplicationPage
    {

        private FanfouAPI FanfouAPI = FanfouAPI.Instance;

        public TimelinePage()
        {
            InitializeComponent();

            this.Loaded += TimelinePanorama_Loaded;
            FanfouAPI.HomeTimelineSuccess += FanfouAPI_HomeTimelineSuccess;
            FanfouAPI.HomeTimelineFailed += FanfouAPI_HomeTimelineFailed;
            FanfouAPI.MentionTimelineSuccess += FanfouAPI_MentionTimelineSuccess;
            FanfouAPI.MentionTimelineFailed +=FanfouAPI_MentionTimelineFailed;
            FanfouAPI.VerifyCredentialsSuccess += FanfouAPI_VerifyCredentialsSuccess;
            FanfouAPI.VerifyCredentialsFailed += FanfouAPI_VerifyCredentialsFailed;

        }

        void FanfouAPI_FavoritesDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void FanfouAPI_FavoritesDestroySuccess(object sender, EventArgs e)
        {
        }

        void FanfouAPI_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void FanfouAPI_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            Toolbox.DataContext = FanfouAPI.CurrentUser;
        }

        void FanfouAPI_VerifyCredentialsFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void FanfouAPI_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
             {
                 this.Pivot.DataContext = FanfouAPI.CurrentUser;
             });
        }

        private void FanfouAPI_MentionTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
        }
        private void FanfouAPI_HomeTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
        }


        private void FanfouAPI_MentionTimelineSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.MentionTimeLineListBox.ItemsSource = FanfouAPI.MentionTimeLineStatus;
            });
        }


        private void FanfouAPI_HomeTimelineSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.HomeTimeLineListBox.ItemsSource = FanfouAPI.HomeTimeLineStatus;
            });

        }

        void TimelinePanorama_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.FanfouAPI.HomeTimeLineStatus.Count != 0  && this.FanfouAPI.MentionTimeLineStatus.Count != 0)
            {
                FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
                FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
                FanfouAPI.VerifyCredentials();
            }
            else {
                FanfouAPI.StatusHomeTimeline();
                FanfouAPI.StatusMentionTimeline();
                FanfouAPI.VerifyCredentials();
     
            }

        }

        private void HomeTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.HomeTimeLineListBox.SelectedItem != null)
            {
                var item = this.HomeTimeLineListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void MentionTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MentionTimeLineListBox.SelectedItem != null)
            {
                var item = this.MentionTimeLineListBox.SelectedItem;

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
            FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
            FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoPage.xaml",UriKind.Relative));
        }

       

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }
    }
}