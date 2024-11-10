using HoiThiDV.DAO;
using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.Controller
{
    public class CauHinhController
    {
        private FormServer view;
        private QuestionDAO qdao;
        private ContestantDAO cdao;

        public CauHinhController(FormServer view)
        {
            this.view = view;
            this.qdao = new QuestionDAO();
            this.cdao = new ContestantDAO();
        }

        #region cauhinhdoichoi
        public List<Contestant> loadContestants()
        {
            List<Contestant> list = cdao.getAllContestants();
            this.view.setContestants(list);
            return list;
        }

        public void saveContestants()
        {
            try
            {
                if (cdao.InsertContestant(view.getListContestants()))
                {
                    view.showMessage("Cập nhật thành công.");
                }
                else
                {
                    view.showMessage("Có lỗi xảy ra.");
                }
            }
            catch (OleDbException ex)
            {
                view.showMessage("Có lỗi xảy ra: " + ex.Message);
            }

        }

        #endregion

        #region cauhinhkhoidong
        public void getRound1Questions()
        {
            this.view.fillGridKhoiDong(qdao.GetAllQuest(1));
        }

        public void saveRound1Question(int questRound)
        {
            try
            {
                Question q = this.view.getRound1Question();
                if (qdao.GetQuest(q).Rows.Count == 0)
                {
                    qdao.InsertQuest(q);
                }
                else
                {
                    qdao.UpdateQuest(q);
                }
                this.view.fillGridKhoiDong(qdao.GetAllQuest(questRound));
            }
            catch (Exception ex)
            {
                this.view.showMessage("Có lỗi xảy ra : " + ex.Message);
            }
        }

        public void loadRound1Question(int questRound)
        {
            try
            {
                this.view.fillGridKhoiDong(qdao.GetAllQuest(questRound));
            }
            catch (Exception ex)
            {
                this.view.showMessage("Có lỗi xảy ra : " + ex.Message);
            }
        }

        public void importRound1Questions(int questRound)
        {
            string path = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + this.view.getRound1ExcelPath() + "; Extended Properties=\"Excel 12.0;HDR=Yes;\";";
            OleDbConnection con = new OleDbConnection(path);
            OleDbDataAdapter adapter = new OleDbDataAdapter("Select [TT], [Content],[A],[B],[C],[D],[Answer]  from [Sheet1$]", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            Question q = new Question();
            q.QuestRound = questRound;
            qdao.DeleteAllQuestByRound(q);
            List<string> value = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            if (dt != null && dt.Columns.Count >= 6)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0] != null && !string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                    {
                        if (value.Contains(dt.Rows[i][0].ToString())) // num tu 1-10 thi xu li
                        {
                            q.QuestNum = int.Parse(dt.Rows[i][0].ToString());
                            if (string.IsNullOrEmpty(dt.Rows[i][2].ToString()))
                            {
                                q.QuestContent = dt.Rows[i][1].ToString();
                            }
                            else
                            {
                                q.QuestContent = dt.Rows[i][1].ToString() + "\r\n\r\n" + ("A. " + dt.Rows[i][2].ToString()) + "\r\n" + ("B. " + dt.Rows[i][3].ToString()) + "\r\n" + ("C. " + dt.Rows[i][4].ToString())
                                 + "\r\n" + ("D. " + dt.Rows[i][5].ToString());
                            }
                            
                            dt.Rows[i][1] = q.QuestContent;
                            q.QuestAnswer = dt.Rows[i][6].ToString();
                            if (qdao.GetQuest(q).Rows.Count == 0)
                            {
                                qdao.InsertQuest(q);
                            }
                            else
                            {
                                qdao.UpdateQuest(q);
                            }
                        }
                    }
                }
            }
            this.view.fillGridKhoiDong(qdao.GetAllQuest(questRound));
        }
        #endregion

        #region cauhinhtangtoc
        public void getRound2Questions()
        {
            DataTable dt = qdao.GetAllQuest(2);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            this.view.fillGridTangToc(dt);

            for (int i = 1; i <= 15; i++)
            {
                this.view.fillOChu(i, "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            }
            foreach (DataRow dr in dt.Rows)
            { 
                this.view.fillOChu(int.Parse(dr[0].ToString()), dr[2].ToString());
            }
        }

        public void saveRound2Question()
        {
            try
            {
                Question q = this.view.getRound2Question();
                if (qdao.GetQuest(q).Rows.Count == 0)
                {
                    if (qdao.InsertQuest(q)) view.showMessage("Cập nhật thành công.");
                }
                else
                {
                    qdao.UpdateQuest(q);
                }
                DataTable dt = qdao.GetAllQuest(2);
                dt.DefaultView.Sort = "Num";
                dt = dt.DefaultView.ToTable();
                this.view.fillGridTangToc(dt);
            }
            catch (Exception ex)
            {
                this.view.showMessage("Có lỗi xảy ra : " + ex.Message);
            }
        }
        public void GetRowDefaulRound2()
        {
            Question q = this.view.getHideRowRound2();
            DataTable dt = qdao.GetRowDefaulRound2(q);
            
            this.view.setHideRowRound2(dt.Rows.Count>0?dt.Rows[0].ItemArray[0].ToString():"");
        }

        public void saveHideRowRound2()
        {
            try
            {
                Question q = this.view.getHideRowRound2();
                if (qdao.GetRowDefaulRound2(q).Rows.Count == 0)
                {
                    if (qdao.InsertHideRowDefaulRound2(q)) view.showMessage("Thêm mới thành công.");
                }
                else
                {
                    qdao.UpdateHideRowDefaulRound2(q); 
                    view.showMessage("Cập nhật thành công.");
                }
            }
            catch (Exception ex)
            {
                this.view.showMessage("Có lỗi xảy ra : " + ex.Message);
            }
        }

        public void importRound2Questions()
        {
            string path = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + this.view.getRound2ExcelPath() + "; Extended Properties=\"Excel 12.0;HDR=No;\";";
            OleDbConnection con = new OleDbConnection(path);
            OleDbDataAdapter adapter = new OleDbDataAdapter("Select * from [Sheet1$]", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            Question q = new Question();
            q.QuestRound = 2;
            qdao.DeleteAllQuestByRound(q);
            List<string> value = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16","17","18","19","20" };
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0] != null && !string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                    {
                        if (value.Contains(dt.Rows[i][0].ToString())) // num tu 1-20 thi xu li
                        {
                            q.QuestRound = 2;
                            q.QuestNum = int.Parse(dt.Rows[i][0].ToString());
                            q.QuestContent = dt.Rows[i][1].ToString();
                            q.QuestAnswer = dt.Rows[i][2].ToString();
                            q.QuestAnswer_vi = dt.Rows[i][3].ToString();
                            q.ShowQuestNum =string.IsNullOrEmpty(dt.Rows[i][4].ToString())? new int() : int.Parse(dt.Rows[i][4].ToString());
                            if (qdao.GetQuest(q).Rows.Count == 0)
                            {
                                qdao.InsertQuest(q);
                            }
                            else
                            {
                                qdao.UpdateQuest(q);
                            }
                        }
                    }
                }
                this.view.fillGridTangToc(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    if (value.Contains(dr[0].ToString())) this.view.fillOChu(int.Parse(dr[0].ToString()), dr[2].ToString());
                }
            }


        }

        public void updateRound2Questions()
        {
            Question[] qlist = this.view.getRound2ListQuestions();
            DataTable dttemp;
            foreach (Question q in qlist)
            {
                dttemp = qdao.GetQuest(q);
                if (dttemp.Rows.Count > 0)
                {
                    q.QuestContent = (string)dttemp.Rows[0][1];

                    qdao.UpdateQuest(q);
                }
                else if (q.QuestAnswer.Length > 0 || q.QuestContent.Length > 0)
                {
                    qdao.InsertQuest(q);
                }
            }

            DataTable dt = qdao.GetAllQuest(2);
            dt.DefaultView.Sort = "Num";
            dt = dt.DefaultView.ToTable();
            this.view.fillGridTangToc(dt);
            foreach (DataRow dr in dt.Rows)
            {
                this.view.fillOChu(int.Parse(dr[0].ToString()), dr[2].ToString());
            }
        }
        #endregion
    }
}
