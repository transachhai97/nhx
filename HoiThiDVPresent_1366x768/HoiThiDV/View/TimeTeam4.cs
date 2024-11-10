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
using System.Media;

namespace HoiThiDV.View
{
    public partial class TimeTeam4 : UserControl
    {
        private MasterController ctrl;
        public DataServer dataServer;
        private string score = "0";
        public int checkBC = 0;
        private SoundPlayer mo_dap_an, tin_hieu_tl;
        public TimeTeam4()
        {
            InitializeComponent();
            loadFile();
            checkBC = 0;
        }
        public void loadFile()
        {
            try
            {
                //mo_dap_an = new SoundPlayer(Application.StartupPath + "\\TT_mở_đáp_án_2015.wav");
                tin_hieu_tl = new SoundPlayer(Application.StartupPath + "\\VĐ_tín_hiệu_trả_lời_2015.wav");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }

        #region Init TeamAnswer
        public void InitTeamAnswer(DataServer data)
        {
            score = "0";
            if (data.checkBC == 0)
            {
                tin_hieu_tl.Play();
            }
                
            resetForm(data);

        }
        #endregion

        public void resetForm(DataServer data)
        {
            //for (int i = 1; i <= data.ContestTeams.Count; i++)
            //{
            //    CompBackColor("R3_Team" + i, Color.FromArgb(255, 198, 47));//màu vàng (mặc định)
            //    CompForeColor("R3_Team" + i, Color.FromArgb(0, 82, 78));//màu xanh (mặc định)
            //}

            if (data != null && data.ContestTeams != null)
            {
                var answerNotNull = data.ContestTeams.Where(n => !string.IsNullOrEmpty(n.Answer)).OrderBy(n => double.Parse(n.Time)).ToList();
                var answerNull = data.ContestTeams.Where(n => string.IsNullOrEmpty(n.Answer)).ToList();

                data.ContestTeams.Clear();
                data.ContestTeams.AddRange(answerNotNull);
                data.ContestTeams.AddRange(answerNull);

                if (data.ContestTeams.Count > 0)
                {
                    R3_Team1.Text = data.ContestTeams[0].Name;
                    R3_Time1.Text = string.IsNullOrEmpty(data.ContestTeams[0].Time) ? "" : data.ContestTeams[0].Time + " s";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R3_Team2.Text = data.ContestTeams[1].Name;
                    R3_Time2.Text = string.IsNullOrEmpty(data.ContestTeams[1].Time) ? "" : data.ContestTeams[1].Time + " s";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R3_Team3.Text = data.ContestTeams[2].Name;
                    R3_Time3.Text = string.IsNullOrEmpty(data.ContestTeams[2].Time) ? "" : data.ContestTeams[2].Time + " s";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R3_Team4.Text = data.ContestTeams[3].Name;
                    R3_Time4.Text = string.IsNullOrEmpty(data.ContestTeams[3].Time) ? "" : data.ContestTeams[3].Time + " s";
                }
            }
        }

        private void TimeTeam5_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.SuspendLayout();
            this.DoubleBuffered = true;
            //this.BackgroundImage = Properties.Resources.VHRR_Soft2;
            tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
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
        public string GetTextComp(String componentName)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            return comp.Text;
        }
        public void CompForeColor(string compName, Color color)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.ForeColor = color;
        }
    }
}
