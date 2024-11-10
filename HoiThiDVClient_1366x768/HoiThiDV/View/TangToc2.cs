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
    public partial class TangToc2 : UserControl
    {
        private DateTime startTime;
        private TimeSpan timeElapsed;
        System.Threading.Timer TheTimer = null;
        private MasterController ctrl;
        public bool isInitOchu = false;
        private bool isAnswer = true;
        //public bool isOpenCol = false;

        public TangToc2()
        {
            InitializeComponent();
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }

        #region Init TangToc
        public void InitTangToc(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                SetTextComp("R2_Team" + i, "");
                SetTextComp("R2_Point" + i, "");
            }
            this.R2_Clock.Text = "0.00";
            R2_TeamAnswer.Text = "";
            R2_TxtAnswer.Text = "";
            //CompDisableEnable("R2_TxtAnswer", false);
            TextboxReadOnly("R2_TxtAnswer", true);
            CompDisableEnable("R2_BtnAnswer", false);
            if (TheTimer != null) TheTimer.Dispose();

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
            }


            R2_Question.Text = "";

        }
        public void InitOChu(DataServer data)
        {

            // Init
            //Color whiteColor = Color.White;
            //Color yellowColor = Color.Yellow;
            //for (int j = 1; j <= 15; j++)
            //{
            //    CompVisible("R2_Cau" + j, true);
            //    CompDisableEnable("R2_Cau" + j, true);
            //    for (int k = 1; k <= 29; k++)
            //    {
            //        CompVisible("R2_D" + j + "_C" + k, true);
            //        SetTextComp("R2_D" + j + "_C" + k, "");
            //        if (k == 15)
            //        {
            //            CompBackColor("R2_D" + j + "_C" + k, yellowColor);
            //        }
            //        else
            //        {
            //            CompBackColor("R2_D" + j + "_C" + k, whiteColor);
            //        }
            //    }
            //}
            if (data != null && data.InitOchu != null && data.InitOchu.Count > 0 && !isInitOchu)
            {
                for (int j = 1; j <= 15; j++)
                {
                    CompBackColor("R2_Cau" + j, Color.FromArgb(233, 33, 39));
                    SetTextComp("R2_D" + j + "_Count", "");
                }

                Color whiteColor = Color.White;
                Color yellowColor = Color.Yellow;
                Color darkgrayColor = Color.DarkGray;
                // Set
                for (int dong = 1; dong <= data.InitOchu.Count; dong++)
                {
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
                            }
                            //CompVisible("R2_D" + dong + "_C" + cot, true);
                            isDongEmpty = false;
                        }
                    }
                    if (isDongEmpty)
                    {
                        CompVisible("R2_Cau" + dong, false);
                        SetTextComp("R2_D" + dong + "_Count", "");
                    }
                    else
                    {
                        Control comp = (Control)this.Controls.Find("R2_Cau" + dong, true).FirstOrDefault();
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
                        //CompVisible("R2_Cau" + dong, true);

                        string ochu = data.InitOchu[dong - 1].Replace("$", "");
                        if (ochu.Length == 1 && !data.InitOchu[dong - 1][14].ToString().Equals("$"))
                        {
                            CompDisableEnable("R2_Cau" + dong, false);
                            SetTextComp("R2_D" + dong + "_Count", "(" + ochu.Length + ")");
                        }
                        else
                        {
                            CompDisableEnable("R2_Cau" + dong, true);
                            SetTextComp("R2_D" + dong + "_Count", "(" + ochu.Length + ")");
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
                this.panel12.Refresh();
            }
        }
        #endregion
        #region Action UpdateScore
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
            }

        }
        #endregion

        #region Action Mo o chu
        public void showRound2Answer(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                SetTextComp("R2_Team" + i, "");
                SetTextComp("R2_Point" + i, "");
            }
            this.R2_Clock.Text = "0.00";
            TextboxReadOnly("R2_TxtAnswer", true);
            CompDisableEnable("R2_BtnAnswer", false);
            if (TheTimer != null) TheTimer.Dispose();

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
            }
            fillOChu(data.QuestionNum, data.QuestionAnswer);
        }
        #endregion

        #region Dong o chu
        public void closeRound2Answer(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                SetTextComp("R2_Team" + i, "");
                SetTextComp("R2_Point" + i, "");
            }
            this.R2_Clock.Text = "0.00";
            TextboxReadOnly("R2_TxtAnswer", true);
            CompDisableEnable("R2_BtnAnswer", false);
            if (TheTimer != null) TheTimer.Dispose();

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
            }
            dongOChu(data.QuestionNum, data.isOpenCol);
        }
        #endregion
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

        public void LoadQuestion(DataServer data)
        {
            //CompDisableEnable("R2_Cau" + data.QuestionNum, false);
            InitTangToc(data);

            for (int i = 1; i <= data.InitOchu.Count; i++)
            {
                if (i == data.QuestionNum)
                {
                    R2_QuestN_Selected(i);
                }
                else
                {
                    R2_QuestN_UnSelected(i);
                }
            }
        }

        public void R2_QuestN_Selected(int questNum)
        {
            Control comp = (Control)this.Controls.Find("R2_Cau" + questNum, true).FirstOrDefault();
            Color myRgbColor = Color.FromArgb(233, 33, 39);
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

        public void ShowQuestion(DataServer data)
        {
            LoadQuestion(data);
            R2_Question.Text = data.QuestionText;
        }

        public void MoHangDoc(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                SetTextComp("R2_Team" + i, "");
                SetTextComp("R2_Point" + i, "");
            }
            this.R2_Clock.Text = "0.00";
            TextboxReadOnly("R2_TxtAnswer", true);
            CompDisableEnable("R2_BtnAnswer", false);
            if (TheTimer != null) TheTimer.Dispose();

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
            }
            //Mo o chu hang doc
            if (data.HangDoc.Count > 0)
            {
                for (int j = 1; j <= 15; j++)
                {
                    if (!data.HangDoc[j - 1].Equals('$')) SetTextComp("R2_D" + j + "_C15", data.HangDoc[j - 1]);
                }
            }

            //isOpenCol = true;
        }

        #region Action Run Timer

        public void RunTimer(DataServer data)
        {
            isAnswer = false;
            ShowQuestion(data);
            startTime = DateTime.Now;
            //CompDisableEnable("R2_TxtAnswer", true);
            TextboxReadOnly("R2_TxtAnswer", false);
            CompDisableEnable("R2_BtnAnswer", true);
            R2_TxtAnswer.Focus();
            TheTimer = new System.Threading.Timer(
                    this.Tick, null, 0, 10);
        }

        public void Tick(object info)
        {
            timeElapsed = DateTime.Now - startTime;
            if (timeElapsed.TotalMilliseconds >= 30000)
            {
                this.R2_Clock.Text = "30.00";
                //CompDisableEnable("R2_TxtAnswer", false);
                TextboxReadOnly("R2_TxtAnswer", true);
                CompDisableEnable("R2_BtnAnswer", false);
                this.TheTimer.Dispose();
                return;
            }
            this.R2_Clock.Text = timeElapsed.TotalSeconds.ToString("0.00");
        }

        #endregion

        #region Common Event
        public void showMessage(string msg)
        {
            MessageBox.Show(msg);
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
        public void CompVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
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
        #endregion

        #region Answer The Question
        private void R2_TxtAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (R2_TxtAnswer.Text.Trim() != "")
                {
                    AnswerTheQuestion();
                }
                else
                {
                    R2_TxtAnswer.Text = "";
                }
            }
        }

        private void R2_BtnAnswer_Click(object sender, EventArgs e)
        {
            AnswerTheQuestion();
        }
        public void AnswerTheQuestion()
        {
            if (!isAnswer)
            {
                string answer = R2_TxtAnswer.Text.Trim();
                TimeSpan timeElapsed = DateTime.Now - startTime;
                if (timeElapsed.TotalMilliseconds > 30000) return;
                string txt = answer + " - " + timeElapsed.TotalSeconds.ToString("0.00 s");
                this.R2_TeamAnswer.Text = txt;
                //CompDisableEnable("R2_TxtAnswer", false);
                R2_TxtAnswer.Text = "";
                R2_TxtAnswer.ReadOnly = true;
                R2_TxtAnswer.Focus();
                CompDisableEnable("R2_BtnAnswer", false);
                isAnswer = true;
                // Send to server
                this.ctrl.SendAnswer(answer, timeElapsed.TotalSeconds.ToString("0.00"), 2);
            }

        }

        #endregion

        private void TangToc2_Load(object sender, EventArgs e)
        {
            R2_TxtAnswer.Focus();
            R2_TxtAnswer.Select();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void R2_Question_Click(object sender, EventArgs e)
        {
            R2_TxtAnswer.Focus();
        }

        private void R2_Cau15_Click(object sender, EventArgs e)
        {
            R2_TxtAnswer.Focus();
        }

        private void R2_Question_Click(object sender, MouseEventArgs e)
        {
            R2_TxtAnswer.Focus();
        }
        public void TxtAnswerFocus()
        {
            R2_TxtAnswer.Focus();
        }
    }
}
