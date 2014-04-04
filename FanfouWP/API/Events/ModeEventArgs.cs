using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.API.Event
{
    public class ModeEventArgs : EventArgs
    {
        public FanfouAPI.RefreshMode RefreshMode{ get; set; }

        public ModeEventArgs() { }
     
        public ModeEventArgs(FanfouAPI.RefreshMode mode)
        {
            this.RefreshMode = mode;
        }
    }
}
