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
    public partial class BamChuong : UserControl
    {
        private int _lastFormSize;
        private DateTime startTime;
        private TimeSpan timeElapsed;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;
        public DataServer dataServer;
        private bool isAnswer = true;
        private SoundPlayer tingfile, warnfile, ticktok, mo_cau_hoi, tin_hieu_tl;

        private int seconds;
        private int minutes;
        private int initMin = 15;
        private int rangeStop;
        private int secondsStop;
        private bool play;
        int point = 0;
        public BamChuong()
        {
            InitializeComponent();
            loadFile();

            for (int i = 1; i <= 6; i++)
            {
                CompDisableLocation("R_BC_BtnStar" + i, -400, 3);
            }
            
            CheckForIllegalCrossThreadCalls = false;
            lbl_nameRound.Text = "";

            this.Resize += new EventHandler(R_BC_Question_Resize);
            _lastFormSize = GetFormArea(this.Size);
        }

        public void loadFile()
        {
            try
            {
                tingfile = new SoundPlayer(Application.StartupPath + "\\ting.wav");
                warnfile = new SoundPlayer(Application.StartupPath + "\\VĐ_15s_2015.wav");
                ticktok = new SoundPlayer(Application.StartupPath + "\\tiktoc.wav");
                mo_cau_hoi = new SoundPlayer(Application.StartupPath + "\\TT_mở_câu_hỏi_2015.wav");
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

        #region Action Load Question , Show Name Round

        public void LoadQuestion(DataServer data)
        {
            showNameRound(data);
            InitBamChuong(data);
            rangeStop = 0;
            secondsStop = 0;
            //R1_Question.TextAlign = ContentAlignment.MiddleCenter;
            R_BC_Question.Font = new Font("Myriad Pro", 50, FontStyle.Bold);
            if (data.questionRound == 5)
            {
                point = 0;

                if (data.QuestionNum == 1 || data.QuestionNum == 4 || data.QuestionNum == 7)
                {
                    point = 20;
                }
                else
                {
                    point = 30;
                }
                R_BC_Question.Text = "CÂU " + data.QuestionNum + " - " + point + " điểm";
            }
            else if (data.questionRound == 6)
            {
                R_BC_Question.Text = "CÂU " + data.QuestionNum;
            }
            else if (data.questionRound == 7)
            {
                R_BC_Question.Text = "TÌNH HUỐNG";
            }
        }
        public void showNameRound(DataServer data)
        {
            if (data.questionRound == 5)
            {
                lbl_nameRound.Text = "HIỂU NGHIỆP VỤ";
            }
            else if (data.questionRound == 6)
            {
                lbl_nameRound.Text = "CÂU HỎI PHỤ";
            }
            else if (data.questionRound == 7)
            {
                point = 0;
                lbl_nameRound.Text = "VỀ ĐÍCH";
            }
        }
        #endregion

        #region Action Show Question, Answer,

        public void ShowQuestion(DataServer data)
        {
            InitBamChuong(data);
            mo_cau_hoi.Play();

            data.description = "START";
            if (data.questionRound != 7)
            {
                RunTimer(data);
            }
            
            //R1_Question.TextAlign = ContentAlignment.TopLeft;
            if (data.QuestionText.Length < 400)
            {
                R_BC_Question.Font = new Font("Myriad Pro", 20, FontStyle.Regular);
            }
            else if (data.QuestionText.Length < 700)
            {
                R_BC_Question.Font = new Font("Myriad Pro", 18, FontStyle.Regular);
            }
            else
            {
                R_BC_Question.Font = new Font("Myriad Pro", 16, FontStyle.Regular);
            }


            if (point != 0)
            {
                R_BC_Question.Text = "CÂU " + data.QuestionNum + " - " + point + " điểm:" + "\n" + data.QuestionText;
            }
            else
            {
                R_BC_Question.Text = "Tình huống " + data.QuestionNum + "\n" + data.QuestionText;
            }
            
        }
        
        #endregion

        #region Action Show Star

        public void ShowStar(DataServer data)
        {

            //R_BC_BtnStar1.Visible = true;
            if (data.team == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    CompDisableLocation("R_BC_BtnStar" + i, -400, 3);
                }
            }
            else
            {
                CompDisableLocation("R_BC_BtnStar" + data.team, 10, 3);
            }

        }
        public void showBell(DataServer data)
        {
            if (data.team == 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    CompBackColor("pnBC" + i, Color.FromArgb(0, 51, 48)); 
                    //CompBackColor("R_BC_BtnStar" + i, Color.FromArgb(0, 51, 48));
                    //CompBackColor("R_BC_PanStar" + i, Color.FromArgb(0, 51, 48));
                }
            }
            else
            {
                CompBackColor("pnBC" + data.team, Color.FromArgb(0, 102, 255));
                tin_hieu_tl.Play();
                //CompBackColor("R_BC_BtnStar" + data.team, Color.FromArgb(0, 102, 255));
                //CompBackColor("R_BC_PanStar" + data.team, Color.FromArgb(0, 102, 255));
            }
        }

        #endregion

        #region Dừng bấm chuông(disable nút bấm chuông)
        public void stopBamChuong(DataServer data)
        {
            showBell(data);
        }
        #endregion

        #region Action Run Timer

        public void timeAddStop(DataServer data)
        {
            //warnfile.Play();
            //rangeStop = 15;
            //secondsStop = 16;
            if (data.questionRound != 7)
            {
                warnfile.Play();
                rangeStop = 15;
                secondsStop = 16;
            }
            else
            {
                RunTimer(data);
            }
        }
        public void RunTimer(DataServer data)
        {
            isAnswer = false;
            //ShowQuestion(data);
            startTime = DateTime.Now;
            initMin = data.minutes;
            

            if (data.description == "START")
            {
                if (initMin != 0)
                {
                    this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
                }
                else
                {
                    R_BC_Timer.Enabled = false;
                }

                return;
            }
            else if (data.description == "PAUSE")
            {
                R_BC_Timer.Enabled = false;
                return;
            }
            else if (data.description == "STOP")
            {
                setupTime();
                return;
            }

        }

        public void setupTime()
        {
            R_BC_Timer.Enabled = false;
            minutes = 0;
            seconds = 0;
            lblMin.Text = "00";
            lblSecond.Text = "00";
            play = false;
            ticktok.Stop();
            tingfile.Stop();
            warnfile.Stop();

        }
        private void R_BC_Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            if (seconds > 59)
            {
                minutes += 1;
                seconds = 0;
            }

            //if(initMin == 10)//10 giây (tăng tốc)
            //{
            //    if (seconds == 10)
            //    {
            //        tingfile.Play();
            //        R_BC_Timer.Enabled = false;
            //    }
            //}
            if (rangeStop == 15)
            {
                secondsStop = secondsStop - 1;
                if (secondsStop == 0)
                {
                    R_BC_Timer.Enabled = false;
                }
            }
            else if(initMin == 1)//2 phút (hùng biện)
            {
                //ticktok.Play();
                if (minutes == 1)
                {
                    //ticktok.Stop();
                    tingfile.Play();
                    R_BC_Timer.Enabled = false;
                }
            }
            else if (initMin == 2)//2 phút (hùng biện)
            {
                //ticktok.Play();
                if (minutes == 2)
                {
                    //ticktok.Stop();
                    tingfile.Play();
                    R_BC_Timer.Enabled = false;
                }
            }
            else if (initMin == 5)//4 phút  (hùng biện)
            {
                /*if (minutes == 5 && seconds >= 1)
                {
                    tingfile.Play();
                }*/
                /*if(minutes > 4 && minutes < 5)
                {
                    tingfile.Stop();
                    //ticktok.Play();
                }*/
                if (minutes == 5)
                {
                    //ticktok.Stop();
                    tingfile.Play();
                    R_BC_Timer.Enabled = false;
                }

            }
            else if (initMin == 30)//30s về đích
            {
                //ticktok.Play();
                if (seconds == 30)
                {
                    //ticktok.Stop();
                    tingfile.Play();
                    R_BC_Timer.Enabled = false;
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

        #region Init BamChuong
        public void InitBamChuong(DataServer data)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (i <= data.ContestTeams.Count)
                {
                    if (data.questionRound != 6)
                    {
                        SetTextComp("R_BC_Team" + i, "");
                        SetTextComp("R_BC_Point" + i, "");
                    }

                    CompDisableLocation("R_BC_Team" + i, 0, -3, "show");
                    CompDisableLocation("R_BC_Point" + i, 0, -3, "show");
                }
                else
                {
                    CompDisableLocation("R_BC_Team" + i, -400, -3, "hide");
                    CompDisableLocation("R_BC_Point" + i, -400, -3, "hide");
                }
                
            }

            setupTime();
            if (TheTimer != null) TheTimer.Dispose();
            if (data.questionRound != 6)
            {
                if (data != null && data.ContestTeams != null)
                {
                    if (data.ContestTeams.Count > 0)
                    {
                        R_BC_Team1.Text = data.ContestTeams[0].Name;
                        R_BC_Point1.Text = data.ContestTeams[0].Mark + "";
                    }
                    if (data.ContestTeams.Count > 1)
                    {
                        R_BC_Team2.Text = data.ContestTeams[1].Name;
                        R_BC_Point2.Text = data.ContestTeams[1].Mark + "";
                    }
                    if (data.ContestTeams.Count > 2)
                    {
                        R_BC_Team3.Text = data.ContestTeams[2].Name;
                        R_BC_Point3.Text = data.ContestTeams[2].Mark + "";
                    }
                    if (data.ContestTeams.Count > 3)
                    {
                        R_BC_Team4.Text = data.ContestTeams[3].Name;
                        R_BC_Point4.Text = data.ContestTeams[3].Mark + "";
                    }
                    if (data.ContestTeams.Count > 4)
                    {
                        R_BC_Team5.Text = data.ContestTeams[4].Name;
                        R_BC_Point5.Text = data.ContestTeams[4].Mark + "";
                    }

                    if (data.ContestTeams.Count > 5)
                    {
                        R_BC_Team6.Text = data.ContestTeams[5].Name;
                        R_BC_Point6.Text = data.ContestTeams[5].Mark + "";
                    }

                }

            }

            R_BC_Question.Text = "";
        }
        #endregion

        #region Action UpdateScore and Change Name
        public void UpdateScore(DataServer data)
        {

            if (data != null && data.ContestTeams != null)
            {
                if (data.ContestTeams.Count > 0)
                {
                    R_BC_Team1.Text = data.ContestTeams[0].Name;
                    R_BC_Point1.Text = data.ContestTeams[0].Mark + "";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R_BC_Team2.Text = data.ContestTeams[1].Name;
                    R_BC_Point2.Text = data.ContestTeams[1].Mark + "";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R_BC_Team3.Text = data.ContestTeams[2].Name;
                    R_BC_Point3.Text = data.ContestTeams[2].Mark + "";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R_BC_Team4.Text = data.ContestTeams[3].Name;
                    R_BC_Point4.Text = data.ContestTeams[3].Mark + "";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R_BC_Team5.Text = data.ContestTeams[4].Name;
                    R_BC_Point5.Text = data.ContestTeams[4].Mark + "";
                }
                if (data.ContestTeams.Count > 5)
                {
                    R_BC_Team6.Text = data.ContestTeams[5].Name;
                    R_BC_Point6.Text = data.ContestTeams[5].Mark + "";
                }
            }

        }
        public void ChangeName(DataServer data)
        {

            if (data != null && data.ContestTeams != null)
            {
                if (data.point == 6)// áp dụng cho phần thi phụ
                {
                    if (data.team == 1)
                    {
                        R_BC_Team1.Text = data.name;
                        R_BC_Point1.Text = "0";

                        for (int i = 2; i <= 5; i++)
                        {
                            SetTextComp("R_BC_Team" + i, "");
                            SetTextComp("R_BC_Point" + i, "");
                        }
                    }
                    if (data.team == 2)
                    {
                        R_BC_Team2.Text = data.name;
                        R_BC_Point2.Text = "0";

                        for (int i = 3; i <= 5; i++)
                        {
                            SetTextComp("R_BC_Team" + i, "");
                            SetTextComp("R_BC_Point" + i, "");
                        }
                    }
                    if (data.team == 3)
                    {
                        R_BC_Team3.Text = data.name;
                        R_BC_Point3.Text = "0";

                        for (int i = 4; i <= 5; i++)
                        {
                            SetTextComp("R_BC_Team" + i, "");
                            SetTextComp("R_BC_Point" + i, "");
                        }
                    }
                    if (data.team == 4)
                    {
                        R_BC_Team4.Text = data.name;
                        R_BC_Point4.Text = "0";

                        for (int i = 5; i <= 5; i++)
                        {
                            SetTextComp("R_BC_Team" + i, "");
                            SetTextComp("R_BC_Point" + i, "");
                        }
                    }
                    if (data.team == 5)
                    {
                        R_BC_Team5.Text = data.name;
                        R_BC_Point5.Text = "0";
                    }
                }
            }

        }
        #endregion

        #region Click to Answer

        #endregion

        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            comp.Text = text;
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

        public void CompDisableLocation(string compName, int x, int y, string status)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Dock = status == "show" ? DockStyle.Fill : DockStyle.None;
            comp.Location = new Point(x, y);

        }

        public void TextboxReadOnly(string compName, bool flag)
        {
            TextBox comp = (TextBox)this.Controls.Find(compName, true).FirstOrDefault();
            comp.ReadOnly = flag;
        }

        private void BamChuong_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        public void CompBackColor(string compName, Color color)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            if (comp != null)
            {
                comp.BackColor = color;
            }
            
        }

        private int GetFormArea(Size size)
        {
            return size.Height * size.Width;
        }
        private void R_BC_Question_Resize(object sender, EventArgs e)
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
