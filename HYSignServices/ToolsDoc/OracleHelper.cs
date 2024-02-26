using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

using System.Data.OracleClient;
using System.Text;
using HYSignServices.HYService;
using Newtonsoft.Json;

//using Oracle.DataAccess.Client;

/// <summary>
/// OracleHelper 的摘要说明
/// </summary>

    public class OracleHelper
    {
        public static readonly string ConnectionStringHIKTransaction = ConfigurationManager.AppSettings["LOCAL_Conn"];
        public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["LOCAL_Conn_212"];
        public static readonly string ConnectionStringUTCSTransaction = ConfigurationManager.AppSettings["UTCS_Conn"];
        public static readonly string ConnectionStringLocalTransaction212 = ConfigurationManager.AppSettings["LOCAL_Conn_212"];
        public static readonly string ConnectionStringHIKTransaction0249 = ConfigurationManager.AppSettings["HIK_Conn0249"];
        public static string localSeq = "";
        public OracleHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //

        }
        /// <summary>
        /// 无锡所查询命令
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static DataSet Query_utcs(string SQLString)
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringUTCSTransaction))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }
        public static DataSet Query(string SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringHIKTransaction0249))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }

        public static DataSet Query212(string SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction212))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }

        /*
         * 非查询数据库操作
         * SQLString 操作语句
         * 返回值success：执行成功
         * fail或错误信息：执行失败
         */
        public static DataSet Query0249(string SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringHIKTransaction0249))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }
        /// <summary>
        /// 249
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static String ExecuteU_Dev(String SQLString)
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleCommand comm = new System.Data.OracleClient.OracleCommand(SQLString, connection);
                    int res = comm.ExecuteNonQuery();
                    if (res > 0)
                    {
                        return "success";
                    }
                    return "fail";
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 212
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static int ExecuteArray212(List<string> SQLString)
        {
            int resCount = 0;
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction212))
            {
                try
                {
                    connection.Open();
                    foreach (string sql in SQLString)
                    {
                        System.Data.OracleClient.OracleCommand comm = new System.Data.OracleClient.OracleCommand(sql, connection);
                        int res = comm.ExecuteNonQuery();
                        resCount += res;
                    }
                    return resCount;
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        public static String ExecuteU_Dev_212(String SQLString)
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction212))
            {
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleCommand comm = new System.Data.OracleClient.OracleCommand(SQLString, connection);
                    int res = comm.ExecuteNonQuery();
                    if (res > 0)
                    {
                        return "success";
                    }
                    return "fail";
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 249
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static int ExecuteArray(List<string> SQLString)
        {
            int resCount = 0;
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                try
                {
                    connection.Open();
                    foreach (string sql in SQLString)
                    {
                        System.Data.OracleClient.OracleCommand comm = new System.Data.OracleClient.OracleCommand(sql, connection);
                        int res = comm.ExecuteNonQuery();
                        resCount += res;
                    }
                    return resCount;
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 249
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static DataSet QueryU_Dev(string SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }

        public static List<DataSet> QueryLocalArray(List<string> listSelectSql) 
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                List<DataSet> list = new List<DataSet>();
                string sqltemp = "";
                try
                {
                    
                    connection.Open();
                    foreach (string sql in listSelectSql)
                    {
                        sqltemp = sql;
                        OracleDataAdapter command = new OracleDataAdapter(sql, connection);
                        DataSet ds = new DataSet();
                        command.Fill(ds, "ds");
                        if (Tools.DSisNull(ds))
                        {
                            list.Add(ds);
                        }
                        sqltemp = "";
                    }
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message + sqltemp);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return list;
            }
        }

        public static Dictionary<string, DataSet> QueryU_DevArray(Dictionary<string, string> SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                DataSet ds;
                Dictionary<string, DataSet> dsList = new Dictionary<string, DataSet>();
                string sql = "";
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
                    //StringBuilder strSql = new StringBuilder();
                    //strSql.Append("begin ");
                    foreach (KeyValuePair<string, string> kvp in SQLString)
                    {
                        sql = kvp.Value;
                        ds = new DataSet();
                        System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(kvp.Value, connection);
                        command.Fill(ds, "ds");
                        dsList.Add(kvp.Key,ds);
                        ds = null;
                        sql = "";
                    }
                    
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    throw new Exception(ex.Message + sql);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return dsList;
            }
        }

        public static String ExecuteU_Tran(String SQLString)
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringLocalTransaction))
            {
                connection.Open();
                OracleTransaction transaction;
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand(SQLString, connection);
                    command.Transaction = transaction;
                    int res = command.ExecuteNonQuery();
                    transaction.Commit();
                    if (res > 0)
                    {
                        return "success";
                    }
                    return "fail";
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        public static string SetReportCtrl(string id, string planNo)
        {
            localSeq = GetSeq();

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<Message>";
            xml += "<Version>1.1</Version>";
            xml += "<Token>hy_Token</Token>";
            xml += "<From>";
            xml += "<Address><Sys>TICP</Sys>";
            xml += "<SubSys/>";
            xml += "<Instance/>";
            xml += "</Address>";
            xml += "</From>";
            xml += "<To><Address>";
            xml += "<Sys>UTCS</Sys><SubSys/><Instance/>";
            xml += "</Address></To>";
            xml += "<Type>REQUEST</Type>";
            xml += "<Seq>" + localSeq + "</Seq>";
            xml += "<Body><Operation order=\"1\" name=\"Set\">";
            xml += "<CrossPlan>";
            xml += "<CrossID>" + id + "</CrossID>";
            xml += "<ControlMode>53</ControlMode>";
            xml += "<PlanNo>" + planNo + "</PlanNo>";
            xml += "</CrossPlan>";
            xml += "</Operation></Body></Message>";
            return xml;
        }
        public static string GetSeq()
        {
            DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string seq = currentTime.ToString("yyyyMMddHHmmss") + "000001";
            return seq;
        }

        /// <summary>
        /// ES查询块
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public static HYSignServices.ToolsDoc.RetModel ESQuery(string sqlStr)
        {
            HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            query.query = sqlStr;
            string queryJson = JsonConvert.SerializeObject(query);
            string postModel = HYSignServices.ToolsDoc.HttpHelper.HttpPost(HYSignServices.ToolsDoc.Tools.ESUrlStr, queryJson);
            HYSignServices.ToolsDoc.RetModel retModel = JsonConvert.DeserializeObject<HYSignServices.ToolsDoc.RetModel>(postModel);
            return retModel;
        }


        public static DataSet Query_local249(string SQLString)
        {

            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringHIKTransaction))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleDataAdapter command = new System.Data.OracleClient.OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return ds;
            }
        }

        public static String Execute_local249(String SQLString)
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(ConnectionStringHIKTransaction))
            {
                try
                {
                    connection.Open();
                    System.Data.OracleClient.OracleCommand comm = new System.Data.OracleClient.OracleCommand(SQLString, connection);
                    int res = comm.ExecuteNonQuery();
                    if (res > 0)
                    {
                        return "success";
                    }
                    return "fail";
                }
                catch (System.Data.OracleClient.OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
    }
