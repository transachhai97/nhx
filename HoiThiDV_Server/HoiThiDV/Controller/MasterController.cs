using HoiThiDV.Controller;
using HoiThiDV.DAO;
using HoiThiDV.Model;
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
        private FormServer view;
        public static int game = 1;
        private int currentRound = 0;
        private CauHinhController ctrlCauHinh;
        private KhoiDongController ctrlKhoiDong;
        private TangTocController ctrlTangToc;
        private BamChuongController ctrlBamChuong;
        public DataServer dataServer;


        private byte[] buffer = new byte[1995096];
        public List<SimpleContest> clientList { get; set; }
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public MasterController(FormServer view)
        {
            this.view = view;
            this.view.setController(this);
            this.view.checkForIllegalThreadCalls(false);
            this.view.setGameDefault(game);


            this.ctrlCauHinh = new CauHinhController(view);
            this.ctrlKhoiDong = new KhoiDongController(view);
            this.ctrlTangToc = new TangTocController(view);
            this.ctrlBamChuong = new BamChuongController(view);

            // Load Data Server
            dataServer = new DataServer();
            loadContestants();
            // Setup Socket
            SetUpServer();
        }

        #region Common Event
        public void setGame()
        {
            game = view.getGame();
            dataServer = new DataServer();
        }
        public void SetUpServer()
        {
            clientList = new List<SimpleContest>();

            Console.WriteLine("Setting up server . . .");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            serverSocket.Listen(1);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);

        }

        private void AppceptCallback(IAsyncResult ar)
        {
            //Socket socket = serverSocket.EndAccept(ar);

            //Console.WriteLine("Client moi connect: ");
            ////this.view.showMessage("Có " +clientList.Count+ " clients đã connect.");
            //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            //serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Console.WriteLine("We accepted a connection .");
                clientSocket = serverSocket.EndAccept(ar);
                if (clientSocket.Connected)
                {
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), clientSocket);
                    // Add to client list
                    SimpleContest simCon = new SimpleContest();
                    simCon.socket = clientSocket;
                    //simCon.id = data.teamId;

                    string enpointIp = clientSocket.RemoteEndPoint.ToString().Substring(0, clientSocket.RemoteEndPoint.ToString().IndexOf(":"));
                    removeDuplicateIp(enpointIp);

                    clientList.Add(simCon);

                    Console.WriteLine("Count: " + clientList.Count);
                    Accept();
                }
                else
                {
                    Accept();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                clientSocket.Close();
                Accept();
            }
        }
        public void removeDuplicateIp(String remoteEndpoint)
        {
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i].socket.RemoteEndPoint.ToString().Contains(remoteEndpoint))
                {
                    clientList.RemoveAt(i);
                    break;
                }
            }
        }
        public void Accept()
        {
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {

            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception)
                {
                    // client đóng kết nối
                    for (int i = 0; i < clientList.Count; i++)
                    {
                        if (clientList[i].socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            //this.view.showMessage("Client co ID = " + clientList[i].id + " bi mat ket noi server");
                            if (i < clientList.Count)
                                clientList.RemoveAt(i);
                        }
                    }

                    // xóa trong list
                    return;
                }
                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(buffer, dataBuf, received);
                    object obj = SendObject.Deserialize(new Messages { Data = dataBuf });
                    DataClient data = null;
                    if (obj is DataClient)
                    {
                        data = (DataClient)obj;
                        if (data.isInit)
                        {

                            // Send first package to client
                            SendOneClient(socket);
                        }
                        else
                        {
                            if (data.round == 1)
                            {
                                // Gan dap an vao dataserver
                                dataServer.ContestTeams[data.teamId - 1].Answer = data.teamAnswer;
                                dataServer.ContestTeams[data.teamId - 1].Time = data.timeAnswer;

                                this.ctrlKhoiDong.showTeamAnswer(data);
                            }
                            else if (data.round == 2)
                            {
                                if (dataServer.outTeam == null || !dataServer.outTeam.Contains(data.teamId.ToString()))//đội bị loại sẽ ko ghi nhận đáp án nếu nhấn trả lời
                                {
                                    // Gan dap an vao dataserver
                                    dataServer.ContestTeams[data.teamId - 1].Answer = data.teamAnswer;
                                    dataServer.ContestTeams[data.teamId - 1].Time = data.timeAnswer;
                                }

                                this.ctrlTangToc.showTeamAnswer(data, dataServer);
                            }
                            else if (data.round == 5)
                            {
                                // Gan dap an vao dataserver
                                dataServer.ContestTeams[data.teamId - 1].Answer = data.teamAnswer;
                                dataServer.ContestTeams[data.teamId - 1].Time = data.timeAnswer;

                                //transformDataBC();
                                this.ctrlBamChuong.showTeamAnswer(data);

                                
                                //if (dataServer.checkBC == 0)
                                //{
                                //    dataServer.checkBC = 1;
                                //}
                                //if (dataServer.checkBC == 0)
                                //{
                                //    dataServer.checkBC = 1;
                                //    dataServer.team = data.teamId;
                                //    //hiển thị ng bấm chuông ở server
                                //    this.ctrlBamChuong.showPerBell(data);
                                //    // call lại client để dừng bấm chuông (xác định đc ng bấm)
                                //    stopBamChuong();
                                //}

                            }
                        }
                    }
                    if (obj is PingData)
                    {
                        PingData abc = (PingData)obj;
                        abc.ketqua = "Test";
                        Sendata(socket, abc);
                    }
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
                else
                {
                    for (int i = 0; i < clientList.Count; i++)
                    {
                        if (clientList[i].socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            //this.view.showMessage("Client co ID = " + clientList[i].id + " bi mat ket noi server");
                            if (i < clientList.Count)
                                clientList.RemoveAt(i);
                        }
                    }
                }
            }

        }
        public void Sendata(Socket socket, object noidung)
        {
            try
            {
                Messages msg = SendObject.Serialize(noidung);
                byte[] data = msg.Data;
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            }
            catch (Exception ex)
            {

                throw;
            }
            

        }


        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        public void SendToClient()
        {

            for (int i = 0; i < clientList.Count; i++)
            {
                dataServer.clientState = clientList[i].socket.Connected;
                Sendata(clientList[i].socket, dataServer);
                if (!clientList[i].socket.Connected)
                {
                    clientList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SendOneClient(Socket socket)
        {
            dataServer.clientState = socket.Connected;
            Sendata(socket, dataServer);
        }

        #endregion

        #region Switch Screen
        public void SwitchClientToRound1()
        {
            currentRound = 1;
            dataServer.Round = 1;
            dataServer.Action = Constant.ACTION_INIT;
            SendToClient();
        }

        public void SwitchClientToRound_GT()
        {
            currentRound = 4;
            dataServer.Round = 4;
            dataServer.Action = Constant.ACTION_INIT;
            SendToClient();
        }
        public void SwitchClientToRound_BC()
        {
            currentRound = 5;
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_INIT;
            SendToClient();
        }

        public void SwitchClientToRound2()
        {
            GetRowDefaulRound2();
            currentRound = 2;
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_INIT;
            SendToClient();
        }
        public void SwitchClientToRound0()
        {
            currentRound = 0;
            dataServer.Round = 0;
            dataServer.check = 1;
            SendToClient();
        }
        public void SwitchClientToRound_GK()
        {
            currentRound = 6;
            dataServer.Round = 6;
            SendToClient();
        }
        #endregion

        #region cauhinh
        public void loadContestants()
        {
            dataServer.ContestTeams = this.ctrlCauHinh.loadContestants();
            dataServer.Round = 0;
        }

        public void saveContestants()
        {
            ctrlCauHinh.saveContestants();

            // Send to socket
            dataServer.ContestTeams = this.ctrlCauHinh.loadContestants();
            dataServer.Round = 0;

            SendToClient();
        }
        public void getRound1Questions()
        {
            ctrlCauHinh.getRound1Questions();
        }

        public void saveRound1Question(int questRound)
        {
            ctrlCauHinh.saveRound1Question(questRound);
        }

        public void loadRound1QuestionCauHinh(int questRound)
        {
            ctrlCauHinh.loadRound1Question(questRound);
        }

        public void importRound1Questions(int questRound)
        {
            ctrlCauHinh.importRound1Questions(questRound);
        }

        public void getRound2Questions()
        {
            ctrlCauHinh.getRound2Questions();
        }

        public void saveRound2Question()
        {
            ctrlCauHinh.saveRound2Question();
        }
        public void GetRowDefaulRound2()
        {
            ctrlCauHinh.GetRowDefaulRound2();
        }
        public void saveHideRowRound2()
        {
            ctrlCauHinh.saveHideRowRound2();
        }

        public void importRound2Questions()
        {
            ctrlCauHinh.importRound2Questions();
        }

        public void updateRound2Questions()
        {
            this.ctrlCauHinh.updateRound2Questions();
        }
        #endregion

        #region khoidong
        public void showRound_1_bell(int team, bool status)
        {
            //if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 1;
            dataServer.Action = Constant.ACTION_SHOW_BELL;
            dataServer.team = team;
            dataServer.status = status;
            SendToClient();
        }

        public void loadRound1Question(int chooseRound)
        {
            this.ctrlKhoiDong.loadRound1Question(chooseRound);

            if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 1;
            dataServer.questionRound = chooseRound;
            dataServer.Action = Constant.ACTION_LOAD_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            SendToClient();
        }

        public void showRound1Question(int chooseRound)
        {
            if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 1;
            dataServer.Action = Constant.ACTION_SHOW_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionText = this.ctrlKhoiDong.loadRound1Question(chooseRound).QuestContent;
            SendToClient();
        }

        public void showRound1Answer(int chooseRound)
        {
            if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 1;
            dataServer.Action = Constant.ACTION_SHOW_ANSWER;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionAnswer = this.ctrlKhoiDong.loadRound1Question(chooseRound).QuestAnswer;
            SendToClient();
        }

        public void startRound1Timer()
        {
            if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 1;
            dataServer.Action = Constant.ACTION_RUN_TIMER;
            dataServer.QuestionNum = this.view.currentQuest;
            SendToClient();
        }

        public void loadRound1()
        {
            this.view.InitRound1(dataServer);
        }

        public void updateMark(int teamId, float mark, int round)
        {
            if (teamId != 0)
            {
                this.dataServer.ContestTeams[teamId - 1].Mark += mark;
            }

            dataServer.Round = round;
            dataServer.Action = Constant.ACTION_UPDATE_SCORE;
            SendToClient();
        }
        #endregion

        #region TangToc
        public void showRound_2_AddPoint(int team, bool status)
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_ADD_POINT;
            dataServer.team = team;
            dataServer.status = status;
            SendToClient();
        }
        public void showRound2Question()
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_SHOW_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionText = this.ctrlTangToc.loadRound2Question().QuestContent;
            SendToClient();
        }

        public void showRound2Answer()
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_SHOW_ANSWER;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionAnswer = this.ctrlTangToc.loadRound2Question().QuestAnswer;
            dataServer.InitOchu[this.view.currentQuest - 1] = dataServer.QuestionAnswer;
            // Show answer in server
            this.view.OpenRow(this.view.currentQuest, dataServer.QuestionAnswer);

            SendToClient();
        }

        public void showRound2DACT()
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_SHOW_DACT;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionAnswer = this.ctrlTangToc.loadRound2Question().QuestAnswer_vi;
            SendToClient();
        }

        public void showRound_2_bell(int team, bool status)
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_SHOW_BELL;
            dataServer.team = team;
            dataServer.status = status;
            SendToClient();
        }

        public void teamEliminatedRound2(int[] lock_unlockTeam, bool status)
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_OUT_TEAM;
            dataServer.lock_unlockTeam = lock_unlockTeam;
            dataServer.status = status;
            SendToClient();
        }

        public void closeRound2Answer()
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_CLOSE_ANSWER;
            dataServer.QuestionNum = this.view.currentQuest;
            String ochu = new String(dataServer.InitOchu[this.view.currentQuest - 1].Select(r => r == '$' ? '$' : '#').ToArray());
            if (dataServer.isOpenCol)
            {
                dataServer.InitOchu[this.view.currentQuest - 1] = ochu.Remove(14, 1).Insert(14, dataServer.HangDoc[this.view.currentQuest - 1]);
            }
            else
            {
                dataServer.InitOchu[this.view.currentQuest - 1] = ochu;
            }


            // Show answer in server
            this.view.CloseRow(this.view.currentQuest);

            SendToClient();
        }

        public void showRow(string[] myarr)
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_SHOW_ROW;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.myArrayShowRound2 = myarr;

            // Show row in server
            //this.view.ShowRow(row);

            SendToClient();
        }

        public void hideRow(int[]myarr)
        {
            //if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_HIDE_ROW;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.myArrayRound2 = myarr;

            // Show row in server
            //this.view.HideRow(row);

            SendToClient();
        }

        public void loadRound2()
        {
            this.ctrlTangToc.loadRound2(ref dataServer);
        }

        public void loadOChu()
        {
            this.ctrlTangToc.loadOChu(ref dataServer);
        }

        public void moHangDoc()
        {
            this.ctrlTangToc.loadHangDoc(ref dataServer);

            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_OPEN_HANG_DOC;

            this.view.MoHangDoc(dataServer);
            if (dataServer.HangDoc.Count > 0)
            {
                for (int j = 0; j < 20; j++)
                {
                    //if (!dataServer.HangDoc[j - 1].Equals('$')) SetTextComp(2, "R2_D" + j + "_C15", data.HangDoc[j - 1]);
                    dataServer.InitOchu[j] = dataServer.InitOchu[j].Remove(14, 1).Insert(14, dataServer.HangDoc[j]);
                }
            }

            SendToClient();
        }

        public void loadRound2Question()
        {
            this.ctrlTangToc.loadRound2Question();

            if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_LOAD_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            SendToClient();
        }

        public void startRound2Timer(string timeTT)
        {
            if (currentRound != 2) SwitchClientToRound2();
            dataServer.Round = 2;
            dataServer.Action = Constant.ACTION_RUN_TIMER;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.flagTimeRound2 = timeTT;
            SendToClient();
        }
        #endregion

        #region Giới thiệu
        public void loadGioithieu()
        {
            this.view.InitGioithieu(dataServer);
        }

        public void showRound_GT_content(string content)
        {
            //if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 4;
            dataServer.Action = Constant.ACTION_SHOW_CONTENT;
            dataServer.QuestionText = content;
            SendToClient();
        }

        public void startRoundGTTimer(string action, int minutes)
        {
            if (currentRound != 1) SwitchClientToRound_GT();
            dataServer.Round = 4;
            dataServer.Action = Constant.ACTION_RUN_TIMER;
            dataServer.description = action;
            dataServer.minutes = minutes;
            SendToClient();
        }

        #endregion

        #region Bấm chuông
        public void loadBamchuong()
        {
            this.view.InitBamChuong(dataServer);
        }
        public void stopBamChuong()
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_STOP_BAM_CHUONG;
            SendToClient();
        }
        public void transformDataBC()
        {
            dataServer.Round = 7;
            SendToClient();
        }
        public void startRoundBCTimer(string action, int minutes)
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_RUN_TIMER;
            dataServer.description = action;
            dataServer.minutes = minutes;
            SendToClient();
        }
        public void showRound_BC_star(int team, bool status)
        {
            //if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_SHOW_STAR;
            dataServer.team = team;
            dataServer.status = status;
            SendToClient();
        }

        public void showRound_BC_bell(int team, bool status)
        {
            //if (currentRound != 1) SwitchClientToRound1();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_SHOW_BELL;
            dataServer.team = team;
            dataServer.status = status;
            SendToClient();
        }

        public void startRoundBCTimer()
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_RUN_TIMER;
            dataServer.QuestionNum = this.view.currentQuest;
            SendToClient();
        }
        public void loadRoundBCQuestion(int chooseRound)
        {
            this.ctrlBamChuong.loadRoundBCQuestion(chooseRound);

            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_LOAD_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.questionRound = chooseRound;
            SendToClient();
        }

        public void showNameRoundBC(int chooseRound)
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_SHOW_NAME_ROUND;
            dataServer.questionRound = chooseRound;
            SendToClient();
        }

        public void showRoundBCQuestion(int chooseRound)
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_SHOW_QUESTION;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionText = this.ctrlBamChuong.loadRoundBCQuestion(chooseRound).QuestContent;
            SendToClient();
        }
        public void showRoundBCAnswer(int chooseRound)
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_SHOW_ANSWER;
            dataServer.QuestionNum = this.view.currentQuest;
            dataServer.QuestionAnswer = this.ctrlBamChuong.loadRoundBCQuestion(chooseRound).QuestAnswer;
            SendToClient();
        }
        public void changeNameBC(int team, string name, int point)
        {
            if (currentRound != 5) SwitchClientToRound_BC();
            dataServer.Round = 5;
            dataServer.Action = Constant.ACTION_CHANGE_NAME;
            dataServer.team = team;
            dataServer.name = name;
            dataServer.point = point;
            SendToClient();
        }
        #endregion

    }
}
