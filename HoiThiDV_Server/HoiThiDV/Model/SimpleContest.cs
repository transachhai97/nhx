﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{
    [Serializable]
    public class SimpleContest
    {
        public Socket socket { get; set; }
        public int id { get; set; }
    }
}