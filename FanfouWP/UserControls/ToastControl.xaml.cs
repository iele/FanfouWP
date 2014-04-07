using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using Coding4Fun.Toolkit.Controls;

namespace FanfouWP.UserControls
{
    public partial class ToastControl : UserControl
    {
        public ToastControl()
        {
            InitializeComponent();

            this.Loaded += ToastControl_Loaded;
        }

        public void NewToast(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LayoutRoot.Visibility = Visibility.Visible;

                ToastIn.Stop();
                ToastOut.Stop();
                this.Text.Text = text;
                ToastIn.Begin();
            });
        }

        void ToastControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LayoutRoot.Visibility = Visibility.Collapsed;
                this.LayoutRoot.RenderTransform = this.TranslateTransform;
            });
        }

        private void ToastIn_Completed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
              {
                  this.ToastOut.Begin();
              });
        }

        private void ToastOut_Completed(object sender, EventArgs e)
        {
            this.LayoutRoot.Visibility = Visibility.Collapsed;
        }
    }
}
