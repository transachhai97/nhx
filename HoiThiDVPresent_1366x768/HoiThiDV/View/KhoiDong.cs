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
    public partial class KhoiDong : UserControl
    {
        private DateTime startTime;
        private TimeSpan timeElapsed;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;
        private bool isAnswer = true;
        private SoundPlayer tingfile, warnfile, mo_cau_hoi;
        private int seconds;
        private int minutes;
        private int rangeStop;
        private int secondsStop;
        public KhoiDong()
        {
            InitializeComponent();

            loadFile();

            CheckForIllegalCrossThreadCalls = false;
            this.Resize += new EventHandler(R1_Question_Resize);
            _lastFormSize = GetFormArea(this.Size);

        }
        public void loadFile()
        {
            try
            {
                tingfile = new SoundPlayer(Application.StartupPath + "\\ting.wav");
                warnfile = new SoundPlayer(Application.StartupPath + "\\VĐ_15s_2015.wav");
                mo_cau_hoi = new SoundPlayer(Application.StartupPath + "\\KĐ_mở_câu_hỏi_2015.wav");
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

        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            if (comp != null)
            {
                comp.Text = text;
            }
            
        }

        #region Action Load Question

        public void LoadQuestion(DataServer data)
        {

            InitKhoiDong(data);
            rangeStop = 0;
            secondsStop = 0;
            //R1_Question.TextAlign = ContentAlignment.MiddleCenter;
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
        private int _lastFormSize;
        public void ShowQuestion(DataServer data)
        {
            InitKhoiDong(data);
            mo_cau_hoi.Play();
            RunTimer(data);
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
            }
            else
            {
                R1_Question.Text = "CÂU " + data.QuestionNum + ": \n" + data.QuestionText;
            }
        }

        #endregion

        #region Action Run Timer

        public void timeAddStop()
        {
            warnfile.Play();
            rangeStop = 15;
            secondsStop = 16;
        }
        public void RunTimer(DataServer data)
        {
            isAnswer = false;
            //ShowQuestion(data);
            startTime = DateTime.Now;
            

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
            
            if (TheTimer != null) TheTimer.Dispose();

            if (data != null && data.ContestTeams != null)
            {
                var sortedTeams = data.ContestTeams.OrderByDescending(team => team.Mark).ToList();
                if (data.ContestTeams.Count > 0)
                {
                    R1_Team1.Text = sortedTeams[0].Name;
                    R1_Point1.Text = sortedTeams[0].Mark.ToString("F2") + "";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R1_Team2.Text = sortedTeams[1].Name;
                    R1_Point2.Text = sortedTeams[1].Mark.ToString("F2") + "";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R1_Team3.Text = sortedTeams[2].Name;
                    R1_Point3.Text = sortedTeams[2].Mark.ToString("F2") + "";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R1_Team4.Text = sortedTeams[3].Name;
                    R1_Point4.Text = sortedTeams[3].Mark.ToString("F2") + "";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R1_Team5.Text = sortedTeams[4].Name;
                    R1_Point5.Text = sortedTeams[4].Mark.ToString("F2") + "";
                }
            }

            R1_Question.Text = "";
        }
        #endregion

        #region Action UpdateScore
        public void UpdateScore(DataServer data)
        {

            if (data != null && data.ContestTeams != null)
            {
                //for (int i = 1; i <= data.ContestTeams.Count; i++)
                //{
                //    SetTextComp("R1_Team" + i, data.ContestTeams[i - 1].Name);
                //    SetTextComp("R1_Point" + i, data.ContestTeams[i - 1].Mark.ToString());
                //}
                // Sort ContestTeams by Mark in descending order
                var sortedTeams = data.ContestTeams.OrderByDescending(team => team.Mark).ToList();

                for (int i = 1; i <= sortedTeams.Count; i++)
                {
                    SetTextComp("R1_Team" + i, sortedTeams[i - 1].Name);
                    SetTextComp("R1_Point" + i, sortedTeams[i - 1].Mark.ToString("F2"));
                }
                //if (data.ContestTeams.Count > 0)
                //{
                //    R1_Team1.Text = data.ContestTeams[0].Name;
                //    R1_Point1.Text = data.ContestTeams[0].Mark + "";
                //}
                //if (data.ContestTeams.Count > 1)
                //{
                //    R1_Team2.Text = data.ContestTeams[1].Name;
                //    R1_Point2.Text = data.ContestTeams[1].Mark + "";
                //}
                //if (data.ContestTeams.Count > 2)
                //{
                //    R1_Team3.Text = data.ContestTeams[2].Name;
                //    R1_Point3.Text = data.ContestTeams[2].Mark + "";
                //}
                //if (data.ContestTeams.Count > 3)
                //{
                //    R1_Team4.Text = data.ContestTeams[3].Name;
                //    R1_Point4.Text = data.ContestTeams[3].Mark + "";
                //}
                //if (data.ContestTeams.Count > 4)
                //{
                //    R1_Team5.Text = data.ContestTeams[4].Name;
                //    R1_Point5.Text = data.ContestTeams[4].Mark + "";
                //}
            }

        }
        #endregion

        private void KhoiDong_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
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
        private int GetFormArea(Size size)
        {
            return size.Height * size.Width;
        }
        private void R1_Question_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            float scaleFactor = (float)GetFormArea(control.Size) / (float)_lastFormSize;

            ResizeFont(this.Controls, scaleFactor);

            _lastFormSize = GetFormArea(control.Size);
        }
        private void ResizeFont(Control.ControlCollection coll, float scaleFactor)
        {
            foreach (Control c in coll)
            {
                if (!c.HasChildren)
                {
                    ResizeFont(c.Controls, scaleFactor);
                }
                else
                {
                    //if (c.GetType().ToString() == "System.Windows.Form.Label")
                    if (true)
                    {
                        // scale font
                        c.Font = new Font(c.Font.FontFamily.Name, c.Font.Size * scaleFactor);
                    }
                }
            }
        }
    }
}
