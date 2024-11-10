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
    public partial class KhoiDong : UserControl
    {
        private DateTime startTime;
        private TimeSpan timeElapsed;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;
        private bool isAnswer = true;
        private int seconds;
        private int minutes;
        private int initMin = 10;
        private int rangeStop;
        private int secondsStop;
        public KhoiDong()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }


        #region Action Load Question

        public void LoadQuestion(DataServer data)
        {

            InitKhoiDong(data);
            CompDisableEnable("R1_TxtAnswer", false);
            CompDisableEnable("R1_BtnAnswer", false);
            rangeStop = 0;
            secondsStop = 0;
            R1_Question.Font = new Font("Myriad Pro", 50, FontStyle.Bold);

            if (data.questionRound == 9)
            {
                R1_Question.Text = "DỰ PHÒNG - CÂU " + data.QuestionNum;
            }
            else
            {
                R1_Question.Text = "CÂU " + data.QuestionNum;
            }
        }

        #endregion

        #region Action Show Question

        public void ShowQuestion(DataServer data)
        {
            InitKhoiDong(data);
            RunTimer(data);
            CompDisableEnable("R1_TxtAnswer", true);
            CompDisableEnable("R1_BtnAnswer", true);
            R1_BtnAnswer.BackColor = System.Drawing.Color.Red;
            R1_TxtAnswer.Focus();

            if (data.QuestionText.Length < 400)
            {
                R1_Question.Font = new Font("Myriad Pro", 20, FontStyle.Regular);
            }
            else if (data.QuestionText.Length < 700)
            {
                R1_Question.Font = new Font("Myriad Pro", 18, FontStyle.Regular);
            }
            else
            {
                R1_Question.Font = new Font("Myriad Pro", 16, FontStyle.Regular);
            }
            

            if (data.questionRound == 9)
            {
                R1_Question.Text = "DỰ PHÒNG - CÂU " + data.QuestionNum + ": \n" + data.QuestionText;
            } else
            {
                R1_Question.Text = "CÂU " + data.QuestionNum + ": \n" + data.QuestionText;
            }
        }

        #endregion

        #region Action Run Timer
        public void timeAddStop()
        {
            rangeStop = 15;
            secondsStop = 16;
        }
        public void RunTimer(DataServer data)
        {
            isAnswer = false;
            //ShowQuestion(data);
            startTime = DateTime.Now;
            initMin = data.minutes;

            this.Invoke(new Action(() => { R1_Timer.Enabled = true; }));
        }
        public void setupTime()
        {
            R1_Timer.Enabled = false;
            minutes = 0;
            seconds = 0;
            lblMin.Text = "00";
            lblSecond.Text = "00";

        }
        private void R1_Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            if (seconds > 59)
            {
                minutes += 1;
                seconds = 0;
            }

            if (rangeStop == 15)
            {
                secondsStop = secondsStop - 1;
                if (secondsStop == 0)
                {
                    R1_Timer.Enabled = false;
                    CompDisableEnable("R1_TxtAnswer", false);
                    CompDisableEnable("R1_BtnAnswer", false);
                    R1_BtnAnswer.BackColor = System.Drawing.Color.Gray;
                }
            }
            if (minutes.ToString().Length < 2)
                lblMin.Text = "0" + minutes.ToString();
            else
                lblMin.Text = minutes.ToString();
            if (seconds.ToString().Length < 2)
                lblSecond.Text = "0" + seconds.ToString();
            else
                lblSecond.Text = seconds.ToString();
        }

        #endregion

        #region Init KhoiDong
        public void InitKhoiDong(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (i <= data.ContestTeams.Count)
                {
                    SetTextComp("R1_Team" + i, "");
                    SetTextComp("R1_Point" + i, "");

                    CompDisableLocation("R1_Team" + i, 0, 0, "show");
                    CompDisableLocation("R1_Point" + i, 0, 0, "show");
                }
                else
                {
                    CompDisableLocation("R1_Team" + i, -300, 0, "hide");
                    CompDisableLocation("R1_Point" + i, -300, 0, "hide");
                }

            }

            setupTime();
            ToggleBtnAnswer(false);
            if (TheTimer != null) TheTimer.Dispose();

            if (data != null && data.ContestTeams != null)
            {
                if (data.ContestTeams.Count > 0)
                {
                    R1_Team1.Text = data.ContestTeams[0].Name;
                    R1_Point1.Text = data.ContestTeams[0].Mark + "";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R1_Team2.Text = data.ContestTeams[1].Name;
                    R1_Point2.Text = data.ContestTeams[1].Mark + "";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R1_Team3.Text = data.ContestTeams[2].Name;
                    R1_Point3.Text = data.ContestTeams[2].Mark + "";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R1_Team4.Text = data.ContestTeams[3].Name;
                    R1_Point4.Text = data.ContestTeams[3].Mark + "";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R1_Team5.Text = data.ContestTeams[4].Name;
                    R1_Point5.Text = data.ContestTeams[4].Mark + "";
                }
            }

            R1_Question.Text = "";
            R1_TeamAnswer.Text = "";
            R1_TxtAnswer.Text = "";
        }
        #endregion

        #region Action UpdateScore
        public void UpdateScore(DataServer data)
        {

            if (data != null && data.ContestTeams != null)
            {
                for (int i = 1; i <= data.ContestTeams.Count; i++)
                {
                    SetTextComp("R1_Team" + i, data.ContestTeams[i - 1].Name);
                    SetTextComp("R1_Point" + i, data.ContestTeams[i - 1].Mark.ToString());
                }
            }

        }
        #endregion

        #region Toggle Button Answer
        private void ToggleBtnAnswer(bool state)
        {
            //this.R1_BtnA.Enabled = state;
            //this.R1_BtnB.Enabled = state;
            //this.R1_BtnC.Enabled = state;
            //this.R1_BtnD.Enabled = state;
        }

        #endregion

        #region Click to Answer
        private void R1_BtnA_Click(object sender, EventArgs e)
        {
            BtnAnswerClick("A");
        }

        private void R1_BtnB_Click(object sender, EventArgs e)
        {
            BtnAnswerClick("B");
        }

        private void R1_BtnC_Click(object sender, EventArgs e)
        {
            BtnAnswerClick("C");
        }

        private void R1_BtnD_Click(object sender, EventArgs e)
        {
            BtnAnswerClick("D");
        }

        public void BtnAnswerClick(String answer)
        {
            TimeSpan timeElapsed = DateTime.Now - startTime;
            string txt = "ĐÁP ÁN " + answer + " - " + timeElapsed.TotalSeconds.ToString("0.00000 s");
            this.R1_TeamAnswer.Text = txt;
            ToggleBtnAnswer(false);

            // Send to server
            this.ctrl.SendAnswer("ĐÁP ÁN " + answer, timeElapsed.TotalSeconds.ToString("0.00000"), 1);
        }
        #endregion

        private void KhoiDong_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void R1_BtnAnswer_Click(object sender, EventArgs e)
        {
            AnswerTheQuestion();
        }
        public void AnswerTheQuestion()
        {
            if (!string.IsNullOrWhiteSpace(R1_TxtAnswer.Text.Trim()))
            {
                string answer = R1_TxtAnswer.Text.Trim().ToUpper(); // Chuyển đổi thành chữ hoa
                // Sử dụng HashSet để kiểm tra câu trả lời hợp lệ
                HashSet<string> validAnswers = new HashSet<string> { "A", "B", "C", "D" };

                if (!validAnswers.Contains(answer))
                {
                    MessageBox.Show("Vui lòng nhập A, B, C hoặc D."); // Thông báo lỗi
                    return; // Dừng thực hiện nếu không hợp lệ
                }
                TimeSpan timeElapsed = DateTime.Now - startTime;
                //if (timeElapsed.TotalMilliseconds > 10000) return;
                string txt = answer + " - " + timeElapsed.TotalSeconds.ToString("0.00000 s");
                this.R1_TeamAnswer.Text = txt;
                R1_TxtAnswer.Text = "";
                CompDisableEnable("R1_TxtAnswer", false);
                //R1_TxtAnswer.ReadOnly = true;
                //R1_TxtAnswer.Focus();
                CompDisableEnable("R1_BtnAnswer", false);
                R1_BtnAnswer.BackColor = System.Drawing.Color.Gray;
                isAnswer = true;
                // Send to server
                this.ctrl.SendAnswer(answer, timeElapsed.TotalSeconds.ToString("0.00000"), 1);
            }

        }
        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            comp.Text = text;
        }

        public void CompDisableEnable(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Enabled = flag;
        }

        public void TextboxReadOnly(string compName, bool flag)
        {
            TextBox comp = (TextBox)this.Controls.Find(compName, true).FirstOrDefault();
            comp.ReadOnly = flag;
        }

        public void CompDisableVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
        }

        public void CompDisableLocation(string compName, int x, int y, string status)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Dock = status == "show" ? DockStyle.Fill : DockStyle.None;
            comp.Location = new Point(x, y);

        }

        private void R1_TxtAnswer_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only A, B, C, D and control keys (like backspace), and disallow Enter
            if (!char.IsControl(e.KeyChar) && !("ABCD".Contains(e.KeyChar.ToString().ToUpper())) || e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Ignore the key press
            }
        }
    }
}
