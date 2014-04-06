using FanfouWP.API.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.API.Event
{
    public class FailedEventArgs : EventArgs
    {
        private Error _error;
        public Error error
        {
            get {
                return _error;
            }
            set
            {
            if (value.error == null)
                value.error = "";
            _error = value;
        } }


        public FailedEventArgs() { }
        public FailedEventArgs(Error error)
        {
            this.error = error;
        }
    }
}
