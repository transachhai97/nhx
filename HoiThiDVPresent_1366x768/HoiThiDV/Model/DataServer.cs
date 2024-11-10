using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Model
{
    [Serializable]
    public class DataServer
    {
        public List<Contestant> ContestTeams { get; set; }
        public List<Judges> JudgesTeams { get; set; }
        public Dictionary<int, List<Judges>> arrJudgesTeams { get; set; }
        public int Round { get; set; }

        public int check { get; set; }

        public string Action { get; set; }
        public string QuestionText { get; set; }
        public int QuestionNum { get; set; }
        public string QuestionAnswer { get; set; }
        public List<string> InitOchu { get; set; }
        public List<string> HangDoc { get; set; }
        public DateTime startTime { get; set; }
        public List<int> questRound1 { get; set; }
        public List<int> questRound2 { get; set; }
        public bool isOpenCol { get; set; }
        public bool clientState { get; set; }
        public string description { get; set; }
        public int minutes { get; set; }
        public int team { get; set; }
        public bool status { get; set; }
        public int questionRound { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public int point { get; set; }
        public int oneTeam { get; set; }
        public string nameOneTeam { get; set; }
        public int checkBC { get; set; }
        public string flagTimeRound2 { get; set; }
        public int[] myArrayRound2 { get; set; }
        public string[] myArrayShowRound2 { get; set; }
        public string[] outTeam { get; set; }
        public int[] lock_unlockTeam { get; set; }
        public Dictionary<int, string> dicShowQuestnumRound2 { get; set; }
    }
}
