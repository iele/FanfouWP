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
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Media;
using Coding4Fun.Toolkit.Controls;

namespace FanfouWP
{
    public partial class SendPage : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;

        private enum PageType { Normal, Resend, Reply };
        private PageType currentPageType = PageType.Normal;

        private dynamic is_sending = false;
        private FanfouWP.API.Items.Status status;

        private bool is_image;
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
                is_image = true;
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

            if (is_image == true)
            {
                addPicture();
            }
        }

        private void addPicture()
        {
            PhotoChooserTask cpt = new PhotoChooserTask();
            cpt.Completed += (s, e2) =>
            {
                if (e2.TaskResult == TaskResult.Cancel)
                {
                    return;
                }

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e2.ChosenPhoto);
                this.image = new WriteableBitmap(bitmapImage);
                this.Image.Source = image;
            };
            cpt.Show();
        }


        void FanfouAPI_PhotosUploadFailed(object sender, API.Event.FailedEventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("照片上传失败:( " + e.error.error);
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
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("消息发送失败:( " + e.error.error);
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
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                });

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
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
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

        private void PictureButton_Click(object sender, EventArgs e)
        {
            addPicture();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            var input = new InputPrompt
            {
                Title = "话题",
                Message = "输入你想提到的话题",
            };
            input.Completed += (s, e1) =>
            {
                if (e1.PopUpResult == PopUpResult.Ok)
                {
                    this.Status.Text = this.Status.Text + "#" + e1.Result + "#";
                }
            };

            input.Show();
        }

        private void MusicButton_Click(object sender, EventArgs e)
        {
            try
            {
                string name = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong.Name;
                string album = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong.Album.Name;

                if (name != null && album != null)
                {
                    this.Status.Text = this.Status.Text + "#我正在听#" + name + " - " + album;
                    return;
                }
                else
                {
                    goto Noplaying;
                }
            }
            catch (Exception) { }

        Noplaying:
            MessageBox.Show("当前没有播放音乐:(");

        }
    }
}