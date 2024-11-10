using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{
    [Serializable]
    public class Contestant
    {
        public Contestant() { }

        public Contestant(string name)
        {
            this.Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float Mark { get; set; }
        public string Answer { get; set; }
        public string Time { get; set; }
        public float BonusPoint { get; set; }
    }
}
