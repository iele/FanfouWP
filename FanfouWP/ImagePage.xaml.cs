using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using System.IO;
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class ImagePage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.Status status;
        private double initialScale;

        private Toast toast = new Toast();

        public ImagePage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("ImagePage"))
            {
                status = PhoneApplicationService.Current.State["ImagePage"] as FanfouWP.API.Items.Status;

                this.DataContext = status;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                if (State.ContainsKey("ImagePage_status"))
                    State["ImagePage_status"] = this.status;
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("ImagePage_status"))
                this.status = State["ImagePage_status"] as API.Items.Status;

            if (status != null && status.photo != null)
            {
                var converter = new FanfouWP.ItemControls.ValueConverter.ImageSourceToCacheConverter();
                converter.ImageCompleted += (s, e2) => Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = Visibility.Collapsed;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                });
                BitmapImage bm = new BitmapImage();
                bm = (BitmapImage)converter.Convert(this.status.photo.largeurl, bm.GetType(), null, System.Globalization.CultureInfo.CurrentCulture);
                this.image.Source = bm;
            }
            base.OnNavigatedTo(e);
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
        }

        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            transform.TranslateX = transform.TranslateY = 0;
            transform.ScaleX = transform.ScaleY = 1;
        }

        private void OnHold(object sender, GestureEventArgs e)
        {
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            transform.TranslateX += e.HorizontalChange;
            transform.TranslateY += e.VerticalChange;
        }

        private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
        }

        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            Point point0 = e.GetPosition(image, 0);
            Point point1 = e.GetPosition(image, 1);
            Point midpoint = new Point((point0.X + point1.X) / 2, (point0.Y + point1.Y) / 2);
            image.RenderTransformOrigin = new Point(midpoint.X / image.ActualWidth, midpoint.Y / image.ActualHeight);
            initialScale = transform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            transform.ScaleX = transform.ScaleY = initialScale * e.DistanceRatio;
        }

        private void OnPinchCompleted(object sender, PinchGestureEventArgs e)
        {
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            var ms = new MemoryStream();
            var source = image.Source;
            var bitmap = new WriteableBitmap((BitmapImage)source);
            bitmap.SaveJpeg(ms, bitmap.PixelWidth, bitmap.PixelHeight, 0, 85);
            ms.Seek(0, SeekOrigin.Begin);
            MediaLibrary library = new MediaLibrary();
            Picture pic = library.SavePicture("FanfouSavedPicture-" + status.id + ".jpg", ms);
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            Dispatcher.BeginInvoke(() => this.toast.NewToast("图片已保存:)"));
        }

    }
}