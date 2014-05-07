using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
    public class ToastUtil
    {
        public void NewToast(string msg) {
            ToastPrompt tp = new ToastPrompt();
            tp.Title = "饭窗";
            tp.Message = msg;
            tp.MillisecondsUntilHidden = 1000;
            tp.Show();
        }
    }
}
