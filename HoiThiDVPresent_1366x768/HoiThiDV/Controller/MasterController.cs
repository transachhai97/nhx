using HoiThiDV.Model;
using HoiThiDV.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoiThiDV
{
    public class MasterController
    {
        private DataClient dataClient;
        private Form1 view;
        private TeamManage viewTeamManage;
        private KhoiDong viewKhoiDong;
        private TangToc viewTangToc;
        private TeamAnswer2 viewTeamAnswer;
        private TeamAnswer24 viewTeamAnswer24;
        private TeamAnswer25 viewTeamAnswer25;
        private TeamAnswer26 viewTeamAnswer26;
        private TeamAnswer24_BC viewTeamAnswer24_BC;
        private TeamAnswer25_BC viewTeamAnswer25_BC;
        private TeamAnswer26_BC viewTeamAnswer26_BC;
        private GioiThieu viewGioiThieu;
        private BamChuong viewBamChuong;
        private Team6 viewTeam6;
        private Team6VD viewTeam6VD;
        private Team5VD viewTeam5VD;
        private Team5 viewTeam5;
        private Team4 viewTeam4;
        private TimeTeam4 viewTimeTeam4;
        private TimeTeam5 viewTimeTeam5;
        private Doi6 viewDoi6;

        private Socket clientSocket;
        byte[] receivedBuf = new byte[1995096];
        public MasterController(Form1 v)
        {
            #region view

            this.view = v;
            this.view.setController(this);
            this.view.checkForIllegalThreadCalls(false);

            // Set up TeamManage view
            this.viewTeamManage = new TeamManage();
            this.viewTeamManage.setController(this);

            // Set up KhoiDong view
            this.viewKhoiDong = new KhoiDong();
            this.viewKhoiDong.setController(this);

            // Set up TangToc view
            this.viewTangToc = new TangToc();
            this.viewTangToc.setController(this);

            // Set up TeamAnswer view
            this.viewTeamAnswer = new TeamAnswer2();
            this.viewTeamAnswer.setController(this);

            this.viewTeamAnswer24 = new TeamAnswer24();
            this.viewTeamAnswer24.setController(this);

            this.viewTeamAnswer25 = new TeamAnswer25();
            this.viewTeamAnswer25.setController(this);

            this.viewTeamAnswer26 = new TeamAnswer26();
            this.viewTeamAnswer26.setController(this);

            this.viewTeamAnswer24_BC = new TeamAnswer24_BC();
            this.viewTeamAnswer24_BC.setController(this);

            this.viewTeamAnswer25_BC = new TeamAnswer25_BC();
            this.viewTeamAnswer25_BC.setController(this);

            this.viewTeamAnswer26_BC = new TeamAnswer26_BC();
            this.viewTeamAnswer26_BC.setController(this);

            // Set up GioiThieu view
            this.viewGioiThieu = new GioiThieu();
            this.viewGioiThieu.setController(this);

            // Set up BamChuong view
            this.viewBamChuong = new BamChuong();
            this.viewBamChuong.setController(this);

            this.viewTeam6 = new Team6();
            this.viewTeam6.setController(this);

            this.viewTeam6VD = new Team6VD();
            this.viewTeam6VD.setController(this);

            this.viewTeam5 = new Team5();
            this.viewTeam5.setController(this);

            this.viewTeam4 = new Team4();
            this.viewTeam4.setController(this);

            this.viewTimeTeam5 = new TimeTeam5();
            this.viewTimeTeam5.setController(this);

            this.viewTimeTeam4 = new TimeTeam4();
            this.viewTimeTeam4.setController(this);

            this.viewDoi6 = new Doi6();
            this.viewDoi6.setController(this);

            #endregion
            // Get config
            GetConfig();
            // Init Socket Client
            SetUpClient();

            // Send info to server
            SendToServer();
        }

        #region Comment Event

        private void GetConfig()
        {
            dataClient = new DataClient();
            dataClient.isInit = true;
            dataClient.serverIp = System.Configuration.ConfigurationManager.AppSettings["ServerIP"];
            dataClient.teamId = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TeamID"]);
        }

        #endregion

        #region Socket
        public void SendToServer()
        {
            Messages msg = SendObject.Serialize(dataClient);
            byte[] data = msg.Data;
            clientSocket.Send(data);
        }
        public void SetUpClient()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LoopConnect();
            clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int received = socket.EndReceive(ar);
                byte[] dataBuf = new byte[received];
                Array.Copy(receivedBuf, dataBuf, received);
                object obj = SendObject.Deserialize(new Messages { Data = dataBuf });
                DataServer data = null;
                if (obj is DataServer)
                {
                    data = obj as DataServer;
                    if (!data.clientState)
                    {
                        clientSocket.Close();
                        SetUpClient();
                    }
                    if (data.Round == 0)
                    {
                        data.title = "DANH SÁCH ĐỘI THI";
                        if (data.ContestTeams.Count == 5)
                        {
                            this.showTeam5();
                            this.viewTeam5.setData(data);
                            this.viewTeam5.Refresh();
                        }
                        else if (data.ContestTeams.Count == 6)
                        {
                            if (data.check != null)
                            {
                                this.showDoi6();
                                this.viewDoi6.setData(data);
                                this.viewDoi6.Refresh();
                            }
                            else
                            {
                                this.showTeam6();
                                this.viewTeam6.setData(data);
                                this.viewTeam6.Refresh();
                            }
                        }
                        else
                        {
                            this.showTeam4();
                            this.viewTeam4.setData(data);
                            this.viewTeam4.Refresh();
                        }
                        //this.showTeamManage();
                        //this.viewTeamManage.setData(data);
                        //this.viewTeamManage.Refresh();
                    }
                    else if (data.Round == 1)
                    {
                        if (data.Action.Equals(Constant.ACTION_INIT))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.InitKhoiDong(data);
                            this.viewKhoiDong.Refresh();
                        }
                        
                        else if (data.Action.Equals(Constant.ACTION_LOAD_QUESTION))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.LoadQuestion(data);
                            this.viewKhoiDong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_QUESTION))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.ShowQuestion(data);
                            this.viewKhoiDong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_ANSWER))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25();
                                this.viewTeamAnswer25.ShowAnswerCT(data);
                                this.viewTeamAnswer25.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26();
                                this.viewTeamAnswer26.ShowAnswerCT(data);
                                this.viewTeamAnswer26.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24();
                                this.viewTeamAnswer24.ShowAnswerCT(data);
                                this.viewTeamAnswer24.Refresh();
                            }
                            //this.showTeamAnswer();
                            //this.viewTeamAnswer.ShowAnswerCT(data);
                            //this.viewTeamAnswer.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RUN_TIMER))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.timeAddStop();
                            this.viewKhoiDong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_UPDATE_SCORE))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.UpdateScore(data);
                            this.viewKhoiDong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RESET))
                        {
                            this.showKhoiDong();
                            this.viewKhoiDong.InitKhoiDong(data);
                            this.viewKhoiDong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RESET_ALL))
                        {
                            data.title = "DANH SÁCH ĐỘI THI";
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeam5();
                                this.viewTeam5.setData(data);
                                this.viewTeam5.Refresh();
                            }
                            else
                            {
                                this.showTeam4();
                                this.viewTeam4.setData(data);
                                this.viewTeam4.Refresh();
                            }
                            this.viewKhoiDong.InitKhoiDong(data);
                            this.viewTangToc.isInitOchu = false;
                            this.viewTangToc.InitTangToc(data,"INIT");
                            this.viewTeam6.resetDiemBGK();
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_BELL))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25();
                                this.viewTeamAnswer25.showBell(data);
                                this.viewTeamAnswer25.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24();
                                this.viewTeamAnswer24.showBell(data);
                                this.viewTeamAnswer24.Refresh();
                            }
                        }
                    }
                    else if (data.Round == 2)
                    {

                        if (!this.viewTangToc.isInitOchu)
                        {
                            this.viewTangToc.InitOChu(data);
                            this.viewTangToc.Refresh();
                        }
                        if (data.Action.Equals(Constant.ACTION_INIT))
                        {
                            this.showTangToc();
                            this.viewTangToc.InitTangToc(data,"INIT");
                            this.viewTangToc.Refresh();
                        }
                        if (data.Action.Equals(Constant.ACTION_SHOW_ROW))
                        {
                            this.showTangToc();
                            this.viewTangToc.ShowRow(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_HIDE_ROW))
                        {
                            this.showTangToc();
                            this.viewTangToc.HideRow(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_LOAD_QUESTION))
                        {
                            this.showTangToc();
                            this.viewTangToc.LoadQuestion(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_QUESTION))
                        {
                            this.showTangToc();
                            this.viewTangToc.ShowQuestion(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_DACT))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25();
                                this.viewTeamAnswer25.ShowAnswerCT(data);
                                this.viewTeamAnswer25.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26();
                                this.viewTeamAnswer26.ShowAnswerCT(data);
                                this.viewTeamAnswer26.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24();
                                this.viewTeamAnswer24.ShowAnswerCT(data);
                                this.viewTeamAnswer24.Refresh();
                            }
                        }
                        else if (data.Action.Equals(Constant.ACTION_ADD_POINT))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25();
                                this.viewTeamAnswer25.showBell(data);
                                this.viewTeamAnswer25.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26();
                                this.viewTeamAnswer26.showBell(data);
                                this.viewTeamAnswer26.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24();
                                this.viewTeamAnswer24.showBell(data);
                                this.viewTeamAnswer24.Refresh();
                            }
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_BELL))
                        {
                            this.showTangToc();
                            this.viewTangToc.showBell(data, "SHOW_BELL");
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_OUT_TEAM))
                        {
                            this.showTangToc();
                            this.viewTangToc.outTeam(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_CLOSE_ANSWER))
                        {
                            this.showTangToc();
                            this.viewTangToc.closeRound2Answer(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RUN_TIMER))
                        {
                            this.showTangToc();
                            if (string.IsNullOrEmpty(data.flagTimeRound2))
                            {
                                this.viewTangToc.timeAddStop();
                            }
                            else
                            {
                                this.viewTangToc.RunTimer(data);
                            }
                                
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_ANSWER))
                        {
                            this.showTangToc();
                            this.viewTangToc.showRound2Answer(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_UPDATE_SCORE))
                        {
                            this.showTangToc();
                            this.viewTangToc.UpdateScore(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_OPEN_HANG_DOC))
                        {
                            this.showTangToc();
                            this.viewTangToc.MoHangDoc(data);
                            this.viewTangToc.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RESET))
                        {
                            this.showTangToc();
                            this.viewTangToc.InitTangToc(data,"RESET");
                            this.viewTangToc.showBell(data, "RESET");
                            this.viewTangToc.Refresh();
                        }
                    }
                    else if (data.Round == 3)
                    {
                        if (data.ContestTeams.Count == 5)
                        {
                            this.showTeamAnswer25();
                            this.viewTeamAnswer25.InitTeamAnswer(data);
                            this.viewTeamAnswer25.Refresh();
                        }
                        else if (data.ContestTeams.Count == 6)
                        {
                            this.showTeamAnswer26();
                            this.viewTeamAnswer26.InitTeamAnswer(data);
                            this.viewTeamAnswer26.Refresh();
                        }
                        else
                        {
                            this.showTeamAnswer24();
                            this.viewTeamAnswer24.InitTeamAnswer(data);
                            this.viewTeamAnswer24.Refresh();
                        }
                        //this.showTeamAnswer();
                        //this.viewTeamAnswer.InitTeamAnswer(data);
                        //this.viewTeamAnswer.Refresh();

                    }
                    else if (data.Round == 4)// màn giới thiệu
                    {
                        //if (data.Action.Equals(Constant.ACTION_INIT))
                        //{
                        //    this.showGioiThieu();
                        //    this.viewGioiThieu.InitGioiThieu(data);
                        //    this.viewGioiThieu.Refresh();
                        //}
                        //else if (data.Action.Equals(Constant.ACTION_SHOW_CONTENT))
                        //{
                        //    this.showGioiThieu();
                        //    this.viewGioiThieu.ShowContent(data);
                        //    this.viewGioiThieu.Refresh();
                        //}
                        //else if (data.Action.Equals(Constant.ACTION_UPDATE_SCORE))
                        //{
                        //    this.showGioiThieu();
                        //    this.viewGioiThieu.UpdateScore(data);
                        //    this.viewGioiThieu.Refresh();
                        //}
                        //else if (data.Action.Equals(Constant.ACTION_RUN_TIMER))
                        //{
                        //    this.showGioiThieu();
                        //    //this.viewGioiThieu.RunTimer(data);
                        //    this.viewGioiThieu.ActionRunTimer(data);
                        //    this.viewGioiThieu.Refresh();
                        //}


                    }
                    else if (data.Round == 5)// màn bấm chuông giành quyền trả lời viewBamChuong
                    {
                        if (data.Action.Equals(Constant.ACTION_INIT))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.InitBamChuong(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_STOP_BAM_CHUONG))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.stopBamChuong(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_NAME_ROUND))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.showNameRound(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_LOAD_QUESTION))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.LoadQuestion(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_QUESTION))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.ShowQuestion(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_RESULT))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25_BC();
                                this.viewTeamAnswer25_BC.InitTeamAnswer(data);
                                this.viewTeamAnswer25_BC.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26_BC();
                                this.viewTeamAnswer26_BC.InitTeamAnswer(data);
                                this.viewTeamAnswer26_BC.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24_BC();
                                this.viewTeamAnswer24_BC.InitTeamAnswer(data);
                                this.viewTeamAnswer24_BC.Refresh();
                            }
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_ANSWER))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25_BC();
                                this.viewTeamAnswer25_BC.ShowAnswerCT(data);
                                this.viewTeamAnswer25_BC.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26_BC();
                                this.viewTeamAnswer26_BC.ShowAnswerCT(data);
                                this.viewTeamAnswer26_BC.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24_BC();
                                this.viewTeamAnswer24_BC.ShowAnswerCT(data);
                                this.viewTeamAnswer24_BC.Refresh();
                            }
                        }
                        else if (data.Action.Equals(Constant.ACTION_RUN_TIMER))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.timeAddStop(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_STAR))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.ShowStar(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_SHOW_BELL))
                        {
                            if (data.ContestTeams.Count == 5)
                            {
                                this.showTeamAnswer25_BC();
                                /*this.viewTeamAnswer25_BC.showBell(data);*/
                                this.viewTeamAnswer25_BC.Refresh();
                            }
                            else if (data.ContestTeams.Count == 6)
                            {
                                this.showTeamAnswer26_BC();
                                this.viewTeamAnswer26_BC.showBell(data);
                                this.viewTeamAnswer26_BC.Refresh();
                            }
                            else
                            {
                                this.showTeamAnswer24_BC();
                                /*this.viewTeamAnswer24_BC.showBell(data);*/
                                this.viewTeamAnswer24_BC.Refresh();
                            }
                        }
                        else if (data.Action.Equals(Constant.ACTION_UPDATE_SCORE))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.UpdateScore(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_CHANGE_NAME))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.ChangeName(data);
                            this.viewBamChuong.Refresh();
                        }
                        else if (data.Action.Equals(Constant.ACTION_RESET))
                        {
                            this.showBamChuong();
                            this.viewBamChuong.InitBamChuong(data);
                            this.viewBamChuong.showBell(data);
                            this.viewBamChuong.ShowStar(data);
                            this.viewBamChuong.Refresh();
                        }

                    }
                    else if (data.Round == 6)//ban giám khảo
                    {
                        if (!string.IsNullOrEmpty(data.Action) && data.Action.Equals(Constant.ACTION_RESET))
                        {
                            if (data.oneTeam == 0)
                            {
                                this.showTeam6();
                                this.viewTeam6.resetDiemBGK();
                                this.viewTeam6.Refresh();
                            }
                            else
                            {
                                this.showTeam6VD();
                                this.viewTeam6VD.resetDiemBGK();
                                this.viewTeam6VD.Refresh();
                            }

                        }
                        else
                        {
                            data.title = "BAN GIÁM KHẢO";
                            if (data.oneTeam == 0)
                            {
                                this.showTeam6();
                                this.viewTeam6.setData(data);
                                this.viewTeam6.Refresh();
                            }
                            else
                            {
                                this.showTeam6VD();
                                this.viewTeam6VD.setData(data);
                                this.viewTeam6VD.Refresh();
                            }
                        }

                    }
                    else if (data.Round == 7)// show giây bấm giờ đội thi phần tăng tốc
                    {
                        if (data.ContestTeams.Count == 5)
                        {
                            this.showTimeTeam5();
                            this.viewTimeTeam5.InitTeamAnswer(data);
                            this.viewTimeTeam5.Refresh();
                        }
                        else
                        {
                            this.showTimeTeam4();
                            this.viewTimeTeam4.InitTeamAnswer(data);
                            this.viewTimeTeam4.Refresh();
                        }

                    }
                }
                if (obj is PingData)
                {
                    this.view.isDisconnected = false;
                }
                clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                this.view.showMessage("Mất kết nối với server! Vui lòng liên hệ cán bộ phụ trách CNTT để khắc phục.");
                //Application.Exit();
            }
        }

        private void LoopConnect()
        {

            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Parse(dataClient.serverIp), 100);
                }
                catch (SocketException)
                {
                    //Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.WriteLine("Connected!");
        }

        public void SendAnswer(String answer, String timeAnswer, int round)
        {
            dataClient.isInit = false;
            dataClient.teamAnswer = answer;
            dataClient.round = round;
            dataClient.timeAnswer = timeAnswer;

            SendToServer();
        }
        public void PingServer()
        {
            PingData ping = new PingData();
            Messages msg = SendObject.Serialize(ping);
            byte[] data = msg.Data;
            clientSocket.Send(data);
        }
        #endregion

        #region Show Screen
        public void showTeamManage()
        {
            this.view.addUserControl(viewTeamManage);
        }
        public void showKhoiDong()
        {
            this.view.addUserControl(viewKhoiDong);
        }
        public void showTangToc()
        {
            this.view.addUserControl(viewTangToc);
            //this.viewTangToc.TxtAnswerFocus();
        }
        public void showTeamAnswer()
        {
            this.view.addUserControl(viewTeamAnswer);
        }

        public void showTeamAnswer24()
        {
            this.view.addUserControl(viewTeamAnswer24);
        }
        public void showTeamAnswer25()
        {
            this.view.addUserControl(viewTeamAnswer25);
        }

        public void showTeamAnswer26()
        {
            this.view.addUserControl(viewTeamAnswer26);
        }

        public void showTeamAnswer24_BC()
        {
            this.view.addUserControl(viewTeamAnswer24_BC);
        }
        public void showTeamAnswer25_BC()
        {
            this.view.addUserControl(viewTeamAnswer25_BC);
        }

        public void showTeamAnswer26_BC()
        {
            this.view.addUserControl(viewTeamAnswer26_BC);
        }

        public void showGioiThieu()
        {
            this.view.addUserControl(viewGioiThieu);
        }
        public void showBamChuong()
        {
            this.view.addUserControl(viewBamChuong);
        }
        public void showTeam6()
        {
            this.view.addUserControl(viewTeam6);
        }

        public void showDoi6()
        {
            this.view.addUserControl(viewDoi6);
        }

        public void showTeam6VD()
        {
            this.view.addUserControl(viewTeam6VD);
        }

        public void showTeam5VD()
        {
            this.view.addUserControl(viewTeam5VD);
        }

        public void showTeam5()
        {
            this.view.addUserControl(viewTeam5);
        }
        public void showTeam4()
        {
            this.view.addUserControl(viewTeam4);
        }
        public void showTimeTeam5()
        {
            this.view.addUserControl(viewTimeTeam5);
        }
        public void showTimeTeam4()
        {
            this.view.addUserControl(viewTimeTeam4);
        }
        #endregion
    }
}
