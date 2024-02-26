using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Timers;
using System.Threading;
using HYSignServices.Entity;
using System.Data;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using HYSignServices.ToolsDoc;
using System.Web.Script.Serialization;

namespace HYSignServices.HYService
{
    /// <summary>
    /// Data_Panel 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Data_Panel : System.Web.Services.WebService
    {
        public static System.Timers.Timer DayTimer;
        public static System.Timers.Timer WeekTimer;
        public static System.Timers.Timer MonthTimer;
        public static System.Timers.Timer YearTimer;
        public static Dictionary<string, int> DayResult = new Dictionary<string, int>();
        public static Dictionary<string, int> MonthResult = new Dictionary<string, int>();
        //当天过车总流量集合（分车牌色）
        public static List<PassCarEntity> listPassCount = new List<PassCarEntity>();

        public static List<PassCarEntity> WeeklistPassCount = new List<PassCarEntity>();
        public static List<PassCarEntity> MonthlistPassCount = new List<PassCarEntity>();
        public static List<YearPassCarEntity> YearGroupByList = new List<YearPassCarEntity>();
        public static List<DayHH24_Pass> DayGroupByList = new List<DayHH24_Pass>();
        public static List<DevOnlinePercent> unLineLsit = new List<DevOnlinePercent>();
        public static DataTable dataTable; //表与路名集合
        /// <summary>
        /// 程序启动时调用
        /// </summary>
        [WebMethod]
        public void StartResuleData()
        {
            ToolsDoc.Tools.GetCrossInfo();
            //if (ToolsDoc.Tools.Crossing_Info == null)
            //{
            //    return "success";
            //}
            //else
            //{
            //    return "fail";
            //}


            //if (DayResult.Count == 0 ||
            //    MonthResult.Count == 0 ||
            //    listPassCount.Count == 0 ||
            //    WeeklistPassCount.Count == 0 ||
            //    MonthlistPassCount.Count == 0 ||
            //    YearGroupByList.Count == 0 ||
            //    DayGroupByList.Count == 0
            //    )
            //{
            //    if (dataTable == null)
            //    {

            //        DateTime sDT = DateTime.Now;
            //        int start_year = sDT.Year;
            //        string sTableY = Tools.ReTableTitle(start_year); //取表头
            //        string sql = "select CONCAT('" + sTableY + "' ,local_id)cid,cross_name from PASS_ID_CORRESPOND";
            //        DataSet ds = OracleHelper.QueryU_Dev(sql);
            //        if (Tools.DSisNull(ds))
            //        {
            //            dataTable = ds.Tables[0];
            //        }
            //    }
            //    if (dataTable != null)
            //    {
            //        if (DayTimer == null)
            //        {

            //            DayTimer = new System.Timers.Timer();
            //        }
            //        DayTimer.Interval = 3000;
            //        DayTimer.Enabled = true;
            //        DayTimer.Elapsed += new System.Timers.ElapsedEventHandler(DayThread);
            //        DayTimer.Start();

            //        if (DayTimer == null)
            //        {
            //            WeekTimer = new System.Timers.Timer();
            //        }
            //        WeekTimer = new System.Timers.Timer();
            //        WeekTimer.Interval = 3000;
            //        WeekTimer.Enabled = true;
            //        WeekTimer.Elapsed += new System.Timers.ElapsedEventHandler(WeekThread);
            //        WeekTimer.Start();

            //        if (DayTimer == null)
            //        {
            //            MonthTimer = new System.Timers.Timer();
            //        }
            //        MonthTimer = new System.Timers.Timer();
            //        MonthTimer.Interval = 3000;
            //        MonthTimer.Enabled = true;
            //        MonthTimer.Elapsed += new System.Timers.ElapsedEventHandler(MonthThread);
            //        MonthTimer.Start();

            //        if (DayTimer == null)
            //        {
            //            YearTimer = new System.Timers.Timer();
            //        }
            //        YearTimer = new System.Timers.Timer();
            //        YearTimer.Interval = 3000;
            //        YearTimer.Enabled = true;
            //        YearTimer.Elapsed += new System.Timers.ElapsedEventHandler(YearThread);
            //        YearTimer.Start();
            //    }
            //}

        }

        #region 天
        public void DayThread(object source, ElapsedEventArgs e)
        {
            
            DayTimer.Interval = 1200000;//设置Interval为想要的间隔时间。
            
            string sTime = "2019-05-16 ";
            string eTime = "2019-05-16 " + DateTime.Now.ToString("HH:mm");
                
                
            Dictionary<string, string> sqlList = new Dictionary<string, string>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string tableName = dataTable.Rows[i][0].ToString();
                string crossName = dataTable.Rows[i][1].ToString();
                    
                string sqlCount = "select sum(blue),sum(yellow),sum(other) ,TO_CHAR (pass_time, 'hh24') HH from " + tableName + " " +
                            "where pass_time >= to_timestamp('" + sTime + "00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "and pass_time <= to_timestamp('" + eTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "GROUP BY TO_CHAR (pass_time, 'hh24') order by TO_CHAR (pass_time, 'hh24')";
                sqlList.Add(crossName, sqlCount);

            }
            Thread t = new Thread(DayThreading);
            t.IsBackground = true;
            t.Start(sqlList);

        }

        public void DayThreading(object request)
        {
            Dictionary<string, string> sqlList = request as Dictionary<string, string>;
            try
            {

                Dictionary<string, DataSet> dsPass = OracleHelper.QueryU_DevArray(sqlList);
                if (dsPass.Count > 0)
                {

                    listPassCount.Clear();
                    DayResult.Clear();
                    List<DayHH24_Pass> list = new List<DayHH24_Pass>();
                    foreach (KeyValuePair<string, DataSet> kvp in dsPass)
                    {
                        DataTable dtPass = kvp.Value.Tables[0];
                        if (dtPass.Rows.Count > 0 && !string.IsNullOrEmpty(dtPass.Rows[0][0].ToString()))
                        {
                            int blue = 0, yellow = 0, other = 0;
                            for (int i = 0; i < dtPass.Rows.Count; i++)
                            {

                                DayHH24_Pass dhp = new DayHH24_Pass();
                                blue += int.Parse(dtPass.Rows[i][0].ToString());
                                yellow += int.Parse(dtPass.Rows[i][1].ToString());
                                other += int.Parse(dtPass.Rows[i][2].ToString());

                                dhp.blue = dtPass.Rows[i][0].ToString();
                                dhp.yellow = dtPass.Rows[i][1].ToString();
                                dhp.other = dtPass.Rows[i][2].ToString();
                                dhp.hh24 = dtPass.Rows[i][3].ToString();
                                dhp.count = (int.Parse(dhp.blue) + int.Parse(dhp.yellow) + int.Parse(dhp.other)).ToString();
                                list.Add(dhp);
                                
                            }
                            PassCarEntity pce = new PassCarEntity();
                            pce.crossName = kvp.Key;
                            pce.blue = blue.ToString();
                            pce.yellow = yellow.ToString();
                            pce.other = other.ToString();
                            int count = blue + yellow + other;
                            DayResult.Add(kvp.Key, count);
                            pce.count = count.ToString();
                            listPassCount.Add(pce);
                            pce = null;
                            
                        }
                    }
                    //每日过车数据按小时分组求和
                    DayGroupByList.Clear();
                    DayGroupByList = (from a in list
                                       group a by new
                                       {
                                           a.hh24
                                       } into b
                                      orderby b.Key.hh24
                                      select new DayHH24_Pass
                                       {
                                           hh24 = b.Key.hh24,
                                           blue = (b.Sum(c => Convert.ToDecimal(c.blue))).ToString(),
                                           yellow = (b.Sum(c => Convert.ToDecimal(c.yellow))).ToString(),
                                           other = (b.Sum(c => Convert.ToDecimal(c.other))).ToString(),
                                           count = (b.Sum(c => Convert.ToDecimal(c.count))).ToString()
                                       }).ToList<DayHH24_Pass>();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("day异常 - " + ex.Message);
            }
        }
        public struct DayHH24_Pass
        {
            public string blue;
            public string yellow;
            public string other;
            public string count;
            public string hh24;
        }
        #endregion

        #region 周
        public void WeekThread(object source, ElapsedEventArgs e)
        {
            WeekTimer.Interval = 1200000;//设置Interval为想要的间隔时间。
            
            string sTime = "2019-05-09 ";
            string eTime = "2019-05-15 23:59:59";

            List<string> sqlList = new List<string>();
                
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string tableName = dataTable.Rows[i][0].ToString();
                string crossName = dataTable.Rows[i][1].ToString();
                string sqlCount = "select sum(blue),sum(yellow),sum(other) from " + tableName + "" +
                                    " where pass_time >= to_timestamp('" + sTime + "00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                                    "and pass_time <= to_timestamp('" + eTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
                sqlList.Add(sqlCount);
                    
            }
            Thread t = new Thread(WeekThreading);
            t.IsBackground = true;
            t.Start(sqlList);

        }

        public void WeekThreading(object request)
        {
            List<string> sqlList = request as List<string>;
            //string sql = request.ToString();
            try
            {
                List<DataSet> dsPass = OracleHelper.QueryLocalArray(sqlList);
                if (dsPass.Count > 0)
                {
                    WeeklistPassCount.Clear();
                    for (int i = 0; i < dsPass.Count; i++)
                    {
                        DataTable dtPass = dsPass[i].Tables[0];

                        if (!string.IsNullOrEmpty(dtPass.Rows[0][0].ToString()))
                        {
                            PassCarEntity pce = new PassCarEntity();
                            pce.blue = dtPass.Rows[0][0].ToString();
                            pce.yellow = dtPass.Rows[0][1].ToString();
                            pce.other = dtPass.Rows[0][2].ToString();
                            pce.count = (int.Parse(pce.blue) + int.Parse(pce.yellow) + int.Parse(pce.other)).ToString();
                            WeeklistPassCount.Add(pce);
                            pce = null;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("week异常 - " + ex.Message);
            }
        }
        #endregion

        #region 月
        public void MonthThread(object source, ElapsedEventArgs e)
        {
            
            MonthTimer.Interval = 1200000;//设置Interval为想要的间隔时间。
            
            string sTime = "2019-05-01 ";
            string eTime = "2019-05-31 23:59";
                
            Dictionary<string, string> sqlList = new Dictionary<string, string>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string tableName = dataTable.Rows[i][0].ToString();
                string crossName = dataTable.Rows[i][1].ToString();
                string sqlCount = "select sum(blue),sum(yellow),sum(other) from " + tableName + "" +
                                    " where pass_time >= to_timestamp('" + sTime + "00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                                    "and pass_time <= to_timestamp('" + eTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
                sqlList.Add(crossName, sqlCount);
                //tc = new ThreadingClass();
                //tc.crossName = crossName;
                //tc.sqlStr = sqlCount;

            }
            Thread t = new Thread(MonthThreading);
            t.IsBackground = true;
            t.Start(sqlList);

        }

        public void MonthThreading(object request)
        {

            Dictionary<string, string> sqlList = request as Dictionary<string, string>;
            try
            {
                Dictionary<string, DataSet> dsPass = OracleHelper.QueryU_DevArray(sqlList);
                if (dsPass.Count > 0)
                {
                    MonthResult.Clear();
                    MonthlistPassCount.Clear();
                    foreach (KeyValuePair<string, DataSet> kvp in dsPass)
                    {
                        DataTable dtPass = kvp.Value.Tables[0];
                        if (!string.IsNullOrEmpty(dtPass.Rows[0][0].ToString()))
                        {
                            PassCarEntity pce = new PassCarEntity();
                            pce.blue = dtPass.Rows[0][0].ToString();
                            pce.yellow = dtPass.Rows[0][1].ToString();
                            pce.other = dtPass.Rows[0][2].ToString();
                            pce.crossName = kvp.Key;
                            int count = int.Parse(pce.blue) + int.Parse(pce.yellow) + int.Parse(pce.other);
                            MonthResult.Add(kvp.Key, count);
                            pce.count = (int.Parse(pce.blue) + int.Parse(pce.yellow) + int.Parse(pce.other)).ToString();
                            MonthlistPassCount.Add(pce);
                            pce = null;
                        }
                    }
                    //DataTable dtPass = dsPass.Tables[0];

                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("month异常 - " + ex.Message);
            }
        }
        #endregion

        #region 年
        public void YearThread(object source, ElapsedEventArgs e)
        {

            
            YearTimer.Interval = 1200000;//设置Interval为想要的间隔时间。
             
            //string sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            //string eTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            //DateTime sDT = Convert.ToDateTime(sTime);
            //int start_year = sDT.Year;
            //string sTableY = Tools.ReTableTitle(start_year); //取表头
            //string sql = "select CONCAT('" + sTableY + "' ,local_id)cid,cross_name from PASS_ID_CORRESPOND";
            //DataSet ds = OracleHelper.QueryU_Dev(sql);

            //if (Tools.DSisNull(ds))
            //{
            //    DataTable dt = ds.Tables[0];
                List<string> sqlList = new List<string>();
                
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string tableName = dataTable.Rows[i][0].ToString();
                    string crossName = dataTable.Rows[i][1].ToString();
                    //string sqlCount = "select sum(blue),sum(yellow),sum(other) from " + tableName + "" +
                    //                    " where pass_time >= to_timestamp('" + sTime + "00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                    //                    "and pass_time <= to_timestamp('" + eTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
                    string sqlCount = "select sum(blue),sum(yellow),sum(other) ,TO_CHAR (pass_time, 'MM') MM from " + tableName + " " +
                                        "GROUP BY TO_CHAR (pass_time, 'MM') " +
                                        "order by TO_CHAR (pass_time, 'MM')";
                    sqlList.Add(sqlCount);
                    
                }
                Thread t = new Thread(YearThreading);
                t.IsBackground = true;
                t.Start(sqlList);
                //YearTimer.Stop();
                //YearTimer = null;
            //}

        }

        public void YearThreading(object request)
        {
            List<string> sqlList = request as List<string>;
            try
            {
                List<DataSet> dsPass = OracleHelper.QueryLocalArray(sqlList);
                if (dsPass.Count > 0)
                {
                    List<YearPassCarEntity> list = new List<YearPassCarEntity>();
                    for (int i = 0; i < dsPass.Count; i++)
                    {
                        DataTable dtPass = dsPass[i].Tables[0];    

                        for (int j = 0; j < dtPass.Rows.Count; j++)
                        {
                            YearPassCarEntity pce = new YearPassCarEntity();
                            pce.blue = dtPass.Rows[j][0].ToString();
                            pce.yellow = dtPass.Rows[j][1].ToString();
                            pce.other = dtPass.Rows[j][2].ToString();
                            pce.month = dtPass.Rows[j][3].ToString();
                            pce.count = (int.Parse(pce.blue) + int.Parse(pce.yellow) + int.Parse(pce.other)).ToString();
                            list.Add(pce);
                            pce = null;
                        }
                    }
                    //根据month分组求和每月各车牌数量
                    YearGroupByList.Clear();
                    YearGroupByList = (from a in list
                                        group a by new
                                        {
                                            a.month   
                                        } into b
                                        orderby b.Key.month
                                        select new YearPassCarEntity
                                        {
                                            month = b.Key.month,
                                            blue = (b.Sum(c => Convert.ToDecimal(c.blue))).ToString(),
                                            yellow = (b.Sum(c => Convert.ToDecimal(c.yellow))).ToString(),
                                            other = (b.Sum(c => Convert.ToDecimal(c.other))).ToString(),
                                            count = (b.Sum(c => Convert.ToDecimal(c.count))).ToString()
                                        }).ToList<YearPassCarEntity>();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("year异常 - " + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 查询当日车流量排名前十
        /// </summary>
        /// <param name="type">day/month</param>
        /// <returns>crossName-count</returns>
        [WebMethod]
        public List<string> OrderPassCount(string type)
        {
            
            string sTime = "";
            switch (type)
            {
                case "day":
                    sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    break;
                case "month":
                    sTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                default:
                    sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    break;
            }
            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime));
            //long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00"));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            query.query = "select crossing_id,count(1) num from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime + "  group by crossing_id order by num desc limit 10";
            string queryJson = JsonConvert.SerializeObject(query);
            string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
            RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);

            if (ToolsDoc.Tools.Crossing_Info == null)
            {
                ToolsDoc.Tools.GetCrossInfo();
            }
            if (ToolsDoc.Tools.Crossing_Info != null)
            {
                List<string> list = new List<string>();
                foreach (string[] item in retModel.rows)
                {
                    foreach (KeyValuePair<string,string> keyvalue in ToolsDoc.Tools.Crossing_Info)
                    {
                        if (keyvalue.Key == item[0])
                        {
                            string crossName = keyvalue.Value;
                            string passNum = item[1];
                            list.Add(crossName + "-" + passNum);
                        }
                    }
                }
                return list;
            }
            return null;
        }


        /// <summary>
        /// 查询过车总量
        /// </summary>
        /// <param name="type">day/week/month/year</param>
        /// <returns>过车量按车牌颜色汇总</returns>
        [WebMethod]
        public PassCarEntity GetAllCrossPass(string type)
        {
            string sTime = "";
            int multiple = 1;
            switch (type)
            {
                case "day":
                    sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    break;
                case "week":
                    sTime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "month":
                    sTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "year":
                    sTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                    multiple = 12;
                    //sTime = DateTime.Now.AddYears(-1).Year + "-01-01 00:00:00";
                    break;
                default:
                    break;
            }
            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            query.query = "select count(plate_color) num,";
            query.query += "(case plate_color ";
            query.query += "  when  '1'  then 'yellow' ";
            query.query += "  when  '2'  then 'blue' ";
            query.query += "  when  '5'  then 'green' ";
            query.query += "  else 'other' ";
            query.query += "end) color ";
            query.query += "from traffic_vehicle_pass_pic  ";
            query.query += "where pass_time >= " + startTime;
            query.query += " and pass_time <= " + endTime;
            query.query += " group by plate_color";
            //query.query = "select count(1) from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime + " ";
            string queryJson = JsonConvert.SerializeObject(query);

            string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
            RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);

            PassCarEntity pce = new PassCarEntity();
            long other = 0;
            long count = 0;
            foreach (string[] item in retModel.rows)
            {
                //如果查的是年总数，将每个月的数量*12
                long num = long.Parse(item[0]) * multiple;
                switch (item[1])
                {
                    case "blue":
                        pce.blue = (num).ToString();
                        break;
                    case "yellow":
                        pce.yellow = (num).ToString();
                        break;
                    case "green":
                        pce.green = (num).ToString();
                        break;
                    default:
                        other += num;
                        break;
                }
                count += num;
            }
            pce.other = other.ToString();
            pce.count = count.ToString();
            return pce;
        }


        /// <summary>
        /// 查询违章排名前十
        /// </summary>
        /// <returns>CrossName-Count</returns>
        [WebMethod]
        public List<AlarmEntity> OrderAlarm()
        {
            string sTime = DateTime.Now.ToString("yyyy-MM-dd");
            string eTime = sTime + " " + DateTime.Now.ToString("HH:mm:ss");
            //string sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            //string eTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string sql = "select * from ( " +
                        "select b.crossing_name,count(a.pass_time)num from TRAFFIC_VIOLATIVE_ALARM a,TRAFFIC_CROSSING_INFO b where " +
                        "a.pass_time >= to_timestamp('" + sTime + " 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                        "and a.pass_time < to_timestamp('" + eTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                        "and a.crossing_id = b.crossing_id group by b.crossing_name order by num desc ) " +
                        "where rownum < 6";
            DataSet ds = OracleHelper.Query(sql);
            if (Tools.DSisNull(ds))
            {
                List<AlarmEntity> list = new List<AlarmEntity>();
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AlarmEntity ae = new AlarmEntity();
                    ae.CrossName = dt.Rows[i][0].ToString();
                    ae.num = dt.Rows[i][1].ToString();
                    list.Add(ae);
                    ae = null;
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 查询12个月车流量
        /// </summary>
        /// <returns>从当前月份往前推12个月,返回月总数和月份</returns>
        [WebMethod]
        public List<YearPassCarEntity> GroupYearByMonth()
        {
            List<YearPassCarEntity> list = new List<YearPassCarEntity>();
            for (int i = 0; i > -12; i--)
            {
                int j = i - 1; 

                string eTime = DateTime.Now.AddMonths(i).ToString("yyyy-MM-dd HH:mm:ss");
                string sTime = DateTime.Now.AddMonths(j).ToString("yyyy-MM-dd HH:mm:ss");
                string month = DateTime.Now.AddMonths(i).Month.ToString();

                long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime));
                long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(eTime));
                HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
                query.query = "select count(1) from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime;
                string queryJson = JsonConvert.SerializeObject(query);
                try
                {
                    string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
                    RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);
                    YearPassCarEntity ypce = new YearPassCarEntity();
                    //ypce.count = retModel.rows[0][0];
                    string item = retModel.rows[0][0];
                    double yCount = double.Parse(item) / 10000;
                    ypce.count = Math.Round(yCount, 5).ToString();
                    ypce.month = month;
                    list.Add(ypce);
                }
                catch (Exception)
                {
                    
                }
                
            }
            return list;
        }
        /// <summary>
        /// 查询24小时内车流量总数
        /// </summary>
        /// <returns>每小时对应的总数</returns>
        [WebMethod]
        public List<DayHH24_Pass> GroupDayByHH24()
        {
            List<DayHH24_Pass> list = new List<DayHH24_Pass>();
            for (int i = 0; i > -24; i--)
            {
                int j = i - 1;

                string eTime = DateTime.Now.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss");
                string sTime = DateTime.Now.AddHours(j).ToString("yyyy-MM-dd HH:mm:ss");
                string hour = DateTime.Now.AddHours(i).Hour.ToString();

                long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime));
                long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(eTime));

                HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();

                query.query = "select count(1) from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime;
                string queryJson = JsonConvert.SerializeObject(query);
                try
                {
                    string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
                    RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);
                    DayHH24_Pass dhp = new DayHH24_Pass();
                    dhp.count = retModel.rows[0][0];
                    dhp.hh24 = hour;
                    list.Add(dhp);
                }
                catch (Exception ex)
                {
                    
                }
                
            }
            return list;
        }
        /// <summary>
        /// 返回不在线设备列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<DevOnlinePercent> GetUnlineDev()
        {
            if (unLineLsit.Count > 0)
            {
                return unLineLsit;
            }
            return null;
        }
        /// <summary>
        /// 计算设备在线率
        /// </summary>
        /// <returns>Ramp-卡口，Electric-电警，Monitor-球机</returns>
        [WebMethod]
        public DevOnlineClass GetDevOnlinePercent()
        {
            //1-电警  2-违停 3-监控  10-卡口
            string[] typeArray = new string[3] { "2,6","1","10"};
            DevOnlineClass online = new DevOnlineClass();
            foreach (string type in typeArray)
            {
                string url = "http://192.168.96.210:8081/api/Device/GetDevNetSummary?devStatus=1&devOnlineState=-1&devTypeArrStr=" + type;
                string postModel = HttpHelper.HttpGet(url);
                JavaScriptSerializer Jss = new JavaScriptSerializer();
                Dictionary<string, object> DicText = (Dictionary<string, object>)Jss.DeserializeObject(postModel);
                if (DicText.ContainsKey("status"))
                {
                    string status = DicText["status"].ToString();
                    if (status == "200")
                    {
                        Dictionary<string, object> value = (Dictionary<string, object>)DicText["response"];
                        if (value == null)
                            continue;

                        string OnLineRate = "";
                        string OnLineCount = "";
                        string DevCount = "";
                        
                        foreach (KeyValuePair<string,object> item in value)
                        {
                            switch (item.Key)
                            {
                                case "OnLineRate"://在线率
                                    OnLineRate = item.Value.ToString();
                                    break;
                                case "OnLineCount"://在线数量
                                    OnLineCount = item.Value.ToString();
                                    break;
                                case "DevCount"://总数
                                    DevCount = item.Value.ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                        ////在线率
                        //string OnLineRate = DicText["OnLineRate"].ToString();
                        ////在线数量
                        //string OnLineCount = DicText["OnLineCount"].ToString();
                        ////不在线数量
                        //string OffLineCount = DicText["OffLineCount"].ToString();
                        ////总数
                        //string DevCount = DicText["DevCount"].ToString();
                        ////检测时间
                        //string CheckDate = DicText["CheckDate"].ToString();

                        switch (type)
                        {
                            case "2,6":
                                HYSignServices.Entity.Monitor monitor = new HYSignServices.Entity.Monitor();
                                monitor.online = OnLineCount;
                                monitor.count = DevCount;
                                monitor.percent = OnLineRate;
                                online.monitor = monitor;
                                break;
                            case "1":
                                Electric electric = new Electric();
                                electric.online = OnLineCount;
                                electric.count = DevCount;
                                electric.percent = OnLineRate;
                                online.electric = electric;
                                break;
                            case "10":
                                Ramp ramp = new Ramp();
                                ramp.online = OnLineCount;
                                ramp.count = DevCount;
                                ramp.percent = OnLineRate;
                                online.ramp = ramp;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return online;
            
        }

        private void ExcutePingArray(List<DevOnlinePercent> list)
        {
            tagArray = new List<string>();
            //Tools.WriteErrLog("执行PING命令:" + list.Count);
            foreach (DevOnlinePercent dev in list)
            {
                Tools.WriteErrLog("执行PING命令:" + dev.ip);
                Thread thread = new Thread(PingHost);
                thread.IsBackground = true;
                thread.Start(dev.ip + "," + dev.tag);
            }
        }
        //存放ping不通的IP的tag
        private List<string> tagArray;
        /// <summary>
        /// ping每个地址，将ping不通的地址tag存进tagArray中
        /// </summary>
        /// <param name="ip"></param>
        private void PingHost(object iptag)
        {
            try
            {
                string obj = iptag.ToString();
                string[] objStr = obj.Split(',');
                string device_ip = objStr[0];
                string tag = objStr[1];
                if (device_ip.Contains("192.168.32"))
                {
                    return;
                }
                
                Ping ping = new Ping();
                PingReply pingreply = ping.Send(device_ip);
                if (pingreply.Status != IPStatus.Success)
                {
                    tagArray.Add(tag);
                }
                
            }
            catch (Exception ex)
            {

            }
        }

    }
}
