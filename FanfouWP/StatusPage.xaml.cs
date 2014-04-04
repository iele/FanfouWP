using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FanfouWP
{
    public partial class StatusPage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.Status status;
        public StatusPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
            {
                status = PhoneApplicationService.Current.State["StatusPage"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("StatusPage");
            }

            FanfouWP.API.FanfouAPI.Instance.StatusDestroySuccess += Instance_StatusDestroySuccess;

            FanfouWP.API.FanfouAPI.Instance.StatusDestroyFailed += Instance_StatusDestroyFailed;

            Loaded += StatusPage_Loaded;
        }

        void Instance_StatusDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void Instance_StatusDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.NavigationService.GoBack();
            });
        }

        private void StatusPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.DataContext = status;
                NewAppBar();

                if (this.status.location != "")
                {
                    try
                    {
                        char[] s = { ',' };
                        string[] l = this.status.location.Split(s);
                        this.map.Center = new GeoCoordinate(double.Parse(l[0]), double.Parse(l[1]));
                        this.map.ZoomLevel = 15;
                        this.map.Layers.Clear();
                        var mapLayer = new MapLayer();
                        this.map.Layers.Add(mapLayer);
                        MapOverlay overlay = new MapOverlay();
                        Ellipse mark = new Ellipse();
                        mark.Fill = new SolidColorBrush(Colors.Blue);
                        mark.Height = 20;
                        mark.Width = 20;
                        mark.Opacity = 50;
                        overlay.Content = mark;

                        overlay.GeoCoordinate = new GeoCoordinate(double.Parse(l[0]), double.Parse(l[1]));
                        overlay.PositionOrigin = new Point(0.0, 1.0);
                        mapLayer.Add(overlay);
                        this.map.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {
                        this.map.Visibility = Visibility.Collapsed;
                    }
                }

                this.loading.Visibility = System.Windows.Visibility.Collapsed;

            });
        }

        private void NewAppBar()
        {
            if (!status.favorited)
            {
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IconUri = new Uri("/Assets/AppBar/favs.addto.png", UriKind.Relative);
                FavButton.Text = "收藏";
            }
            else
            {
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IconUri = new Uri("/Assets/AppBar/minus.png", UriKind.Relative);
                FavButton.Text = "取消收藏";
            
          }
            if (this.status.user.id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
            {
                (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = true;
            }
            else {
                (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
            }
        }

        private void UserButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
            {
                PhoneApplicationService.Current.State.Remove("UserPage");
            }
            PhoneApplicationService.Current.State.Add("UserPage", status.user);
            NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
        }

        private void ResendButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                PhoneApplicationService.Current.State.Remove("ReSend");
            }
            PhoneApplicationService.Current.State.Add("ReSend", status);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void FavButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });

            if (status.favorited == false)
            {
                FanfouWP.API.FanfouAPI.Instance.FavoritesCreate(status.id);
                FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess += Instance_FavoritesCreateSuccess;
                FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed += Instance_FavoritesCreateFailed;
            }
            else
            {
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroy(status.id);
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess += Instance_FavoritesDestroySuccess;
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed += Instance_FavoritesDestroyFailed;
            }
        }

        void Instance_FavoritesDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
        }

        void Instance_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status.favorited = false;
                this.DataContext = status;
                NewAppBar();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
        }

        void Instance_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
        }

        void Instance_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status.favorited = true;
                this.DataContext = status;
                NewAppBar();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
        }

        private void ReplyButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", status);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            FanfouWP.API.FanfouAPI.Instance.StatusDestroy(this.status.id);
        }
    }
}