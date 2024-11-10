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
    public partial class Team6VD : UserControl
    {
        private MasterController ctrl;
        private int widthUserControl;
        public Team6VD()
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
            bgkTeam1.Text = "Đội: " + data.nameOneTeam;

            for (int i = 1; i <= 6; i++)
            {
                if (data.arrJudgesTeams[1].Count != 0)
                {
                    Label txt = (Label)this.Controls.Find("txtTeam" + i, true)[0];
                    txt.Text = data.arrJudgesTeams[1][i - 1].Name;

                    Label txtMarkGK = (Label)this.Controls.Find("txtMarkGK1" + i, true)[0];
                    txtMarkGK.Text = data.arrJudgesTeams[1][i - 1].Mark;

                    if (data.arrJudgesTeams[1].Count == i)
                    {
                        break;
                    }
                }
            }
        }
        public void resetDiemBGK()
        {
            //reset điểm của ban giám khảo
            bgkTeam1.Text = "";
            for (int i = 1; i <= 6; i++)
            {
                SetTextComp("txtMarkGK1" + i, "");
                SetTextComp("bgkTeam1", "");
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

        private void Team6VD_Load(object sender, EventArgs e)
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
