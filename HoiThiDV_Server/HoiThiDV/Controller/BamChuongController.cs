using HoiThiDV.DAO;
using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HoiThiDV.Controller
{
    public class BamChuongController
    {
        private FormServer view;
        private QuestionDAO qdao;
        private GamePlayDAO gdao;
        public BamChuongController(FormServer view)
        {
            this.view = view;
            this.qdao = new QuestionDAO();
            this.gdao = new GamePlayDAO();
        }

        public Question loadRoundBCQuestion(int chooseRound)
        {
            Question q = new Question(chooseRound, this.view.currentQuest);
            q.QuestAnswer = "";
            DataTable dt = qdao.GetQuest(q);
            if (dt.Rows.Count > 0)
            {
                q.QuestContent = dt.Rows[0][1].ToString();
                q.QuestAnswer = dt.Rows[0][2].ToString();
                this.view.setRoundBCQuestion(q);
            }
            return q;
        }

        public void showTeamAnswer(DataClient data)
        {
            this.view.SetTextComp(5, "R_BC_TeamAnswer" + data.teamId, data.teamAnswer);
            this.view.SetTextComp(5, "R_BC_TimeAnswer" + data.teamId, data.timeAnswer);
            //ghi vaodb
            //this.gdao.InsertAnswer(data);
        }

        public void showPerBell(DataClient data)//show người bấm chuông
        {
            this.view.CompBackColor("R_BC_LblTeam" + data.teamId, Color.FromArgb(0, 102, 255));
        }
    }
}
