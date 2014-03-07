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

namespace FanfouWP
{
    public partial class TimelinePanorama : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;

        public TimelinePanorama()
        {
            InitializeComponent();

            this.Loaded += TimelinePanorama_Loaded;
            FanfouAPI.HomeTimelineSuccess += FanfouAPI_HomeTimelineSuccess;
            FanfouAPI.HomeTimelineFailed += FanfouAPI_HomeTimelineFailed;
            FanfouAPI.MentionTimelineSuccess += FanfouAPI_MentionTimelineSuccess;
            FanfouAPI.PublicTimelineFailed += FanfouAPI_MentionTimelineFailed;
            FanfouAPI.VerifyCredentialsSuccess += FanfouAPI_VerifyCredentialsSuccess;
            FanfouAPI.VerifyCredentialsFailed += FanfouAPI_VerifyCredentialsFailed;
        }

        void FanfouAPI_VerifyCredentialsFailed(object sender, API.Event.FailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void FanfouAPI_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
             {
                 this.Panorama.DataContext = FanfouAPI.CurrentUser;
             });
        }

        private void FanfouAPI_MentionTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FanfouAPI_MentionTimelineSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.MentionTimeLineListBox.ItemsSource = FanfouAPI.MentionTimeLineStatus;
            });

        }

        private void FanfouAPI_HomeTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            throw new NotImplementedException();
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
            FanfouAPI.StatusHomeTimeline();
            FanfouAPI.StatusMentionTimeline();
            FanfouAPI.VerifyCredentials();
        }
    }
}