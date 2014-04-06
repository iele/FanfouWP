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
using FanfouWP.API.Event;
using Microsoft.Phone.Scheduler;
using FanfouWP.Storage;
using System.Collections.ObjectModel;

namespace FanfouWP
{
    internal class PivotDataItem
    {
        public string Title { get; set; }
        public string Count { get; set; }

        public PivotDataItem(string title, string count)
        {
            this.Title = title;
            this.Count = count;
        }
    }

    public partial class TimelinePage : PhoneApplicationPage
    {
        private ObservableCollection<PivotDataItem> pdi = new ObservableCollection<PivotDataItem>();

        private FanfouAPI FanfouAPI = FanfouAPI.Instance;
        private SettingManager setting = SettingManager.GetInstance();

        private bool is_session_restored = false;
        public TimelinePage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage"))
            {
                is_session_restored = true;
                PhoneApplicationService.Current.State.Remove("TimelinePage");
            }

            this.Loaded += TimelinePanorama_Loaded;
            FanfouAPI.HomeTimelineSuccess += FanfouAPI_HomeTimelineSuccess;
            FanfouAPI.HomeTimelineFailed += FanfouAPI_HomeTimelineFailed;
            FanfouAPI.MentionTimelineSuccess += FanfouAPI_MentionTimelineSuccess;
            FanfouAPI.MentionTimelineFailed += FanfouAPI_MentionTimelineFailed;
            FanfouAPI.VerifyCredentialsSuccess += FanfouAPI_VerifyCredentialsSuccess;
            FanfouAPI.VerifyCredentialsFailed += FanfouAPI_VerifyCredentialsFailed;
            FanfouAPI.RestoreDataCompleted += FanfouAPI_RestoreDataCompleted;
        }

        void FanfouAPI_RestoreDataCompleted(object sender, EventArgs e)
        {
            switch (sender as string)
            {
                case "home":
                    this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
                    FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
                    break;
                case "mention":
                    this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;
                    FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
                    break;
            }
        }

        void FanfouAPI_FavoritesDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Collapsed);
            Dispatcher.BeginInvoke(() => { toast.NewToast("取消收藏失败:( " + e.error.error); });
        }

        void FanfouAPI_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Collapsed);
            Dispatcher.BeginInvoke(() => { toast.NewToast("取消收藏成功:）"); });
        }

        void FanfouAPI_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Collapsed);
            Dispatcher.BeginInvoke(() => { toast.NewToast("创建收藏失败:( " + e.error.error); });
        }

        void FanfouAPI_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Collapsed);
            Dispatcher.BeginInvoke(() => { toast.NewToast("创建收藏成功:）"); });
        }

        void FanfouAPI_VerifyCredentialsFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Collapsed);
        }

        void FanfouAPI_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
             {
                 this.TitleControl.DataContext = FanfouAPI.CurrentUser;
                 Toolbox.DataContext = FanfouAPI.CurrentUser;
             });

            AgentWriter.WriteAgentParameter(setting.username, setting.password, setting.oauthToken, setting.oauthSecret);
            StartPeriodicAgent();
        }

        private void FanfouAPI_MentionTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.MentionTimeLineListBox.HideRefreshPanel();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                Dispatcher.BeginInvoke(() => { toast.NewToast("时间线获取失败:( " + e.error.error); });
            });
        }
        private void FanfouAPI_HomeTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.HomeTimeLineListBox.HideRefreshPanel();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                Dispatcher.BeginInvoke(() => { toast.NewToast("时间线获取失败:( " + e.error.error); });
            });
        }


        private void FanfouAPI_MentionTimelineSuccess(object sender, ModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                (this.Pivot.Items[1] as PivotItem).Header = new PivotDataItem(pdi[1].Title, FanfouAPI.MentionTimeLineStatusCount.ToString());

                var count = 1;
                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Back)
                    count = this.MentionTimeLineListBox.ItemsSource.Count;

                this.MentionTimeLineListBox.HideRefreshPanel();
                this.MentionTimeLineListBox.ItemsSource = FanfouAPI.MentionTimeLineStatus;

                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Back)
                    this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[count - 1]);

                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }


        private void FanfouAPI_HomeTimelineSuccess(object sender, ModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                (this.Pivot.Items[0] as PivotItem).Header = new PivotDataItem(pdi[0].Title, FanfouAPI.HomeTimeLineStatusCount.ToString());
                var count = 1;
                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Back)
                    count = this.HomeTimeLineListBox.ItemsSource.Count;

                this.HomeTimeLineListBox.HideRefreshPanel();
                this.HomeTimeLineListBox.ItemsSource = FanfouAPI.HomeTimeLineStatus;

                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Back)
                    this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[count - 1]);

                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });

        }

        void TimelinePanorama_Loaded(object sender, RoutedEventArgs e)
        {
            this.Pivot.Visibility = Visibility.Collapsed;
            pdi = new ObservableCollection<PivotDataItem>();
            pdi.Add(new PivotDataItem("我的消息", "0"));
            pdi.Add(new PivotDataItem("提及我的", "0"));
            pdi.Add(new PivotDataItem("工具箱", "0"));
            (this.Pivot.Items[0] as PivotItem).Header = pdi[0];
            (this.Pivot.Items[1] as PivotItem).Header = pdi[1];
            (this.Pivot.Items[2] as PivotItem).Header = pdi[2];

            this.Pivot.Visibility = Visibility.Visible;
            this.toast.Visibility = Visibility.Visible;
            NavigationService.RemoveBackEntry();

            if (this.is_session_restored)
            {
                this.TitleControl.DataContext = this.FanfouAPI.CurrentUser;
                Toolbox.DataContext = FanfouAPI.CurrentUser;
                FanfouAPI.VerifyCredentials();

                if (FanfouAPI.HomeTimeLineStatus.Count != 0)
                {
                    this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
                    FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
                }
                if (FanfouAPI.MentionTimeLineStatus.Count != 0)
                {
                    this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;
                    FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
                }
            }
            else
            {
                FanfouAPI.StatusHomeTimeline();
                FanfouAPI.StatusMentionTimeline();
                FanfouAPI.VerifyCredentials();

            }

        }

        private void HomeTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", e.AddedItems[0]);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
            this.HomeTimeLineListBox.SelectedItem = null;
        }

        private void MentionTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", e.AddedItems[0]);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
            this.HomeTimeLineListBox.SelectedItem = null;
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);

            FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
            FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoPage.xaml", UriKind.Relative));
        }



        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void HomeTimeLineListBox_RefreshTriggered(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
            FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Behind);
        }

        private void MentionTimeLineListBox_RefreshTriggered(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
            FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Behind);
        }

        private void MentionTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.MentionTimeLineListBox.ItemsSource[this.MentionTimeLineListBox.ItemsSource.Count - 1])
            {

                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                FanfouAPI.StatusMentionTimeline(FanfouAPI.RefreshMode.Back);
                Dispatcher.BeginInvoke(() => { toast.NewToast("正在获取"); });
            }
        }

        private void HomeTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.HomeTimeLineListBox.ItemsSource[this.HomeTimeLineListBox.ItemsSource.Count - 1])
            {
                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                FanfouAPI.StatusHomeTimeline(FanfouAPI.RefreshMode.Back);
                Dispatcher.BeginInvoke(() => { toast.NewToast("正在获取"); });
            }
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            if (FanfouAPI.CurrentUser != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", FanfouAPI.CurrentUser);
                NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PublicPage.xaml", UriKind.Relative));
        }
        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        #region PeriodicTask
        PeriodicTask periodicTask;

        string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = true;
        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        private void StartPeriodicAgent()
        {
            agentsAreEnabled = true;
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            periodicTask.Description = "该后台用于定期获取最新消息通知";

            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {

                }
            }
            catch (SchedulerServiceException)
            {
            }
        }
        #endregion
    }
}