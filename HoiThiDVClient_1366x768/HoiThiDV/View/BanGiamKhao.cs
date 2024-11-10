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
    public partial class BanGiamKhao : UserControl
    {
        private MasterController ctrl;
        private int widthUserControl;
        public BanGiamKhao()
        {
            InitializeComponent();
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }
        public void setData(DataServer data)
        {

            for (int i = 1; i <= data.JudgesTeams.Count; i++)
            {
                Label txt = (Label)this.Controls.Find("txtGK" + i, true)[0];
                txt.Text = data.JudgesTeams[i - 1].Name;

                Label txtMarkGK = (Label)this.Controls.Find("txtMarkGK" + i, true)[0];
                txtMarkGK.Text = data.JudgesTeams[i - 1].Mark;

                CompDisableLocation("pnGK6", 1016, 20);
                int tungphantubentrong = widthUserControl / data.JudgesTeams.Count;
                CompDisableLocation("pnGK" + i, (widthUserControl / data.JudgesTeams.Count - tungphantubentrong) / 2 + widthUserControl / data.JudgesTeams.Count * (i - 1), 123);
            }

            
        }
        public void CompDisableVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
        }

        private void BanGiamKhao_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            widthUserControl = this.Size.Width;
        }

        public void CompDisableLocation(string compName, int x, int y)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Location = new Point(x, y);
        }
    }
}
