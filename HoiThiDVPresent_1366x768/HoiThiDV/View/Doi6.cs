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
    public partial class Doi6 : UserControl
    {
        private MasterController ctrl;
        private int widthUserControl;
        public Doi6()
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
            if (data.Round == 6)
            {
                if (data.JudgesTeams != null)
                {
                    //data.JudgesTeams = data.JudgesTeams.OrderBy(n => n.Name).ToList();
                    for (int i = 1; i <= data.JudgesTeams.Count; i++)
                    {
                        Label txt = (Label)this.Controls.Find("txtTeam" + i, true)[0];
                        txt.Text = data.JudgesTeams[i - 1].Name;

                        Label txtMarkGK = (Label)this.Controls.Find("txtMarkTeam" + i, true)[0];
                        txtMarkGK.Text = data.JudgesTeams[i - 1].Mark;
                    }
                }
            }
            else
            {
                if (data.ContestTeams != null)
                {
                    data.ContestTeams = data.ContestTeams.OrderBy(n => n.Id).ToList();
                    for (int i = 1; i <= data.ContestTeams.Count; i++)
                    {
                        Label txt = (Label)this.Controls.Find("txtTeam" + i, true)[0];
                        txt.Text = data.ContestTeams[i - 1].Name;

                        Label txtMarkGK = (Label)this.Controls.Find("txtMarkTeam" + i, true)[0];
                        txtMarkGK.Text = data.ContestTeams[i - 1].Mark.ToString();
                    }
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

        private void Doi6_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.SuspendLayout();
            this.DoubleBuffered = true;
            //this.BackgroundImage = Properties.Resources.VHRR_Soft2;
            tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
