using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.API;
using FanfouWP.API.Event;
using Microsoft.Phone.Scheduler;
using FanfouWP.Storage;
using System.Collections.ObjectModel;
using System.Windows.Threading;

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
        private bool run_once = true;
        public TimelinePage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage"))
            {
                is_session_restored = true;
            }

            this.Loaded += TimelinePanorama_Loaded;

            FanfouAPI.HomeTimelineSuccess += FanfouAPI_HomeTimelineSuccess;
            FanfouAPI.HomeTimelineFailed += FanfouAPI_HomeTimelineFailed;
            FanfouAPI.MentionTimelineSuccess += FanfouAPI_MentionTimelineSuccess;
            FanfouAPI.MentionTimelineFailed += FanfouAPI_MentionTimelineFailed;
            FanfouAPI.AccountNotificationSuccess += FanfouAPI_AccountNotificationSuccess;
            FanfouAPI.AccountNotificationFailed += FanfouAPI_AccountNotificationFailed;
        }

        void AvatarImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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

        void FanfouAPI_AccountNotificationFailed(object sender, FailedEventArgs e)
        {
        }

        void FanfouAPI_AccountNotificationSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if ((sender as FanfouWP.API.Items.Notifications).direct_messages != 0)
                    this.Toolbox.DirectMsgTile.Notification = (sender as FanfouWP.API.Items.Notifications).direct_messages + "条新私信";

            });
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
                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Behind)
                    (this.Pivot.Items[1] as PivotItem).Header = new PivotDataItem(pdi[1].Title, FanfouAPI.MentionTimeLineStatusCount.ToString());

                if (e.RefreshMode == API.FanfouAPI.RefreshMode.New && this.FanfouAPI.MentionTimeLineStatus.Count != 0)
                { 
                    this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;
                    this.MentionTimeLineListBox.ScrollTo(FanfouAPI.MentionTimeLineStatus.First());
                }
                this.MentionTimeLineListBox.HideRefreshPanel();

                this.loading.Visibility = System.Windows.Visibility.Collapsed;

                var item = ShellTile.ActiveTiles.First();
                var data = new IconicTileData();
                data.Title = "饭窗";
                data.WideContent1 = FanfouAPI.CurrentUser.screen_name;
                data.WideContent2 = "三围 " + FanfouAPI.CurrentUser.statuses_count + " " + FanfouAPI.CurrentUser.followers_count + " " + FanfouAPI.CurrentUser.friends_count;
                if (FanfouAPI.MentionTimeLineStatus != null && FanfouAPI.MentionTimeLineStatus.Count > 0)
                    data.WideContent3 = "最近提及 " + FanfouAPI.MentionTimeLineStatus.First().user.screen_name;
                else
                    data.WideContent3 = "最近提及 无";
                item.Update(data);
            });
        }


        private void FanfouAPI_HomeTimelineSuccess(object sender, ModeEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.RefreshMode == API.FanfouAPI.RefreshMode.Behind)
                    (this.Pivot.Items[0] as PivotItem).Header = new PivotDataItem(pdi[0].Title, FanfouAPI.HomeTimeLineStatusCount.ToString());

                if (e.RefreshMode == API.FanfouAPI.RefreshMode.New&& this.FanfouAPI.HomeTimeLineStatus.Count != 0)
                { 
                    this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;                  
                      this.HomeTimeLineListBox.ScrollTo(FanfouAPI.HomeTimeLineStatus.First());
                }
                this.HomeTimeLineListBox.HideRefreshPanel();

                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });

        }

        void TimelinePanorama_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += (s, et) =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                FanfouAPI.StatusHomeTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind);
                FanfouAPI.StatusMentionTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind);
            };
            timer.Start();
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
            this.MentionTimeLineListBox.SelectedItem = null;
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);

            Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind));
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind));
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Image"))
            {
                PhoneApplicationService.Current.State.Remove("SendPage_Image");
            }
            PhoneApplicationService.Current.State.Add("SendPage_Image", true);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));

        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void HomeTimeLineListBox_RefreshTriggered(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind));
        }

        private void MentionTimeLineListBox_RefreshTriggered(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Behind));
        }

        private void MentionTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.MentionTimeLineListBox.ItemsSource[this.MentionTimeLineListBox.ItemsSource.Count - 1] && !FanfouAPI.MentionTimeLineEnded)
            {

                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Back));
                Dispatcher.BeginInvoke(() => { toast.NewToast("正在回溯时间线"); });
            }
        }

        private void HomeTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.HomeTimeLineListBox.ItemsSource[this.HomeTimeLineListBox.ItemsSource.Count - 1] && !FanfouAPI.HomeTimeLineEnded)
            {
                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                FanfouAPI.StatusHomeTimeline(setting.defaultCount, FanfouAPI.RefreshMode.Back);
                Dispatcher.BeginInvoke(() => { toast.NewToast("正在回溯时间线"); });
            }
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelfPage.xaml", UriKind.Relative));

        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PublicPage.xaml", UriKind.Relative));
        }
        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_4(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RequestPage.xaml", UriKind.Relative));
        }

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
                //                ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(60));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (run_once == true)
            {
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

                    if (FanfouAPI.HomeTimeLineStatus.Count != 0)
                    {
                        this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
                        this.HomeTimeLineListBox.ScrollTo(FanfouAPI.HomeTimeLineStatus.First());
                    }
                    Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount));
                    if (FanfouAPI.MentionTimeLineStatus.Count != 0)
                    {
                        this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;
                        this.MentionTimeLineListBox.ScrollTo(FanfouAPI.MentionTimeLineStatus.First());
                    }
                    Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount));
                }
                else
                {     
                    Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount));
                    Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount));
                }
            }
            else
            {
                this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
                this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;

                Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount));
                Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount));
            }

            Dispatcher.BeginInvoke(() =>
            {
                this.TitleControl.DataContext = FanfouAPI.CurrentUser;
                Toolbox.DataContext = FanfouAPI.CurrentUser;
            });
            FanfouAPI.AccountNotification();

            AgentWriter.WriteAgentParameter(setting.username, setting.password, setting.oauthToken, setting.oauthSecret, setting.backgroundFeq);
            StartPeriodicAgent();
            run_once = false;

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (setting.quit_confirm == true)
            {
                if (MessageBox.Show("是否退出饭窗?", "确认退出?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            base.OnBackKeyPress(e);
        }


        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot.SelectedIndex == 2)
                this.Toolbox.GroupTileAnimationEnable(true);
            else
                this.Toolbox.GroupTileAnimationEnable(false);
        }

        private void HomeTimeLineListBox_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            (this.Pivot.Items[0] as PivotItem).Header = new PivotDataItem(pdi[0].Title, "0");
        }

        private void MentionTimeLineListBox_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            (this.Pivot.Items[1] as PivotItem).Header = new PivotDataItem(pdi[1].Title, "0");
        }

        private void FanfouImage_Click(object sender, RoutedEventArgs e)
        {
            switch (Pivot.SelectedIndex)
            {
                case 0:
                    if (this.HomeTimeLineListBox.ItemsSource != null && this.HomeTimeLineListBox.ItemsSource.Count != 0)
                        this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[0]);
                    break;
                case 1:
                    if (this.MentionTimeLineListBox.ItemsSource != null && this.MentionTimeLineListBox.ItemsSource.Count != 0)
                        this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[0]);
                    break;
                default:
                    break;

            }
        }
    }
}