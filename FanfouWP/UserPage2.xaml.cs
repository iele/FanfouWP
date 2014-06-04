using FanfouWP.Storage;
using FanfouWP.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace FanfouWP
{
    public partial class UserPage2 : PhoneApplicationPage
    {
        private API.Items.User user;
        private ObservableCollection<FanfouWP.API.Items.Status> status;
        private dynamic tag;
        private dynamic friends;
        private dynamic follows;
        private dynamic fav;
        private Toast toast = new Toast();

        public UserPage2()
        {
            InitializeComponent();
            if (PhoneApplicationService.Current.State.ContainsKey("UserPage2"))
            {
                user = PhoneApplicationService.Current.State["UserPage2"] as FanfouWP.API.Items.User;
            }
            this.Loaded += UserPage2_Loaded;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;
            FanfouWP.API.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouWP.API.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            FanfouWP.API.FanfouAPI.Instance.TagListSuccess += Instance_TagListSuccess;
            FanfouWP.API.FanfouAPI.Instance.TagListFailed += Instance_TagListFailed;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateSuccess += Instance_FriendshipsCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateFailed += Instance_FriendshipsCreateFailed;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroySuccess += Instance_FriendshipsDestroySuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroyFailed += Instance_FriendshipsDestroyFailed;

            FanfouWP.API.FanfouAPI.Instance.FriendshipsExistsSuccess += Instance_FriendshipsExistsSuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsExistsFailed += Instance_FriendshipsExistsFailed;
        }

        void Instance_FriendshipsExistsFailed(object sender, API.Event.FailedEventArgs e)
        {
        }


        void Instance_FriendshipsExistsSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user.following && (bool)sender == true)
                {
                    this.friendship.Source = new BitmapImage(new Uri("/Assets/connected.png", UriKind.Relative));
                    return;
                }
                if (user.following)
                {
                    this.friendship.Source = new BitmapImage(new Uri("/Assets/follow.png", UriKind.Relative));
                    return;
                }
                if ((bool)sender)
                {
                    this.friendship.Source = new BitmapImage(new Uri("/Assets/friend.png", UriKind.Relative));
                    return;
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["UserPage2_user"] = this.user;
                State["UserPage2_status"] = this.status;
                State["UserPage2_tag"] = this.tag;
                State["UserPage2_friends"] = this.friends;
                State["UserPage2_follows"] = this.follows;
                State["UserPage2_fav"] = this.fav;
                State["UserPage2_index"] = this.pivot.SelectedIndex;
            }

            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("UserPage2_user"))
            {
                this.user = State["UserPage2_user"] as FanfouWP.API.Items.User;
            }
            if (State.ContainsKey("UserPage2_status"))
            {
                this.status = State["UserPage2_status"] as ObservableCollection<FanfouWP.API.Items.Status>;
                ;
                if (status == null || status.Count == 0)
                    this.FirstStatusText.Text = "此用户尚未发送任何消息= =!";
                else
                    this.FirstStatusText.Text = HttpUtility.HtmlDecode(status.First().text);
                this.TimeLineListBox.ItemsSource = status;
            }
            if (State.ContainsKey("UserPage2_tag"))
            {
                this.tag = State["UserPage2_tag"];
                this.tags.ItemsSource = tag;
            }
            if (State.ContainsKey("UserPage2_friends"))
            {
                this.friends = State["UserPage2_friends"];
                this.FriendsListBox.ItemsSource = friends;
            }
            if (State.ContainsKey("UserPage2_follows"))
            {
                this.follows = State["UserPage2_follows"];
                this.FollowersListBox.ItemsSource = follows;
            }
            if (State.ContainsKey("UserPage2_fav"))
            {
                this.fav = State["UserPage2_fav"];
                this.FavListBox.ItemsSource = fav;
            }
            if (State.ContainsKey("UserPage2_index"))
            {
                this.pivot.SelectedIndex = (int)State["UserPage2_index"];
            }
            base.OnNavigatedTo(e);
        }

        void Instance_TagListFailed(object sender, API.Event.FailedEventArgs e)
        {
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.loading.Visibility = System.Windows.Visibility.Collapsed;
            //});
            //Dispatcher.BeginInvoke(() => { toast.NewToast("标签列表获取失败:( " + e.error.error); });
        }

        void Instance_TagListSuccess(object sender, API.Event.ListEventArgs<string> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                tag = e.Result;
                tags.ItemsSource = tag;
            });
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

        void Instance_UsersFollowersFailed(object sender, API.Event.FailedEventArgs e)
        {
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.loading.Visibility = System.Windows.Visibility.Collapsed;
            //    Dispatcher.BeginInvoke(() => { toast.NewToast("关注列表获取失败:( " + e.error.error); });
            //});
        }

        void Instance_UsersFollowersSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.follows = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.User>).UserStatus;
                this.FollowersListBox.ItemsSource = follows;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void Instance_UsersFriendsFailed(object sender, API.Event.FailedEventArgs e)
        {
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.loading.Visibility = System.Windows.Visibility.Collapsed;
            //    Dispatcher.BeginInvoke(() => { toast.NewToast("好友列表获取失败:( " + e.error.error); });
            //});
        }

        void Instance_UsersFriendsSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.friends = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.User>).UserStatus;
                this.FriendsListBox.ItemsSource = friends;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        private void Instance_FavoritesFailed(object sender, API.Event.FailedEventArgs e)
        {
            //Dispatcher.BeginInvoke(() =>
            //{
            //    this.loading.Visibility = System.Windows.Visibility.Collapsed;
            //    Dispatcher.BeginInvoke(() => { toast.NewToast("收藏列表获取失败:( " + e.error.error); });
            //});
        }

        private void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.fav = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.Status>).UserStatus;
                this.FavListBox.ItemsSource = fav;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        private void Instance_UserTimelineSuccess(object sender, API.Event.UserTimelineEventArgs<FanfouWP.API.Items.Status> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status = e.UserStatus;
                this.TimeLineListBox.ItemsSource = status;
                if (status == null || status.Count == 0)
                    this.FirstStatusText.Text = "此用户尚未发送任何消息= =!";
                else
                    this.FirstStatusText.Text = HttpUtility.HtmlDecode(status.First().text);
            
                if (this.friends == null)
                    FanfouWP.API.FanfouAPI.Instance.UsersFriends(this.user.id);
                if (this.follows == null)
                    FanfouWP.API.FanfouAPI.Instance.UsersFollowers(this.user.id);
                if (this.fav == null)
                    FanfouWP.API.FanfouAPI.Instance.FavoritesId(this.user.id);
                if (this.tag == null)
                    FanfouWP.API.FanfouAPI.Instance.TaggedList(this.user.id);
            });
        }

        void Instance_UserTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("消息列表获取失败:( " + e.error.error); });
        }

        void UserPage2_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = user;
            if (this.status == null)
                FanfouWP.API.FanfouAPI.Instance.StatusUserTimeline(SettingManager.GetInstance().defaultCount2 * 10 + 20, this.user.id);
            FanfouWP.API.FanfouAPI.Instance.FriendshipExists(user.id, FanfouWP.API.FanfouAPI.Instance.CurrentUser.id);
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
        private void TimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TimeLineListBox.SelectedItem != null)
            {
                var item = this.TimeLineListBox.SelectedItem;
                this.TimeLineListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void StackPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 1;
        }

        private void StackPanel_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 2;
        }

        private void StackPanel_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 3;
        }
        private void StackPanel_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 4;
        }

        private void FavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FavListBox.SelectedItem != null)
            {
                var item = this.FavListBox.SelectedItem;
                this.FavListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void FollowersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FollowersListBox.SelectedItem != null)
            {
                var item = this.FollowersListBox.SelectedItem;
                this.FollowersListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        private void FriendsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FriendsListBox.SelectedItem != null)
            {
                var item = this.FriendsListBox.SelectedItem;
                this.FriendsListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
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

        private void tags_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("TagPage"))
            {
                PhoneApplicationService.Current.State.Remove("TagPage");
            }
            PhoneApplicationService.Current.State.Add("TagPage", this.user);
            NavigationService.Navigate(new Uri("/TagPage.xaml", UriKind.Relative));

        }

        private void DirectMenu_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("MessagePage_User"))
            {
                PhoneApplicationService.Current.State.Remove("MessagePage_User");
            }
            PhoneApplicationService.Current.State.Add("MessagePage_User", this.user);
            NavigationService.Navigate(new Uri("/MessagePage.xaml", UriKind.Relative));

        }

        private void PhotosMenu_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ViewerPage"))
            {
                PhoneApplicationService.Current.State.Remove("ViewerPage");
            }
            PhoneApplicationService.Current.State.Add("ViewerPage", this.user);
            NavigationService.Navigate(new Uri("/ViewerPage.xaml", UriKind.Relative));

        }

        private void ReplayButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReplyWithoutStatus"))
            {
                PhoneApplicationService.Current.State.Remove("ReplyWithoutStatus");
            }
            PhoneApplicationService.Current.State.Add("ReplyWithoutStatus", this.user);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });

            this.DataContext = user;
            FanfouWP.API.FanfouAPI.Instance.StatusUserTimeline(SettingManager.GetInstance().defaultCount2 * 10 + 20, this.user.id);
            checkMenu();
        }

        private void UrlText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(user.url);
            wbt.Show();
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("SearchUserPage"))
            {
                PhoneApplicationService.Current.State.Remove("SearchUserPage");
            }
            PhoneApplicationService.Current.State.Add("SearchUserPage", user);
            NavigationService.Navigate(new Uri("/SearchUserPage.xaml", UriKind.Relative));
        }

        private void MessageButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("MessagePage_User"))
            {
                PhoneApplicationService.Current.State.Remove("MessagePage_User");
            }
            PhoneApplicationService.Current.State.Add("MessagePage_User", this.user);
            NavigationService.Navigate(new Uri("/MessagePage.xaml", UriKind.Relative));
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    (this.pivot.Items[0] as UIElement).Opacity = 1;
                    (this.pivot.Items[1] as UIElement).Opacity = 0;
                    (this.pivot.Items[2] as UIElement).Opacity = 0;
                    (this.pivot.Items[3] as UIElement).Opacity = 0;
                    break;
                case 1:
                    (this.pivot.Items[0] as UIElement).Opacity = 0;
                    (this.pivot.Items[1] as UIElement).Opacity = 1;
                    (this.pivot.Items[2] as UIElement).Opacity = 0;
                    (this.pivot.Items[3] as UIElement).Opacity = 0;
                    break;
                case 2:
                    (this.pivot.Items[0] as UIElement).Opacity = 0;
                    (this.pivot.Items[1] as UIElement).Opacity = 0;
                    (this.pivot.Items[2] as UIElement).Opacity = 1;
                    (this.pivot.Items[3] as UIElement).Opacity = 0;
                    break;
                case 3:
                    (this.pivot.Items[0] as UIElement).Opacity = 0;
                    (this.pivot.Items[1] as UIElement).Opacity = 0;
                    (this.pivot.Items[2] as UIElement).Opacity = 0;
                    (this.pivot.Items[3] as UIElement).Opacity = 1;
                    break;
                default:
                    break;
            }
        }
    }
}