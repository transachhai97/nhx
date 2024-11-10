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
    public partial class GioiThieu : UserControl
    {
        private DateTime startTime;
        private TimeSpan timeElapsed;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;

        private int seconds;
        private int minutes;
        private int initMin = 5;
        private SoundPlayer tingfile, warnfile, ticktok;
        private bool play;

        public GioiThieu()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;


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

        #region Action Load Question

        public void LoadQuestion(DataServer data)
        {

            InitGioiThieu(data);
            //R1_Question.TextAlign = ContentAlignment.MiddleCenter;
            R1_Content.Font = new Font("Myriad Pro", 90, FontStyle.Bold);
            R1_Content.Text = "CÂU " + data.QuestionNum;
        }

        #endregion

        #region Action Show Content

        public void ShowContent(DataServer data)
        {
            InitGioiThieu(data);
            //R1_Question.TextAlign = ContentAlignment.TopLeft;
            //R1_Content.Font = new Font("Myriad Pro", 17, FontStyle.Regular);
            labTitle.Text = data.QuestionText;
        }

        #endregion

        #region Action Run Timer

        public void RunTimer(DataServer data)
        {
            //ShowContent(data);
            InitGioiThieu(data);
            startTime = DateTime.Now;
            TheTimer = new System.Threading.Timer(
                    this.Tick, null, 0, 10);
        }

        public void Tick(object info)
        {
            timeElapsed = DateTime.Now - startTime;
            if (timeElapsed.TotalMilliseconds >= 20000)
            {
                this.R_GT_Clock.Text = "20.00";
                this.TheTimer.Dispose();
                return;
            }
            this.R_GT_Clock.Text = timeElapsed.TotalSeconds.ToString("0.00"); ;
        }

        public void ActionRunTimer(DataServer data)
        {
            initMin = data.minutes;
            if (data.description == "START")
            {
                this.Invoke(new Action(() => { R_GT_Timer.Enabled = true; }));
            }
            else if (data.description == "PAUSE")
            {
                R_GT_Timer.Enabled = false;
                //ticktok.Stop();
                //tingfile.Stop();
                //warnfile.Stop();
                //play = false;
            }
            else if (data.description == "STOP")
            {
                setupTime();
                //if (play)
                //{
                //    ticktok.Stop();
                //    tingfile.Stop();
                //    warnfile.Stop();
                //    play = false;

                //}
            }
        }

        public void setupTime()
        {
            R_GT_Timer.Enabled = false;
            minutes = 0;
            seconds = 0;
            lblMin.Text = "00";
            lblSecond.Text = "00";
            progressBar1.Value = 0;
            play = false;

            if (progressBar1.BackColor != SystemColors.ControlLight)
                progressBar1.BackColor = SystemColors.ControlLight;
            Color inColor = Color.FromArgb(63, 51, 123);
            if (progressBar1.ForeColor != inColor)
                progressBar1.ForeColor = inColor;
            progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            progressBar1.RightToLeftLayout = false;
        }
        #endregion

        #region Init GioiThieu
        public void InitGioiThieu(DataServer data)
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

            this.R_GT_Clock.Text = "0.00";
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

            R1_Content.Text = "";
        }
        #endregion

        #region Action UpdateScore
        public void UpdateScore(DataServer data)
        {

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

        }
        #endregion


        private void R_GT_Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            if (seconds > 59)
            {
                minutes += 1;
                seconds = 0;
            }
            if (initMin == 2)
            {
                if (minutes == 2)
                {
                    R_GT_Timer.Enabled = false;
                }
            }
            else if (initMin == 3)
            {

                if (minutes == 2 && seconds == 45 && !play)
                {
                    play = true;
                    //warnfile.Play();
                    play = false;
                }


                if (((minutes == 3 && seconds >= 1) || (minutes >= 4)) && !play)
                {

                    play = true;
                    //ticktok.Play();
                    play = false;


                }
                if (minutes == 6)
                {
                    R_GT_Timer.Enabled = false;
                }
                if (minutes < 3 || (minutes == 3 && seconds == 0))
                {
                    if (progressBar1.BackColor != SystemColors.ControlLight)
                        progressBar1.BackColor = SystemColors.ControlLight;
                    Color inColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.ForeColor != inColor)
                        progressBar1.ForeColor = inColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                    progressBar1.RightToLeftLayout = false;
                }
                else
                {
                    Color backColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.BackColor != backColor)
                        progressBar1.BackColor = backColor;
                    Color insColor = Color.FromArgb(228, 54, 44);
                    if (progressBar1.ForeColor != insColor)
                        progressBar1.ForeColor = insColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    progressBar1.RightToLeftLayout = true;
                }
            }
            else if (initMin == 5)
            {
                if (minutes == 4 && seconds == 45 && !play)
                {
                    play = true;
                    //warnfile.Play();
                    play = false;
                }

                if (minutes == 5)
                {
                    R_GT_Timer.Enabled = false;
                    play = true;
                    //tingfile.Play();
                    play = false;
                }

                if (((minutes == 5 && seconds == 31) || (minutes == 6 && seconds == 01)
                    || (minutes == 6 && seconds == 31) || (minutes == 7 && seconds == 30)) && !play)
                {
                    play = true;
                    //tingfile.Play();
                    play = false;
                }

                //if (minutes == 7 && seconds == 30)
                //{
                //    timer1.Enabled = false;
                //}



                if (minutes < 5 || (minutes == 5 && seconds == 0))
                {
                    if (progressBar1.BackColor != SystemColors.ControlLight)
                        progressBar1.BackColor = SystemColors.ControlLight;
                    Color inColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.ForeColor != inColor)
                        progressBar1.ForeColor = inColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                    progressBar1.RightToLeftLayout = false;
                }
                else
                {
                    Color backColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.BackColor != backColor)
                        progressBar1.BackColor = backColor;
                    Color insColor = Color.FromArgb(228, 54, 44);
                    if (progressBar1.ForeColor != insColor)
                        progressBar1.ForeColor = insColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    progressBar1.RightToLeftLayout = true;
                }
            }
            else if (initMin == 7)
            {
                if (minutes == 6 && seconds == 45 && !play)
                {
                    play = true;
                    //warnfile.Play();
                    play = false;
                }
                if (((minutes == 7 && seconds == 31) || (minutes == 8 && seconds == 01)
                    || (minutes == 8 && seconds == 31) || (minutes == 9 && seconds == 30)) && !play)
                {
                    play = true;
                    //tingfile.Play();
                    play = false;
                }
                if (minutes == 9 && seconds == 30)
                {
                    R_GT_Timer.Enabled = false;
                }

                if (minutes < 7 || (minutes == 7 && seconds == 0))
                {
                    if (progressBar1.BackColor != SystemColors.ControlLight)
                        progressBar1.BackColor = SystemColors.ControlLight;
                    Color inColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.ForeColor != inColor)
                        progressBar1.ForeColor = inColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                    progressBar1.RightToLeftLayout = false;
                }
                else
                {
                    Color backColor = Color.FromArgb(63, 51, 123);
                    if (progressBar1.BackColor != backColor)
                        progressBar1.BackColor = backColor;
                    Color insColor = Color.FromArgb(228, 54, 44);
                    if (progressBar1.ForeColor != insColor)
                        progressBar1.ForeColor = insColor;
                    progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    progressBar1.RightToLeftLayout = true;
                }
            }
            if (progressBar1.Value < progressBar1.Maximum)
            {

                progressBar1.Value = progressBar1.Value + 1;
            }
            else
            {

                progressBar1.Value = 0;
                progressBar1.Value = progressBar1.Value + 1;

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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GioiThieu_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
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
    }
}
