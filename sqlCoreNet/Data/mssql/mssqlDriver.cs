using sqlCoreNet.Collections;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace sqlCoreNet.Data.mssql
{
    [Serializable]
    public class mssqlDriver
    {
        public mssqlDriver() { }
        public mssqlDriver(string strConnection)
        {
            this.strConnection = strConnection;
            conn = new SqlConnection(strConnection);
        }

        public void Open()
        {
            conn = new SqlConnection(strConnection);
        }

        public DataTable GetSchema()
        {
            return conn.GetSchema();
        }


        public DataTable Select(string strSql)
        {
            DataTable dt = new DataTable();
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                dt.Load(dr);
            }

            conn.Close();
            return dt;
        }

        public string simpleSelect(string strSql)
        {
            string resultValue = "";
            conn.Open();

            try
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
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
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
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



        // 有名字看情况进行
        public void execProcedure(string ProcedureName)
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = ProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (_inputProcList != null)
                {
                    for (int i = 0; i < _inputProcList.Count; i++)
                    {
                        cmd.Parameters.Add(buildInputParameter(_inputProcList[i] as proceTuple));
                    }
                }

                if (_outputProcList != null)
                {
                    for (int i = 0; i < _outputProcList.Count; i++)
                    {
                        cmd.Parameters.Add(buildOutputParameter(_outputProcList[i] as proceTuple));
                    }
                }

                if (_returnProc != null)
                {
                    SqlParameter sqlParam = new SqlParameter(_returnProc.ParamName, getSqlDbType(_returnProc.TypeName), _returnProc.TypeLegnth);
                    sqlParam.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(sqlParam);
                }

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        public void execProcedure(string ProcedureName, ArrayList paramList)
        {
            conn.Open();

            _inputProcList = paramList;
            _outputProcList = null;
            _returnProc = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = ProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < _inputProcList.Count; i++)
                {
                    cmd.Parameters.Add(buildInputParameter(_inputProcList[i] as proceTuple));
                }

                cmd.ExecuteNonQuery();
            }

            conn.Close();
            return;
        }

        // 只是进行输入、输出运算
        public void execProcedure(string ProcedureName, ArrayList paramList, ArrayList outValueList)
        {
            conn.Open();

            _inputProcList = paramList;
            _outputProcList = outValueList;
            _returnProc = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = ProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < _inputProcList.Count; i++)
                {
                    cmd.Parameters.Add(buildInputParameter(_inputProcList[i] as proceTuple));
                }

                for (int i = 0; i < _outputProcList.Count; i++)
                {
                    cmd.Parameters.Add(buildOutputParameter(_outputProcList[i] as proceTuple));
                }

                cmd.ExecuteNonQuery();
            }

            conn.Close();
            return;
        }

        // 全部都上
        public void execProcedure(string ProcedureName, ArrayList paramList, ArrayList outValueList, proceTuple returnValue)
        {
            conn.Open();

            _inputProcList = paramList;
            _outputProcList = outValueList;
            _returnProc = returnValue;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = ProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < _inputProcList.Count; i++)
                {
                    cmd.Parameters.Add(buildInputParameter(_inputProcList[i] as proceTuple));
                }

                for (int i = 0; i < _outputProcList.Count; i++)
                {
                    cmd.Parameters.Add(buildOutputParameter(_outputProcList[i] as proceTuple));
                }

                SqlParameter sqlParam = new SqlParameter(_returnProc.ParamName, getSqlDbType(_returnProc.TypeName), _returnProc.TypeLegnth);
                sqlParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(sqlParam);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
            return;
        }

        public string setConnection
        {
            get
            {
                return strConnection;
            }

            set
            {
                strConnection = value;
            }
        }

        public ArrayList inputProcList
        {
            get
            {
                return _inputProcList;
            }

            set
            {
                _inputProcList = value;
            }
        }

        public ArrayList outputProcList
        {
            get
            {
                return _outputProcList;
            }

            set
            {
                _outputProcList = value;
            }
        }

        public object returnProc
        {
            get
            {
                return _returnProc;
            }
        }

        private string strConnection;
        private SqlConnection conn;

        // 存储过程数据
        private ArrayList _inputProcList = null;
        private ArrayList _outputProcList = null;
        private proceTuple _returnProc = null;

        private SqlParameter buildInputParameter(proceTuple pt)
        {
            SqlParameter sqlParam = new SqlParameter(pt.ParamName, getSqlDbType(pt.TypeName), pt.TypeLegnth);
            sqlParam.Value = pt.inputData;

            return sqlParam;
        }

        private SqlParameter buildOutputParameter(proceTuple pt)
        {
            SqlParameter sqlParam = new SqlParameter(pt.ParamName, getSqlDbType(pt.TypeName), pt.TypeLegnth);
            sqlParam.Direction = ParameterDirection.Output;
            return sqlParam;
        }

        private SqlDbType getSqlDbType(string DataInfo)
        {
            SqlDbType sqltype;

            switch (DataInfo.ToLower())
            {
                case "int":
                    sqltype = SqlDbType.Int;
                    break;

                case "char":
                    sqltype = SqlDbType.Char;
                    break;

                case "varchar":
                    sqltype = SqlDbType.VarChar;
                    break;

                case "nvarchar":
                    sqltype = SqlDbType.NVarChar;
                    break;

                case "text":
                    sqltype = SqlDbType.Text;
                    break;

                case "ntext":
                    sqltype = SqlDbType.NText;
                    break;

                case "varbinary":
                    sqltype = SqlDbType.VarBinary;
                    break;

                case "nchar":
                    sqltype = SqlDbType.NChar;
                    break;

                default:
                    sqltype = SqlDbType.NVarChar;
                    break;
            }

            return sqltype;
        }
    }
}
