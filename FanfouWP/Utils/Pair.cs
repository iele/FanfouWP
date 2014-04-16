using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
    public class Pair<TFirst, TSecond>
    {
        public Pair(TFirst First, TSecond Second) {
            this.First = First;
            this.Second = Second;        
        }


        public TFirst First { get; set; }
        public TSecond Second { get; set; }
    }
}
