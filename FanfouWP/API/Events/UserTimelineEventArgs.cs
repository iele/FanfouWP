using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.API.Event
{
    public class UserTimelineEventArgs : EventArgs
    {
        public ObservableCollection<FanfouWP.API.Items.Status> UserStatus { get; set; }
    }
}
