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
using System.Windows.Media.Imaging;

namespace FanfouWP
{
    public partial class SendPage : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;

        private enum PageType { Normal, Resend, Reply };
        private PageType currentPageType = PageType.Normal;

        private dynamic is_sending = false;
        private FanfouWP.API.Items.Status status;

        private WriteableBitmap image;

        private string position = "";
        public SendPage()
        {
            InitializeComponent();
            this.Loaded += SendPage_Loaded;

            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                status = PhoneApplicationService.Current.State["ReSend"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("ReSend");
                currentPageType = PageType.Resend;
            } if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                status = PhoneApplicationService.Current.State["Reply"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("Reply");
                currentPageType = PageType.Reply;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Image"))
            {
                image = PhoneApplicationService.Current.State["SendPage_Image"] as WriteableBitmap;
                PhoneApplicationService.Current.State.Remove("SendPage_Image");
            }
        }

        private void SendPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouAPI.StatusUpdateSuccess += FanfouAPI_StatusUpdateSuccess;
            FanfouAPI.StatusUpdateFailed += FanfouAPI_StatusUpdateFailed;
            FanfouAPI.PhotosUploadSuccess += FanfouAPI_PhotosUploadSuccess;
            FanfouAPI.PhotosUploadFailed += FanfouAPI_PhotosUploadFailed;

            Dispatcher.BeginInvoke(async () =>
            {
                position = await FanfouWP.Utils.GeoLocatorUtils.getGeolocator();
                this.location.Text = position;
            });

            switch (currentPageType)
            {
                case PageType.Normal:
                    titleText.Text = "你在做什么？";
                    break;
                case PageType.Resend:
                    Dispatcher.BeginInvoke(() =>
                    {
                        titleText.Text = "转发" + status.user.screen_name + "的消息";
                        this.Status.Text = this.status.text;
                        this.Status.IsReadOnly = true;
                    });
                    break;
                case PageType.Reply:
                    Dispatcher.BeginInvoke(() =>
                    {
                        titleText.Text = "回复" + status.user.screen_name;
                        this.Status.Text = "@" + this.status.user.screen_name;
                    });
                    break;
                default:
                    break;
            }

            if (image != null)
            {
                this.Image.Source = image;
            }
        }

        void FanfouAPI_PhotosUploadFailed(object sender, API.Event.FailedEventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void FanfouAPI_PhotosUploadSuccess(object sender, EventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.NavigationService.GoBack();
            });
        }

        void FanfouAPI_StatusUpdateFailed(object sender, API.Event.FailedEventArgs e)
        {
            is_sending = false;

            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void FanfouAPI_StatusUpdateSuccess(object sender, EventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.NavigationService.GoBack();
            });
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (image != null)
            {
                FanfouAPI.PhotoUpload(this.Status.Text, image);
                return;
            }

            if (this.Status.Text.Count() == 0)
                return;

            if (!is_sending)
            {
                is_sending = true;

                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                });

                switch (currentPageType)
                {
                    case PageType.Normal:
                        FanfouAPI.StatusUpdate(this.Status.Text, location: this.position);
                        break;
                    case PageType.Resend:
                        FanfouAPI.StatusUpdate(this.Status.Text, "", "", status.id, location: this.position);
                        break;
                    case PageType.Reply:
                        FanfouAPI.StatusUpdate(this.Status.Text, status.id, status.user.id, location: this.position);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}