﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.API.Items
{
    public class Error : Item
    {
        public string request { get; set; }
        public string error { get; set; }
    }
}
