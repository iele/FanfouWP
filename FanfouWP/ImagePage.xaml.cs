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

namespace FanfouWP
{
    public partial class ImagePage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.Status status;
        private double initialAngle;
        private double initialScale;
        public ImagePage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("ImagePage"))
            {
                status = PhoneApplicationService.Current.State["ImagePage"] as FanfouWP.API.Items.Status;

                this.DataContext = status;
            }
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
        }

        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            transform.TranslateX = transform.TranslateY = 0;
            transform.Rotation = 0;
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
            initialAngle = transform.Rotation;
            initialScale = transform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            transform.Rotation = initialAngle + e.TotalAngleDelta;
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
            string fileName = "save-image.jpg";
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(fileName))
                myStore.DeleteFile(fileName);
            IsolatedStorageFileStream myFileStream = myStore.CreateFile(fileName);

            var source = image.Source;
            var bitmap = new WriteableBitmap((BitmapImage)source);

            bitmap.SaveJpeg(myFileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 85);
            myFileStream.Close();
            myFileStream = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            MediaLibrary library = new MediaLibrary();
            Picture pic = library.SavePicture("FanfouSavedPicture-" + status.id + ".jpg", myFileStream);
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
        }

    }
}