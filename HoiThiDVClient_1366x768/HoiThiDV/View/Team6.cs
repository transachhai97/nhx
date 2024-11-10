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
    public partial class Team6 : UserControl
    {
        private MasterController ctrl;
        private int widthUserControl;
        public Team6()
        {
            InitializeComponent();

        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }
        public void setData(DataServer data)
        {
            lblTitleBGK.Text = data.title;
            data.ContestTeams = data.ContestTeams.OrderBy(n => n.Id).ToList();

            if (data.arrJudgesTeams != null)
            {
                for (int j = 1; j <= data.arrJudgesTeams.Count; j++)
                {
                    for (int i = 1; i <= data.arrJudgesTeams[j].Count; i++)
                    {
                        Label txtTeam = (Label)this.Controls.Find("bgkTeam" + j, true)[0];
                        txtTeam.Text = data.ContestTeams[j - 1].Name;

                        Label txt = (Label)this.Controls.Find("txtTeam" + i, true)[0];
                        txt.Text = data.arrJudgesTeams[j][i - 1].Name;

                        Label txtMarkGK = (Label)this.Controls.Find("txtMarkGK" + j + i, true)[0];
                        txtMarkGK.Text = data.arrJudgesTeams[j][i - 1].Mark;
                    }
                }
            }

            //ẩn dòng thứ 5 nếu lượt thi có 4 đội
            if (data.ContestTeams.Count < 5)
            {
                bgkTeam5.Text = "";
                CompBackColor("bgkTeam5", Color.Transparent);
                for (int i = 1; i <= 6; i++)
                {
                    SetTextComp("txtMarkGK5" + i, "");
                    CompBackColor("txtMarkGK5" + i, Color.Transparent);
                }
            }
            else if (data.ContestTeams.Count == 5)
            {
                CompBackColor("bgkTeam5", Color.FromArgb(255, 198, 47));
                for (int i = 1; i <= 6; i++)
                {
                    CompBackColor("txtMarkGK5" + i, Color.FromArgb(0, 51, 48));
                }
            }
        }
        public void resetDiemBGK()
        {
            //reset điểm của ban giám khảo
            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 5; j++)
                {
                    SetTextComp("txtMarkGK" + j + i, "");
                    SetTextComp("bgkTeam" + j, ""); 
                }
            }
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

        private void Team6_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }
        public void CompBackColor(string compName, Color color)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.BackColor = color;
        }
        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            comp.Text = text;
        }
    }
}
