using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{
    [Serializable]
    public class Judges
    {
        public Judges() { }

        public Judges(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public string Mark { get; set; }
    }
}
