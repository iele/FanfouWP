using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using FanfouWP.Storage;
using Windows.Storage;
using Microsoft.Phone.Shell;

namespace FanfouWP
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private SettingManager settingManager = SettingManager.GetInstance();

        public SettingPage()
        {
            InitializeComponent();

            this.Loaded += SettingPage_Loaded;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            settingManager.SaveSettings();

            base.OnNavigatedFrom(e);
        }

        void SettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.QualityListPicker.ItemsSource = new string[] { "2G (最大长宽600px,压缩80%)", "3G (最大长宽800px,压缩90%)", "Wi-Fi (最大长宽1280px,压缩100%)", "无限制" };
            this.CountListPicker.ItemsSource = new string[] { "20", "30", "40", "50", "60" };
            this.CacheListPicker.ItemsSource = new string[] { "100", "300", "500", "1000" };
            this.FrequencyListPicker.ItemsSource = new string[] { "30分钟", "1小时", "2小时", "关闭" };
            this.TimelineFreqListPicker.ItemsSource = new string[] { "1分钟", "2分钟", "5分钟", "10分钟", "关闭" };
            Dispatcher.BeginInvoke(() =>
            {
                this.QuitCheckBox.IsChecked = settingManager.quit_confirm;
                this.LocationCheckBox.IsChecked = settingManager.enableLocation;
                this.ImageCheckBox.IsChecked = settingManager.displayImage;
                this.QualityListPicker.SelectedIndex = settingManager.imageQuality;
                this.CacheListPicker.SelectedIndex = settingManager.cacheSize;
                this.FrequencyListPicker.SelectedIndex = settingManager.backgroundFeq;
                this.ContextCheckBox.IsChecked = settingManager.reverseContext;
                this.CountListPicker.SelectedIndex = settingManager.defaultCount2;
                this.TimelineFreqListPicker.SelectedIndex = settingManager.refreshFreq;

                this.CountListPicker.SelectionChanged += CountListPicker_SelectionChanged;
                this.QualityListPicker.SelectionChanged += QualityListPicker_SelectionChanged;
                this.CacheListPicker.SelectionChanged += CacheListPicker_SelectionChanged;
                this.FrequencyListPicker.SelectionChanged += FrequencyListPicker_SelectionChanged;
                this.TimelineFreqListPicker.SelectionChanged += TimelineFreqListPicker_SelectionChanged;
            });

        }

        void TimelineFreqListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TimelineFreqListPicker.SelectedIndex != -1)
            {
                settingManager.refreshFreq = this.TimelineFreqListPicker.SelectedIndex;
            }
        }

        void CountListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CountListPicker.SelectedIndex != -1)
            {
                settingManager.defaultCount2 = this.CountListPicker.SelectedIndex;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("注销后将回退至登录页面,同时当前用户所有保存数据将删除, 是否继续?", "确认注销?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            settingManager.username = null;
            settingManager.password = null;
            settingManager.oauthToken = null;
            settingManager.oauthSecret = null;
            settingManager.currentUser = null;
            settingManager.SaveSettings();

            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Dispatcher.BeginInvoke(
            async () =>
            {
                var dataFolder = await localFolder.CreateFolderAsync("storage-" + FanfouWP.API.FanfouAPI.Instance.CurrentUser.id, CreationCollisionOption.OpenIfExists);
                foreach (var item in await dataFolder.GetFilesAsync())
                    await item.DeleteAsync();
                await dataFolder.DeleteAsync();

                FanfouWP.API.FanfouAPI.Instance.ResetManager();

                NavigationService.RemoveBackEntry();
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            });
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var status = new FanfouWP.API.Items.Status();
            status.user = new FanfouWP.API.Items.User();
            status.id = "";
            status.user.screen_name = "饭窗";
            status.user.id = "fanwp";


            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", status);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask ect = new EmailComposeTask();
            ect.To = "melephas@outlook.com";
            ect.Subject = "关于饭窗的建议";
            ect.Show();
        }

        private void MarketButton_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask mrt = new MarketplaceReviewTask();
            mrt.Show();
        }

        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateSuccess += (s, e2) => { this.toast.NewToast("成功添加@饭窗为你的好友:)"); };
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateFailed += (s, e2) => { this.toast.NewToast("添加好友失败:( " + e2.error.error); };
            FanfouWP.API.FanfouAPI.Instance.FriendshipCreate("fanwp");
        }

        private void ImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            settingManager.displayImage = this.ImageCheckBox.IsChecked.HasValue ? this.ImageCheckBox.IsChecked.Value : false;
        }

        private void LocationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            settingManager.enableLocation = this.LocationCheckBox.IsChecked.HasValue ? this.LocationCheckBox.IsChecked.Value : false;
        }

        private void QualityListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.QualityListPicker.SelectedIndex != -1)
                settingManager.imageQuality = this.QualityListPicker.SelectedIndex;
        }

        private void CacheListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CacheListPicker.SelectedIndex != -1)
                settingManager.cacheSize = this.CacheListPicker.SelectedIndex;
        }

        private void FrequencyListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FrequencyListPicker.SelectedIndex != -1)
            {
                settingManager.backgroundFeq = this.FrequencyListPicker.SelectedIndex;
                AgentWriter.WriteAgentParameter(settingManager.username, settingManager.password, settingManager.oauthToken, settingManager.oauthSecret, settingManager.backgroundFeq);
            }
        }

        private void QuitCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            settingManager.quit_confirm = this.QuitCheckBox.IsChecked.HasValue ? this.QuitCheckBox.IsChecked.Value : false;
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("该操作将删除当前用户所有缓存数据, 是否继续?", "确认清理?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Dispatcher.BeginInvoke(
            async () =>
            {
                var dataFolder = await localFolder.CreateFolderAsync("storage-" + FanfouWP.API.FanfouAPI.Instance.CurrentUser.id, CreationCollisionOption.OpenIfExists);
                await dataFolder.DeleteAsync();
                dataFolder = await localFolder.CreateFolderAsync("CacheImages", CreationCollisionOption.OpenIfExists);
                await dataFolder.DeleteAsync();
            });
        }

        private void ContextCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            settingManager.reverseContext = this.ContextCheckBox.IsChecked.HasValue ? this.ContextCheckBox.IsChecked.Value : false;

        }


    }
}