using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{
    [Serializable]
    public class Question
    {
        public int QuestNum { get; set; }
        public string QuestContent { get; set; }
        public string QuestAnswer { get; set; }
        public int QuestRound { get; set; }

        public Question() { }

        public Question(int questRound, int questNum)
        {
            this.QuestRound = questRound;
            this.QuestNum = questNum;
        }
    }
}
