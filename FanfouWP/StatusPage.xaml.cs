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

            Loaded += StatusPage_Loaded;
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


            });
        }

        private void NewAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBarIconButton UserButton = new ApplicationBarIconButton();
            UserButton.IconUri = new Uri("/Assets/AppBar/feature.calendar.png", UriKind.Relative);
            UserButton.Text = "个人资料";
            ApplicationBar.Buttons.Add(UserButton);
            UserButton.Click += new EventHandler(UserButton_Click);

            ApplicationBarIconButton ResendButton = new ApplicationBarIconButton();
            ResendButton.IconUri = new Uri("/Assets/AppBar/share.png", UriKind.Relative);
            ResendButton.Text = "转发";
            ApplicationBar.Buttons.Add(ResendButton);
            ResendButton.Click += new EventHandler(ResendButton_Click);

            if (!status.favorited)
            {
                ApplicationBarIconButton FavButton = new ApplicationBarIconButton();
                FavButton.IconUri = new Uri("/Assets/AppBar/favs.addto.png", UriKind.Relative);
                FavButton.Text = "收藏";
                ApplicationBar.Buttons.Add(FavButton);
                FavButton.Click += new EventHandler(FavButton_Click);
            }
            else
            {
                ApplicationBarIconButton FavButton = new ApplicationBarIconButton();
                FavButton.IconUri = new Uri("/Assets/AppBar/minus.png", UriKind.Relative);
                FavButton.Text = "取消收藏";
                ApplicationBar.Buttons.Add(FavButton);
                FavButton.Click += new EventHandler(FavButton_Click);
            }

            ApplicationBarIconButton ReplyButton = new ApplicationBarIconButton();
            ReplyButton.IconUri = new Uri("/Assets/AppBar/upload.png", UriKind.Relative);
            ReplyButton.Text = "回复";
            ApplicationBar.Buttons.Add(ReplyButton);
            ReplyButton.Click += new EventHandler(ReplyButton_Click);

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
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
        }

        void Instance_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.DataContext = status;
                NewAppBar();
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
            });
        }

        void Instance_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
        }

        void Instance_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.DataContext = status;
                NewAppBar();

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
    }
}