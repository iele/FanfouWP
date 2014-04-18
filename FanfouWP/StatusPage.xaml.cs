﻿using System;
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
using System.Windows.Documents;
using FanfouWP.API.Items;
using Microsoft.Phone.Tasks;
using FanfouWP.ItemControls;

namespace FanfouWP
{
    public partial class StatusPage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.Status status;

        private enum TextMode { Text, Url, At, Search };
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

            FanfouWP.API.FanfouAPI.Instance.UsersShowSuccess += Instance_UsersShowSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;

            FanfouWP.API.FanfouAPI.Instance.ContextTimelineSuccess += Instance_ContextTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.ContextTimelineFailed += Instance_ContextTimelineFailed;
            Loaded += StatusPage_Loaded;
        }

        void Instance_ContextTimelineSuccess(object sender, API.Event.UserTimelineEventArgs<Status> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in e.UserStatus)
                {
                    var sic = new StatusItemControl();
                    sic.Tap += sic_Tap;
                    sic.DataContext = item;
                    this.context.Children.Add(sic);
                }

                if (e.UserStatus.Count != 0)
                    this.context.Visibility = Visibility.Visible;

                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void sic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((sender as StatusItemControl).DataContext != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", ((sender as StatusItemControl).DataContext as Status).user);
                NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        void Instance_ContextTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("消息上下文获取失败:)" + e.error.error); });
        }

        void Instance_StatusDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("状态删除失败:( " + e.error.error); });
        }

        void Instance_StatusDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.NavigationService.GoBack();
                Dispatcher.BeginInvoke(() => { toast.NewToast("状态删除成功:)"); });
            });

            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage_To"))
            {
                PhoneApplicationService.Current.State.Remove("TimelinePage_To");
            }
            PhoneApplicationService.Current.State.Add("TimelinePage_To", "");

        }

        private void StatusPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.DataContext = status;

                if (status.in_reply_to_status_id != null && status.in_reply_to_status_id != "")
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.loading.Visibility = System.Windows.Visibility.Visible;
                    });
                    FanfouWP.API.FanfouAPI.Instance.StatusContextTimeline(this.status.id);
                }

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

                string text = HttpUtility.HtmlDecode(this.status.text);
                List<string> sep = new List<string>();
                List<TextMode> t = new List<TextMode>();
                textStateParser(text, out sep, out t);

                Paragraph myParagraph = new Paragraph();
                myParagraph.FontSize = 24;
                for (int i = 0; i < sep.Count; i++)
                {
                    switch (t[i])
                    {
                        case TextMode.Text:
                            Run run = new Run();
                            run.FontSize = 24;
                            run.Text = sep[i];
                            myParagraph.Inlines.Add(run);
                            break;
                        case TextMode.At:
                            Hyperlink link2 = new Hyperlink();
                            var count1 = i;

                            link2.Inlines.Add(sep[i]);
                            link2.Foreground = new SolidColorBrush(Colors.Black);
                            link2.FontSize = 24;
                            link2.Click += (s, et) =>
                            {
                                var item = new FanfouWP.API.Items.Trends();
                                item.query = sep[count1];

                                if (PhoneApplicationService.Current.State.ContainsKey("SearchPage_User"))
                                {
                                    PhoneApplicationService.Current.State.Remove("SearchPage_User");
                                }
                                PhoneApplicationService.Current.State.Add("SearchPage_User", item);
                                NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
                            };

                            myParagraph.Inlines.Add("@");
                            myParagraph.Inlines.Add(link2);
                            myParagraph.Inlines.Add(" ");
                            break;
                        case TextMode.Url:
                            Hyperlink link = new Hyperlink();
                            link.Inlines.Add(sep[i]);
                            link.FontSize = 24;
                            link.Foreground = new SolidColorBrush(Colors.Black);
                            link.NavigateUri = new Uri(sep[i]);
                            link.TargetName = "_blank";
                            myParagraph.Inlines.Add(" ");
                            myParagraph.Inlines.Add(link);
                            myParagraph.Inlines.Add(" ");
                            break;
                        case TextMode.Search:
                            Hyperlink link3 = new Hyperlink();
                            var count2 = i;
                            link3.Inlines.Add(sep[i]);
                            link3.Foreground = new SolidColorBrush(Colors.Black);
                            link3.FontSize = 24;
                            link3.Click += (s, et) =>
                            {
                                var item = new FanfouWP.API.Items.Trends();
                                item.query = sep[count2];

                                if (PhoneApplicationService.Current.State.ContainsKey("SearchPage"))
                                {
                                    PhoneApplicationService.Current.State.Remove("SearchPage");
                                }
                                PhoneApplicationService.Current.State.Add("SearchPage", item);
                                NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
                            };

                            myParagraph.Inlines.Add(" #");
                            myParagraph.Inlines.Add(link3);
                            myParagraph.Inlines.Add("# ");
                            break;
                        default:
                            break;
                    }
                }

                this.richText.Blocks.Clear();
                this.richText.Blocks.Add(myParagraph);

                this.loading.Visibility = System.Windows.Visibility.Collapsed;

            });
        }

        private void textStateParser(string text, out List<string> sep, out List<TextMode> t)
        {
            TextMode state = TextMode.Text;

            sep = new List<string>();
            t = new List<TextMode>();

            int count = 0;
            string tmp = "";
            foreach (var c in text)
            {
                count++;
                switch (state)
                {
                    case TextMode.Text:
                        if (c == '#')
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.Search;
                            tmp = "";
                        }
                        else if (c == '@')
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.At;
                            tmp = "";
                        }
                        else if (text.Length > count + 7 && text.Substring(count, 7) == "http://")
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.Url;
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case TextMode.Url:
                        if (c == ' ')
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.Text;
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case TextMode.At:
                        if (c == ' ')
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.Text;
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case TextMode.Search:
                        if (c == '#')
                        {
                            sep.Add(tmp);
                            t.Add(state);
                            state = TextMode.Text;
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    default:
                        break;
                }
            }
            sep.Add(tmp);
            t.Add(state);
        }

        private void NewAppBar()
        {
            if (!status.favorited)
            {
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IconUri = new Uri("/Assets/AppBar/favs.addto.png", UriKind.Relative);
                FavButton.Text = "收藏";
                FavButton.IsEnabled = true;
            }
            else
            {
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IconUri = new Uri("/Assets/AppBar/minus.png", UriKind.Relative);
                FavButton.Text = "取消收藏";
                FavButton.IsEnabled = true;
            }
            if (this.status.user.id == FanfouWP.API.FanfouAPI.Instance.CurrentUser.id)
            {
                (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = true;
            }
            else
            {
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
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IsEnabled = false;
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
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IsEnabled = false;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;

            Dispatcher.BeginInvoke(() => { toast.NewToast("收藏取消失败:( " + e.error.error); });
        }

        void Instance_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status.favorited = false;
                this.DataContext = status;
                NewAppBar();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.fav.Text = "";
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
            Dispatcher.BeginInvoke(() => { toast.NewToast("收藏取消成功:)"); });

            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage_To"))
            {
                PhoneApplicationService.Current.State.Remove("TimelinePage_To");
            }
            PhoneApplicationService.Current.State.Add("TimelinePage_To", "");


        }

        void Instance_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBarIconButton FavButton = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                FavButton.IsEnabled = false;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
            Dispatcher.BeginInvoke(() => { toast.NewToast("收藏创建失败:( " + e.error.error); });
        }

        void Instance_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status.favorited = true;
                this.DataContext = status;
                this.fav.Text = "已收藏";
                NewAppBar();
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;

            Dispatcher.BeginInvoke(() => { toast.NewToast("收藏创建成功:)"); });

            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage_To"))
            {
                PhoneApplicationService.Current.State.Remove("TimelinePage_To");
            }
            PhoneApplicationService.Current.State.Add("TimelinePage_To", "");

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

        private void map_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "fa2df181-7b08-43aa-a80c-553ca63a8cf4";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "Uq2CsIyc-vRLRRx1oKJATQ";
        }

        private void TextBlock_Tap_InReply(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.status.in_reply_to_user_id != null || this.status.in_reply_to_user_id != "")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    this.ReplyTextBlock.IsHitTestVisible = false;
                });
                FanfouWP.API.FanfouAPI.Instance.UsersShow(this.status.in_reply_to_user_id);
            }
        }

        private void TextBlock_Tap_Repost(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.status.repost_user_id != null || this.status.repost_user_id != "")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    this.RepostTextBlock.IsHitTestVisible = false;
                });
                FanfouWP.API.FanfouAPI.Instance.UsersShow(this.status.repost_user_id);
            }
        }
        private void Instance_SearchUserFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.ReplyTextBlock.IsHitTestVisible = false;
                this.RepostTextBlock.IsHitTestVisible = false;
            });
            Dispatcher.BeginInvoke(() => { toast.NewToast("用户信息获取失败:( " + e.error.error); });

        }

        private void Instance_UsersShowSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.ReplyTextBlock.IsHitTestVisible = false;
                this.RepostTextBlock.IsHitTestVisible = false;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", sender as FanfouWP.API.Items.User);
                NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            });

        }


    }
}