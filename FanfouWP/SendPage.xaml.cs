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
using FanfouWP.Storage;
using FanfouWP.Utils;
using Microsoft.Xna.Framework.Media;
using Windows.Storage;
using System.Runtime.Serialization.Json;

namespace FanfouWP
{
    public partial class SendPage : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;

        private enum PageType { Normal, Resend, Reply, ReplyWithoutStatus };
        private PageType currentPageType = PageType.Normal;

        private dynamic is_sending = false;
        private FanfouWP.API.Items.Status status;
        private FanfouWP.API.Items.User reply_user;

        private bool is_image;
        private WriteableBitmap image;
        private string text;

        private string position = "";

        private ToastUtil toast = new ToastUtil();

        public SendPage()
        {
            InitializeComponent();

            FanfouAPI.StatusUpdateSuccess += FanfouAPI_StatusUpdateSuccess;
            FanfouAPI.StatusUpdateFailed += FanfouAPI_StatusUpdateFailed;
            FanfouAPI.PhotosUploadSuccess += FanfouAPI_PhotosUploadSuccess;
            FanfouAPI.PhotosUploadFailed += FanfouAPI_PhotosUploadFailed;

            this.Loaded += SendPage_Loaded;

            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                status = PhoneApplicationService.Current.State["ReSend"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("ReSend");
                currentPageType = PageType.Resend;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("ReplyWithoutStatus"))
            {
                reply_user = PhoneApplicationService.Current.State["ReplyWithoutStatus"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("ReplyWithoutStatus");
                currentPageType = PageType.ReplyWithoutStatus;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                status = PhoneApplicationService.Current.State["Reply"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("Reply");
                currentPageType = PageType.Reply;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Image"))
            {
                PhoneApplicationService.Current.State.Remove("SendPage_Image");
                is_image = true;
            }
        }

        private void SendPage_Loaded(object sender, RoutedEventArgs e)
        {

            Dispatcher.BeginInvoke(async () =>
            {
                if (SettingManager.GetInstance().enableLocation == true)
                {
                    var result = await FanfouWP.Utils.GeoLocatorUtils.getGeolocator();
                    this.position = result.Second;
                    if (result.First == true)
                        this.location.Text = "已定位";
                    else
                    {
                        this.location.Text = result.Second;
                    }
                }
                else
                {
                    this.location.Text = "";
                }
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
                        try
                        {
                            this.Status.Text = ("转@" + status.user.screen_name + " " + this.status.text).Substring(0, 140);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            this.Status.Text = "转@" + status.user.screen_name + " " + this.status.text;
                        }
                        this.Status.SelectionStart = 0;
                    });
                    break;
                case PageType.Reply:
                    Dispatcher.BeginInvoke(() =>
                    {
                        titleText.Text = "回复" + status.user.screen_name;
                        this.Status.Text = "@" + this.status.user.screen_name + " ";
                    });
                    break;
                case PageType.ReplyWithoutStatus:
                    Dispatcher.BeginInvoke(() =>
                    {
                        titleText.Text = "回复" + reply_user.screen_name;
                        if (!this.Status.Text.StartsWith("@" + this.reply_user.screen_name))
                            this.Status.Text = "@" + this.reply_user.screen_name + " " + this.Status.Text;
                        else
                        {
                            this.Status.Text = text;
                        }
                    });
                    break;
                default:
                    break;
            }

            if (is_image == true)
            {
                if (this.image == null)
                    addPicture();
            }
        }

        private void addPicture()
        {
            PhotoChooserTask cpt = new PhotoChooserTask();
            cpt.ShowCamera = true;
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
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("照片上传失败:( " + e.error.error);
            });
        }

        void FanfouAPI_PhotosUploadSuccess(object sender, EventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                ToastPrompt tp = new ToastPrompt();
                tp.Title = "饭窗";
                tp.Message = "照片上传成功:)";
                tp.MillisecondsUntilHidden = 1000;
                tp.Completed += (s2, e2) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.loading.Visibility = System.Windows.Visibility.Collapsed;

                        if (NavigationService.CurrentSource == new Uri("/SendPage.xaml", UriKind.Relative) && NavigationService.CanGoBack)
                        {
                            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage_To"))
                            {
                                PhoneApplicationService.Current.State.Remove("TimelinePage_To");
                            }
                            PhoneApplicationService.Current.State.Add("TimelinePage_To", "");
                            this.NavigationService.GoBack();
                        }
                        else
                        {
                            this.Status.Text = "";
                            this.image = null;
                            this.Image.Source = null;

                            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                        }
                    });
                };
                tp.Show();
            });
        }

        void FanfouAPI_StatusUpdateFailed(object sender, API.Event.FailedEventArgs e)
        {
            is_sending = false;

            Dispatcher.BeginInvoke(() =>
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("状态发送失败:( " + e.error.error);
            });
        }

        void FanfouAPI_StatusUpdateSuccess(object sender, EventArgs e)
        {
            is_sending = false;
            Dispatcher.BeginInvoke(() =>
            {
                ToastPrompt tp = new ToastPrompt();
                tp.Title = "饭窗";
                tp.Message = "状态发送成功:)";
                tp.MillisecondsUntilHidden = 1000;
                tp.Completed += (s2, e2) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        this.loading.Visibility = System.Windows.Visibility.Collapsed;
                        if (NavigationService.CurrentSource == new Uri("/SendPage.xaml", UriKind.Relative) && NavigationService.CanGoBack)
                        {
                            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage_To"))
                            {
                                PhoneApplicationService.Current.State.Remove("TimelinePage_To");
                            }
                            PhoneApplicationService.Current.State.Add("TimelinePage_To", "");
                            this.NavigationService.GoBack();
                        }
                        else
                        {
                            this.Status.Text = "";
                            this.image = null;
                            this.Image.Source = null;

                            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
                        }
                    });
                };
                tp.Show();
            });
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            this.Focus();
            if (image != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                });

                FanfouAPI.PhotoUpload(this.Status.Text, image, position);
                return;
            }

            if (this.Status.Text == "")
            {
                this.toast.NewToast("无法发送空消息:(");
                return;
            }

            if (!is_sending)
            {
                is_sending = true;

                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                    (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
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
                    case PageType.ReplyWithoutStatus:
                        FanfouAPI.StatusUpdate(this.Status.Text, in_reply_to_user_id: reply_user.id, location: this.position);
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
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }

        Noplaying:
            MessageBox.Show("当前没有播放音乐:(");

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["SendPage_status"] = this.status;
                State["SendPage_reply_user"] = this.reply_user;
                State["SendPage_is_image"] = this.is_image;
                State["SendPage_position"] = this.position;
                State["SendPage_currentPageType"] = this.currentPageType;
                text = this.Status.Text;
                State["SendPage_text"] = text;
                if (image != null)
                {
                    var ms = new MemoryStream();
                    image.SaveJpeg(ms, image.PixelWidth, image.PixelHeight, 0, 100);
                    Dispatcher.BeginInvoke(async () =>
                    {
                        try
                        {
                            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                            var dataFolder = await localFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
                            using (var writeStream = await dataFolder.OpenStreamForWriteAsync("SendImageSave.jpg", CreationCollisionOption.ReplaceExisting))
                            {
                                ms.CopyTo(writeStream);
                            }
                            State["SendPage_image"] = true;
                        }
                        catch (Exception exception)
                        {
                            System.Diagnostics.Debug.WriteLine(exception.Message);
                        }
                    });
                }
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (State.ContainsKey("SendPage_status"))
                this.status = State["SendPage_status"] as FanfouWP.API.Items.Status;
            if (State.ContainsKey("SendPage_reply_user"))
                this.reply_user = State["SendPage_reply_user"] as FanfouWP.API.Items.User;
            if (State.ContainsKey("SendPage_is_image"))
                this.is_image = (bool)State["SendPage_is_image"];
            if (State.ContainsKey("SendPage_image"))
            {
                var ms = new MemoryStream();
                Dispatcher.BeginInvoke(async () =>
                {
                    try
                    {
                        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                        var dataFolder = await localFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
                        using (var readStream = await dataFolder.OpenStreamForReadAsync("SendImageSave.jpg"))
                        {
                            readStream.CopyTo(ms);
                       }

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(ms);
                        image = new WriteableBitmap(bitmap);
                        this.Image.Source = image;
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                    }
                });
            }
            if (State.ContainsKey("SendPage_position"))
                this.position = State["SendPage_position"] as string;
            if (State.ContainsKey("SendPage_currentPageType"))
                this.currentPageType = (PageType)State["SendPage_currentPageType"];
            if (State.ContainsKey("SendPage_text"))
            {
                text = State["SendPage_text"] as string;
                this.Status.Text = text;
            }
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {

                if (NavigationContext.QueryString.ContainsKey("voiceCommandName"))
                {

                    string voiceCommandName
                      = NavigationContext.QueryString["voiceCommandName"];

                    switch (voiceCommandName)
                    {
                        case "发送新消息":
                            Dispatcher.BeginInvoke(() =>
                            {
                                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                            });
                            FanfouAPI.RestoreDataSuccess += FanfouAPI_RestoreDataSuccess;
                            FanfouAPI.RestoreDataFailed += FanfouAPI_RestoreDataFailed;
                            Dispatcher.BeginInvoke(async () => await FanfouAPI.TryRestoreData());
                            break;
                        case "发送图片":
                            Dispatcher.BeginInvoke(() =>
                            {
                                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                                (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                            });
                            is_image = true;
                            FanfouAPI.RestoreDataSuccess += FanfouAPI_RestoreDataSuccess;
                            FanfouAPI.RestoreDataFailed += FanfouAPI_RestoreDataFailed;
                            Dispatcher.BeginInvoke(async () => await FanfouAPI.TryRestoreData());
                            break;
                        default:

                            break;
                    }
                }
                else if (NavigationContext.QueryString.ContainsKey("FromTile"))
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                        (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                        (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                        (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                    });
                    FanfouAPI.RestoreDataSuccess += FanfouAPI_RestoreDataSuccess;
                    FanfouAPI.RestoreDataFailed += FanfouAPI_RestoreDataFailed;
                    Dispatcher.BeginInvoke(async () => await FanfouAPI.TryRestoreData());

                }
                else
                {
                    IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;
                    if (queryStrings.ContainsKey("FileId"))
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
                            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                            (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
                            MediaLibrary library = new MediaLibrary();
                            Picture photoFromLibrary = library.GetPictureFromToken(queryStrings["FileId"]);
                            BitmapImage bitmapFromPhoto = new BitmapImage();
                            bitmapFromPhoto.SetSource(photoFromLibrary.GetImage());
                            this.Image.Source = bitmapFromPhoto;
                            this.image = new WriteableBitmap(bitmapFromPhoto);
                            this.is_image = true;
                        });
                        FanfouAPI.RestoreDataSuccess += FanfouAPI_RestoreDataSuccess;
                        FanfouAPI.RestoreDataFailed += FanfouAPI_RestoreDataFailed;
                        Dispatcher.BeginInvoke(async () => await FanfouAPI.TryRestoreData());

                    }
                }
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Friend"))
                {
                    reply_user = PhoneApplicationService.Current.State["SendPage_Friend"] as FanfouWP.API.Items.User;
                    this.currentPageType = PageType.ReplyWithoutStatus;
                }
            }

            base.OnNavigatedTo(e);
        }

        void FanfouAPI_RestoreDataFailed(object sender, API.Event.FailedEventArgs e)
        {
            MessageBox.Show("你现在都没登录呢,怎么直接发送消息呢。。", "没登录怎么办?", MessageBoxButton.OK);
        }

        void FanfouAPI_RestoreDataSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        private void TileButton_Click(object sender, EventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("FromTile=true"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {
                StandardTileData data = new StandardTileData();
                data.BackgroundImage = new Uri("/Assets/icon-send.png", UriKind.Relative);
                data.Title = "饭窗 - 发送状态";
                ShellTile.Create(new Uri("/SendPage.xaml?FromTile=true", UriKind.Relative), data);
                ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("FromTile=true"));
            }
        }

        private void AtItem_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("MentionUserPage"))
            {
                PhoneApplicationService.Current.State.Remove("MentionUserPage");
            }
            PhoneApplicationService.Current.State.Add("MentionUserPage", FanfouAPI.Instance.CurrentUser);
            NavigationService.Navigate(new Uri("/MentionUserPage.xaml", UriKind.Relative));
        }

    }

}