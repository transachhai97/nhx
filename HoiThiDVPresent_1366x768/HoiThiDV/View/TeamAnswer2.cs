using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoiThiDV.Model;

namespace HoiThiDV.View
{
    public partial class TeamAnswer2 : UserControl
    {
        private MasterController ctrl;
        public TeamAnswer2()
        {
            InitializeComponent();

            CompDisableLocation("_lblAnswer", -400, 648);
            CompDisableLocation("lblAnswerCT", -400, 648);
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }
        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            comp.Text = text;
        }
        #region Init TeamAnswer
        public void InitTeamAnswer(DataServer data)
        {
            CompDisableLocation("_lblAnswer", -400, 648);
            CompDisableLocation("lblAnswerCT", -400, 648);

            if (data != null)
            {
                if (data.ContestTeams != null)
                {
                    if (data.ContestTeams.Count < 5)
                    {
                        CompDisableLocation("pnAnswerTeam5", -1032, 21);
                        CompDisableLocation("pnParentAnswerTeam", 177, 123);
                    }
                    else
                    {
                        CompDisableLocation("pnAnswerTeam5", 1032, 20);
                        CompDisableLocation("pnParentAnswerTeam", 30, 120);
                    }
                }
            }

            if (data != null && data.ContestTeams != null)
            {
                data.ContestTeams = data.ContestTeams.OrderBy(n => n.Time).ToList();
                if (data.ContestTeams.Count > 0)
                {
                    R3_Team1.Text = data.ContestTeams[0].Name + " (" + data.ContestTeams[0].Mark + ")";
                    R3_Answer1.Text = data.ContestTeams[0].Answer;
                    R3_Time1.Text = data.ContestTeams[0].Time;
                }
                if (data.ContestTeams.Count > 1)
                {
                    R3_Team2.Text = data.ContestTeams[1].Name + " (" + data.ContestTeams[1].Mark + ")";
                    R3_Answer2.Text = data.ContestTeams[1].Answer;
                    R3_Time2.Text = data.ContestTeams[1].Time;
                }
                if (data.ContestTeams.Count > 2)
                {
                    R3_Team3.Text = data.ContestTeams[2].Name + " (" + data.ContestTeams[2].Mark + ")";
                    R3_Answer3.Text = data.ContestTeams[2].Answer;
                    R3_Time3.Text = data.ContestTeams[2].Time;
                }
                if (data.ContestTeams.Count > 3)
                {
                    R3_Team4.Text = data.ContestTeams[3].Name + " (" + data.ContestTeams[3].Mark + ")";
                    R3_Answer4.Text = data.ContestTeams[3].Answer;
                    R3_Time4.Text = data.ContestTeams[3].Time;
                }
                if (data.ContestTeams.Count > 4)
                {
                    R3_Team5.Text = data.ContestTeams[4].Name + " (" + data.ContestTeams[4].Mark + ")";
                    R3_Answer5.Text = data.ContestTeams[4].Answer;
                    R3_Time5.Text = data.ContestTeams[4].Time;
                }
            }

        }
        #endregion

        public void ShowAnswerCT(DataServer data)
        {
            CompDisableLocation("_lblAnswer", 47, 648);
            CompDisableLocation("lblAnswerCT", 423, 648);

            lblAnswerCT.Text = data.QuestionAnswer;
        }

        private void TeamAnswer2_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        public void CompDisableVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
        }
        public void CompDisableLocation(string compName, int x, int y)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Location = new Point(x, y);
        }
    }
}
