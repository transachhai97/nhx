using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.DAO
{
    public class ContestantDAO
    {
        public bool InsertContestant(List<Contestant> listContestants)
        {
            bool result = false;
            string name1 = listContestants[0].Name;
            string name2 = listContestants[1].Name;
            string name3 = listContestants[2].Name;
            string name4 = listContestants[3].Name;
            string name5 = listContestants[4].Name;
            string name6 = listContestants[5].Name;

            string contestantNames = name1 + ";" + name2 + ";" + name3 + ";" + name4 + ";" + name5;

            OleDbCommand cmd = new OleDbCommand("Delete from Contestants where Game = " + MasterController.game);
            cmd.Connection = ConnectDB.getConnection();
            try
            {
                cmd.ExecuteNonQuery();

                OleDbCommand cmd1 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(1, @name1, @game1) ");
                cmd1.Connection = ConnectDB.getConnection();
                cmd1.Parameters.Add("@name1", OleDbType.VarChar).Value = name1;
                cmd1.Parameters.Add("@game1", OleDbType.Integer).Value = MasterController.game;
                cmd1.ExecuteNonQuery();

                OleDbCommand cmd2 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(2, @name2, @game2) ");
                cmd2.Connection = ConnectDB.getConnection();
                cmd2.Parameters.Add("@name2", OleDbType.VarChar).Value = name2;
                cmd2.Parameters.Add("@game2", OleDbType.Integer).Value = MasterController.game;
                cmd2.ExecuteNonQuery();

                OleDbCommand cmd3 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(3, @name3, @game3) ");
                cmd3.Connection = ConnectDB.getConnection();
                cmd3.Parameters.Add("@name3", OleDbType.VarChar).Value = name3;
                cmd3.Parameters.Add("@game3", OleDbType.Integer).Value = MasterController.game;
                cmd3.ExecuteNonQuery();

                OleDbCommand cmd4 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(4, @name4, @game4) ");
                cmd4.Connection = ConnectDB.getConnection();
                cmd4.Parameters.Add("@name4", OleDbType.VarChar).Value = name4;
                cmd4.Parameters.Add("@game4", OleDbType.Integer).Value = MasterController.game;
                cmd4.ExecuteNonQuery();

                if (!string.IsNullOrEmpty(name5))
                {
                    OleDbCommand cmd5 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(5, @name5, @game5) ");
                    cmd5.Connection = ConnectDB.getConnection();
                    cmd5.Parameters.Add("@name5", OleDbType.VarChar).Value = name5;
                    cmd5.Parameters.Add("@game5", OleDbType.Integer).Value = MasterController.game;
                    cmd5.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(name6))
                {
                    OleDbCommand cmd6 = new OleDbCommand("INSERT into Contestants (ID, Name, Game) Values(6, @name6, @game6) ");
                    cmd6.Connection = ConnectDB.getConnection();
                    cmd6.Parameters.Add("@name6", OleDbType.VarChar).Value = name6;
                    cmd6.Parameters.Add("@game6", OleDbType.Integer).Value = MasterController.game;
                    cmd6.ExecuteNonQuery();
                }

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

        public List<Contestant> getAllContestants()
        {
            OleDbCommand cmd = new OleDbCommand("Select ID, Name, Mark from Contestants where Game = " + MasterController.game);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            List<Contestant> list = new List<Contestant>();
            Contestant ct;
            cmd.Connection = ConnectDB.getConnection();
            try
            {
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                    foreach (DataRow row in dt.Rows)
                    {
                        ct = new Contestant();
                        ct.Id = (int)row[0];
                        ct.Name = row[1].ToString();
                        ct.Mark = (int)row[2];
                        list.Add(ct);
                    }

                ConnectDB.closeConnection();

            }

            catch (OleDbException ex)
            {
                ConnectDB.closeConnection();
                throw ex;
            }

            return list;
        }

        // ... existing code ...
        public bool UpdateContestantMark(int gameId, int contestantId, float newMark)
        {
            bool result = false;
            OleDbCommand cmd = new OleDbCommand("UPDATE Contestants SET Mark = @newMark WHERE ID = @contestantId AND GAME = @gameId" );
            cmd.Connection = ConnectDB.getConnection();
            cmd.Parameters.Add("@newMark", OleDbType.Single).Value = newMark;
            cmd.Parameters.Add("@contestantId", OleDbType.Integer).Value = contestantId;
            cmd.Parameters.Add("@gameId", OleDbType.Integer).Value = gameId;

            try
            {
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            finally
            {
                ConnectDB.closeConnection();
            }
            return result;
        }
        // ... existing code ...

        public bool SaveContestant(int gameId, Contestant contestant)
        {
            bool result = false;
            OleDbCommand cmd = new OleDbCommand("UPDATE Contestants SET Mark = @newMark WHERE ID = @contestantId AND GAME = @gameId");
            cmd.Connection = ConnectDB.getConnection();
            cmd.Parameters.Add("@newMark", OleDbType.Single).Value = contestant.Mark;
            cmd.Parameters.Add("@contestantId", OleDbType.Integer).Value = contestant.Id;
            cmd.Parameters.Add("@gameId", OleDbType.Integer).Value = gameId;

            try
            {
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            finally
            {
                ConnectDB.closeConnection();
            }
            return result;
        }
    }
}
