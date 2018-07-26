using System;
using System.Data;
using System.Data.OleDb;

namespace sqlCoreNet.Data.oledb
{
    public class oledbDriver
    {
        public oledbDriver(string strConnection)
        {
            this.strConnection = strConnection;
            conn = new OleDbConnection(this.strConnection);
        }

        public DataTable GetSchema()
        {
            return conn.GetSchema();
        }

        public OleDbDataReader Select(string strSql)
        {
            OleDbDataReader dr = null;
            conn.Open();

            using (OleDbCommand cmd = new OleDbCommand(strSql, conn))
            {
                dr = cmd.ExecuteReader();
            }

            conn.Close();
            return dr;
        }

        public string simpleSelect(string strSql)
        {
            string resultValue = "";
            conn.Open();

            try
            {
                using (OleDbCommand cmd = new OleDbCommand(strSql, conn))
                {
                    resultValue = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                resultValue = "0x00000020";                 // 数据库操作执行错误
                return resultValue;
            }

            conn.Close();
            return resultValue;
        }

        public int exec(string strSql)
        {
            int resultValue = 0;
            conn.Open();

            try
            {
                using (OleDbCommand cmd = new OleDbCommand(strSql, conn))
                {
                    resultValue = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                resultValue = -1000;
                return resultValue;
            }

            conn.Close();
            return resultValue;
        }


        private string strConnection;
        private OleDbConnection conn;
    }
}
