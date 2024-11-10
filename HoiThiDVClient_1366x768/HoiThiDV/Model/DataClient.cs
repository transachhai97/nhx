using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{

    [Serializable]
    public class DataClient
    {
        public String serverIp { get; set; }
        public int teamId { get; set; }
        public String teamAnswer { get; set; }
        public String timeAnswer { get; set; }
        public bool isInit { get; set; }

        public int round { get; set; }
    }
}
