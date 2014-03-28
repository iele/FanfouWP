using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Nokia.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Microsoft.Xna.Framework.Media;
using FanfouWP.ItemControls;
using System.IO;
using Windows.Storage;
using System.Windows.Media;

namespace FanfouWP
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        private IFilter[] filters = { new CartoonFilter(), new FogFilter(), new CurvesFilter(), new ChromaKeyFilter() };

        private string[] names = { "Cartoon", "Fog", "Curves", "ChromaKey" };

        private WriteableBitmap backup;

        private WriteableBitmap target;
        public PhotoPage()
        {
            InitializeComponent();

            this.Loaded += PhotoPage_Loaded;

        }

        void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChoose = new PhotoChooserTask();
            photoChoose.ShowCamera = true;
            photoChoose.Show();
            photoChoose.Completed += photoChoose_Completed;

        }

        void photoChoose_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                GetPicture(e.ChosenPhoto);
            }
            else
            {
                this.NavigationService.GoBack();
            }
        }

        private async void GetPicture(Stream stream)
        {


            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var dataFolder = await localFolder.CreateFolderAsync("photo", CreationCollisionOption.OpenIfExists);
            using (var writeStream = await dataFolder.OpenStreamForWriteAsync("photo", CreationCollisionOption.ReplaceExisting))
            {
                stream.CopyTo(writeStream);
            }

            using (var readStream = await dataFolder.OpenStreamForReadAsync("photo"))
            {
                var image = new BitmapImage();
                image.SetSource(stream);
                backup = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                backup.SetSource(stream);
                target = new WriteableBitmap(image.DecodePixelWidth, image.DecodePixelHeight);
                target.SetSource(stream);
                this.Image.Source = target;

                this.ListPicker.ItemsSource = names;
            }
        }

        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListPicker.SelectedIndex != -1)
            {
                IFilter filter = filters[this.ListPicker.SelectedIndex];
                Dispatcher.BeginInvoke(async () =>
                {
                    Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    var dataFolder = await localFolder.CreateFolderAsync("photo", CreationCollisionOption.OpenIfExists);

                    using (var readStream = await dataFolder.OpenStreamForReadAsync("photo"))
                    {

                        using (var fs = new FilterEffect(new StreamImageSource(readStream)))
                        {
                            fs.Filters = new IFilter[] { filter };
                            target = new WriteableBitmap((int)Image.ActualWidth, (int)Image.ActualHeight);
                            using (var renderer = new WriteableBitmapRenderer(fs, target))
                            {
                                await renderer.RenderAsync();
                                Image.Source = target;
                            }
                        }
                    }
                });

            }
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Image"))
            {
                PhoneApplicationService.Current.State.Remove("SendPage_Image");
            }
            PhoneApplicationService.Current.State.Add("SendPage_Image", target);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

    }
}