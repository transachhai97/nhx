using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.DAO
{
    public class QuestionDAO
    {
        public DataTable GetAllQuest(int questRound)
        {
            OleDbCommand cmd = new OleDbCommand("Select Questnum as Num, Questcontent as Content, Questanswer, QuestAnswer_vi, ShowQuestNum from Question where Game = " + MasterController.game + " and QuestRound =  " + questRound);
            if (questRound == 6 || questRound == 7 || questRound == 8 || questRound == 9)// câu hỏi phần câu hỏi phụ (6) ,hùng biện (7), dự phòng phàn thi bấm chuông (8), dự phòng phần thi khởi động(9) 
            {
                cmd = new OleDbCommand("Select Questnum as Num, Questcontent as Content, Questanswer, QuestAnswer_vi, ShowQuestNum from Question where QuestRound =  " + questRound);
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            cmd.Connection = ConnectDB.getConnection();
            try
            {
                adapter.Fill(dt);
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

            return dt;
        }

        public DataTable GetQuest(Question quest)
        {
            OleDbCommand cmd = new OleDbCommand("Select questnum as Num, questcontent as Content, questanswer, QuestAnswer_vi, ShowQuestNum from Question where Game = " + MasterController.game + " and questround =  " + quest.QuestRound + " and questnum = " + quest.QuestNum);
            if (quest.QuestRound == 6 || quest.QuestRound == 7 || quest.QuestRound == 8 || quest.QuestRound == 9)
            {
                cmd = new OleDbCommand("Select questnum as Num, questcontent as Content, questanswer, QuestAnswer_vi, ShowQuestNum from Question where questround =  " + quest.QuestRound + " and questnum = " + quest.QuestNum);
            }
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            cmd.Connection = ConnectDB.getConnection();
            try
            {
                adapter.Fill(dt);
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

            return dt;
        }

        public bool InsertQuest(Question quest)
        {
            bool result = false;
            OleDbCommand cmd = new OleDbCommand("Insert into Question (Game, QuestRound, QuestNum , QuestContent, QuestAnswer, QuestAnswer_vi, ShowQuestNum) Values ("
                + MasterController.game + " , " + quest.QuestRound + " , " + quest.QuestNum + ", '" + quest.QuestContent
                + "', '" + quest.QuestAnswer + "' , '" + (string.IsNullOrEmpty(quest.QuestAnswer_vi) ? "" : quest.QuestAnswer_vi) 
                + "' , '" + quest.ShowQuestNum + "')");
            if (quest.QuestRound == 6 || quest.QuestRound == 7 || quest.QuestRound == 8 || quest.QuestRound == 9)// câu hỏi phần câu hỏi phụ (6) ,hùng biện (7), dự phòng phàn thi bấm chuông (8), dự phòng phần thi khởi động(9) 
            {
                cmd = new OleDbCommand("Insert into Question (Game, QuestRound, QuestNum , QuestContent, QuestAnswer, QuestAnswer_vi, ShowQuestNum) Values ("
                + MasterController.game + " , " + quest.QuestRound + " , " + quest.QuestNum + ", '" + quest.QuestContent 
                + "', '" + quest.QuestAnswer + "' , '" + (string.IsNullOrEmpty(quest.QuestAnswer_vi) ? "" : quest.QuestAnswer_vi) 
                + "' , '" + quest.ShowQuestNum + "')");
            }
            cmd.Connection = ConnectDB.getConnection();

            try
            {
                cmd.ExecuteNonQuery();
                ConnectDB.closeConnection();
                result = true;
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }
            return result;
        }

        public void UpdateQuest(Question quest)
        {
            OleDbCommand cmd = new OleDbCommand();

            string query = "";
            if (quest.QuestAnswer == "")
            {
                query = "Update Question set QuestContent = '" + quest.QuestContent + "' where Game = " + MasterController.game + " and QuestRound = " + quest.QuestRound + " and  QuestNum = " + quest.QuestNum;
                if (quest.QuestRound == 6 || quest.QuestRound == 7 || quest.QuestRound == 8 || quest.QuestRound == 9)// câu hỏi phần câu hỏi phụ (6) ,hùng biện (7), dự phòng phàn thi bấm chuông (8), dự phòng phần thi khởi động(9) 
                {
                    query = "Update Question set QuestContent = '" + quest.QuestContent + "' where QuestRound = " + quest.QuestRound + " and  QuestNum = " + quest.QuestNum;
                }
            }
            else
            {
                query = "Update Question set QuestContent = '" + quest.QuestContent + "', QuestAnswer = '" + quest.QuestAnswer + "' where Game = " + MasterController.game + " and QuestRound = " + quest.QuestRound + " and  QuestNum = " + quest.QuestNum;
                if (quest.QuestRound == 6 || quest.QuestRound == 7 || quest.QuestRound == 8 || quest.QuestRound == 9)// câu hỏi phần câu hỏi phụ (6) ,hùng biện (7), dự phòng phàn thi bấm chuông (8), dự phòng phần thi khởi động(9) 
                {
                    query = "Update Question set QuestContent = '" + quest.QuestContent + "', QuestAnswer = '" + quest.QuestAnswer + "' where QuestRound = " + quest.QuestRound + " and  QuestNum = " + quest.QuestNum;
                }
            }

            cmd.CommandText = query;
            cmd.Connection = ConnectDB.getConnection();

            try
            {
                cmd.ExecuteNonQuery();
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

        }

        public void DeleteAllQuestByRound(Question quest)
        {
            OleDbCommand cmd = new OleDbCommand();

            string query = "Delete from Question where Game = " + MasterController.game + " and QuestRound = " + quest.QuestRound;
            if (quest.QuestRound == 6 || quest.QuestRound == 7 || quest.QuestRound == 8 || quest.QuestRound == 9)// câu hỏi phần câu hỏi phụ (6) ,hùng biện (7), dự phòng phàn thi bấm chuông (8), dự phòng phần thi khởi động(9) 
            {
                query = "Delete from Question where QuestRound = " + quest.QuestRound;
            }

            cmd.CommandText = query;
            cmd.Connection = ConnectDB.getConnection();

            try
            {
                cmd.ExecuteNonQuery();
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

        }

        public DataTable GetRowDefaulRound2(Question quest)
        {
            OleDbCommand cmd = new OleDbCommand("Select AnswerText from Answer where Game = " + MasterController.game + " and Round =  " + quest.QuestRound);

            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            cmd.Connection = ConnectDB.getConnection();
            try
            {
                adapter.Fill(dt);
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

            return dt;
        }

        public bool InsertHideRowDefaulRound2(Question quest)
        {
            bool result = false;
            OleDbCommand cmd = new OleDbCommand("Insert into Answer(Game, Round, AnswerText) Values ("
                + MasterController.game + " , " + quest.QuestRound + " , " + quest.QuestContent + ")");

            cmd.Connection = ConnectDB.getConnection();

            try
            {
                cmd.ExecuteNonQuery();
                ConnectDB.closeConnection();
                result = true;
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }
            return result;
        }

        public void UpdateHideRowDefaulRound2(Question quest)
        {
            OleDbCommand cmd = new OleDbCommand();

            string query = "";

            query = "Update Answer set AnswerText = '" + quest.QuestContent + "' where Game = " + MasterController.game + " and Round = " + quest.QuestRound;

            cmd.CommandText = query;
            cmd.Connection = ConnectDB.getConnection();

            try
            {
                cmd.ExecuteNonQuery();
                ConnectDB.closeConnection();
            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

        }

    }
}
