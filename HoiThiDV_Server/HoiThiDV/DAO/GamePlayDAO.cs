using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.DAO
{
    public class GamePlayDAO
    {
        public bool InsertAnswer(DataClient data)
        {
            bool result = false;
            try
            {
                OleDbCommand cmd = new OleDbCommand("INSERT into Answer  (Game, Round, TeamID, Question, AnswerText, TimeAnswer) Values(@game, @round, @teamid, @question, @answer, @time) ");
                cmd.Connection = ConnectDB.getConnection();
                cmd.Parameters.Add("@game", OleDbType.Integer).Value = MasterController.game;
                cmd.Parameters.Add("@round", OleDbType.Integer).Value = data.round;
                cmd.Parameters.Add("@teamid", OleDbType.Integer).Value = data.teamId;
                cmd.Parameters.Add("@question", OleDbType.Integer).Value = data.teamQuestion;
                cmd.Parameters.Add("@answer", OleDbType.VarChar).Value = data.teamAnswer;
                cmd.Parameters.Add("@time", OleDbType.VarChar).Value = data.timeAnswer;
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

        public DataClient GetAnswer()
        {
            DataClient dc = new DataClient();
            return dc;
        }
    }
}
