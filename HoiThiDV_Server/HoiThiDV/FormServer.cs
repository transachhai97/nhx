
using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoiThiDV.DAO;
//using HoiThiDV.Api;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace HoiThiDV
{
    public partial class FormServer : Form
    {

        private MasterController ctrl;
        private DateTime startTime1;
        private DateTime startTime2;
        private DateTime startTimeGT;
        private DateTime startTimeBC;

        private TimeSpan timeElapsed1;
        private TimeSpan timeElapsed2;
        private TimeSpan timeElapsedGT;
        private TimeSpan timeElapsedBC;
        private bool isInitOChu = false;
        private bool play;
        private SoundPlayer round1Sound, round2Sound;
        //private List<int> questRound2;
        //private List<int> questRound1;
        //private bool isOpenCol = false;

        private int answeredCount = 0;
        private int viewbgk = 0;

        public int currentQuest { get; set; }
        public int countContestTeams = 0;
        private int diemthuong = 0;
        //Tăng tốc
        private int secondsR2;
        private int minutesR2 = 0;
        private int initMinR2 = 30;//30 giây

        //Thiết lập thời gian dành cho màn Giới thiệu
        private int seconds;
        private int minutes;
        private int initMin;
        private String diemphat;
        private SoundPlayer tingfile, warnfile, ticktok;

        //MÀN BẤM CHUÔNG
        private int secondsBC;
        private int minutesBC = 0;
        private int initMinBC = 15;//10 giây
        public int[] myArray = new int[] { };
        public FormServer()
        {
            InitializeComponent();
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }

        public void checkForIllegalThreadCalls(bool b)
        {
            CheckForIllegalCrossThreadCalls = b;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            loadSound();
            this.ctrl.dataServer.questRound1 = new List<int>();
            this.ctrl.dataServer.questRoundBC = new List<int>();
            this.ctrl.dataServer.questRoundBC_phu = new List<int>();
            this.ctrl.dataServer.questRoundBC_hungbien = new List<int>();
            this.ctrl.dataServer.questRoundBC_duphongvong1 = new List<int>();
            this.ctrl.dataServer.questRoundBC_duphongvong2 = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                this.ctrl.dataServer.questRound1.Add(0);
                this.ctrl.dataServer.questRoundBC.Add(0);
                this.ctrl.dataServer.questRoundBC_phu.Add(0);
                this.ctrl.dataServer.questRoundBC_hungbien.Add(0);
                this.ctrl.dataServer.questRoundBC_duphongvong1.Add(0);
                this.ctrl.dataServer.questRoundBC_duphongvong2.Add(0);
            }
            this.ctrl.dataServer.questRound2 = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                this.ctrl.dataServer.questRound2.Add(0);
            }
            this.ctrl.loadOChu();

            //load combobox điểm thưởng
            loadCboBonusPoint();

            //load câu hình phần thi (mục cấu hình khởi động)
            loadCboCauHinhRound();
        }

        public void loadCboBonusPoint()
        {
            Dictionary<int, int> dicBonus = new Dictionary<int, int>();
            dicBonus.Add(1, 10);
            dicBonus.Add(2, 15);
            dicBonus.Add(3, 20);
            dicBonus.Add(4, 25);
            cboBonusPoint1.DataSource = new BindingSource(dicBonus, null);
            cboBonusPoint1.ValueMember = "Key";
            cboBonusPoint1.DisplayMember = "Value";

            cboBonusPoint2.DataSource = new BindingSource(dicBonus, null);
            cboBonusPoint2.ValueMember = "Key";
            cboBonusPoint2.DisplayMember = "Value";

            cboBonusPoint3.DataSource = new BindingSource(dicBonus, null);
            cboBonusPoint3.ValueMember = "Key";
            cboBonusPoint3.DisplayMember = "Value";

            cboBonusPoint4.DataSource = new BindingSource(dicBonus, null);
            cboBonusPoint4.ValueMember = "Key";
            cboBonusPoint4.DisplayMember = "Value";

            cboBonusPoint5.DataSource = new BindingSource(dicBonus, null);
            cboBonusPoint5.ValueMember = "Key";
            cboBonusPoint5.DisplayMember = "Value";

            Dictionary<int, int> dicBonusChange = new Dictionary<int, int>();
            dicBonusChange.Add(0, 200);
            dicBonusChange.Add(200, 175);
            dicBonusChange.Add(175, 150);
            dicBonusChange.Add(150, 125);

            cboDiemResetR1.DataSource = new BindingSource(dicBonusChange, null);
            cboDiemResetR1.ValueMember = "Key";
            cboDiemResetR1.DisplayMember = "Value";
        }

        public void loadCboCauHinhRound()
        {
            Dictionary<int, string> dicBonus = new Dictionary<int, string>();
            dicBonus.Add(1, "Khởi động");
            dicBonus.Add(5, "HIỂU NGHIỆP VỤ");
            dicBonus.Add(6, "Câu hỏi phụ");
            dicBonus.Add(7, "VỀ ĐÍCH");
            dicBonus.Add(9, "Dự phòng khởi động");
            dicBonus.Add(8, "Dự phòng HIỂU NGHIỆP VỤ");
            cboCauHinhRound.DataSource = new BindingSource(dicBonus, null);
            cboCauHinhRound.ValueMember = "Key";
            cboCauHinhRound.DisplayMember = "Value";
        }

        public void loadSound()
        {
            try
            {
                round1Sound = new SoundPlayer(Application.StartupPath + "\\15s.wav");
                round2Sound = new SoundPlayer(Application.StartupPath + "\\30s.wav");
                tingfile = new SoundPlayer(Application.StartupPath + "\\ting.wav");
                warnfile = new SoundPlayer(Application.StartupPath + "\\15s.wav");
                ticktok = new SoundPlayer(Application.StartupPath + "\\tiktoc.wav");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Common event and function
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab.Name == "tabTeamConfig")
                this.ctrl.loadContestants();
            else if (tabControl2.SelectedTab.Name == "tabRound1Config")
                this.ctrl.getRound1Questions();
            else if (tabControl2.SelectedTab.Name == "tabRound2Config")
            {
                //getHideRowRound2
                this.ctrl.GetRowDefaulRound2();
                addItemmyArray(C3_txtHideRow.Text);

                this.ctrl.getRound2Questions();
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pp.SelectedTab.Name == "tabRound1")
                this.ctrl.loadRound1();
            else if (pp.SelectedTab.Name == "tabRound2")
            {
                //getHideRowRound2
                this.ctrl.GetRowDefaulRound2();
                addItemmyArray(C3_txtHideRow.Text);

                this.ctrl.loadRound2();
            }

            else if (pp.SelectedTab.Name == "tabGioiThieu")
                this.ctrl.loadGioithieu();
            else if (pp.SelectedTab.Name == "tabBamChuong")
                this.ctrl.loadBamchuong();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        public int getGame()
        {
            return C1_cbxGame.SelectedIndex >= 0 ? (C1_cbxGame.SelectedIndex + 1) : 0;
        }

        public void setGameDefault(int game)
        {
            C1_cbxGame.SelectedIndex = game - 1;
        }

        public void showMessage(String msg)
        {
            MessageBox.Show(this, msg);
        }

        public void resetDiem()
        {
            for (int i = 1; i <= this.ctrl.dataServer.ContestTeams.Count; i++)
            {
                this.ctrl.dataServer.ContestTeams[i - 1].Mark = 0;
            }
        }
        private void cmdScreenRound1_Click(object sender, EventArgs e)
        {
            resetDiem();
            this.ctrl.SwitchClientToRound1();
        }
        private void cmdScreenTeam_Click(object sender, EventArgs e)
        {
            this.ctrl.SwitchClientToRound0();
        }
        private void cmdScreenRound2_Click(object sender, EventArgs e)
        {
            resetDiem();
            //getHideRowRound2
            this.ctrl.GetRowDefaulRound2();
            addItemmyArray(C3_txtHideRow.Text);
            this.ctrl.dataServer.myArrayRound2 = myArray;
            this.ctrl.SwitchClientToRound2();
        }
        private void cmdScreenRound_GT_Click(object sender, EventArgs e)
        {
            this.ctrl.SwitchClientToRound_GT();
        }
        private void cmdScreenRound_BC_Click(object sender, EventArgs e)
        {
            resetDiem();
            this.ctrl.showNameRoundBC(5);//hiển thị tên vòng thi (bấm chuông)
            this.ctrl.SwitchClientToRound_BC();
        }
        private void cmdScreenJudges_Click(object sender, EventArgs e)
        {
            loadGiamKhao(0);
            viewbgk = 0;
        }

        private void cmdScreenJudges1doi_Click(object sender, EventArgs e)
        {
            loadGiamKhao(1);
            viewbgk = 1;
        }

        #region Toggle support

        public void ToggleCompEnableDisable(int round, String componentName, bool flag)
        {

            Control comp = (Control)((TabPage)pp.Controls["tabRound" + round]).Controls[componentName];
            comp.Enabled = flag;
        }

        public void ToggleCompEnableDisable_BC(int round, String componentName, bool flag)
        {

            Control comp = (Control)((TabPage)pp.Controls["tabBamChuong"]).Controls[componentName];
            comp.Enabled = flag;
        }

        public void ToggleCompVisiable(int round, String componentName, bool flag)
        {

            Control comp = (Control)((TabPage)pp.Controls["tabRound" + round]).Controls[componentName];
            if (comp == null)
            {
                comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            }
            comp.Visible = flag;
        }

        public void ToggleCompVisiable_BC(int round, String componentName, bool flag)
        {

            Control comp = (Control)((TabPage)pp.Controls["tabBamChuong"]).Controls[componentName];
            comp.Visible = flag;
        }

        public void SetTextComp(int round, String componentName, String text)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            if (comp != null)
            {
                comp.Text = text;
            }
           
        }

        public string GetTextComp(int round, String componentName)
        {

            Control comp = (Control)this.Controls.Find(componentName, true).FirstOrDefault();
            if (comp != null)
            {
                return comp.Text;
            }

            return null;
            
        }
        public void CompBackColor(string compName, Color color)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.BackColor = color;
        }
        public void CompLabelBorderStyle(string compName, BorderStyle border)
        {
            Label comp = (Label)this.Controls.Find(compName, true).FirstOrDefault();
            comp.BorderStyle = border;

        }

        #endregion
        public void EditScoreOfTeamN(object sender, EventArgs e)
        {
            Label lblTeam = (Label)sender;
            int teamId = Int32.Parse(lblTeam.Name.Substring(lblTeam.Name.Length - 1));
            string teamName = GetTextComp(1, lblTeam.Name.Replace("Point", "Team"));
            using (var form = new ChangeScore())
            {
                form.setLabelChangeScore("ĐỔI ĐIỂM ĐỘI THI " + teamName);
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    float point = form.newScore;

                    // Apply
                    lblTeam.Text = point + "";
                    this.ctrl.dataServer.ContestTeams[teamId - 1].Mark = point;
                    this.ctrl.dataServer.Action = Constant.ACTION_UPDATE_SCORE;

                    int currentRound = getGame();

                    ContestantDAO contestantDAO = new ContestantDAO();
                    bool success = contestantDAO.UpdateContestantMark(currentRound, teamId, point);

                    if (success)
                    {
                        MessageBox.Show("Mark updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to update mark.");
                    }

                    this.ctrl.SendToClient();
                }
            }
        }
        #endregion

        #region Tab Danh sach doi
        private void C1_BtnSave_Click(object sender, EventArgs e)
        {
            this.ctrl.saveContestants();
        }

        public void setContestants(List<Contestant> list)
        {
            countContestTeams = list.Count;
            for (int i = 1; i <= 6; i++)
            {
                SetTextComp(0, "C1_txtTeam" + i, "");
            }
            if (list.Count > 0)
                C1_txtTeam1.Text = list[0].Name;
            if (list.Count > 1)
                C1_txtTeam2.Text = list[1].Name;
            if (list.Count > 2)
                C1_txtTeam3.Text = list[2].Name;
            if (list.Count > 3)
                C1_txtTeam4.Text = list[3].Name;
            if (list.Count > 4)
                C1_txtTeam5.Text = list[4].Name;
            if (list.Count > 5)
                C1_txtTeam6.Text = list[5].Name;
        }

        public List<Contestant> getListContestants()
        {
            List<Contestant> listContestants = new List<Contestant>();
            listContestants.Add(new Contestant(C1_txtTeam1.Text));
            listContestants.Add(new Contestant(C1_txtTeam2.Text));
            listContestants.Add(new Contestant(C1_txtTeam3.Text));
            listContestants.Add(new Contestant(C1_txtTeam4.Text));
            listContestants.Add(new Contestant(C1_txtTeam5.Text));
            listContestants.Add(new Contestant(C1_txtTeam6.Text));
            return listContestants;
        }
        #endregion

        #region Cấu hình ban giám khảo
        private void btnRefreshGK_Click(object sender, EventArgs e)
        {
            loadGiamKhao(viewbgk);
        }

        private void btnResetBGK_Click(object sender, EventArgs e)
        {
            resetDiemBGK();
            this.ctrl.dataServer.Round = 6;
            this.ctrl.dataServer.oneTeam = viewbgk;//1: hiển thị 1 đội thi, 0 là hiển thị 4 or 5 đội thi 
            this.ctrl.dataServer.nameOneTeam = txtNameTeamBGK.Text;//1: hiển thị 1 đội thi, 0 là hiển thị 4 or 5 đội thi
            this.ctrl.dataServer.Action = Constant.ACTION_RESET;
            this.ctrl.SendToClient();
        }
        public void loadGiamKhao(int oneTeam)
        {
            Dictionary<int, List<Judges>> arrBGK = new Dictionary<int, List<Judges>>();
            for (int j = 1; j <= this.ctrl.dataServer.ContestTeams.Count; j++)
            {
                List<Judges> lstju = new List<Judges>();
                for (int i = 1; i <= 6; i++)
                {
                    Judges ju = new Judges();
                    ju.Name = GetTextComp(6, "txtGK" + i);
                    ju.Mark = GetTextComp(6, "txtMarkGK" + j + i);
                    if (!string.IsNullOrEmpty(ju.Name) && !string.IsNullOrEmpty(ju.Mark))
                    {
                        lstju.Add(ju);
                    }
                }
                arrBGK.Add(j, lstju);
            }


            // Send to client
            this.ctrl.dataServer.Round = 6;
            this.ctrl.dataServer.Action = Constant.ACTION_UPDATE_SCORE;
            this.ctrl.dataServer.oneTeam = oneTeam;//1: hiển thị 1 đội thi, 0 là hiển thị 4 or 5 đội thi 
            this.ctrl.dataServer.nameOneTeam = txtNameTeamBGK.Text;//1: hiển thị 1 đội thi, 0 là hiển thị 4 or 5 đội thi
            this.ctrl.dataServer.arrJudgesTeams = arrBGK;
            this.ctrl.SendToClient();
        }
        #endregion

        #region Tab Cau hinh Khoi dong
        private void C2_BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(C2_TxtQuestContent.Text.Trim()))
            {
                showMessage("Hãy nhập nội dung câu hỏi.");
                return;
            }
            this.ctrl.saveRound1Question(int.Parse(cboCauHinhRound.SelectedValue.ToString()));
            C2_CbQuestNum.SelectedIndex = -1;
            C2_TxtQuestContent.Text = "";
        }

        private void C2_GridKhoiDong_SelectionChanged(object sender, EventArgs e)
        {
            if (C2_GridKhoiDong.SelectedRows != null && C2_GridKhoiDong.SelectedRows.Count > 0 && !C2_GridKhoiDong.SelectedRows[0].Cells[0].Value.ToString().Equals(""))
            {

                C2_CbQuestNum.SelectedIndex = int.Parse(C2_GridKhoiDong.SelectedRows[0].Cells[0].Value.ToString()) - 1;// C2_GridKhoiDong.SelectedRows[0].Cells[0].Value;
                                                                                                                       //      C2_CbQuestNum.SelectedIndex = C2_CbQuestNum.Items.IndexOf(C2_GridKhoiDong.SelectedRows[0].Cells[0].Value);
                C2_TxtQuestContent.Text = C2_GridKhoiDong.SelectedRows[0].Cells[1].Value.ToString();
            }
        }

        private void C2_BtnFile_Click(object sender, EventArgs e)
        {
            Thread t = new Thread((ThreadStart)(() =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.C2_TxtFileName.Text = dialog.FileName;
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void C2_BtnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(C2_TxtFileName.Text.Trim()))
            {
                showMessage("Hãy chọn file để import!");
                return;
            }
            this.ctrl.importRound1Questions(int.Parse(cboCauHinhRound.SelectedValue.ToString()));
            this.C2_TxtFileName.Text = "";
        }

        public void fillGridKhoiDong(DataTable dt)
        {
            //if (dt == null || dt.Columns.Count == 0)
            //{
            //    dt = new DataTable();
            //    dt.Columns.Add("Num");
            //    dt.Columns.Add("Content");
            //}
            C2_GridKhoiDong.DataSource = dt;
            C2_GridKhoiDong.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            C2_GridKhoiDong.ForeColor = Color.Black;

            if (C2_GridKhoiDong.ColumnCount == 0)
            {
                C2_GridKhoiDong.Columns[0].HeaderText = "Câu hỏi";
                C2_GridKhoiDong.Columns[0].Name = "Num";
                C2_GridKhoiDong.Columns[0].DataPropertyName = "Num";


                C2_GridKhoiDong.Columns[1].HeaderText = "Nội dung";
                C2_GridKhoiDong.Columns[1].Name = "Content";
                C2_GridKhoiDong.Columns[1].DataPropertyName = "Content";
                C2_GridKhoiDong.Columns[2].Visible = false;
                if (C2_GridKhoiDong.Columns.Count >= 6)
                {
                    C2_GridKhoiDong.Columns[3].Visible = false;
                    C2_GridKhoiDong.Columns[4].Visible = false;
                    C2_GridKhoiDong.Columns[5].Visible = false;
                }

            }

            C2_GridKhoiDong.Sort(C2_GridKhoiDong.Columns[0], ListSortDirection.Ascending);
            C2_GridKhoiDong.Columns[0].HeaderText = "Câu hỏi";
            C2_GridKhoiDong.Columns[1].HeaderText = "Nội dung";
            C2_GridKhoiDong.Columns[2].Visible = false;
            if (C2_GridKhoiDong.Columns.Count >= 6)
            {
                C2_GridKhoiDong.Columns[3].Visible = false;
                C2_GridKhoiDong.Columns[4].Visible = false;
                C2_GridKhoiDong.Columns[5].Visible = false;
            }
        }

        public Question getRound1Question()
        {
            Question q = new Question();
            q.QuestRound = int.Parse(cboCauHinhRound.SelectedValue.ToString());
            q.QuestNum = int.Parse(C2_CbQuestNum.SelectedItem.ToString());
            q.QuestContent = C2_TxtQuestContent.Text.Trim();
            q.QuestAnswer = C2_TxtQuestAnswer.Text.Trim();

            return q;
        }

        public string getRound1ExcelPath()
        {
            return C2_TxtFileName.Text.Trim();
        }

        private void R0_BtnGame_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc muốn chọn lượt mới?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                this.ctrl.setGame();

                this.ctrl.dataServer = new DataServer();
                this.ctrl.loadContestants();
                isInitOChu = false;
                this.ctrl.dataServer.questRound1 = new List<int>();
                this.ctrl.dataServer.questRoundBC = new List<int>();
                this.ctrl.dataServer.questRoundBC_phu = new List<int>();
                this.ctrl.dataServer.questRoundBC_hungbien = new List<int>();
                this.ctrl.dataServer.questRoundBC_duphongvong1 = new List<int>();
                this.ctrl.dataServer.questRoundBC_duphongvong2 = new List<int>();
                for (int i = 0; i < 15; i++)
                {
                    this.ctrl.dataServer.questRound1.Add(0);
                    this.ctrl.dataServer.questRoundBC.Add(0);
                    this.ctrl.dataServer.questRoundBC_phu.Add(0);
                    this.ctrl.dataServer.questRoundBC_hungbien.Add(0);
                    this.ctrl.dataServer.questRoundBC_duphongvong1.Add(0);
                    this.ctrl.dataServer.questRoundBC_duphongvong2.Add(0);
                }

                //reset danh sách ẩn mặc định
                myArray = new int[] { };
                this.ctrl.dataServer.myArrayRound2 = new int[] { }; 
                this.ctrl.dataServer.myArrayShowRound2 = new string[] { };
                //this.ctrl.dataServer.lock_unlockTeam = new int[] { };
                this.ctrl.dataServer.outTeam = new string[] { };
                this.ctrl.dataServer.status = false;
                R2_btnUnlockTeam_Click(sender, e);
                //getHideRowRound2
                this.ctrl.GetRowDefaulRound2();
                addItemmyArray(C3_txtHideRow.Text);

                this.ctrl.dataServer.questRound2 = new List<int>();
                for (int i = 0; i < 20; i++)
                {
                    this.ctrl.dataServer.questRound2.Add(0);
                    CompBackColor("R2_Quest" + (i + 1), Color.FromArgb(233, 33, 39));
                }
                //reset điểm của ban giám khảo
                resetDiemBGK();

                // set cau hinh trang

                SetTextComp(0, "C2_TxtQuestContent", "");
                SetTextComp(0, "C3_TxtQuestContent", "");

                // khoi dong      
                resetDataKhoiDong(0);

                // Bấm chuông      
                resetDataBamChuong(0);

                //Giới thiệu
                for (int i = 1; i <= 6; i++)
                {
                    SetTextComp(4, "R_GT_LblPoint" + i, "");
                    SetTextComp(4, "R_GT_LblTeam" + i, "");
                }

                // tang toc
                for (int i = 1; i <= 15; i++)
                {
                    for (int j = 1; j <= 29; j++)
                    {
                        SetTextComp(2, "R2_D" + i + "_C" + j, "");
                        ToggleCompVisiable(2, "R2_D" + i + "_C" + j, true);
                    }
                }

                for (int i = 1; i <= 6; i++)
                {
                    SetTextComp(2, "R2_TeamAnswer" + i, "");
                    SetTextComp(2, "R2_TimeAnswer" + i, "");
                    ToggleCompVisiable(2, "R2_BtnTrue" + i, false);
                    SetTextComp(2, "R2_LblPoint" + i, "");
                    SetTextComp(2, "R2_LblTeam" + i, "");
                }

                R2_QuestContent.Text = "";
                setupTimeR2();
                R2_BtnShow.Enabled = false;
                R2_BtnTime.Enabled = false;
                R2_BtnShowAnswer.Enabled = false;
                R2_BtnShowAnswerCT.Enabled = false;
                R2_BtnAddPoint.Enabled = false;
                R2_BtnOpenRow.Enabled = false;
                R2_BtnCloseRow.Enabled = false;
                R2_BtnReset.Enabled = false;
                R2_BtnOpenCol.Enabled = true;
                R2_btn30shangdoc.Enabled = false;
                R2_btnPause.Enabled = false;
                R2_btnResume.Enabled = false;
                for (int j = 1; j <= 15; j++)
                {
                    ToggleCompVisiable(2, "R2_Quest" + j, true);
                    ToggleCompEnableDisable(2, "R2_Quest" + j, true);

                }
                answeredCount = 0;
                // Send to client
                this.ctrl.dataServer.Round = 0;
                /*this.ctrl.dataServer.Action = Constant.ACTION_RESET_ALL;*/
                this.ctrl.loadOChu();
                this.ctrl.SendToClient();
            }
        }
        public void resetDiemBGK()
        {
            txtNameTeamBGK.Text = "";
            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    SetTextComp(0, "txtMarkGK" + j + i, "");
                }
            }
        }
        public void resetDataBamChuong(int reset)
        {
            R_BC_Question.Text = "";
            setupTimeBC();
            for (int i = 1; i <= 12; i++)
            {
                ToggleCompEnableDisable_BC(5, "R_BC_Quest" + i, true);

            }

            for (int i = 1; i <= 6; i++)
            {
                ToggleCompVisiable_BC(5, "R_BC_BtnTrue" + i, false);
                ToggleCompVisiable_BC(5, "R_BC_BtnTrue_x2_" + i, false);
                ToggleCompVisiable_BC(5, "R_BC_BtnTrue_chia2_" + i, false);

                if (reset == 0)
                {//chọn đội thi khác thì reset.
                    SetTextComp(5, "R_BC_LblPoint" + i, "");
                    SetTextComp(5, "R_BC_LblTeam" + i, "");
                }

            }

            ToggleCompEnableDisable_BC(5, "R_BC_BtnShow", false);
            ToggleCompEnableDisable_BC(5, "R_BC_BtnTime", false);
            ToggleCompEnableDisable_BC(5, "R_BC_BtnAddPoint", false);
            ToggleCompEnableDisable_BC(5, "R_BC_BtnReset", false);
        }

        public void resetDataKhoiDong(int reset)
        {
            R1_Question.Text = "";
            R1_Clock.Text = "0.00";
            for (int i = 1; i <= 15; i++)
            {
                ToggleCompEnableDisable(1, "R1_Quest" + i, true);

            }

            for (int i = 1; i <= 5; i++)
            {
                ToggleCompVisiable(1, "R1_BtnTrue" + i, false);
                SetTextComp(1, "R1_TeamAnswer" + i, "");
                SetTextComp(1, "R1_TimeAnswer" + i, "");
                if (reset == 0)
                {//chọn đội thi khác thì reset.
                    SetTextComp(1, "R1_LblPoint" + i, "");
                    SetTextComp(1, "R1_LblTeam" + i, "");
                }
            }

            ToggleCompEnableDisable(1, "R1_BtnShow", false);
            ToggleCompEnableDisable(1, "R1_BtnTime", false);
            ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswer", false);
            ToggleCompEnableDisable(1, "R1_BtnReset", false);
        }
        #endregion

        #region Tab cau hinh tang toc
        private void C3_BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(C3_TxtQuestContent.Text.Trim()))
            {
                MessageBox.Show("Hãy nhập dữ kiện của câu hỏi.");
                return;
            }
            this.ctrl.saveRound2Question();
            C3_CbQuestNum.SelectedIndex = -1;
            C3_TxtQuestContent.Text = "";
        }

        private void C3_GridTangToc_SelectionChanged(object sender, EventArgs e)
        {
            if (C3_GridTangToc.SelectedRows != null && C3_GridTangToc.SelectedRows.Count > 0)
            {
                C3_CbQuestNum.SelectedIndex = int.Parse(C3_GridTangToc.SelectedRows[0].Cells[0].Value.ToString()) - 1;
                C3_TxtQuestContent.Text = C3_GridTangToc.SelectedRows[0].Cells[1].Value.ToString();
            }
        }

        private void C3_BtnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.C3_TxtFile.Text = dialog.FileName;
            }
        }

        private void C3_BtnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(C3_TxtFile.Text.Trim()))
            {
                showMessage("Hãy chọn file để import!");
                return;
            }
            this.ctrl.importRound2Questions();
            C3_TxtFile.Text = "";
        }

        private void C3_BtnOChu_Click(object sender, EventArgs e)
        {
            isInitOChu = false;
            this.ctrl.updateRound2Questions();
            showMessage("Nhập ô chữ thành công!");
        }
        public void setHideRowRound2(string val)
        {
            C3_txtHideRow.Text = val;
        }
        public void fillGridTangToc(DataTable dt)
        {
            if (dt == null || dt.Columns.Count == 0)
            {
                dt = new DataTable();
                dt.Columns.Add("Num");
                dt.Columns.Add("Content");
                dt.Columns.Add("QuestAnswer_vi");
                dt.Columns.Add("ShowQuestNum");

            }
            C3_GridTangToc.DataSource = dt;
           // C3_GridTangToc.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            C3_GridTangToc.ForeColor = Color.Black;

            if (C3_GridTangToc.ColumnCount == 0)
            {
                C3_GridTangToc.Columns[0].HeaderText = "Câu hỏi";
                C3_GridTangToc.Columns[0].Name = "Num";
                C3_GridTangToc.Columns[0].DataPropertyName = "Num";

                C3_GridTangToc.Columns[1].HeaderText = "Nội dung";
                C3_GridTangToc.Columns[1].Name = "Content";
                C3_GridTangToc.Columns[1].DataPropertyName = "Content";

                C3_GridTangToc.Columns[2].Visible = false;

                C3_GridTangToc.Columns[3].HeaderText = "Câu trả lời";
                C3_GridTangToc.Columns[3].Name = "QuestAnswer_vi";
                C3_GridTangToc.Columns[3].DataPropertyName = "QuestAnswer_vi";

                C3_GridTangToc.Columns[4].HeaderText = "Số hiển thị";
                C3_GridTangToc.Columns[4].Name = "ShowQuestNum";
                C3_GridTangToc.Columns[4].DataPropertyName = "ShowQuestNum";

            }

            C3_GridTangToc.Sort(C3_GridTangToc.Columns[0], ListSortDirection.Ascending);
            C3_GridTangToc.Columns[0].HeaderText = "Câu hỏi";
            C3_GridTangToc.Columns[1].HeaderText = "Nội dung";
            C3_GridTangToc.Columns[2].Visible = false;
            C3_GridTangToc.Columns[3].HeaderText = "Câu trả lời";
            C3_GridTangToc.Columns[4].HeaderText = "Số hiển thị";
            
        }

        public void fillOChu(int hang, string ochu)
        {
            if (!string.IsNullOrEmpty(ochu))
            {
                ochu = ochu.TrimStart();
                ochu = ochu.TrimEnd();
                if (ochu.Length != 29)
                {
                    showMessage("Ô chữ " + ochu + " sai định dạng");
                    return;
                }
                TextBox txtTemp;
                Label lblTemp;
                for (int j = 0; j < 29; j++)
                {
                    string x = "C3_D" + hang + "_C" + (j + 1) + "";
                    if (hang > 15)
                    {
                        lblTemp = (Label)tabRound2Config.Controls[x];
                        if (ochu[j].ToString() != "$")
                        {
                            lblTemp.Text = ochu[j].ToString();
                        }
                        else
                        {
                            lblTemp.Text = "";
                        }
                    }
                    else
                    {
                        txtTemp = (TextBox)tabRound2Config.Controls[x];
                        if (ochu[j].ToString() != "$")
                        {
                            txtTemp.Text = ochu[j].ToString();
                        }
                        else
                        {
                            txtTemp.Text = "";
                        }
                    }


                }

            }
        }

        public Question getRound2Question()
        {
            Question q = new Question();
            q.QuestRound = 2;
            q.QuestNum = int.Parse(C3_CbQuestNum.SelectedItem.ToString());
            q.QuestContent = C3_TxtQuestContent.Text.Trim();
            q.QuestAnswer = "";

            return q;
        }

        public Question getHideRowRound2()
        {
            Question q = new Question();
            q.QuestRound = 2;
            q.QuestContent = C3_txtHideRow.Text.Trim();

            return q;
        }

        public string getRound2ExcelPath()
        {
            return C3_TxtFile.Text.Trim();
        }

        public Question[] getRound2ListQuestions()
        {
            Question[] qlist = new Question[20];
            Question q;
            string ochu = "";
            for (int i = 1; i <= 20; i++) // update 20 hang chu
            {
                ochu = "";
                TextBox txtTemp;
                Label lblTemp;
                for (int j = 1; j <= 29; j++)
                {
                    string x = "C3_D" + i + "_C" + j + "";
                    if (i > 15)
                    {
                        lblTemp = (Label)tabRound2Config.Controls[x];
                        if (lblTemp.Text != "") ochu += lblTemp.Text;
                        else ochu += "$";
                    }
                    else
                    {
                        txtTemp = (TextBox)tabRound2Config.Controls[x];
                        if (txtTemp.Text != "") ochu += txtTemp.Text;
                        else ochu += "$";
                    }
                }
                q = new Question();
                q.QuestRound = 2;
                q.QuestNum = i;
                q.QuestAnswer = ochu;
                qlist[i - 1] = q;
            }
            return qlist;
        }


        #endregion

        #region Tab Khoi dong
        private void clickColor1_Click(object sender, EventArgs e)
        {
            R1_LblTeam1.BackColor = Color.FromArgb(0, 102, 255);
            this.ctrl.showRound_1_bell(1, true);
        }
        private void R1_QuestN_Click(object sender, EventArgs e)
        {
            this.currentQuest = int.Parse(((Button)sender).Text);
            this.ctrl.loadRound1Question(int.Parse(cboChooseRoundVong1.SelectedValue.ToString()));
            ToggleCompEnableDisable(1, "R1_BtnShow", true);
            ToggleCompEnableDisable(1, "R1_BtnTime", false);
            ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswer", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswerCT", false);
            ToggleCompEnableDisable(1, "R1_BtnReset", false);

            //reset điểm cho từng câu hỏi
            diemthuong = 0;
        }

        private void R1_BtnShow_Click(object sender, EventArgs e)
        {
            ToggleCompEnableDisable(1, "R1_BtnShow", false);
            ToggleCompEnableDisable(1, "R1_BtnTime", true);
            ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswer", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswerCT", false);
            ToggleCompEnableDisable(1, "R1_BtnReset", false);

            this.ctrl.dataServer.minutes = 15;

            this.ctrl.showRound1Question(int.Parse(cboChooseRoundVong1.SelectedValue.ToString()));

            for (int i = 1; i <= 15; i++)
            {
                ToggleCompEnableDisable(1, "R1_Quest" + i, false);
            }
            // Reset dap an thi sinh
            for (int j = 0; j < countContestTeams; j++)
            {
                this.ctrl.dataServer.ContestTeams[j].Answer = "";
                this.ctrl.dataServer.ContestTeams[j].Time = "";
            }

            if (int.Parse(cboChooseRoundVong1.SelectedValue.ToString()) == 1)//áp dụng trong trường hợp đổi phần thi ở vòng khởi động (quay lại mặc định)
            {
                this.ctrl.dataServer.questRound1[this.currentQuest - 1] = 1;
            }
            else if (int.Parse(cboChooseRoundVong1.SelectedValue.ToString()) == 9)//áp dụng trong trường hợp đổi phần thi ở vòng khởi động (câu hỏi dự phòng vòng khởi động)
            {
                this.ctrl.dataServer.questRoundBC_duphongvong1[this.currentQuest - 1] = 1;
            }
        }

        private void R1_BtnTime_Click(object sender, EventArgs e)
        {
            ToggleCompEnableDisable(1, "R1_BtnShow", false);
            ToggleCompEnableDisable(1, "R1_BtnTime", false);
            ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswer", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswerCT", false);
            ToggleCompEnableDisable(1, "R1_BtnReset", false);

            if (!play)
            {
                play = true;
                //round1Sound.Play();
                play = false;
            }
            startTime1 = DateTime.Now;
            R1_Timer.Start();
            this.ctrl.dataServer.startTime = startTime1;
            this.ctrl.startRound1Timer();
        }

        private void R1_Timer_Tick(object sender, EventArgs e)
        {

            timeElapsed1 = DateTime.Now - startTime1;
            if (timeElapsed1.TotalMilliseconds >= 15000)
            {
                this.R1_Timer.Stop();
                this.R1_Clock.Text = "15.00";
                ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
                ToggleCompEnableDisable(1, "R1_BtnShowAnswer", true);
                ToggleCompEnableDisable(1, "R1_BtnShowAnswerCT", false);
                ToggleCompEnableDisable(1, "R1_BtnReset", false);
                return;
            }
            this.R1_Clock.Text = timeElapsed1.TotalSeconds.ToString("0.00");

        }



        public void setRound1Question(Question q)
        {
            R1_Question.Text = q.QuestContent;
        }

        public void InitRound1(DataServer data)
        {
            Dictionary<int, string> dicBonus = new Dictionary<int, string>();
            dicBonus.Add(1, "mặc định");
            dicBonus.Add(9, "dự phòng");
            cboChooseRoundVong1.DataSource = new BindingSource(dicBonus, null); ;
            cboChooseRoundVong1.ValueMember = "Key";
            cboChooseRoundVong1.DisplayMember = "Value";
            // Set Team
            if (data != null && data.ContestTeams != null)
            {
                if (data.ContestTeams.Count > 0)
                {
                    R1_LblTeam1.Text = data.ContestTeams[0].Name;
                    R1_LblPoint1.Text = data.ContestTeams[0].Mark + "";
                }
                if (data.ContestTeams.Count > 1)
                {
                    R1_LblTeam2.Text = data.ContestTeams[1].Name;
                    R1_LblPoint2.Text = data.ContestTeams[1].Mark + "";
                }
                if (data.ContestTeams.Count > 2)
                {
                    R1_LblTeam3.Text = data.ContestTeams[2].Name;
                    R1_LblPoint3.Text = data.ContestTeams[2].Mark + "";
                }
                if (data.ContestTeams.Count > 3)
                {
                    R1_LblTeam4.Text = data.ContestTeams[3].Name;
                    R1_LblPoint4.Text = data.ContestTeams[3].Mark + "";
                }
                if (data.ContestTeams.Count > 4)
                {
                    R1_LblTeam5.Text = data.ContestTeams[4].Name;
                    R1_LblPoint5.Text = data.ContestTeams[4].Mark + "";
                }
            }

            QuestionDAO qdao = new QuestionDAO();
            DataTable dtNumQuestion = qdao.GetAllQuest(1);
            for (int i = 1; i <= dtNumQuestion.Rows.Count; i++)
            {
                ToggleCompVisiable(1, "R1_Quest" + i, true);
            }
        }

        public void R1_BtnTrueN_Click(int number, object sender)
        {
            int point = 0;
            Control timeComp = (Control)tabRound1.Controls["R1_TimeAnswer" + number];
            string time = timeComp.Text;
            Control answer = (Control)tabRound1.Controls["R1_TeamAnswer" + number];
            string answ = answer.Text;
            Control bonus = (Control)tabRound1.Controls["cboBonusPoint" + number];

            if (answ != "" && time != "")
            {
                point = int.Parse(bonus.Text);
            }
            Control comp = (Control)this.panel3.Controls.Find("R1_LblPoint" + number, true).FirstOrDefault();
            comp.Text = this.ctrl.dataServer.ContestTeams[number - 1].Mark + point + "";
            this.ctrl.updateMark(number, point, 1);
            ((PictureBox)sender).Visible = false;
        }


        private void btnChooseRoundVong1_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc muốn chọn phần thi mới?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                resetDataKhoiDong(1);
                QuestionDAO qdao = new QuestionDAO();
                DataTable dtNumQuestion = qdao.GetAllQuest(int.Parse(cboChooseRoundVong1.SelectedValue.ToString()));//longvh
                for (int i = 1; i <= 15; i++)
                {
                    ToggleCompVisiable(1, "R1_Quest" + i, false);
                }
                for (int i = 1; i <= dtNumQuestion.Rows.Count; i++)
                {
                    ToggleCompVisiable(1, "R1_Quest" + i, true);
                }
            }
        }

        private void R1_BtnTrue1_Click(object sender, EventArgs e)
        {
            R1_BtnTrueN_Click(1, sender);
        }

        private void R1_BtnTrue2_Click(object sender, EventArgs e)
        {
            R1_BtnTrueN_Click(2, sender);
        }

        private void R1_BtnTrue3_Click(object sender, EventArgs e)
        {
            R1_BtnTrueN_Click(3, sender);
        }

        private void R1_BtnTrue4_Click(object sender, EventArgs e)
        {
            R1_BtnTrueN_Click(4, sender);
        }

        private void R1_BtnTrue5_Click(object sender, EventArgs e)
        {
            R1_BtnTrueN_Click(5, sender);
        }

        private void R1_BtnShowAnswer_Click(object sender, EventArgs e)
        {
            R1_BtnShowAnswer.Enabled = false;
            R1_BtnShowAnswerCT.Enabled = true;
            R1_BtnAddPoint.Enabled = true;
            btnChangeScoreR1.Enabled = true;
            this.ctrl.dataServer.Round = 3;

            for (int i = 1; i <= 5; i++)
            {
                ToggleCompVisiable(1, "clickColor" + i, true);
            }

            this.ctrl.SendToClient();

        }
        private void R1_BtnShowAnswerCT_Click(object sender, EventArgs e)
        {
            R1_BtnShowAnswerCT.Enabled = false;
            this.ctrl.showRound1Answer(int.Parse(cboChooseRoundVong1.SelectedValue.ToString()));
        }

        private void R1_BtnAddPoint_Click(object sender, EventArgs e)
        {
            R1_BtnReset.Enabled = true;
            R1_BtnAddPoint.Enabled = false;
            R1_BtnShowAnswerCT.Enabled = true;
            for (int i = 1; i <= 5; i++)
            {
                ToggleCompVisiable(1, "R1_BtnTrue" + i, false);
                ToggleCompVisiable(1, "cboBonusPoint" + i, false);
            }
            //cộng điểm tự động

            foreach (var item in this.ctrl.dataServer.ContestTeams)
            {
                if (item.BonusPoint != 0)
                {
                    float point = (float)item.BonusPoint;

                    Control comp = (Control)this.Controls.Find("R1_LblPoint" + item.Id, true).FirstOrDefault();
                    comp.Text = this.ctrl.dataServer.ContestTeams[item.Id - 1].Mark + point + "";
                    this.ctrl.dataServer.ContestTeams[item.Id - 1].Mark += point;

                }
            }
            this.ctrl.updateMark(0, 0, 1);
        }

        private void btnChangeScoreR1_Click(object sender, EventArgs e)
        {
            diemthuong = int.Parse(cboDiemResetR1.SelectedValue.ToString());
        }
        private void R1_BtnReset_Click(object sender, EventArgs e)
        {
            //var sortedTeams = this.ctrl.dataServer.ContestTeams.OrderByDescending(team => team.Mark).ToList();
            // Reset Client
            this.ctrl.dataServer.Action = Constant.ACTION_RESET;
            this.ctrl.dataServer.Round = 1;
            this.ctrl.dataServer.ContestTeams = this.ctrl.dataServer.ContestTeams;
            this.ctrl.SendToClient();

            //// Save all ContestTeams to ContestantDAO DB
            //ContestantDAO contestantDAO = new ContestantDAO();
            //foreach (var team in this.ctrl.dataServer.ContestTeams)
            //{
            //    contestantDAO.SaveContestant(getGame() , team); // Assuming SaveContestant is the method to save a contestant
            //}

            // Reset server
            R1_Question.Text = "";
            R1_Clock.Text = "0.00";
            if (int.Parse(cboChooseRoundVong1.SelectedValue.ToString()) == 1)//áp dụng trong trường hợp đổi phần thi ở vòng khởi động (quay lại mặc định)
            {
                for (int i = 1; i <= 15; i++)
                {
                    if (this.ctrl.dataServer.questRound1[i - 1] == 0)
                    {
                        ToggleCompEnableDisable(1, "R1_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable(1, "R1_Quest" + i, false);
                    }
                }
            }
            else if (int.Parse(cboChooseRoundVong1.SelectedValue.ToString()) == 9)//áp dụng trong trường hợp đổi phần thi ở vòng khởi động (câu hỏi dự phòng)
            {
                for (int i = 1; i <= 15; i++)
                {
                    if (this.ctrl.dataServer.questRoundBC_duphongvong1[i - 1] == 0)
                    {
                        ToggleCompEnableDisable(1, "R1_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable(1, "R1_Quest" + i, false);
                    }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                ToggleCompVisiable(1, "R1_BtnTrue" + i, false);
                ToggleCompVisiable(1, "cboBonusPoint" + i, false);
                SetTextComp(1, "R1_TeamAnswer" + i, "");
                SetTextComp(1, "R1_TimeAnswer" + i, "");
                CompBackColor("R1_LblTeam" + i, Color.FromArgb(0, 51, 48));
                ToggleCompVisiable(1, "clickColor" + i, false);

            }

            //reset điểm cộng tự động
            for (int i = 1; i <= this.ctrl.dataServer.ContestTeams.Count; i++)
            {
                this.ctrl.dataServer.ContestTeams[i - 1].BonusPoint = 0;
            }
            diemthuong = 0;
            // reset đổi điểm
            cboDiemResetR1.SelectedValue = 0;

            ToggleCompEnableDisable(1, "R1_BtnShow", false);
            ToggleCompEnableDisable(1, "R1_BtnTime", false);
            ToggleCompEnableDisable(1, "R1_BtnAddPoint", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswer", false);
            ToggleCompEnableDisable(1, "R1_BtnShowAnswerCT", false);
            ToggleCompEnableDisable(1, "btnChangeScoreR1", false);
            ToggleCompEnableDisable(1, "R1_BtnReset", false);
        }
        #endregion

        #region Tab Tang Toc
        private void R2_QuestN_Click(object sender, EventArgs e)
        {
            this.currentQuest = int.Parse(((Button)sender).Text);


            this.ctrl.loadRound2Question();
            ToggleCompEnableDisable(2, "R2_BtnShow", true);
            ToggleCompEnableDisable(2, "R2_BtnReset", false);

            for (int i = 1; i <= this.ctrl.dataServer.InitOchu.Count; i++)
            {
                if (Array.IndexOf(myArray, i) >= 0)
                {
                    //Your stuff goes here
                }
                else
                {
                    if (i == this.currentQuest)
                    {
                        R2_QuestN_Selected(i);
                    }
                    else
                    {
                        R2_QuestN_UnSelected(i);
                    }
                }

            }
        }
        public void R2_QuestN_Selected(int questNum)
        {
            Control comp = (Control)this.Controls.Find("R2_Quest" + questNum, true).FirstOrDefault();
            Color myRgbColor = Color.FromArgb(233, 33, 39);
            if (comp.Enabled)
                for (int i = 1; i <= 29; i++)
                {

                    CompBackColor("R2_D" + questNum + "_C" + i, myRgbColor);
                }
        }
        public void R2_QuestN_UnSelected(int questNum)
        {
            Control comp = (Control)this.Controls.Find("R2_Quest" + questNum, true).FirstOrDefault();
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

        private void R2_BtnShow_Click_1(object sender, EventArgs e)
        {
            for (int i = 1; i <= 20; i++)
                ToggleCompEnableDisable(2, "R2_Quest" + i, false);
            ToggleCompEnableDisable(2, "R2_BtnShow", false);
            ToggleCompEnableDisable(2, "R2_BtnTime", true);
            R2_BtnReset.Enabled = true;

            //questRound2[currentQuest - 1] = 1;
            this.ctrl.dataServer.minutes = 30;
            this.ctrl.showRound2Question();

            // Reset dap an thi sinh
            for (int j = 0; j < countContestTeams; j++)
            {
                this.ctrl.dataServer.ContestTeams[j].Answer = "";
                this.ctrl.dataServer.ContestTeams[j].Time = "";
            }
        }

        private void R2_BtnTime_Click(object sender, EventArgs e)
        {
            ToggleCompEnableDisable(2, "R2_BtnShow", false);
            ToggleCompEnableDisable(2, "R2_BtnTime", false);
            if (!play)
            {
                play = true;
                //round2Sound.Play();
                play = false;
            }
            startTime2 = DateTime.Now;
            R2_Timer.Start();
            this.ctrl.dataServer.startTime = startTime2;
            this.ctrl.dataServer.description = "START";//longvh1010
            this.ctrl.dataServer.minutes = 30;//longvh1010
            this.ctrl.startRound2Timer("");
        }
        private void R2_btn30shangdoc_Click(object sender, EventArgs e)
        {
            startTime2 = DateTime.Now;
            R2_Timer.Start();
            R2_BtnReset.Enabled = true;
            this.ctrl.dataServer.startTime = startTime2;
            this.ctrl.dataServer.description = "START";//longvh1010
            this.ctrl.dataServer.minutes = 30;//longvh1010
            this.ctrl.startRound2Timer("30shangdoc");
        }

        private void R2_btnResume_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { R2_Timer.Enabled = true; }));
            this.ctrl.dataServer.description = "START";//longvh1010
            // Send to control
            this.ctrl.startRound2Timer("30spause");
        }

        private void R2_btnPause_Click(object sender, EventArgs e)
        {
            R2_Timer.Enabled = false;
            this.ctrl.dataServer.description = "PAUSE";//longvh1010
            // Send to control time
            this.ctrl.startRound2Timer("30spause");
        }

        private void R2_BtnOpenRow_Click(object sender, EventArgs e)
        {
            R2_BtnReset.Enabled = true;
            this.ctrl.dataServer.questRound2[currentQuest - 1] = 1;
            this.ctrl.showRound2Answer();

        }

        private void R2_BtnCloseRow_Click(object sender, EventArgs e)
        {
            R2_BtnReset.Enabled = true;
            this.ctrl.dataServer.questRound2[currentQuest - 1] = 2;
            this.ctrl.closeRound2Answer();

        }

        private void R2_btnShowRow_Click(object sender, EventArgs e)
        {
            var _val = R2_txtRowShowHide.Text.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
            foreach (var item in _val)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ShowRow(int.Parse(item));
                    myArray = myArray.Where(val => val != int.Parse(item)).ToArray();
                }
            }
            this.ctrl.dataServer.myArrayRound2 = myArray;
            this.ctrl.showRow(_val);
        }

        private void R2_btnHideRow_Click(object sender, EventArgs e)
        {
            var _val = R2_txtRowShowHide.Text.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
            foreach (var item in _val)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    HideRow(int.Parse(item));
                    myArray = myArray.Concat(new int[] { int.Parse(item) }).ToArray();


                }
            }
            //int[] myInts = Array.ConvertAll(_val, s => int.Parse(s));
            this.ctrl.hideRow(myArray);
        }

        public void addItemmyArray(string rowhide)
        {
            var _val = rowhide.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
            foreach (var item in _val)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    myArray = myArray.Concat(new int[] { int.Parse(item) }).ToArray();
                }
            }
        }


        public void OpenRow(int row, string answer)
        {

            Color whiteColor = Color.White;
            Color yellowColor = Color.Yellow;

            for (int i = 1; i <= 29; i++)
            {
                if (i == 15)
                {
                    CompBackColor("R2_D" + row + "_C" + i, yellowColor);
                }
                else
                {
                    CompBackColor("R2_D" + row + "_C" + i, whiteColor);
                }

            }
            for (int j = 0; j < answer.Length; j++)
            {
                if (answer[j] != '$')
                {
                    SetTextComp(2, "R2_D" + row + "_C" + (j + 1), answer[j].ToString());
                }
            }

        }
        public void CloseRow(int row)
        {

            Color myRgbColor = Color.DarkGray;

            for (int i = 1; i <= 29; i++)
            {
                if (this.ctrl.dataServer.isOpenCol)
                {
                    if (i != 15)
                    {
                        SetTextComp(2, "R2_D" + row + "_C" + i, "");
                    }
                }
                else
                {
                    SetTextComp(2, "R2_D" + row + "_C" + i, "");
                }

                CompBackColor("R2_D" + row + "_C" + i, myRgbColor);
            }

        }
        public void ShowRow(int row)
        {

            Color whiteColor = Color.White;
            Color yellowColor = Color.Yellow;

            for (int i = 1; i <= 29; i++)
            {
                if (i == 15)
                {
                    CompBackColor("R2_D" + row + "_C" + i, yellowColor);
                }
                else
                {
                    CompBackColor("R2_D" + row + "_C" + i, whiteColor);
                }
                CompLabelBorderStyle("R2_D" + row + "_C" + i, BorderStyle.FixedSingle);
            }

        }
        public void HideRow(int row)
        {

            //Color myRgbColor = Color.DarkGray;

            for (int i = 1; i <= 29; i++)
            {
                if (this.ctrl.dataServer.isOpenCol)
                {
                    if (i != 15)
                    {
                        SetTextComp(2, "R2_D" + row + "_C" + i, "");
                    }
                }
                else
                {
                    SetTextComp(2, "R2_D" + row + "_C" + i, "");
                }

                CompBackColor("R2_D" + row + "_C" + i, Color.FromArgb(0, 82, 78));
                CompLabelBorderStyle("R2_D" + row + "_C" + i, BorderStyle.None);
            }

        }

        private void R2_Timer_Tick(object sender, EventArgs e)
        {
            secondsR2 += 1;
            if (secondsR2 > 59)
            {
                minutesR2 += 1;
                secondsR2 = 0;
            }

            if (minutesR2 == 30)//30 giây (tăng tốc)
            {
                if (secondsR2 == 0)
                {
                    R2_Timer.Enabled = false;
                    ToggleCompEnableDisable(2, "R2_BtnShowAnswer", true);
                    ToggleCompEnableDisable(2, "R2_BtnShowAnswerCT", false);
                }
            }
            
            if (minutesR2.ToString().Length < 2)
                lblMinR2.Text = "0" + minutesR2.ToString();
            else
                lblMinR2.Text = minutesR2.ToString();
            if (secondsR2.ToString().Length < 2)
                lblSecondR2.Text = "0" + secondsR2.ToString();
            else
                lblSecondR2.Text = secondsR2.ToString();

        }

        public void setupTimeR2()
        {
            R2_Timer.Enabled = false;
            minutesR2 = 0;
            secondsR2 = 0;
            lblMinR2.Text = "00";
            lblSecondR2.Text = "00";
            play = false;

        }

        public void setRound2Question(Question q)
        {
            R2_QuestContent.Text = q.QuestContent;
        }

        public void InitRound2(DataServer data)
        {

            // Set Team
            if (data != null && data.ContestTeams != null)
            {
                for (int i = 1; i <= data.ContestTeams.Count; i++)
                {
                    SetTextComp(2, "R2_LblTeam" + i, data.ContestTeams[i - 1].Name);
                    SetTextComp(2, "R2_LblPoint" + i, data.ContestTeams[i - 1].Mark + "");
                }



                //init ô chữ
                if (data.InitOchu.Count > 0 && !isInitOChu)
                {
                    Color whiteColor = Color.White;
                    Color yellowColor = Color.Yellow;
                    // Init 
                    for (int j = 1; j <= 20; j++)
                    {
                        ToggleCompEnableDisable(2, "R2_Quest" + j, true);
                        for (int k = 1; k <= 29; k++)
                        {
                            SetTextComp(2, "R2_D" + j + "_C" + k, "");
                            if (k == 15)
                            {
                                CompBackColor("R2_D" + j + "_C" + k, yellowColor);
                            }
                            else
                            {
                                CompBackColor("R2_D" + j + "_C" + k, whiteColor);
                            }
                        }
                    }
                    // Set
                    int n = 0;
                    foreach (string s in data.InitOchu)
                    {
                        bool isDongEmpty = true;
                        n++;
                        if (s.Equals("")) continue;
                        for (int j = 1; j <= 29; j++)
                        {
                            char s1 = s[j - 1];
                            if (s1.Equals('$'))
                            {
                                ToggleCompVisiable(2, "R2_D" + n + "_C" + j, false);
                            }
                            else
                            {
                                ToggleCompVisiable(2, "R2_D" + n + "_C" + j, true);
                                isDongEmpty = false;
                            }
                            CompLabelBorderStyle("R2_D" + n + "_C" + j, BorderStyle.FixedSingle);
                        }

                        if (isDongEmpty)
                        {
                            this.ctrl.dataServer.questRound2[n - 1] = 3;
                            ToggleCompVisiable(2, "R2_Quest" + n, false);
                        }
                        else
                        {
                            ToggleCompVisiable(2, "R2_Quest" + n, true);

                            string ochu = data.InitOchu[n - 1].Replace("$", "");
                            if (ochu.Length == 1 && !data.InitOchu[n - 1][14].ToString().Equals("$"))
                            {
                                ToggleCompEnableDisable(2, "R2_Quest" + n, false);
                                this.ctrl.dataServer.questRound2[n - 1] = 1;
                            }
                            else
                            {
                                ToggleCompEnableDisable(2, "R2_Quest" + n, true);

                            }
                        }
                    }
                    isInitOChu = true;
                }

                //ẩn dòng theo mặc định đc setup trước
                this.ctrl.GetRowDefaulRound2();
                addItemmyArray(C3_txtHideRow.Text);
                var tempList = myArray.ToList();
                //tempList.Add(5);
                myArray = tempList.ToArray();

                foreach (var item in myArray)
                {
                    // Show row in server
                    HideRow(item);

                }

                this.ctrl.hideRow(myArray);
            }

        }

        public void MoHangDoc(DataServer data)
        {
            //Mo o chu hang doc
            if (data.HangDoc.Count > 0)
            {
                for (int j = 1; j <= 20; j++)
                {
                    if (!data.HangDoc[j - 1].Equals('$')) SetTextComp(2, "R2_D" + j + "_C15", data.HangDoc[j - 1]);
                }
            }


        }

        private void R2_BtnShowAnswer_Click(object sender, EventArgs e)
        {
            R2_BtnShowAnswer.Enabled = false;
            R2_BtnShowAnswerCT.Enabled = true;
            R2_BtnAddPoint.Enabled = true;
            for (int i = 1; i <= 6; i++)
            {
                ToggleCompVisiable(2, "R2_BtnTrue" + i, true);
            }
            this.ctrl.dataServer.Round = 3;

            this.ctrl.SendToClient();

        }

        private void R2_BtnShowAnswerCT_Click(object sender, EventArgs e)
        {
            R2_BtnShowAnswerCT.Enabled = false;
            this.ctrl.showRound2DACT();
        }


        private void R2_BtnAddPoint_Click(object sender, EventArgs e)
        {
            R2_BtnShowAnswer.Enabled = false;
            R2_BtnShowAnswerCT.Enabled = false;
            R2_BtnOpenRow.Enabled = true;
            R2_BtnCloseRow.Enabled = true;
            R2_BtnAddPoint.Enabled = false;
            for (int i = 1; i <= 6; i++)
            {
                ToggleCompVisiable(2, "R2_BtnTrue" + i, false);
            }

            //cộng điểm tự động
            foreach (var item in this.ctrl.dataServer.ContestTeams)
            {
                if (item.BonusPoint != 0)
                {
                    Control comp = (Control)this.Controls.Find("R2_LblPoint" + item.Id, true).FirstOrDefault();
                    comp.Text = this.ctrl.dataServer.ContestTeams[item.Id - 1].Mark + item.BonusPoint + "";
                    this.ctrl.dataServer.ContestTeams[item.Id - 1].Mark += item.BonusPoint;

                }
            }
            this.ctrl.updateMark(0, 0, 2);
        }

        private void R2_BtnReset_Click(object sender, EventArgs e)
        {
            // Reset Client
            this.ctrl.dataServer.Action = Constant.ACTION_RESET;
            this.ctrl.dataServer.Round = 2;
            this.ctrl.SendToClient();
            // Reset Server
            for (int i = 1; i <= 6; i++)
            {
                SetTextComp(2, "R2_TeamAnswer" + i, "");
                SetTextComp(2, "R2_TimeAnswer" + i, "");
                CompBackColor("R2_LblTeam" + i, Color.FromArgb(0, 51, 48));
                ToggleCompVisiable(2, "R2_BtnTrue" + i, false);
            }
            R2_QuestContent.Text = "";
            R2_BtnAddPoint.Enabled = false;
            R2_BtnOpenRow.Enabled = false;
            R2_BtnCloseRow.Enabled = false;
            R2_BtnReset.Enabled = false;
            R2_BtnShowAnswer.Enabled = false;
            R2_BtnShowAnswerCT.Enabled = false;
            R2_BtnTime.Enabled = false;
            setupTimeR2();
            for (int j = 1; j <= 20; j++)//longvh1010
            {
                if (this.ctrl.dataServer.questRound2[j - 1] == 0)
                {
                    ToggleCompEnableDisable(2, "R2_Quest" + j, true);

                }
                else if (this.ctrl.dataServer.questRound2[j - 1] == 1)
                {
                    ToggleCompEnableDisable(2, "R2_Quest" + j, false);
                }
                else if (this.ctrl.dataServer.questRound2[j - 1] == 2)
                {
                    ToggleCompEnableDisable(2, "R2_Quest" + j, true);
                    CompBackColor("R2_Quest" + j, Color.DarkGray);
                }
            }

            //reset điểm cộng tự động
            for (int i = 1; i <= this.ctrl.dataServer.ContestTeams.Count; i++)
            {
                this.ctrl.dataServer.ContestTeams[i - 1].BonusPoint = 0;
            }

            answeredCount += 1;
            if (answeredCount == 1)
            {
                R2_BtnOpenCol.Enabled = true;
                R2_btn30shangdoc.Enabled = true;
                R2_btnPause.Enabled = true;
                R2_btnResume.Enabled = true;
            }
        }

        #region Add Point for Team
        public void R2_BtnTrueN_Click(int number, object sender)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[number - 1].Answer))
            {
                CompBackColor("R2_LblTeam" + number, Color.FromArgb(0, 102, 255));
                this.ctrl.dataServer.ContestTeams[number - 1].BonusPoint = 20;//30 điểm mỗi câu
                this.ctrl.showRound_2_AddPoint(number, true);
            }
        }
        private void R2_BtnTrue1_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(1, sender);
        }

        private void R2_BtnTrue2_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(2, sender);
        }

        private void R2_BtnTrue3_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(3, sender);
        }

        private void R2_BtnTrue4_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(4, sender);
        }

        private void R2_BtnTrue5_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(5, sender);
        }

        private void R2_BtnTrue6_Click(object sender, EventArgs e)
        {
            R2_BtnTrueN_Click(6, sender);
        }



        #endregion

        private void R2_BtnOpenCol_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc muốn mở ô chữ hàng dọc không?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                R2_BtnOpenCol.Enabled = false;
                this.ctrl.dataServer.isOpenCol = true;
                this.ctrl.moHangDoc();
            }
        }

        public void OpenCloseMultiRow(object sender, EventArgs e)
        {
            using (var form = new More())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string type = form.type;
                    string quest = form.quest;

                    string[] quests = quest.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
                    // Apply
                    if ("Mở ô chữ".Equals(type))
                    {
                        int num = 0;
                        foreach (string q in quests)
                        {
                            int.TryParse(q, out num);
                            if (num != 0 && num != 21)
                            {
                                this.ctrl.dataServer.questRound2[num - 1] = 1;
                                this.currentQuest = num;
                                this.ctrl.showRound2Answer();
                                ToggleCompEnableDisable(2, "R2_Quest" + num, false);
                                CompBackColor("R2_Quest" + num, Color.FromArgb(233, 33, 39));
                            }
                            else if (num == 21)
                            {
                                this.ctrl.dataServer.isOpenCol = true;
                                this.ctrl.moHangDoc();
                            }
                        }
                    }
                    else if ("Đóng ô chữ".Equals(type))
                    {
                        int num = 0;
                        foreach (string q in quests)
                        {
                            int.TryParse(q, out num);
                            if (num != 0 && num != 21)
                            {
                                ToggleCompEnableDisable(2, "R2_Quest" + num, true);
                                CompBackColor("R2_Quest" + num, Color.DarkGray);

                                this.ctrl.dataServer.questRound2[num - 1] = 2;
                                this.currentQuest = num;
                                this.ctrl.closeRound2Answer();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region tab Giới thiệu
        private void cboTitle_TextChanged(object sender, EventArgs e)
        {
            labTitle.Text = cboTitle.Text;
        }

        public void InitGioithieu(DataServer data)
        {

            // Set Team
            if (data != null && data.ContestTeams != null)
            {
                for (int i = 1; i <= data.ContestTeams.Count; i++)
                {
                    SetTextComp(4, "R_GT_LblTeam" + i, data.ContestTeams[i - 1].Name);
                    SetTextComp(4, "R_GT_LblPoint" + i, data.ContestTeams[i - 1].Mark + "");
                }
            }

            init();
            loadFile();

        }
        public void init()
        {
            initMin = 5;
            pauseToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            progressBar1.Maximum = 300;
            CheckForIllegalCrossThreadCalls = false;
            setupTime();

        }
        public void loadFile()
        {
            try
            {
                tingfile = new SoundPlayer(Application.StartupPath + "\\ting.wav");
                warnfile = new SoundPlayer(Application.StartupPath + "\\15s.wav");
                ticktok = new SoundPlayer(Application.StartupPath + "\\tiktoc1s.wav");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        private void R_GT_BtnShow_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_GT_content(cboTitle.Text);
        }


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
                if (minutes == 5 && seconds == 30)
                {
                    R_GT_Timer.Enabled = false;
                    //Tinh diem tru
                    tinhDiemVuotTime();
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

                if (minutes == 7 && seconds == 30)
                {
                    R_GT_Timer.Enabled = false;
                    //Tinh diem tru
                    tinhDiemVuotTime();
                }



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




            //timeElapsedGT = DateTime.Now - startTimeGT;

            //if (timeElapsedGT.TotalMilliseconds >= 20000)
            //{
            //    this.R_GT_Timer.Stop();
            //    this.R_GT_Clock.Text = "20.00";
            //    return;
            //}
            //this.R_GT_Clock.Text = timeElapsedGT.TotalSeconds.ToString("0.00");
        }

        private void R_GT_BtnTime_Click(object sender, EventArgs e)
        {
            //if (!play)
            //{
            //    play = true;
            //    round1Sound.Play();
            //    play = false;
            //}
            startTimeGT = DateTime.Now;
            R_GT_Timer.Start();
            this.ctrl.dataServer.startTime = startTimeGT;
            this.ctrl.startRoundGTTimer("", 0);
        }

        public void tinhDiemVuotTime()
        {
            diemphat = "";
            if (initMin == 3)
            {
                if (minutes == 3 && seconds == 31)
                {
                    diemphat = "Bạn bị trừ 5 điểm";
                }
                else if (minutes == 4 && seconds == 01)
                {
                    diemphat = "Bạn bị trừ 10 điểm";
                }
                else if (minutes == 4 && seconds == 29)
                {
                    diemphat = "Bạn bị trừ 20 điểm";
                }
            }
            else if (initMin == 5)
            {
                if (minutes == 5 && seconds == 31)
                {
                    diemphat = "Bạn bị trừ 5 điểm";
                }
                else if (minutes == 6 && seconds == 01)
                {
                    diemphat = "Bạn bị trừ 10 điểm";
                }
                else if (minutes == 6 && seconds == 29)
                {
                    diemphat = "Bạn bị trừ 20 điểm";
                }
            }
            else if (initMin == 7)
            {

            }
            lblDiemPhat.Text = diemphat;


        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { R_GT_Timer.Enabled = true; }));
            pauseToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = true;
            change5MinsToolStripMenuItem.Enabled = false;
            change7MinsToolStripMenuItem.Enabled = false;
            change3MinsToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = false;
            //lblDiemPhat.Text = "";
            // Send to control
            this.ctrl.startRoundGTTimer("START", initMin);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            R_GT_Timer.Enabled = false;

            pauseToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            change5MinsToolStripMenuItem.Enabled = false;
            change7MinsToolStripMenuItem.Enabled = false;
            change3MinsToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;

            //Tinh diem tru
            tinhDiemVuotTime();

            ticktok.Stop();
            tingfile.Stop();
            warnfile.Stop();
            play = false;

            // Send to control
            this.ctrl.startRoundGTTimer("PAUSE", initMin);
        }
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setupTime();

            pauseToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            change5MinsToolStripMenuItem.Enabled = true;
            change7MinsToolStripMenuItem.Enabled = true;
            change3MinsToolStripMenuItem.Enabled = true;
            startToolStripMenuItem.Enabled = true;
            lblDiemPhat.Text = "";

            if (play)
            {
                ticktok.Stop();
                tingfile.Stop();
                warnfile.Stop();
                play = false;

            }

            // Send to control
            this.ctrl.startRoundGTTimer("STOP", initMin);
        }

        private void change3MinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initMin = 3;
            progressBar1.Maximum = 180;
            setupTime();
        }

        private void change2MinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initMin = 2;
            progressBar1.Maximum = 180;
            setupTime();
        }


        private void change5MinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initMin = 5;
            progressBar1.Maximum = 300;
            setupTime();
        }

        private void change7MinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initMin = 7;
            progressBar1.Maximum = 420;
            setupTime();
        }

        #endregion

        #region tab Bấm chuông để trả lời
        private void R_BC_QuestN_Click(object sender, EventArgs e)
        {
            this.currentQuest = int.Parse(((Button)sender).Text);
            this.ctrl.loadRoundBCQuestion(int.Parse(cboChooseRound.SelectedValue.ToString()));//longvh
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShow", true);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnAddPoint", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", false);

        }
        public void InitBamChuong(DataServer data)
        {
            Dictionary<int, string> dicBonus = new Dictionary<int, string>();
            dicBonus.Add(5, "mặc định");
            dicBonus.Add(6, "loại trực tiếp (phần thi phụ)");
            dicBonus.Add(7, "VỀ ĐÍCH");
            dicBonus.Add(8, "dự phòng");
            cboChooseRound.DataSource = new BindingSource(dicBonus, null); ;
            cboChooseRound.ValueMember = "Key";
            cboChooseRound.DisplayMember = "Value";


            // Set Team
            if (data != null && data.ContestTeams != null)
            {
                for (int i = 1; i <= data.ContestTeams.Count; i++)
                {
                    SetTextComp(2, "R_BC_LblTeam" + i, data.ContestTeams[i - 1].Name);
                    SetTextComp(2, "R_BC_LblPoint" + i, data.ContestTeams[i - 1].Mark + "");
                }
            }

            QuestionDAO qdao = new QuestionDAO();
            DataTable dtNumQuestion = qdao.GetAllQuest(5);//longvh
            for (int i = 1; i <= dtNumQuestion.Rows.Count; i++)
            {
                ToggleCompVisiable_BC(1, "R_BC_Quest" + i, true);
            }
        }
        private void btnChooseRound_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc muốn chọn phần thi mới?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                R_BC_BtnGianhQuyenTL.Text = "Cơ hội cho đội khác";
                resetDataBamChuong(1);
                QuestionDAO qdao = new QuestionDAO();
                DataTable dtNumQuestion = qdao.GetAllQuest(int.Parse(cboChooseRound.SelectedValue.ToString()));//longvh
                for (int i = 1; i <= 12; i++)
                {
                    ToggleCompVisiable_BC(1, "R_BC_Quest" + i, false);
                }
                for (int i = 1; i <= dtNumQuestion.Rows.Count; i++)
                {
                    ToggleCompVisiable_BC(1, "R_BC_Quest" + i, true);
                }

                //disable trước các phần thi
                btn1p_BC.Enabled = false;
                btn2p_BC.Enabled = false;
                btnStartTimeBC.Enabled = false;
                btnStopTimeBC.Enabled = false;
                R_BC_Timer.Interval = 1000;
                initMinBC = 15;

                if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 6)//phần thi phụ
                {
                    btnChangeNameBC.Enabled = true;
                    R_BC_BtnGianhQuyenTL.Text = "Thời gian phần thi phụ";
                }
                else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5 || int.Parse(cboChooseRound.SelectedValue.ToString()) == 8)//mặc định, dự phòng
                {
                    btnChangeNameBC.Enabled = false;
                    btnStartTimeBC.Enabled = true;
                    btnStopTimeBC.Enabled = true;
                }
                else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 7)// phần thi hùng biện
                {
                    btnChangeNameBC.Enabled = false;
                    R_BC_Timer.Interval = 1000;
                    btn1p_BC.Enabled = true;
                    btn2p_BC.Enabled = true;
                    btnStopTimeBC.Enabled = true;
                    initMinBC = 2;

                }

                this.ctrl.showNameRoundBC(int.Parse(cboChooseRound.SelectedValue.ToString()));//hiển thị tên vòng thi (bấm chuông)
            }

        }

        private void R_BC_BtnShow_Click(object sender, EventArgs e)
        {
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShow", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", true);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnAddPoint", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnGianhQuyenTL", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", false);

            this.ctrl.dataServer.minutes = 10;

            this.ctrl.showRoundBCQuestion(int.Parse(cboChooseRound.SelectedValue.ToString()));//longvh

            for (int i = 1; i <= 12; i++)
            {
                ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, false);
            }

            // Reset dap an thi sinh
            for (int j = 0; j < countContestTeams; j++)
            {
                this.ctrl.dataServer.ContestTeams[j].Answer = "";
                this.ctrl.dataServer.ContestTeams[j].Time = "";
            }

            if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 6)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (câu hỏi phụ)
            {
                ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", false);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnGianhQuyenTL", true);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", true);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", true);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", true);
                this.ctrl.dataServer.questRoundBC_phu[this.currentQuest - 1] = 1;
            }
            else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                this.ctrl.dataServer.questRoundBC[this.currentQuest - 1] = 1;
            }
            else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 7)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (hùng biện)
            {
                ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", false);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnGianhQuyenTL", false);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", true);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", true);
                ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", true);
                this.ctrl.dataServer.questRoundBC_hungbien[this.currentQuest - 1] = 1;
            }
            if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 8)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (dự phòng vòng 2)
            {
                this.ctrl.dataServer.questRoundBC_duphongvong2[this.currentQuest - 1] = 1;
            }
        }
        #region Cộng điểm trả lời đúng?

        private void R_BC_BtnTrue1_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(1, "default", sender);
        }

        private void R_BC_BtnTrue2_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(2, "default", sender);
        }

        private void R_BC_BtnTrue3_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(3, "default", sender);
        }

        private void R_BC_BtnTrue4_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(4, "default", sender);
        }

        private void R_BC_BtnTrue5_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(5, "default", sender);
        }

        private void R_BC_BtnTrue6_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(6, "default", sender);
        }

        public void R_BC_BtnTrueN_Click(int number, string action, object sender)
        {
            int point = 0;

            if (currentQuest == 1 || currentQuest == 4 || currentQuest == 7)
            {
                point = 20;
            }
            else
            {
                point = 30;
            }

            /*if (currentQuest == 1 || currentQuest == 2 || currentQuest == 3 || currentQuest == 4 || currentQuest == 5)
            {
                point = 30;
            }
            else if (currentQuest == 6 || currentQuest == 7)
            {
                point = 50;
            }*/
            //else if (currentQuest == 8 || currentQuest == 9)
            //{
            //    point = 30;
            //}
            //else if (currentQuest == 10)
            //{
            //    point = 40;
            //}
            if (action == "default")
            {
                Control comp = (Control)this.tabBamChuong.Controls.Find("R_BC_LblPoint" + number, true).FirstOrDefault();
                comp.Text = this.ctrl.dataServer.ContestTeams[number - 1].Mark + point + "";

                //this.ctrl.dataServer.ContestTeams[number - 1].BonusPoint = point;//huhu

                this.ctrl.updateMark(number, point, 5);//huhu
            }
            else if (action == "x2")
            {
                Control comp = (Control)this.tabBamChuong.Controls.Find("R_BC_LblPoint" + number, true).FirstOrDefault();
                comp.Text = this.ctrl.dataServer.ContestTeams[number - 1].Mark + (point * 2) + "";
                this.ctrl.updateMark(number, (point * 2), 5);
            }
            else
            {
                Control comp = (Control)this.tabBamChuong.Controls.Find("R_BC_LblPoint" + number, true).FirstOrDefault();
                comp.Text = this.ctrl.dataServer.ContestTeams[number - 1].Mark + ((point) * (-1)) + "";
                this.ctrl.updateMark(number, (point) * (-1), 5);

            }
            ((PictureBox)sender).Visible = false;//huhu
        }
        private void R_BC_BtnTrue_x2_1_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(1, "x2", sender);
        }
        private void R_BC_BtnTrue_x2_2_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(2, "x2", sender);
        }
        private void R_BC_BtnTrue_x2_3_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(3, "x2", sender);
        }
        private void R_BC_BtnTrue_x2_4_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(4, "x2", sender);
        }
        private void R_BC_BtnTrue_x2_5_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(5, "x2", sender);
        }

        private void R_BC_BtnTrue_x2_6_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(6, "x2", sender);
        }

        private void R_BC_BtnTrue_chia2_1_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(1, "chia2", sender);
        }
        private void R_BC_BtnTrue_chia2_2_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(2, "chia2", sender);
        }
        private void R_BC_BtnTrue_chia2_3_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(3, "chia2", sender);
        }
        private void R_BC_BtnTrue_chia2_4_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(4, "chia2", sender);
        }
        private void R_BC_BtnTrue_chia2_5_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(5, "chia2", sender);
        }

        private void R_BC_BtnTrue_chia2_6_Click(object sender, EventArgs e)
        {
            R_BC_BtnTrueN_Click(6, "chia2", sender);
        }

        #endregion

        #region Ấn chuông trả lời

        private void R_BC_BtnBell1_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam1.BackColor = Color.FromArgb(0, 102, 255);


            // Send to control
            this.ctrl.showRound_BC_bell(1, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell2_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam2.BackColor = Color.FromArgb(0, 102, 255);
            // Send to control
            this.ctrl.showRound_BC_bell(2, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell3_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam3.BackColor = Color.FromArgb(0, 102, 255);
            // Send to control
            this.ctrl.showRound_BC_bell(3, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell4_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam4.BackColor = Color.FromArgb(0, 102, 255);
            // Send to control
            this.ctrl.showRound_BC_bell(4, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell5_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam5.BackColor = Color.FromArgb(0, 102, 255);
            // Send to control
            this.ctrl.showRound_BC_bell(5, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell6_Click(object sender, EventArgs e)
        {
            R_BC_LblTeam6.BackColor = Color.FromArgb(0, 102, 255);
            // Send to control
            this.ctrl.showRound_BC_bell(6, true);

            /*if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                // Send to control time
                R_BC_Timer.Enabled = false;
                this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
            }*/
        }

        private void R_BC_BtnBell1_DoubleClick(object sender, EventArgs e)
        {
            for (int i = 1; i <= 6; i++)
            {
                CompBackColor("R_BC_LblTeam" + i, Color.FromArgb(0, 51, 48));
            }
            this.ctrl.showRound_BC_bell(0, true);
        }

        #endregion
        private void R_BC_BtnAddPoint_Click(object sender, EventArgs e)
        {
            R_BC_BtnReset.Enabled = true;
            R_BC_BtnAddPoint.Enabled = false;
            R_BC_BtnGianhQuyenTL.Enabled = false;
            R_BC_BtnShowAnswerCT.Enabled = false;
            R_BC_BtnShowAnswer.Enabled = true;
            for (int i = 1; i <= 6; i++)
            {
                ToggleCompVisiable_BC(1, "R_BC_BtnTrue" + i, true);
                ToggleCompVisiable_BC(1, "R_BC_BtnTrue_x2_" + i, true);
                ToggleCompVisiable_BC(1, "R_BC_BtnTrue_chia2_" + i, true);
            }
        }

        private void R_BC_BtnReset_Click(object sender, EventArgs e)
        {
            // Reset Client
            this.ctrl.dataServer.Action = Constant.ACTION_RESET;
            this.ctrl.dataServer.Round = 5;
            this.ctrl.dataServer.team = 0;// reset đội bấm chuông trả lời
            this.ctrl.SendToClient();
            // Reset server
            this.ctrl.dataServer.checkBC = 0;
            R_BC_Question.Text = "";
            //reset time
            setupTimeBC();

            if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 6)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (phần thi phụ)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (this.ctrl.dataServer.questRoundBC_phu[i - 1] == 0)
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, false);
                    }
                }
            }
            else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 5)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (quay lại mặc định)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (this.ctrl.dataServer.questRoundBC[i - 1] == 0)
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, false);
                    }
                }
            }
            else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 7)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (hùng biện)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (this.ctrl.dataServer.questRoundBC_hungbien[i - 1] == 0)
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, false);
                    }
                }
            }
            else if (int.Parse(cboChooseRound.SelectedValue.ToString()) == 8)//áp dụng trong trường hợp đổi phần thi ở vòng bấm chuông (dự phòng bấm chuông)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (this.ctrl.dataServer.questRoundBC_duphongvong2[i - 1] == 0)
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, true);
                    }
                    else
                    {
                        ToggleCompEnableDisable_BC(1, "R_BC_Quest" + i, false);
                    }
                }
            }

            try
            {
                for (int i = 1; i <= 6; i++)
                {
                    ToggleCompVisiable_BC(1, "R_BC_BtnTrue" + i, false);
                    ToggleCompVisiable_BC(1, "R_BC_BtnTrue_x2_" + i, false);
                    ToggleCompVisiable_BC(1, "R_BC_BtnTrue_chia2_" + i, false);
                    SetTextComp(5, "R_BC_TeamAnswer" + i, "");
                    SetTextComp(5, "R_BC_TimeAnswer" + i, "");
                    CompBackColor("R_BC_LblTeam" + i, Color.FromArgb(0, 51, 48));
                }
            }
            catch (Exception)
            {

                throw;
            }
            
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShow", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnAddPoint", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnGianhQuyenTL", false);
        }
        #region Chọn Ngôi sao hy vọng

        private void R_BC_BtnStar1_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(1, true);
        }

        private void R_BC_BtnStar1_DoubleClick(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(0, false);
        }

        private void R_BC_BtnStar2_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(2, true);
        }

        private void R_BC_BtnStar3_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(3, true);
        }

        private void R_BC_BtnStar4_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(4, true);
        }

        private void R_BC_BtnStar5_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(5, true);
        }

        private void R_BC_BtnStar6_Click(object sender, EventArgs e)
        {
            this.ctrl.showRound_BC_star(6, true);
        }

        #endregion
        private void R_BC_Timer_Tick(object sender, EventArgs e)
        {
            secondsBC += 1;
            if (secondsBC > 59)
            {
                minutesBC += 1;
                secondsBC = 0;
            }

            if (initMinBC == 15)//10 giây (tăng tốc)
            {
                if (secondsBC == 15)
                {
                    //tingfile.Play();
                    ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", true);
                    ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", false);
                    R_BC_Timer.Enabled = false;
                }
            }
            else if (initMinBC == 2)//2 phút (hùng biện)
            {
                //ticktok.Play();
                if (minutesBC == 2)
                {
                    ticktok.Stop();
                    R_BC_Timer.Enabled = false;
                }
            }
            else if (initMinBC == 4)//4 phút  (hùng biện)
            {
                if (minutesBC == 4 && secondsBC >= 1)
                {
                    //tingfile.Play();
                }
                if (minutesBC > 4 && minutes < 5)
                {
                    tingfile.Stop();
                    //ticktok.Play();
                }
                if (minutesBC == 5 && secondsBC == 01)
                {
                    ticktok.Stop();
                    R_BC_Timer.Enabled = false;
                }
            }

            if (minutesBC.ToString().Length < 2)
                lblMinBC.Text = "0" + minutesBC.ToString();
            else
                lblMinBC.Text = minutesBC.ToString();
            if (secondsBC.ToString().Length < 2)
                lblSecondBC.Text = "0" + secondsBC.ToString();
            else
                lblSecondBC.Text = secondsBC.ToString();
        }
        public void setupTimeBC()
        {
            R_BC_Timer.Enabled = false;
            minutesBC = 0;
            secondsBC = 0;
            lblMinBC.Text = "00";
            lblSecondBC.Text = "00";
            play = false;

        }

        private void R_BC_BtnShowAnswerCT_Click(object sender, EventArgs e)
        {
            R_BC_BtnGianhQuyenTL.Enabled = false;
            R_BC_BtnShowAnswerCT.Enabled = false;

            this.ctrl.showRoundBCAnswer(int.Parse(cboChooseRound.SelectedValue.ToString()));
        }

        private void cboCauHinhRound_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ctrl.loadRound1QuestionCauHinh(cboCauHinhRound.SelectedValue.ToString().Length > 2 ? 1 : int.Parse(cboCauHinhRound.SelectedValue.ToString()));
        }

        private void btnGetApiDiemGK_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://gorest.co.in/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("public/v2/users").Result;
            if (response.IsSuccessStatusCode)
            {
                //var xx = response.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            }
        }

        private void btnChangeNameBC_Click(object sender, EventArgs e)
        {
            this.ctrl.changeNameBC(int.Parse(cboTeamBC.Text), txtNameTeamBC.Text, int.Parse(cboChooseRound.SelectedValue.ToString()));
        }

        public float getdiemthuong()
        {
            if (diemthuong == 0)
            {
                diemthuong = 200;
            }
            else if (diemthuong == 200)
            {
                diemthuong = 175;
            }
            else if (diemthuong == 175)
            {
                diemthuong = 150;
            }
            //else if (diemthuong == 15 || diemthuong == 10)
            else if (diemthuong == 150)
            {
                diemthuong = 125;
            }
            return diemthuong;
        }

        private void clickColor1_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[0].Answer))
            {
                R1_LblTeam1.BackColor = Color.FromArgb(0, 102, 255);
                this.ctrl.dataServer.ContestTeams[0].BonusPoint = getdiemthuong() / 100;

                this.ctrl.showRound_1_bell(1, true);
            }
        }

        private void clickColor2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[1].Answer))
            {
                R1_LblTeam2.BackColor = Color.FromArgb(0, 102, 255);
                this.ctrl.dataServer.ContestTeams[1].BonusPoint = getdiemthuong() / 100;

                this.ctrl.showRound_1_bell(2, true);
            }
        }

        private void clickColor3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[2].Answer))
            {
                R1_LblTeam3.BackColor = Color.FromArgb(0, 102, 255);

                this.ctrl.dataServer.ContestTeams[2].BonusPoint = getdiemthuong() / 100;
                this.ctrl.showRound_1_bell(3, true);
            }
        }

        private void clickColor4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[3].Answer))
            {
                R1_LblTeam4.BackColor = Color.FromArgb(0, 102, 255);
                this.ctrl.dataServer.ContestTeams[3].BonusPoint = getdiemthuong() / 100;
                this.ctrl.showRound_1_bell(4, true);

            }
        }

        private void clickColor5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ctrl.dataServer.ContestTeams[4].Answer))
            {
                R1_LblTeam5.BackColor = Color.FromArgb(0, 102, 255);
                this.ctrl.dataServer.ContestTeams[4].BonusPoint = getdiemthuong() / 100;
                this.ctrl.showRound_1_bell(5, true);

            }
        }

        private void clickColor1_DoubleClick(object sender, EventArgs e)
        {
             diemthuong = 0;

            for (int i = 1; i <= this.ctrl.dataServer.ContestTeams.Count; i++)
            {
                CompBackColor("R1_LblTeam" + i, Color.FromArgb(0, 51, 48));
                this.ctrl.dataServer.ContestTeams[i - 1].BonusPoint = 0;
            }

            this.ctrl.showRound_1_bell(0, true);
           
        }

        private void btnStartTimeBC_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            R_BC_BtnBell1_DoubleClick(sender, e);

            // Send to control
            this.ctrl.startRoundBCTimer("START", initMinBC);
        }
        private void btnPauseTimeBC_Click(object sender, EventArgs e)
        {
            R_BC_Timer.Enabled = false;

            // Send to control time
            this.ctrl.startRoundBCTimer("PAUSE", initMinBC);
        }

        private void btnStopTimeBC_Click(object sender, EventArgs e)
        {
            setupTimeBC();
            //R_BC_BtnBell1_DoubleClick(sender, e);

            // Send to control
            this.ctrl.startRoundBCTimer("STOP", initMinBC);
        }

        private void btn1p_BC_Click(object sender, EventArgs e)
        {
            initMinBC = 1;
            //reset time
            setupTimeBC();
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            this.ctrl.startRoundBCTimer("START", initMinBC);
        }

        private void R_BC_BtnGianhQuyenTL_Click(object sender, EventArgs e)
        {
            R_BC_BtnBell1_DoubleClick(sender, e);
            this.ctrl.dataServer.checkBC = 0;
            initMinBC = 0;
            //reset time
            setupTimeBC();
            this.ctrl.startRoundBCTimer("START", initMinBC);
            initMinBC = 10;
        }

        private void R_BC_BtnShowAnswer_Click(object sender, EventArgs e)
        {
            R_BC_BtnShowAnswer.Enabled = false;
            R_BC_BtnShowAnswerCT.Enabled = true;
            R_BC_BtnAddPoint.Enabled = true;
            this.ctrl.dataServer.Round = 5;
            this.ctrl.dataServer.Action = Constant.ACTION_SHOW_RESULT;
            //for (int i = 1; i <= 5; i++)//huhu
            //{
            //    ToggleCompVisiable_BC(1, "R_BC_BtnTrue" + i, true);
            //    ToggleCompVisiable_BC(1, "R_BC_BtnTrue_x2_" + i, true);
            //    ToggleCompVisiable_BC(1, "R_BC_BtnTrue_chia2_" + i, true);
            //}
            this.ctrl.SendToClient();
        }

        private void C3_btnHideRow_Click(object sender, EventArgs e)
        {
            this.ctrl.saveHideRowRound2();
        }

        private void R2_LblTeam1_Click(object sender, EventArgs e)
        {
            R2_LblTeam1.BackColor = Color.FromArgb(0, 102, 255);

            // Send to control
            this.ctrl.showRound_2_bell(1, true);
        }

        private void R2_LblTeam2_Click(object sender, EventArgs e)
        {
            R2_LblTeam2.BackColor = Color.FromArgb(0, 102, 255);

            // Send to control
            this.ctrl.showRound_2_bell(2, true);
        }

        private void R2_LblTeam3_Click(object sender, EventArgs e)
        {
            R2_LblTeam3.BackColor = Color.FromArgb(0, 102, 255);

            // Send to control
            this.ctrl.showRound_2_bell(3, true);
        }

        private void R2_LblTeam4_Click(object sender, EventArgs e)
        {
            R2_LblTeam4.BackColor = Color.FromArgb(0, 102, 255);

            // Send to control
            this.ctrl.showRound_2_bell(4, true);
        }

        private void R2_LblTeam5_Click(object sender, EventArgs e)
        {
            R2_LblTeam5.BackColor = Color.FromArgb(0, 102, 255);

            // Send to control
            this.ctrl.showRound_2_bell(5, true);
        }

        private void R2_LblTeam1_DoubleClick(object sender, EventArgs e)
        {
            for (int i = 1; i <= 6; i++)
            {
                CompBackColor("R2_LblTeam" + i, Color.FromArgb(0, 51, 48));
            }
            this.ctrl.showRound_2_bell(0, true);
        }

        private void R2_BtnTrue1_DoubleClick(object sender, EventArgs e)
        {
            for (int i = 1; i <= 6; i++)
            {
                CompBackColor("R2_LblTeam" + i, Color.FromArgb(0, 51, 48));
            }
            this.ctrl.showRound_2_AddPoint(0, true);
        }

        private void R2_btnLockTeam_Click(object sender, EventArgs e)
        {
            var _val = R2_txtLockTeam.Text.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
            string[] arrTeam = new string[] { "1", "2", "3", "4", "5", "6" };
            int[] lock_unlockTeam = new int[] { };
            foreach (var item in _val)
            {
                if (!string.IsNullOrEmpty(item) && arrTeam.Contains(item))
                {
                    CompBackColor("R2_LblTeam" + item, Color.DarkGray);
                    CompBackColor("R2_LblPoint" + item, Color.DarkGray);
                    lock_unlockTeam = lock_unlockTeam.Concat(new int[] { int.Parse(item) }).ToArray();
                }
            }
            this.ctrl.teamEliminatedRound2(lock_unlockTeam, true);
            this.ctrl.dataServer.outTeam = _val;
        }

        private void R2_btnUnlockTeam_Click(object sender, EventArgs e)
        {
            var _val = R2_txtLockTeam.Text.Split(new string[] { ";", ",", "." }, StringSplitOptions.None);
            string[] arrTeam = new string[] { "1", "2", "3", "4", "5", "6" };
            int[] lock_unlockTeam = new int[] { };
            foreach (var item in _val)
            {
                if (!string.IsNullOrEmpty(item) && arrTeam.Contains(item))
                {
                    CompBackColor("R2_LblTeam" + item, Color.FromArgb(0, 51, 48));
                    CompBackColor("R2_LblPoint" + item, Color.FromArgb(233, 33, 39));
                    lock_unlockTeam = lock_unlockTeam.Concat(new int[] { int.Parse(item) }).ToArray();
                    
                }
            }
            this.ctrl.teamEliminatedRound2(lock_unlockTeam, false);
            this.ctrl.dataServer.outTeam = new string[] { };
        }

        private void label71_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            initMinBC = 30;
            //reset time
            setupTimeBC();
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            this.ctrl.startRoundBCTimer("START", initMinBC);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            initMinBC = 2;
            //reset time
            setupTimeBC();
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            this.ctrl.startRoundBCTimer("START", initMinBC);
        }

        private void C2_GridKhoiDong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn2p_BC_Click(object sender, EventArgs e)
        {
            initMinBC = 5;
            //reset time
            setupTimeBC();
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            this.ctrl.startRoundBCTimer("START", initMinBC);
        }


        private void R_BC_BtnTime_Click(object sender, EventArgs e)
        {
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShow", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnTime", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnGianhQuyenTL", true);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswer", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnShowAnswerCT", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnAddPoint", false);
            ToggleCompEnableDisable_BC(1, "R_BC_BtnReset", false);
            //if (!play)
            //{
            //    play = true;
            //    round1Sound.Play();
            //    play = false;
            //}
            this.Invoke(new Action(() => { R_BC_Timer.Enabled = true; }));
            startTimeBC = DateTime.Now;
            R_BC_Timer.Start();
            //this.ctrl.dataServer.startTime = startTimeBC;
            //this.ctrl.dataServer.description = "START";
            this.ctrl.startRoundBCTimer("START", initMinBC);

            //show giây bấm giờ đội thi phần tăng tốc
            //this.ctrl.dataServer.Round = 7;
            //this.ctrl.SendToClient();
        }

        public void setRoundBCQuestion(Question q)
        {
            R_BC_Question.Text = q.QuestContent;
        }

        #endregion



    }
}
