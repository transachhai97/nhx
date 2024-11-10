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
using System.Reflection;
using System.Linq.Expressions;

namespace HoiThiDV.View
{
    public partial class TangToc : UserControl
    {
        private int _lastFormSize;
        private DateTime startTime;
        private TimeSpan timeElapsed;
        public bool isInitOchu = false;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;
        public DataServer dataServer;
        private bool isAnswer = true;
        private SoundPlayer warnfile, mo_cau_hoi, tin_hieu_tl, s30_mohangdoc, chon_o_chu, mo_o_chu, dong_o_chu, thoi_gian_tloi, show_hang_doc;

        private int seconds;
        private int minutes;
        private int initMin = 30;
        private int rangeStop;
        private int secondsStop;
        private bool play;
        int point = 0;
        public TangToc()
        {
            InitializeComponent();
            loadFile();

            //for (int i = 1; i <= 5; i++)
            //{
            //    CompDisableLocation("R_BC_BtnStar" + i, -400, 3);
            //}

            CheckForIllegalCrossThreadCalls = false;
            lbl_nameRound.Text = "";

            this.Resize += new EventHandler(R2_Question_Resize);
            _lastFormSize = GetFormArea(this.Size);
        }

        public void loadFile()
        {
            try
            {
                warnfile = new SoundPlayer(Application.StartupPath + "\\TT_10s_right_O22.wav");
                thoi_gian_tloi = new SoundPlayer(Application.StartupPath + "\\TT_30s_right_O22.wav");
                mo_cau_hoi = new SoundPlayer(Application.StartupPath + "\\VCNV_mo_cau_hoi_O15.wav");
                s30_mohangdoc = new SoundPlayer(Application.StartupPath + "\\VCNV_30s_mo_hang_doc_O9.wav");
                chon_o_chu = new SoundPlayer(Application.StartupPath + "\\VCNV_chon_o_chu_O15.wav");
                mo_o_chu = new SoundPlayer(Application.StartupPath + "\\VCNV_mo_o_chu_O15.wav");
                dong_o_chu = new SoundPlayer(Application.StartupPath + "\\KĐ_dong_o_chu_O7.wav");
                show_hang_doc = new SoundPlayer(Application.StartupPath + "\\VCNV_o_mao_hiem_O8.wav");
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

            InitTangToc(data, "RESET");
            rangeStop = 0;
            secondsStop = 0;
            chon_o_chu.Play();
            for (int i = 1; i <= data.InitOchu.Count; i++)
            {
                //if (Array.IndexOf(data.myArrayRound2, i) >= 0)
                //{
                //    //Your stuff goes here
                //}
                //else
                //{
                if (i == data.QuestionNum)
                {
                    R2_QuestN_Selected(i);
                }
                else
                {
                    R2_QuestN_UnSelected(i);
                }
                //}
            }

            R2_Question.Font = new Font("Myriad Pro", 20, FontStyle.Regular);
            R2_Question.Text = "HÀNG NGANG SỐ " + data.QuestionNum + " (" + data.InitOchu[data.QuestionNum - 1].Replace("$", "").Length + " CHỮ CÁI)";
        }

        public void R2_QuestN_Selected(int questNum)
        {
            Control comp = (Control)this.Controls.Find("R2_Cau" + questNum, true).FirstOrDefault();
            Color myRgbColor = Color.FromArgb(255, 198, 47);
            if (comp.Enabled)
                for (int i = 1; i <= 29; i++)
                {

                    CompBackColor("R2_D" + questNum + "_C" + i, myRgbColor);
                }
        }
        public void R2_QuestN_UnSelected(int questNum)
        {
            Control comp = (Control)this.Controls.Find("R2_Cau" + questNum, true).FirstOrDefault();
            Color whiteColor = Color.White;
            Color yellowColor = Color.Yellow;
            if (comp.Enabled)
            {
                if (comp.BackColor == Color.DarkGray)
                {
                    for (int i = 1; i <= 29; i++)
                    {
                        CompBackColor("R2_D" + questNum + "_C" + i, Color.DarkGray);

                    }
                }
                else
                {
                    for (int i = 1; i <= 29; i++)
                    {
                        if (i == 15)
                        {
                            CompBackColor("R2_D" + questNum + "_C" + i, yellowColor);
                        }
                        else
                        {
                            CompBackColor("R2_D" + questNum + "_C" + i, whiteColor);
                        }

                    }
                }
            }
        }
        public void showNameRound(DataServer data)
        {
            lbl_nameRound.Text = "GIẢI Ô CHỮ";
        }
        #endregion

        #region Action Show Question, Answer,

        public void ShowQuestion(DataServer data)
        {
            InitTangToc(data, "RESET");
            mo_cau_hoi.Play();

            data.description = "START";
            data.flagTimeRound2 = "";
            RunTimer(data);
            //data.description = "START";
            //RunTimer(data);
            //R1_Question.TextAlign = ContentAlignment.TopLeft;
            if (data.QuestionText.Length < 400)
            {
                R2_Question.Font = new Font("Myriad Pro", 18, FontStyle.Regular);
            }
            else if (data.QuestionText.Length < 700)
            {
                R2_Question.Font = new Font("Myriad Pro", 16, FontStyle.Regular);
            }
            else
            {
                R2_Question.Font = new Font("Myriad Pro", 14, FontStyle.Regular);
            }
            R2_Question.Text = "HÀNG NGANG SỐ " + data.QuestionNum + " (" + data.InitOchu[data.QuestionNum - 1].Replace("$", "").Length + " CHỮ CÁI)" + ": \n" + data.QuestionText;
        }

        #endregion

        #region Action Show Star, đội bị loại
        public void showBell(DataServer data, string action)
        {
            if (data.team == 0 || action == "RESET")
            {
                //if (data.outTeam == null || data.outTeam.Length == 0)
                //{
                for (int i = 1; i <= 5; i++)
                {
                    if (data.outTeam != null && !data.outTeam.Any(i.ToString().Contains))
                    {
                        CompBackColor("pnBC" + i, Color.FromArgb(0, 51, 48));//xanh mặc định
                    }
                    else if(data.outTeam == null)
                    {
                        CompBackColor("pnBC" + i, Color.FromArgb(0, 51, 48));//xanh mặc định
                    }
                }
                //}
            }
            else
            {
                CompBackColor("pnBC" + data.team, Color.FromArgb(0, 102, 255));//xanh bấm chuông
            }
        }

        public void outTeam(DataServer data)
        {
            int[] lock_unlockTeam = data.lock_unlockTeam;
            foreach (var item in lock_unlockTeam)
            {
                if (data.status == true)
                {
                    CompBackColor("pnBC" + item, Color.DarkGray);//màu xám
                }
                else
                {
                    CompBackColor("pnBC" + item, Color.FromArgb(0, 51, 48));//xanh mặc định}
                }
            }
        }


        #endregion

        #region Action Run Timer

        public void timeAddStop()
        {
            thoi_gian_tloi.Play();
            rangeStop = 10;
            secondsStop = 32;//30s chạy
        }
        public void RunTimer(DataServer data)
        {
            isAnswer = false;
            //ShowQuestion(data);
            startTime = DateTime.Now;
            initMin = data.minutes;
            if (string.IsNullOrEmpty(data.flagTimeRound2) || data.flagTimeRound2 == "30spause")
            {
                //thoi_gian_tloi.Play();
            }
            else
            {
                rangeStop = 10;
                secondsStop = 30;//30s chạy
                setupTime();
                s30_mohangdoc.Play();
            }

            if (data.description == "START")
            {
                if (initMin != 0)
                {
                    this.Invoke(new Action(() => { R2_Timer.Enabled = true; }));
                    if (data.flagTimeRound2 == "30spause")
                    {
                        s30_mohangdoc.Play();
                    }
                }
                else
                {
                    R2_Timer.Enabled = false;
                    s30_mohangdoc.Stop();
                }

                return;
            }
            else if (data.description == "PAUSE")
            {
                R2_Timer.Enabled = false;
                s30_mohangdoc.Stop();
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
            R2_Timer.Enabled = false;
            minutes = 0;
            seconds = 0;
            lblMin.Text = "00";
            lblSecond.Text = "00";
            play = false;
            warnfile.Stop();

        }
        private void R2_Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            if (seconds > 59)
            {
                minutes += 1;
                seconds = 0;
            }

            //if (initMin == 30)//30 giây (tăng tốc)
            //{
            //    if (seconds == 30)
            //    {
            //        R2_Timer.Enabled = false;
            //    }
            //}
            if (rangeStop == 10)
            {
                secondsStop = secondsStop - 1;
                if (secondsStop == 0)
                {
                    R2_Timer.Enabled = false;
                    s30_mohangdoc.Stop();
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

        #region Init Tăng tốc
        public void InitTangToc(DataServer data, string action)
        {
            showNameRound(data);
            for (int i = 1; i <= 6; i++)
            {
                if (i <= data.ContestTeams.Count)
                {
                    if (data.questionRound != 6)
                    {
                        SetTextComp("R2_Team" + i, "");
                        SetTextComp("R2_Point" + i, "");
                    }

                    CompDisableLocation("R2_Team" + i, 0, -3, "show");
                    CompDisableLocation("R2_Point" + i, 0, -3, "show");
                }
                else
                {
                    CompDisableLocation("R2_Team" + i, -400, -3, "hide");
                    CompDisableLocation("R2_Point" + i, -400, -3, "hide");
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
                        R2_Team1.Text = data.ContestTeams[0].Name;
                        R2_Point1.Text = data.ContestTeams[0].Mark + "";
                    }
                    if (data.ContestTeams.Count > 1)
                    {
                        R2_Team2.Text = data.ContestTeams[1].Name;
                        R2_Point2.Text = data.ContestTeams[1].Mark + "";
                    }
                    if (data.ContestTeams.Count > 2)
                    {
                        R2_Team3.Text = data.ContestTeams[2].Name;
                        R2_Point3.Text = data.ContestTeams[2].Mark + "";
                    }
                    if (data.ContestTeams.Count > 3)
                    {
                        R2_Team4.Text = data.ContestTeams[3].Name;
                        R2_Point4.Text = data.ContestTeams[3].Mark + "";
                    }
                    if (data.ContestTeams.Count > 4)
                    {
                        R2_Team5.Text = data.ContestTeams[4].Name;
                        R2_Point5.Text = data.ContestTeams[4].Mark + "";
                    }
                    if (data.ContestTeams.Count > 5)
                    {
                        R2_Team6.Text = data.ContestTeams[5].Name;
                        R2_Point6.Text = data.ContestTeams[5].Mark + "";
                    }

                }

            }

            R2_Question.Text = "";

            data.myArrayShowRound2 = new string[] { };
            for (int i = 1; i <= 20; i++)
            {
                data.myArrayShowRound2 = data.myArrayShowRound2.Concat(new string[] { i.ToString() }).ToArray();
            }
            ShowRow(data);
            HideRow(data);

            //ẩn hàng dọc khi chọn lượt mới
            if (data.HangDoc == null)
            {
                for (int dong = 1; dong <= data.InitOchu.Count; dong++)
                {
                    for (int cot = 1; cot <= 29; cot++)
                    {
                        if (cot == 15)
                        {
                            SetTextComp("R2_D" + dong + "_C100", "");
                        }
                    }
                }
            }

        }

        public void InitOChu(DataServer data)
        {
            if (data != null && data.InitOchu != null && data.InitOchu.Count > 0)
            {
                for (int j = 1; j <= 20; j++)
                {
                    CompBackColor("R2_Cau" + j, Color.FromArgb(255, 198, 47));
                }

                Color whiteColor = Color.White;
                Color yellowColor = Color.Yellow;
                Color darkgrayColor = Color.DarkGray;
                // Set
                for (int dong = 1; dong <= data.InitOchu.Count; dong++)
                {
                    //var _val = data.dicShowQuestnumRound2.Where(x => x.Key == dong).Select(x => x.Value).FirstOrDefault();
                    //SetTextComp("R2_Cau" + dong, _val);

                    bool isDongEmpty = true;

                    for (int cot = 1; cot <= 29; cot++)
                    {
                        if (data.InitOchu[dong - 1][cot - 1].Equals('$'))
                        {
                            Control comp = (Control)this.Controls.Find("R2_D" + dong + "_C" + cot, true).FirstOrDefault();
                            if (InvokeRequired)
                            {
                                comp.Invoke((MethodInvoker)delegate
                                {
                                    comp.Visible = false;
                                });

                            }
                            else
                            {
                                comp.Visible = false;
                            }
                            //CompVisible("R2_D" + dong + "_C" + cot, false);
                        }
                        else
                        {
                            Control comp = (Control)this.Controls.Find("R2_D" + dong + "_C" + cot, true).FirstOrDefault();
                            if (InvokeRequired)
                            {
                                comp.Invoke((MethodInvoker)delegate
                                {
                                    comp.Visible = true;
                                });

                            }
                            else
                            {
                                comp.Visible = true;
                            }
                            if (cot == 15)
                            {
                                CompBackColor("R2_D" + dong + "_C" + cot, yellowColor);
                            }
                            else
                            {
                                CompBackColor("R2_D" + dong + "_C" + cot, whiteColor);
                            }
                            if (data.InitOchu[dong - 1][cot - 1].Equals('#'))
                            {
                                SetTextComp("R2_D" + dong + "_C" + cot, "");

                            }
                            else
                            {
                                SetTextComp("R2_D" + dong + "_C" + cot, data.InitOchu[dong - 1][cot - 1].ToString());
                                if (cot == 15 && !string.IsNullOrEmpty(data.InitOchu[dong - 1][cot - 1].ToString()))
                                {
                                    SetTextComp("R2_D" + dong + "_C100", data.InitOchu[dong - 1][cot - 1].ToString());
                                }
                            }
                            //CompVisible("R2_D" + dong + "_C" + cot, true);
                            isDongEmpty = false;
                        }
                    }
                    if (isDongEmpty)
                    {
                        CompVisible("R2_Cau" + dong, false);
                    }
                    else
                    {
                        CompVisible("R2_Cau" + dong, true);

                        string ochu = data.InitOchu[dong - 1].Replace("$", "");
                        if (ochu.Length == 1 && !data.InitOchu[dong - 1][14].ToString().Equals("$"))
                        {
                            CompDisableEnable("R2_Cau" + dong, false);
                        }
                        else
                        {
                            CompDisableEnable("R2_Cau" + dong, true);
                        }

                        if (data.questRound2[dong - 1] == 1)
                        {
                            CompDisableEnable("R2_Cau" + dong, false);
                        }
                        else if (data.questRound2[dong - 1] == 2)
                        {
                            CompDisableEnable("R2_Cau" + dong, true);
                            CompBackColor("R2_Cau" + dong, Color.DarkGray);
                            for (int f = 1; f <= 29; f++)
                            {
                                CompBackColor("R2_D" + dong + "_C" + f, Color.DarkGray);
                            }
                        }
                    }


                }
                isInitOchu = true;
            }
        }

        #endregion

        #region Dong o chu
        public void closeRound2Answer(DataServer data)
        {
            InitTangToc(data, "RESET");
            dong_o_chu.Play();

            dongOChu(data.QuestionNum, data.isOpenCol);
        }

        public void dongOChu(int hang, bool isOpenCol)
        {

            Color myRgbColor = Color.DarkGray;

            for (int i = 1; i <= 29; i++)
            {
                if (isOpenCol)
                {
                    if (i != 15)
                    {
                        SetTextComp("R2_D" + hang + "_C" + i, "");
                    }
                }
                else
                {
                    SetTextComp("R2_D" + hang + "_C" + i, "");
                }

                CompBackColor("R2_D" + hang + "_C" + i, myRgbColor);
            }
            CompDisableEnable("R2_Cau" + hang, true);
            CompBackColor("R2_Cau" + hang, Color.DarkGray);
        }

        #endregion

        #region Action Mo o chu
        public void showRound2Answer(DataServer data)
        {
            InitTangToc(data, "RESET");
            mo_o_chu.Play();

            fillOChu(data.QuestionNum, data.QuestionAnswer);
        }

        public void fillOChu(int hang, string ochu)
        {
            Color whiteColor = Color.White;
            Color yellowColor = Color.Yellow;

            for (int i = 1; i <= 29; i++)
            {
                if (i == 15)
                {
                    CompBackColor("R2_D" + hang + "_C" + i, yellowColor);
                }
                else
                {
                    CompBackColor("R2_D" + hang + "_C" + i, whiteColor);
                }

            }
            if (!string.IsNullOrEmpty(ochu))
            {
                ochu = ochu.TrimStart();
                ochu = ochu.TrimEnd();
                if (ochu.Length != 29)
                {
                    showMessage("Ô chữ " + ochu + " sai định dạng");
                    return;
                }
                Control txtTemp;
                for (int j = 0; j < 29; j++)
                {
                    if (ochu[j].ToString() != "$")
                    {
                        string x = "R2_D" + hang + "_C" + (j + 1) + "";

                        txtTemp = (Control)this.Controls.Find(x, true).FirstOrDefault();
                        txtTemp.Text = ochu[j].ToString();
                    }
                }
                CompDisableEnable("R2_Cau" + hang, false);
                CompBackColor("R2_Cau" + hang, Color.FromArgb(233, 33, 39));
            }
        }
        #endregion

        #region MoHangDoc
        public void MoHangDoc(DataServer data)
        {
            InitTangToc(data, "RESET");
            show_hang_doc.Play();
            //Mo o chu hang doc
            if (data.HangDoc.Count > 0)
            {
                for (int j = 1; j <= 20; j++)
                {
                    if (!data.HangDoc[j - 1].Equals('$'))
                    {
                        SetTextComp("R2_D" + j + "_C15", data.HangDoc[j - 1]);
                        SetTextComp("R2_D" + j + "_C100", data.HangDoc[j - 1]);
                    }
                }
            }

            //isOpenCol = true;
        }
        #endregion

        #region Action UpdateScore and Change Name
        public void UpdateScore(DataServer data)
        {

            if (data != null && data.ContestTeams != null)
            {
                if (data.ContestTeams.Count > 0)
                {
                    R2_Team1.Text = data.ContestTeams[0].Name;
                    R2_Point1.Text = data.ContestTeams[0].Mark + "";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R2_Team2.Text = data.ContestTeams[1].Name;
                    R2_Point2.Text = data.ContestTeams[1].Mark + "";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R2_Team3.Text = data.ContestTeams[2].Name;
                    R2_Point3.Text = data.ContestTeams[2].Mark + "";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R2_Team4.Text = data.ContestTeams[3].Name;
                    R2_Point4.Text = data.ContestTeams[3].Mark + "";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R2_Team5.Text = data.ContestTeams[4].Name;
                    R2_Point5.Text = data.ContestTeams[4].Mark + "";
                }
                if (data.ContestTeams.Count > 5)
                {
                    R2_Team6.Text = data.ContestTeams[5].Name;
                    R2_Point6.Text = data.ContestTeams[5].Mark + "";
                }
            }

        }

        #endregion

        #region show, hide row
        public void ShowRow(DataServer data)
        {
            foreach (var row in data.myArrayShowRound2)
            {
                if (!string.IsNullOrEmpty(row))
                {
                    switch (int.Parse(row))
                    {
                        case 1:
                            pn1.Location = new Point(0, 0);
                            break;
                        case 2:
                            pn2.Location = new Point(0, 31);
                            break;
                        case 3:
                            pn3.Location = new Point(0, 62);
                            break;
                        case 4:
                            pn4.Location = new Point(0, 93);
                            break;
                        case 5:
                            pn5.Location = new Point(0, 124);
                            break;
                        case 6:
                            pn6.Location = new Point(0, 155);
                            break;
                        case 7:
                            pn7.Location = new Point(0, 186);
                            break;
                        case 8:
                            pn8.Location = new Point(0, 217);
                            break;
                        case 9:
                            pn9.Location = new Point(0, 248);
                            break;
                        case 10:
                            pn10.Location = new Point(0, 279);
                            break;
                        case 11:
                            pn11.Location = new Point(0, 310);
                            break;
                        case 12:
                            pn12.Location = new Point(0, 341);
                            break;
                        case 13:
                            pn13.Location = new Point(0, 371);
                            break;
                        case 14:
                            pn14.Location = new Point(0, 401);
                            break;
                        case 15:
                            pn15.Location = new Point(0, 431);
                            break;
                        case 16:
                            pn16.Location = new Point(0, 461);
                            break;
                        case 17:
                            pn17.Location = new Point(0, 491);
                            break;
                        case 18:
                            pn18.Location = new Point(0, 520);
                            break;
                        case 19:
                            pn19.Location = new Point(0, 549);
                            break;
                        case 20:
                            pn20.Location = new Point(0, 579);
                            break;
                        default:

                            break;
                    }
                }
            }
        }
        public void HideRow(DataServer data)
        {
            foreach (var row in data.myArrayRound2)
            {
                switch (row)
                {
                    case 1:
                        pn1.Location = new Point(1000, 1000);
                        break;
                    case 2:
                        pn2.Location = new Point(1000, 1000);
                        break;
                    case 3:
                        pn3.Location = new Point(1000, 1000);
                        break;
                    case 4:
                        pn4.Location = new Point(1000, 1000);
                        break;
                    case 5:
                        pn5.Location = new Point(1000, 1000);
                        break;
                    case 6:
                        pn6.Location = new Point(1000, 1000);
                        break;
                    case 7:
                        pn7.Location = new Point(1000, 1000);
                        break;
                    case 8:
                        pn8.Location = new Point(1000, 1000);
                        break;
                    case 9:
                        pn9.Location = new Point(1000, 1000);
                        break;
                    case 10:
                        pn10.Location = new Point(1000, 1000);
                        break;
                    case 11:
                        pn11.Location = new Point(1000, 1000);
                        break;
                    case 12:
                        pn12.Location = new Point(1000, 1000);
                        break;
                    case 13:
                        pn13.Location = new Point(1000, 1000);
                        break;
                    case 14:
                        pn14.Location = new Point(1000, 1000);
                        break;
                    case 15:
                        pn15.Location = new Point(1000, 1000);
                        break;
                    case 16:
                        pn16.Location = new Point(1000, 1000);
                        break;
                    case 17:
                        pn17.Location = new Point(1000, 1000);
                        break;
                    case 18:
                        pn18.Location = new Point(1000, 1000);
                        break;
                    case 19:
                        pn19.Location = new Point(1000, 1000);
                        break;
                    case 20:
                        pn20.Location = new Point(1000, 1000);
                        break;
                    default:

                        break;
                }

            }
        }
        #endregion

        #region temp

        public void SetTextComp(String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            comp.Text = text;
        }
        public void CompLabelBorderStyle(string compName, BorderStyle border)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = false;
        }

        public void showMessage(string msg)
        {
            MessageBox.Show(msg);
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
            comp.BackColor = color;
        }
        public void CompVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
        }

        public void CompDisableEnable(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Enabled = flag;
        }
        private int GetFormArea(Size size)
        {
            return size.Height * size.Width;
        }
        private void R2_Question_Resize(object sender, EventArgs e)
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

        #endregion

    }
}
