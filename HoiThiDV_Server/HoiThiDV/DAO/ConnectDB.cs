using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiThiDV.DAO
{
    public static class ConnectDB
    {
        
        const string strConnect = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        private static OleDbConnection con;
        public static OleDbConnection getConnection()
        {
            con = new OleDbConnection();
            string excelPath = System.Configuration.ConfigurationManager.AppSettings["ExcelPath"];
            con.ConnectionString = strConnect + excelPath;

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                return null;
            }

            return con;
        }

        public static void closeConnection()
        {
            con.Close();
        }
    }
}
