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
    public class KhoiDongController
    {
        private FormServer view;
        private QuestionDAO qdao;
        private GamePlayDAO gdao;
        public KhoiDongController(FormServer view)
        {
            this.view = view;
            this.qdao = new QuestionDAO();
            this.gdao = new GamePlayDAO();
        }

        public void InitRound1()
        {

        }

        public Question loadRound1Question(int chooseRound)
        {
            Question q = new Question(chooseRound, this.view.currentQuest);
            q.QuestAnswer = "";
            DataTable dt = qdao.GetQuest(q);
            if (dt.Rows.Count > 0)
            {
                q.QuestContent = dt.Rows[0][1].ToString();
                q.QuestAnswer = dt.Rows[0][2].ToString();
                this.view.setRound1Question(q);
            }
            return q;
        }

        public void showTeamAnswer(DataClient data)
        {
            this.view.SetTextComp(1, "R1_TeamAnswer" + data.teamId, data.teamAnswer);
            this.view.SetTextComp(1, "R1_TimeAnswer" + data.teamId, data.timeAnswer);
            //ghi vaodb
            //this.gdao.InsertAnswer(data);
        }
    }
}
