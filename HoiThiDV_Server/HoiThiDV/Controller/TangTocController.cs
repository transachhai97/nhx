using HoiThiDV.DAO;
using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Controller
{
    public class TangTocController
    {
        private FormServer view;
        private QuestionDAO qdao;
        private GamePlayDAO gdao;
        public TangTocController(FormServer view)
        {
            this.view = view;
            qdao = new QuestionDAO();
            gdao = new GamePlayDAO();
        }

        public Question loadRound2Question()
        {
            Question q = new Question(2, this.view.currentQuest);
            q.QuestAnswer = "";
            DataTable dt = qdao.GetQuest(q);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            if (dt.Rows.Count > 0)
            {
                q.QuestContent = dt.Rows[0][1].ToString();
                q.QuestAnswer = dt.Rows[0][2].ToString();
                q.QuestAnswer_vi = dt.Rows[0][3].ToString();
                this.view.setRound2Question(q);
            }
            return q;
        }

        public void showTeamAnswer(DataClient data, DataServer server)
        {
            if (server.outTeam == null || !server.outTeam.Contains(data.teamId.ToString()))
            {
                this.view.SetTextComp(2, "R2_TeamAnswer" + data.teamId, data.teamAnswer);
                this.view.SetTextComp(2, "R2_TimeAnswer" + data.teamId, data.timeAnswer);
            }
            
            //ghi vaodb
            //this.gdao.InsertAnswer(data);
        }

        public void loadOChu(ref DataServer data)
        {
            data.InitOchu = new List<string>();
            data.dicShowQuestnumRound2 = new Dictionary<int, string>();
            string temp = String.Empty;
            Question q = new Question();
            DataTable dt = qdao.GetAllQuest(2);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            foreach (DataRow row in dt.Rows)
            {
                temp = new String(row[2].ToString().Select(r => r == '$' ? '$' : '#').ToArray());
                data.InitOchu.Add(temp);
                data.dicShowQuestnumRound2.Add(int.Parse(row[0].ToString()), row[4].ToString());
            }
        }

        public void loadHangDoc(ref DataServer data)
        {
            data.HangDoc = new List<string>();
            string temp = String.Empty;
            Question q = new Question();
            DataTable dt = qdao.GetAllQuest(2);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            foreach (DataRow row in dt.Rows)
            {
                temp = row[2].ToString();
                data.HangDoc.Add(temp[14].ToString());
            }
        }

        public void loadRound2(ref DataServer data)
        {
            data.InitOchu = new List<string>();
            data.dicShowQuestnumRound2 = new Dictionary<int, string>();
            string temp = String.Empty;
            Question q = new Question();
            DataTable dt = qdao.GetAllQuest(2);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            foreach (DataRow row in dt.Rows)
            {
                temp = new String(row[2].ToString().Select(r => r == '$' ? '$' : '#').ToArray());
                data.InitOchu.Add(temp);
                data.dicShowQuestnumRound2.Add(int.Parse(row[0].ToString()), row[4].ToString());
            }
            this.view.InitRound2(data);
        }
    }
}
