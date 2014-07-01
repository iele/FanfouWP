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
using Coding4Fun.Toolkit.Controls;
using FanfouWP.API.Items;
using FanfouWP.Utils;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading;

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

        private DispatcherTimer timer;

        private Toast toast = new Toast();

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
            FanfouAPI.AccountNotificationSuccess += FanfouAPI_AccountNotificationSuccess;
            FanfouAPI.AccountNotificationFailed += FanfouAPI_AccountNotificationFailed;
            FanfouAPI.VerifyCredentialsSuccess += FanfouAPI_VerifyCredentialsSuccess;
            FanfouAPI.VerifyCredentialsFailed += FanfouAPI_VerifyCredentialsFailed;
        }

        void FanfouAPI_VerifyCredentialsFailed(object sender, FailedEventArgs e)
        {
        }

        void FanfouAPI_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
        }

        void AvatarImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AccountsPage.xaml", UriKind.Relative));
        }

        void FanfouAPI_AccountNotificationFailed(object sender, FailedEventArgs e)
        {
        }

        void FanfouAPI_AccountNotificationSuccess(object sender, EventArgs e)
        {
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
                {
                    pdi[1].Count = (int.Parse(pdi[1].Count) + FanfouAPI.MentionTimeLineStatusCount).ToString();
                    (this.Pivot.Items[1] as PivotItem).Header = new PivotDataItem(pdi[1].Title, pdi[1].Count);
                }
                if ((e.RefreshMode == API.FanfouAPI.RefreshMode.New && this.FanfouAPI.MentionTimeLineStatus.Count != 0) || (this.MentionTimeLineListBox.ItemsSource == null || this.MentionTimeLineListBox.ItemsSource.Count == 0))
                {
                    this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;
                    if (this.MentionTimeLineListBox.ItemsSource != null && this.MentionTimeLineListBox.ItemsSource.Count != 0)
                        this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[0]);
                }

                if (setting.alwaysTop && this.MentionTimeLineListBox.ItemsSource != null && this.MentionTimeLineListBox.ItemsSource.Count != 0 && (e.RefreshMode == API.FanfouAPI.RefreshMode.Behind || e.RefreshMode == API.FanfouAPI.RefreshMode.New))
                {
                    this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[0]);
                }

                this.MentionTimeLineListBox.HideRefreshPanel();

                this.loading.Visibility = System.Windows.Visibility.Collapsed;

                var item = ShellTile.ActiveTiles.First();
                var data = new IconicTileData();
                data.Title = "饭窗";
                data.Count = 0;
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
                {
                    pdi[0].Count = (int.Parse(pdi[0].Count) + FanfouAPI.HomeTimeLineStatusCount).ToString();
                    (this.Pivot.Items[0] as PivotItem).Header = new PivotDataItem(pdi[0].Title, pdi[0].Count);
                }

                if ((e.RefreshMode == API.FanfouAPI.RefreshMode.New && this.FanfouAPI.HomeTimeLineStatus.Count != 0) || (this.HomeTimeLineListBox.ItemsSource == null || this.HomeTimeLineListBox.ItemsSource.Count == 0))
                {
                    this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
                    if (this.HomeTimeLineListBox.ItemsSource != null && this.HomeTimeLineListBox.ItemsSource.Count != 0)
                        this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[0]);
                }

                if (setting.alwaysTop && this.HomeTimeLineListBox.ItemsSource != null && this.HomeTimeLineListBox.ItemsSource.Count != 0 && (e.RefreshMode == API.FanfouAPI.RefreshMode.Behind || e.RefreshMode == API.FanfouAPI.RefreshMode.New))
                {
                    this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[0]);
                }

                this.HomeTimeLineListBox.HideRefreshPanel();

                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });

        }


        void TimelinePanorama_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (nav_mode == NavigationMode.New && run_once == true && this.is_session_restored)
                {
                    FanfouAPI.VerifyCredentials();
                    FanfouAPI.AccountNotification();

                    if (this.HomeTimeLineListBox.ItemsSource != null && this.HomeTimeLineListBox.ItemsSource.Count != 0)
                    {
                        this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[0]);
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, API.FanfouAPI.RefreshMode.Behind));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20));
                    }
                    if (this.MentionTimeLineListBox.ItemsSource != null && this.MentionTimeLineListBox.ItemsSource.Count != 0)
                    {
                        this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[0]);
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, API.FanfouAPI.RefreshMode.Behind));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20));
                    }
                }
                else
                {
                    if (FanfouAPI.HomeTimeLineStatus.Count != 0)
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouWP.API.FanfouAPI.RefreshMode.Behind));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20));
                    }
                    if (FanfouAPI.HomeTimeLineStatus.Count != 0)
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouWP.API.FanfouAPI.RefreshMode.Behind));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20));
                    }
                }
            });
            Dispatcher.BeginInvoke(() =>
            {
                AgentWriter.WriteAgentParameter(setting.username, setting.password, setting.oauthToken, setting.oauthSecret, setting.backgroundFeq);
                Utils.ScheduledTask.StartPeriodicAgent();
                run_once = false;

                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    var time = 1;
                    switch (setting.refreshFreq)
                    {
                        case 0:
                            time = 1;
                            break;
                        case 1:
                            time = 2;
                            break;
                        case 2:
                            time = 5;
                            break;
                        case 3:
                            time = 10;
                            break;
                        case 4:
                            return;
                        default:
                            break;
                    }
                    timer.Interval = TimeSpan.FromSeconds(time * 60);
                    timer.Tick += (s, et) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            this.loading.Visibility = System.Windows.Visibility.Visible;
                            FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind);
                            FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind);
                        });
                    };
                    timer.Start();
                }
            });
        }

        private void HomeTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                if ((e.AddedItems[0] as Status).is_refresh == true)
                {
                    var id = (e.AddedItems[0] as Status).id;
                    var from = "";
                    var to = "";
                    if (this.FanfouAPI.HomeTimeLineStatus.Count >= 3)
                    {
                        for (int i = 1; i < this.FanfouAPI.HomeTimeLineStatus.Count - 1; i++)
                        {
                            if (id == this.FanfouAPI.HomeTimeLineStatus[i].id && this.FanfouAPI.HomeTimeLineStatus[i].is_refresh)
                            {
                                to = this.FanfouAPI.HomeTimeLineStatus[i - 1].id;
                                from = this.FanfouAPI.HomeTimeLineStatus[i + 1].id;
                                break;
                            }
                        }
                        if (from != "" && to != "")
                        {
                            this.loading.Visibility = System.Windows.Visibility.Visible;
                            FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Center, from, to, id);
                        }
                    }
                }
                else
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                    {
                        PhoneApplicationService.Current.State.Remove("StatusPage");
                    }
                    PhoneApplicationService.Current.State.Add("StatusPage", e.AddedItems[0]);
                    NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
                }
            }
            this.HomeTimeLineListBox.SelectedItem = null;
        }

        private void MentionTimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                if ((e.AddedItems[0] as Status).is_refresh == true)
                {
                    var id = (e.AddedItems[0] as Status).id;
                    var from = "";
                    var to = "";
                    if (this.FanfouAPI.MentionTimeLineStatus.Count >= 3)
                    {
                        for (int i = 1; i < this.FanfouAPI.MentionTimeLineStatus.Count - 1; i++)
                        {
                            if (id == this.FanfouAPI.MentionTimeLineStatus[i].id && this.FanfouAPI.MentionTimeLineStatus[i].is_refresh)
                            {
                                to = this.FanfouAPI.MentionTimeLineStatus[i - 1].id;
                                from = this.FanfouAPI.MentionTimeLineStatus[i + 1].id;
                                break;
                            }
                        }
                        if (from != "" && to != "")
                        {
                            this.loading.Visibility = System.Windows.Visibility.Visible;
                            FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Center, from, to, id);
                        }
                    }
                }
                else
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                    {
                        PhoneApplicationService.Current.State.Remove("StatusPage");
                    }
                    PhoneApplicationService.Current.State.Add("StatusPage", e.AddedItems[0]);
                    NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
                }
            }
            this.MentionTimeLineListBox.SelectedItem = null;
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);

            Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind));
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind));
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
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind));
        }

        private void MentionTimeLineListBox_RefreshTriggered(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
            Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Behind));
        }

        private void MentionTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.MentionTimeLineListBox.ItemsSource[this.MentionTimeLineListBox.ItemsSource.Count - 1] && !FanfouAPI.MentionTimeLineEnded)
            {
                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                Dispatcher.BeginInvoke(() => FanfouAPI.StatusMentionTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Back));
            }
        }
        private void HomeTimeLineListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.HomeTimeLineListBox.ItemsSource[this.HomeTimeLineListBox.ItemsSource.Count - 1] && !FanfouAPI.HomeTimeLineEnded)
            {
                Dispatcher.BeginInvoke(() => this.loading.Visibility = System.Windows.Visibility.Visible);
                Dispatcher.BeginInvoke(() => FanfouAPI.StatusHomeTimeline(setting.defaultCount2 * 10 + 20, FanfouAPI.RefreshMode.Back));
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

        private dynamic nav_mode;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            pdi = new ObservableCollection<PivotDataItem>();
            pdi.Add(new PivotDataItem("我的消息", "0"));
            pdi.Add(new PivotDataItem("提及我的", "0"));
            pdi.Add(new PivotDataItem("更多", "0"));
            (this.Pivot.Items[0] as PivotItem).Header = pdi[0];
            (this.Pivot.Items[1] as PivotItem).Header = pdi[1];
            (this.Pivot.Items[2] as PivotItem).Header = pdi[2];

            this.Pivot.Visibility = Visibility.Visible;
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();

            nav_mode = e.NavigationMode;

            this.HomeTimeLineListBox.ItemsSource = this.FanfouAPI.HomeTimeLineStatus;
            this.MentionTimeLineListBox.ItemsSource = this.FanfouAPI.MentionTimeLineStatus;

            this.TitleControl.DataContext = this.FanfouAPI.CurrentUser;

            Toolbox.DataContext = FanfouAPI.CurrentUser;
            ToolPivot.DataContext = FanfouAPI.CurrentUser;

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["TimelinePage"] = new Object();
            }

        }

        private bool is_back_pressed = false;
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (setting.quit_confirm == true)
            {
                if (!is_back_pressed)
                {
                    ToastPrompt tp = new ToastPrompt();
                    tp.Completed += (s, e2) => { is_back_pressed = false; };
                    tp.Title = "饭窗";
                    tp.Message = "再按一次后退键退出";
                    tp.MillisecondsUntilHidden = 1000;
                    tp.Show();
                    e.Cancel = true;
                    is_back_pressed = true;
                }
            }
            base.OnBackKeyPress(e);
        }

        private int old_pivot_index = 0;
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Pivot.SelectedIndex)
            {
                case 0:
                    (this.Pivot.Items[1] as UIElement).Opacity = 0;
                    (this.Pivot.Items[2] as UIElement).Opacity = 0;
                    if (old_pivot_index == 2)
                        PivotUnexpandStoryBoard.Begin();
                    else
                        (this.Pivot.Items[0] as UIElement).Opacity = 1;
                    break;
                case 1:
                    (this.Pivot.Items[0] as UIElement).Opacity = 0;
                    (this.Pivot.Items[2] as UIElement).Opacity = 0;
                    if (old_pivot_index == 2)
                        PivotUnexpandStoryBoard.Begin();
                    else
                        (this.Pivot.Items[1] as UIElement).Opacity = 1;
                    break;
                case 2:
                    (this.Pivot.Items[0] as UIElement).Opacity = 0;
                    (this.Pivot.Items[1] as UIElement).Opacity = 0;
                    (this.Pivot.Items[2] as UIElement).Opacity = 1;
                    PivotExpandStoryBoard.Begin();
                    break;
                default:
                    break;
            }
            old_pivot_index = Pivot.SelectedIndex;
        }

        private void HomeTimeLineListBox_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            pdi[0].Count = "0";
            (this.Pivot.Items[0] as PivotItem).Header = new PivotDataItem(pdi[0].Title, pdi[0].Count);
        }

        private void MentionTimeLineListBox_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            pdi[1].Count = "0";
            (this.Pivot.Items[1] as PivotItem).Header = new PivotDataItem(pdi[1].Title, pdi[1].Count);
        }

        private void HomeTimeLineListBox_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
        }

        private void MentionTimeLineListBox_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
        }

        private void Grid_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
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

        private void FanfouImage_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.HomeTimeLineListBox.ItemsSource != null && this.HomeTimeLineListBox.ItemsSource.Count != 0)
                this.HomeTimeLineListBox.ScrollTo(this.HomeTimeLineListBox.ItemsSource[0]);

            if (this.MentionTimeLineListBox.ItemsSource != null && this.MentionTimeLineListBox.ItemsSource.Count != 0)
                this.MentionTimeLineListBox.ScrollTo(this.MentionTimeLineListBox.ItemsSource[0]);

        }

        private void PivotExpandStoryBoard_Completed(object sender, EventArgs e)
        {
        }

        private void PivotUnexpandStoryBoard_Completed(object sender, EventArgs e)
        {
            switch (Pivot.SelectedIndex)
            {
                case 0:
                    (this.Pivot.Items[0] as UIElement).Opacity = 1;
                    break;
                case 1:
                    (this.Pivot.Items[1] as UIElement).Opacity = 1;
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
    }
}