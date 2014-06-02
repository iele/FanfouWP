using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FanfouWP.Utils
{
    public class Toast
    {
        public void NewToast(string msg)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ToastPrompt tp = new ToastPrompt();
                tp.Title = "饭窗";
                tp.Message = msg;
                tp.MillisecondsUntilHidden = 1000;
                tp.Show();
            });
        }
    }
}
