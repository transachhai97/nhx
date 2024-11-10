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
    public partial class TeamAnswer26 : UserControl
    {
        private MasterController ctrl;
        private string score = "0";
        private SoundPlayer mo_dap_an;
        public TeamAnswer26()
        {
            InitializeComponent();
            loadFile();
            _lblAnswer.Text = "";
            lblAnswerCT.Text = "";
        }
        public void loadFile()
        {
            try
            {
                mo_dap_an = new SoundPlayer(Application.StartupPath + "\\KĐ_mở_đáp_án_O15.wav");
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
            _lblAnswer.Text = "";
            lblAnswerCT.Text = "";
            score = "0";
            mo_dap_an.Play();
            resetForm(data);

        }
        #endregion

        public void ShowAnswerCT(DataServer data)
        {
            _lblAnswer.Text = "Đáp án: ";
            lblAnswerCT.Text = data.QuestionAnswer;
        }
        public void resetForm(DataServer data)
        {
            for (int i = 1; i <= data.ContestTeams.Count; i++)
            {
                CompBackColor("R3_Team" + i, Color.FromArgb(255, 198, 47));//màu vàng (mặc định)
                CompForeColor("R3_Team" + i, Color.FromArgb(0, 82, 78));//màu xanh (mặc định)
            }

            if (data != null && data.ContestTeams != null)
            {
                var answerNotNull = data.ContestTeams.Where(n => !string.IsNullOrEmpty(n.Answer)).OrderBy(n => double.Parse(n.Time)).ToList();
                var answerNull = data.ContestTeams.Where(n => string.IsNullOrEmpty(n.Answer)).ToList();

                data.ContestTeams.Clear();
                data.ContestTeams.AddRange(answerNotNull);
                data.ContestTeams.AddRange(answerNull);

                if (data.ContestTeams.Count > 0)
                {
                    R3_Team1.Text = data.ContestTeams[0].Name + " (0)";
                    R3_Answer1.Text = data.ContestTeams[0].Answer;
                    R3_Time1.Text = string.IsNullOrEmpty(data.ContestTeams[0].Time) ? "" : data.ContestTeams[0].Time + " s";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R3_Team2.Text = data.ContestTeams[1].Name + " (0)";
                    R3_Answer2.Text = data.ContestTeams[1].Answer;
                    R3_Time2.Text = string.IsNullOrEmpty(data.ContestTeams[1].Time) ? "" : data.ContestTeams[1].Time + " s";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R3_Team3.Text = data.ContestTeams[2].Name + " (0)";
                    R3_Answer3.Text = data.ContestTeams[2].Answer;
                    R3_Time3.Text = string.IsNullOrEmpty(data.ContestTeams[2].Time) ? "" : data.ContestTeams[2].Time + " s";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R3_Team4.Text = data.ContestTeams[3].Name + " (0)";
                    R3_Answer4.Text = data.ContestTeams[3].Answer;
                    R3_Time4.Text = string.IsNullOrEmpty(data.ContestTeams[3].Time) ? "" : data.ContestTeams[3].Time + " s";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R3_Team5.Text = data.ContestTeams[4].Name + " (0)";
                    R3_Answer5.Text = data.ContestTeams[4].Answer;
                    R3_Time5.Text = string.IsNullOrEmpty(data.ContestTeams[4].Time) ? "" : data.ContestTeams[4].Time + " s";
                }

                if (data.ContestTeams.Count > 5)
                {
                    R3_Team6.Text = data.ContestTeams[5].Name + " (0)";
                    R3_Answer6.Text = data.ContestTeams[5].Answer;
                    R3_Time6.Text = string.IsNullOrEmpty(data.ContestTeams[5].Time) ? "" : data.ContestTeams[5].Time + " s";
                }
            }
        }
        public void showBell(DataServer data)
        {
            if (data.team == 0)
            {
                resetForm(data);
                score = "0";
            }
            else
            {
                var answerNotNull = data.ContestTeams.Where(n => !string.IsNullOrEmpty(n.Answer)).OrderBy(n => double.Parse(n.Time)).ToList();
                var answerNull = data.ContestTeams.Where(n => string.IsNullOrEmpty(n.Answer)).ToList();
                data.ContestTeams.Clear();
                data.ContestTeams.AddRange(answerNotNull);
                data.ContestTeams.AddRange(answerNull);

                int pos = 0;

                string nameTeam = "";
                for (int i = 1; i <= data.ContestTeams.Count; i++)
                {
                    if (data.ContestTeams[i - 1].Id == data.team)
                    {
                        pos = i;
                        nameTeam = data.ContestTeams[i - 1].Name;
                        score = data.ContestTeams[i - 1].BonusPoint.ToString();
                        break;
                    }
                }
                //if (score == "0")
                //{
                //    score = "25";
                //}
                //else if (score == "25")
                //{
                //    score = "20";
                //}
                //else if (score == "20")
                //{
                //    score = "15";
                //}
                //else if (score == "15" || score == "10")
                //{
                //    score = "10";
                //}

                if (pos != 0)
                {
                    SetTextComp("R3_Team" + pos, nameTeam + " (" + score + ")");
                    CompBackColor("R3_Team" + pos, Color.FromArgb(0, 102, 255));//màu xanh
                    CompForeColor("R3_Team" + pos, Color.White);//màu trắng
                }
            }
        }

        private void TeamAnswer26_Load(object sender, EventArgs e)
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

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
