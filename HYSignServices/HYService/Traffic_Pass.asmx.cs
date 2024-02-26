using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using HYSignServices.Entity;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using HYSignServices.ToolsDoc;

namespace HYSignServices.HYService
{
    /// <summary>
    /// Traffic_Pass 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class Traffic_Pass : System.Web.Services.WebService
    {
        /// <summary>
        /// 自定义查询参数
        /// </summary>
        /// <param name="type">1.按天计算  2.按小时计算  3.按分钟计算(查询前一分钟)</param>
        /// <param name="range">时间范围(type为1和2时生效)</param>
        /// <param name="crossidlist">路口编号，多个用','隔开</param>
        /// <returns></returns>
        /// 示例:http://192.168.0.236:8011/HYService/Traffic_Pass.asmx/GetVehicleNumByCustom?type=1&range=3&crossidlist=100315,100342
        [WebMethod]
        public string GetVehicleNumByCustom(string type, string range, string crossidlist)
        {
            string res = Traffic_Pass_Tools.GetVehicleNumByCustom_tool(type, range, crossidlist);
            return res;
            
        }
        /// <summary>
        /// 获取前一个小时每分钟的车流量
        /// </summary>
        /// <param name="crossidlist">路口编号，多个用','隔开，为空时查询所有路口</param>
        /// <returns></returns>
        /// 示例：http://192.168.0.236:8011/HYService/Traffic_Pass.asmx/GetCrossVehicleNumPerMi?crossidlist=100315,100342
        [WebMethod]
        public string GetCrossVehicleNumPerMi(string crossidlist)
        {
            string res = Traffic_Pass_Tools.GetCrossVehicleNumPerMi_tool(crossidlist);
            return res;
            
        }
        /// <summary>
        /// 获取单个路口每个方向的车流量
        /// </summary>
        /// <param name="range">时间范围-0：24小时，1：1小时，2：1分钟</param>
        /// <param name="crossidlist">字符串格式的路口编号，多个编号用英文半角‘,’隔开</param>
        /// <returns>路口指定时间段内车流量</returns>
        [WebMethod]
        public List<CorssVehiclePass> GetCrossVehicleNumByTime(string range, string crossidlist)
        {
            if (!string.IsNullOrEmpty(range) && !string.IsNullOrEmpty(crossidlist))
            {
                string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string endTime = "";
                switch (range)
                {
                    case "0":
                        endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case "1":
                        endTime = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case "2":
                        endTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    default:
                        break;
                }
                string crolist = " and 1=1";
                if (!string.IsNullOrEmpty(crossidlist)) crolist = " and crossing_id in (" + crossidlist + ")";
                string sql = "select count(*),crossing_id from TRAFFIC_VEHICLE_PASS where pass_time >= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                             " and pass_time <= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" + crolist + " group by crossing_id";
                DataSet ds = OracleHelper.Query(sql);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    List<CorssVehiclePass> list = new List<CorssVehiclePass>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CorssVehiclePass c = new CorssVehiclePass();
                        c.cross_id = dt.Rows[i][1].ToString();
                        c.num = dt.Rows[i][0].ToString();
                        list.Add(c);
                    }
                    return list;
                    
                }
            }
            
            return null;
        }

        public struct CorssVehiclePass
        {
            public string cross_id;
            public string num;
        }
        /// <summary>
        /// 车流数据
        /// XML
        /// </summary>
        [WebMethod]
        public List<Traffic_Entity> GetCrossCarPermm(string startTime, string endTime, string crossid)
        {

            List<Traffic_Entity> list = new List<Traffic_Entity>();
            string sql = "select (select crossing_id from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id)crossing_id," +
                        "(select lane_id from TRAFFIC_LANE_INFO where lane_no = b.lane_no and crossing_id = a.crossing_id)lane_no," +
                        "count(a.pass_time)numb  from TRAFFIC_CROSSING_INFO c,TRAFFIC_VEHICLE_PASS a,TRAFFIC_LANE_INFO b " +
                        "where a.crossing_id = c.crossing_id and b.lane_no = a.lane_no " +
                        "and b.crossing_id = a.crossing_id and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') and c.crossing_id in (" + crossid + ") " +
                        "group by b.lane_no,a.crossing_id order by a.crossing_id";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                Traffic_Entity traffic;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    traffic = new Traffic_Entity();
                    traffic.crossing_id = dt.Rows[i][0].ToString();
                    traffic.lane_no = dt.Rows[i][1].ToString();
                    traffic.numb = dt.Rows[i][2].ToString();
                    list.Add(traffic);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 目标车辆跟踪
        /// XML
        /// </summary>http://192.168.0.236:8011/HYService/Traffic_Pass.asmx/TargetCarSearch?plate_no=鄂A3Q9B0&startTime=2018-05-07 15:00&endTime=2018-05-07 16:00
        [WebMethod]
        public List<TargetCarInfo> TargetCarSearch(string plate_no, string startTime, string endTime)
        {
            string sql = "select (select lane_id from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)lane_no," +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)lane_name, " +
                        "crossing_id,plate_no,pass_time from TRAFFIC_VEHICLE_PASS a " +
                        "where plate_no = '" + plate_no + "' and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') order by pass_time";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                List<TargetCarInfo> list = new List<TargetCarInfo>();
                DataTable dt = ds.Tables[0];
                TargetCarInfo targetCar;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    targetCar = new TargetCarInfo();
                    string str = "";
                    //最后一个出现的路口，行车方向赋值直行
                    if (dt.Rows.Count == (i + 1))
                    {
                        str = "forward";
                    }
                    else
                    {
                        //计算行车方向
                        str = Tools.NextDirection(dt.Rows[i][1].ToString().Substring(0, 4), dt.Rows[i + 1][1].ToString().Substring(0, 4));
                    }

                    targetCar.lane_no = dt.Rows[i][0].ToString();

                    targetCar.direction = str;
                    targetCar.crossing_id = dt.Rows[i][2].ToString();
                    targetCar.palte_no = dt.Rows[i][3].ToString();
                    targetCar.pass_time = dt.Rows[i][4].ToString();
                    list.Add(targetCar);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 配时数据
        /// </summary>
        [WebMethod]
        public List<Timing_Entity> GetTimingProgramme(string id)
        {

            /*******************计算当前时间区间***********************/
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];

            string overTime = "";
            string mytime = DateTime.Now.ToString("HH:mm");
            string[] timeArray;
            List<string> time = new List<string>();
            string overSql = "select periodtime from WEEKPLAN where week like '%" + week + "%' and projectid in(" + id + ")";
            DataSet overDS = OracleHelper.Query(overSql);
            if (overDS.Tables[0] != null && overDS.Tables[0].Rows.Count > 0)
            {
                string tempTime = "";
                DataTable overDT = overDS.Tables[0];
                for (int i = 0; i < overDT.Rows.Count; i++)
                {
                    tempTime += overDT.Rows[i][0].ToString() + ",";
                }
                timeArray = tempTime.TrimEnd(',').Split(',');

                for (int i = 0; i < timeArray.Length; i++)
                {
                    string oTime = (timeArray[i].Split('-'))[1];
                    if (oTime == "24:00")
                    {
                        oTime = "23:59";
                    }
                    TimeSpan startTime = DateTime.Parse((timeArray[i].Split('-'))[0]).TimeOfDay;
                    TimeSpan endTime = DateTime.Parse(oTime).TimeOfDay;
                    TimeSpan now = (Convert.ToDateTime(mytime)).TimeOfDay;
                    if (now > startTime && now < endTime)
                    {
                        overTime += timeArray[i] + ",";
                    }
                }
            }
            /*******************计算当前时间区间结束***********************/

            //拼接时间区间查询条件
            List<Timing_Entity> listTiming = new List<Timing_Entity>();
            List<Programme_Entity> listProgramme;
            string[] funcTime = overTime.TrimEnd(',').Split(',');
            string timeSql = "";
            for (int i = 0; i < funcTime.Length; i++)
            {
                if (i == 0)
                {
                    timeSql = " and (b.periodtime like '%" + funcTime[i] + "%')";
                }
                else
                {
                    timeSql = timeSql.TrimEnd(')') + " or b.periodtime like '%" + funcTime[i] + "%')";
                }

            }
            //and b.periodtime like '%" + overTime + "%'
            string sql = "select a.projectid,b.week,b.periodtime, c.timingid, c.green1,c.YELLOW1,c.FULLRED1,c.PHASENAME1,c.green2,c.YELLOW2,c.FULLRED2,c.PHASENAME2,c.green3,c.YELLOW3,c.FULLRED3,c.PHASENAME3,c.green4,c.YELLOW4,c.FULLRED4,c.PHASENAME4,c.green5,c.YELLOW5,c.FULLRED5,c.PHASENAME5 from weekplan b,TIMINGPLAN c,project a " +
                        "where a.projectid = b.projectid and b.timingid = c.timingid and a.projectid in(" + id + ") and b.week like '%" + week + "%'" + timeSql;
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                Programme_Entity programme;
                Timing_Entity timing = new Timing_Entity();
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    timing = new Timing_Entity();
                    listProgramme = new List<Programme_Entity>();

                    timing.timing_id = dt.Rows[i][3].ToString();
                    timing.projectName = dt.Rows[i][0].ToString();
                    //timing.week = dt.Rows[i][1].ToString();
                    //timing.periodTime = dt.Rows[i][2].ToString();

                    if (!string.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                    {
                        programme = new Programme_Entity();
                        programme.green = dt.Rows[i][4].ToString();
                        programme.yellow = dt.Rows[i][5].ToString();
                        programme.red = dt.Rows[i][6].ToString();
                        string[] str = dt.Rows[i][7].ToString().Split(',');
                        string phase = str[0];

                        string peopleControl = str.Length == 2 ? str[1] : "";
                        programme.phase = Tools.RePhase(phase) + peopleControl;
                        listProgramme.Add(programme);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i][11].ToString()))
                    {
                        programme = new Programme_Entity();
                        programme.green = dt.Rows[i][8].ToString();
                        programme.yellow = dt.Rows[i][9].ToString();
                        programme.red = dt.Rows[i][10].ToString();
                        string[] str = dt.Rows[i][11].ToString().Split(',');
                        string phase = str[0];
                        string peopleControl = str.Length == 2 ? str[1] : "";
                        programme.phase = Tools.RePhase(phase) + peopleControl;
                        listProgramme.Add(programme);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                    {
                        programme = new Programme_Entity();
                        programme.green = dt.Rows[i][12].ToString();
                        programme.yellow = dt.Rows[i][13].ToString();
                        programme.red = dt.Rows[i][14].ToString();
                        string[] str = dt.Rows[i][15].ToString().Split(',');
                        string phase = str[0];
                        string peopleControl = str.Length == 2 ? str[1] : "";
                        programme.phase = Tools.RePhase(phase) + peopleControl;
                        listProgramme.Add(programme);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i][19].ToString()))
                    {
                        programme = new Programme_Entity();
                        programme.green = dt.Rows[i][16].ToString();
                        programme.yellow = dt.Rows[i][17].ToString();
                        programme.red = dt.Rows[i][18].ToString();
                        string[] str = dt.Rows[i][19].ToString().Split(',');
                        string phase = str[0];
                        string peopleControl = str.Length == 2 ? str[1] : "";
                        programme.phase = Tools.RePhase(phase) + peopleControl;
                        listProgramme.Add(programme);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i][23].ToString()))
                    {
                        programme = new Programme_Entity();
                        programme.green = dt.Rows[i][20].ToString();
                        programme.yellow = dt.Rows[i][21].ToString();
                        programme.red = dt.Rows[i][22].ToString();
                        string[] str = dt.Rows[i][23].ToString().Split(',');
                        string phase = str[0];
                        string peopleControl = str.Length == 2 ? str[1] : "";
                        programme.phase = Tools.RePhase(phase) + peopleControl;
                        listProgramme.Add(programme);
                    }
                    timing.programme = listProgramme;
                    listTiming.Add(timing);
                }

                return listTiming;
            }
            return null;
        }
        /// <summary>
        /// 路段车流量数据 http://192.168.0.236:8008/HYService/Traffic_Pass.asmx/GetRoadCarsNumb?startTime=2018-05-07 15:00&endTime=2018-05-07 16:00&crossA=100315&crossB=100342&directionA=e_w&directionB=w_e
        /// </summary>
        [WebMethod]
        public List<RoadCars> GetRoadCarsNumb(string startTime, string endTime, string crossA, string crossB, string directionA, string directionB)
        {
            List<RoadCars> list = new List<RoadCars>();
            RoadCars roadCar;
            string sql = "select (select crossing_id from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id)crossing_id," +
                        "(select lane_id from TRAFFIC_LANE_INFO where lane_no = b.lane_no and crossing_id = a.crossing_id)lane_no," +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = b.lane_no and crossing_id = a.crossing_id)lane_name," +
                        "count(a.pass_time)numb  " +
                        "from TRAFFIC_CROSSING_INFO c,TRAFFIC_VEHICLE_PASS a,TRAFFIC_LANE_INFO b " +
                        "where a.crossing_id = c.crossing_id and b.lane_no = a.lane_no and b.crossing_id = a.crossing_id " +
                        "and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and ((c.crossing_id = " + crossA + " and lane_name like '%" + Tools.ReDirection(directionA) + "%')" +
                        "or (c.crossing_id = " + crossB + " and lane_name like '%" + Tools.ReDirection(directionB) + "%'))" +
                        "group by b.lane_no,a.crossing_id order by a.crossing_id";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    roadCar = new RoadCars();
                    roadCar.crossing_id = dt.Rows[i][0].ToString();
                    roadCar.lane_no = dt.Rows[i][1].ToString();
                    roadCar.lane_name = dt.Rows[i][2].ToString();
                    roadCar.numb = dt.Rows[i][3].ToString();
                    list.Add(roadCar);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 查询过车数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="corssName">路口名</param>
        /// <param name="rn">分页码</param>
        /// <returns></returns>
        [WebMethod]
        public List<Search_Traffic_Entity> ResearchTrafficPassData(string startTime, string endTime, string crossName, string rn)
        {
            if(string.IsNullOrEmpty(startTime) ||
               string.IsNullOrEmpty(endTime) ||
               string.IsNullOrEmpty(crossName) ||
               string.IsNullOrEmpty(rn))
            {
                return null;
            }
            int rownum = int.Parse(rn);
            string sql = "select * from( " +
                        "select " +
                        "plate_no, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = '1003' and a.plate_color = sysdict_code) plate_color, " +
                        "to_char(pass_time,'yyyy-mm-dd hh24:mi:ss') passtime, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = '1002' and a.vehicle_type = sysdict_code) vehicle_type, " +
                        "(select crossing_name from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id) corss_name," +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)pass_direction, " +
                        "rownum rn " +
                        "from TRAFFIC_VEHICLE_PASS a where crossing_id in (select crossing_id from TRAFFIC_CROSSING_INFO where crossing_name like '%" + crossName + "%')  " +
                        "and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')  " +
                        "and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and rownum <= " + Tools.ROW_LIMIT * rownum + " order by a.pass_time desc) " +
                        "where rn > " + (Tools.ROW_LIMIT * rownum - Tools.ROW_LIMIT) + "";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                List<Search_Traffic_Entity> list = new List<Search_Traffic_Entity>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Search_Traffic_Entity item = new Search_Traffic_Entity();
                    item.plate_no = dt.Rows[i][0].ToString();
                    item.plate_color = dt.Rows[i][1].ToString();
                    item.passtime = dt.Rows[i][2].ToString();
                    item.pass_direction = dt.Rows[i][5].ToString();
                    item.vehicle_type = dt.Rows[i][3].ToString();
                    item.pass_cross = dt.Rows[i][4].ToString();
                    list.Add(item);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 查询当量过车总量
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="corssName">为空时查询所有路口过车总和</param>
        /// <returns></returns>
        [WebMethod]
        public string GetDataCount(string startTime, string endTime, string crossName)
        {
            if (string.IsNullOrEmpty(startTime) ||
               string.IsNullOrEmpty(endTime))
            {
                return null;
            }

            string sql = "select count(1) from TRAFFIC_VEHICLE_PASS where " +
                        "pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')  " +
                        "and pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            if (!string.IsNullOrEmpty(crossName))
            {
                sql += "and crossing_id in (select crossing_id from TRAFFIC_CROSSING_INFO where crossing_name like '%" + crossName + "%')";
            }
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                return dt.Rows[0][0].ToString();
            }
            return null;
        }

        /// <summary>
        /// 查询车牌
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="plate_no">车牌号</param>
        /// <param name="rn">分页码</param>
        /// <returns></returns>
        [WebMethod]
        public List<Search_Traffic_Entity> SearchPassDataByPlate(string startTime, string endTime, string plate_no, string rn)
        {
            //string wherePassTime = "and 1=1 ";
            if (string.IsNullOrEmpty(plate_no) ||
                string.IsNullOrEmpty(rn) ||
                string.IsNullOrEmpty(startTime) ||
                string.IsNullOrEmpty(endTime))
            {
                return null;
            }
            
            int rownum = int.Parse(rn);
            string sql = "select * from( " +
                        "select " +
                        "plate_no, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = '1003' and a.plate_color = sysdict_code) plate_color, " +
                        "to_char(pass_time,'yyyy-mm-dd hh24:mi:ss') passtime, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = '1002' and a.vehicle_type = sysdict_code) vehicle_type, " +
                        "(select crossing_name from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id) corss_name," +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)pass_direction, " +
                        "rownum rn " +  
                        "from TRAFFIC_VEHICLE_PASS a where a.plate_no = '" + plate_no + "'  " +
                        "and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')  " +
                        "and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and rownum <= " + Tools.ROW_LIMIT * rownum + "  order by a.pass_time desc) " +
                        "where rn > " + (Tools.ROW_LIMIT * rownum - Tools.ROW_LIMIT) + "";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                List<Search_Traffic_Entity> list = new List<Search_Traffic_Entity>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Search_Traffic_Entity item = new Search_Traffic_Entity();
                    item.plate_no = dt.Rows[i][0].ToString();
                    item.plate_color = dt.Rows[i][1].ToString();
                    item.passtime = dt.Rows[i][2].ToString();
                    item.pass_direction = dt.Rows[i][5].ToString();
                    item.vehicle_type = dt.Rows[i][3].ToString();
                    item.pass_cross = dt.Rows[i][4].ToString();
                    list.Add(item);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 查询车牌
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="plate_no">车牌号</param>
        /// <param name="rn">分页码</param>
        /// <returns></returns>
        [WebMethod]
        public string GetPlateCount(string startTime, string endTime, string plate_no)
        {
            //string wherePassTime = "and 1=1 ";
            if (string.IsNullOrEmpty(plate_no) ||
                string.IsNullOrEmpty(startTime) ||
                string.IsNullOrEmpty(endTime))
            {
                return null;
            }
            string sql = "select count(1) from TRAFFIC_VEHICLE_PASS where " +
                        "pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')  " +
                        "and pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and plate_no = '" + plate_no + "'";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                
                return dt.Rows[0][0].ToString();
            }
            return null;
        }
        /// <summary>
        /// 查路口名
        /// </summary>
        /// <param name="CN">路口名关键字</param>
        /// <returns>Crossing_Id,Crossing_Name</returns>
        [WebMethod]
        public List<HYSignServices.Entity.Crossing_Info> ResearchCrossName(string CN)
        {
            if (string.IsNullOrEmpty(CN))
            {
                return null;
            }
            string sql = "select crossing_id,crossing_name from TRAFFIC_CROSSING_INFO where crossing_name like '%" + CN + "%'";
            DataSet ds = OracleHelper.Query0249(sql);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0];
                List<HYSignServices.Entity.Crossing_Info> list = new List<HYSignServices.Entity.Crossing_Info>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HYSignServices.Entity.Crossing_Info cross = new HYSignServices.Entity.Crossing_Info();
                    cross.Crossing_Id = int.Parse(dt.Rows[i][0].ToString());
                    cross.Crossing_Name = dt.Rows[i][1].ToString();
                    list.Add(cross);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 查询车流数据，按路口分蓝黄绿其他牌数量总和
        /// </summary>
        /// <param name="CN">路口ID,多个ID用","隔开（需要先调用ResearchCrossName接口获取路口ID）</param>
        /// <param name="sTime">开始时间 yyyy-MM-dd HH:mm</param>
        /// <param name="eTime">结束时间 yyyy-MM-dd HH:mm</param>
        /// <returns>以路口名为单位，每个路口下各车牌总和</returns>
        [WebMethod]
        public List<PassCarEntity> ResearchCrossPassCount(string CN, string sTime, string eTime)
        {
            //sTime = "2019-01-01 00:00";
            //eTime = "2019-01-01 23:59";


            if (string.IsNullOrEmpty(CN))
            {
                return null;
            }
            //string eTime = DateTime.Now.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss");
            //string sTime = DateTime.Now.AddHours(j).ToString("yyyy-MM-dd HH:mm:ss");
            //string hour = DateTime.Now.AddHours(i).Hour.ToString();

            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime + ":00"));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(eTime + ":00"));
            HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            query.query = "select crossing_id, count(plate_color) num,";
            query.query += "(case plate_color ";
            query.query += "  when  '1'  then 'yellow' ";
            query.query += "  when  '2'  then 'blue' ";
            query.query += "  when  '5'  then 'green' ";
            query.query += "  else 'other' ";
            query.query += "end) color ";
            query.query += "from traffic_vehicle_pass_pic  ";
            query.query += "where pass_time >= " + startTime;
            query.query += " and pass_time <= " + endTime;
            query.query += " and crossing_id in("+CN+")";
            query.query += " group by crossing_id,plate_color";
            string queryJson = JsonConvert.SerializeObject(query);
            string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
            RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);
            string[] crossid = CN.Split(',');
            if (ToolsDoc.Tools.Crossing_Info == null)
            {
                ToolsDoc.Tools.GetCrossInfo();
            }
            if (ToolsDoc.Tools.Crossing_Info != null)
            {
                List<PassCarEntity> list = new List<PassCarEntity>();
                foreach (string ids in crossid)
                {
                    string crossName = "";
                    if (ToolsDoc.Tools.Crossing_Info[ids] != null)
                    {
                        //取路口名
                        crossName = ToolsDoc.Tools.Crossing_Info[ids];
                        PassCarEntity pce = new PassCarEntity();
                        pce.crossName = crossName;
                        pce.blue = "0";
                        pce.yellow = "0";
                        pce.green = "0";

                        long other = 0;
                        long count = 0;
                        foreach (string[] item in retModel.rows)
                        {
                            if (item[0] == ids)
                            {
                                long num = long.Parse(item[1]);
                                switch (item[2])
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
                        }
                        pce.other = other.ToString();
                        pce.count = count.ToString();
                        list.Add(pce);
                    }
                }
                return list;
            }
            return null;

        }
        

        /// <summary>
        /// 查询所有路口当天即时过车流量 http://192.168.0.236:8011/HYService/Traffic_Pass.asmx/ResearchAllCrossPassCount
        /// </summary>
        /// <returns>每个路口当天过车总量</returns>
        [WebMethod]
        public List<PassCarEntity> ResearchAllCrossPassCount()
        {
            string sTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            //string eTime = "2022-05-06 23:59:59";
            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(sTime));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));
            //string sql = "select crossing_id,count(1) num from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime + "  group by crossing_id ";
            string sql = "select pass_id from traffic_vehicle_pass_pic order by pass_id desc";
            //HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            //query.query = "select crossing_id,count(1) num from traffic_vehicle_pass_pic where pass_time >= " + startTime + " and pass_time <= " + endTime + "  group by crossing_id ";
            //string queryJson = JsonConvert.SerializeObject(query);
            //string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
            //RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);
            RetModel retModel = OracleHelper.ESQuery(sql);
            //匹配路口名
            if (ToolsDoc.Tools.Crossing_Info == null)
            {
                ToolsDoc.Tools.GetCrossInfo();
            }
            if (ToolsDoc.Tools.Crossing_Info != null)
            {
                List<PassCarEntity> list = new List<PassCarEntity>();
                foreach (string[] item in retModel.rows)
                {
                    foreach (KeyValuePair<string, string> keyvalue in ToolsDoc.Tools.Crossing_Info)
                    {
                        if (keyvalue.Key == item[0])
                        {
                            PassCarEntity pce = new PassCarEntity();
                            pce.crossName = keyvalue.Value;
                            pce.count = item[1];
                            list.Add(pce);
                        }
                    }
                }
                return list;
            }
            return null;
            
        }

        public PassCarEntity AddPass(string sqlStr, string crossName)
        {
            try
            {
                DataSet dsPass = OracleHelper.QueryU_Dev(sqlStr);
                if (Tools.DSisNull(dsPass))
                {
                    PassCarEntity pass;
                    DataTable dtPass = dsPass.Tables[0];
                    if (!string.IsNullOrEmpty(dtPass.Rows[0][0].ToString()))
                    {
                        pass = new PassCarEntity();
                        pass.crossName = crossName;
                        pass.blue = dtPass.Rows[0][0].ToString();
                        pass.yellow = dtPass.Rows[0][1].ToString();
                        pass.other = dtPass.Rows[0][2].ToString();
                        pass.count = (int.Parse(pass.blue) + int.Parse(pass.yellow) + int.Parse(pass.other)).ToString();
                        return pass;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                return null;
            }
        }

        
        /// <summary>
        /// 查询路口违章数据
        /// </summary>
        /// <param name="crossName">电警设备名</param>
        /// <param name="type">day/month/year</param>
        /// <returns></returns>
        [WebMethod]
        public ViolaticeData_Entity GetVIOLATIVE_ALAR_DATA(string id, string type)
        {
            ViolaticeData_Entity Violatice = new ViolaticeData_Entity();
            Violatice.violaticeData = new List<ViolaticeData>();
            string sql = "";
            string dtDate = "";
            string dtDD = DateTime.Now.ToString("yyyy-MM-dd");
            switch (type)
            {
                case "day":
                    sql = "select count(b.plate_no)num ,TO_CHAR (b.pass_time, 'hh24') hour " +
                            "from TRAFFIC_CROSSING_INFO a ,TRAFFIC_VIOLATIVE_ALARM b where a.crossing_id = b.crossing_id and a.crossing_id = " + id + " " +
                            "and b.pass_time >= to_timestamp('" + dtDD + " 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "and b.pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "GROUP BY TO_CHAR (b.pass_time, 'hh24') " +
                            "order by TO_CHAR (b.pass_time, 'hh24')";
                    break;
                case "month":
                    dtDate = DateTime.Now.ToString("yyyy-MM");
                    sql = "select count(b.plate_no)num ,TO_CHAR (b.pass_time, 'dd') hour " +
                            "from TRAFFIC_CROSSING_INFO a ,TRAFFIC_VIOLATIVE_ALARM b where a.crossing_id = b.crossing_id and a.crossing_id = " + id + " " +
                            "and b.pass_time >= to_timestamp('" + dtDate + "-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "and b.pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "GROUP BY TO_CHAR (b.pass_time, 'dd') " +
                            "order by TO_CHAR (b.pass_time, 'dd')";
                    break;
                case "year":
                    dtDate = DateTime.Now.ToString("yyyy");
                    sql = "select count(b.plate_no)num ,TO_CHAR (b.pass_time, 'MM') hour " +
                            "from TRAFFIC_CROSSING_INFO a ,TRAFFIC_VIOLATIVE_ALARM b where a.crossing_id = b.crossing_id and a.crossing_id = " + id + " " +
                            "and b.pass_time >= to_timestamp('" + dtDate + "-01-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "and b.pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                            "GROUP BY TO_CHAR (b.pass_time, 'MM') " +
                            "order by TO_CHAR (b.pass_time, 'MM')";
                    break;
                default:
                    break;
            }
            DataSet ds = OracleHelper.Query(sql);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ViolaticeData vd = new ViolaticeData();
                    vd.num = dt.Rows[i][0].ToString();
                    vd.type = dt.Rows[i][1].ToString();
                    Violatice.violaticeData.Add(vd);
                }
                Violatice.id = id;
                return Violatice;
            }
            return null;
        }
        /// <summary>
        /// 取过车总流量
        /// </summary>
        /// <param name="crossName">电警设备名</param>
        /// <param name="type">day/month/year</param>
        /// <returns></returns>
        [WebMethod]
        public CrossCount_Entity GetCrossCount(string id, string type)
        {
            CrossCount_Entity crossCount = new CrossCount_Entity();
            crossCount.id = id;
            crossCount.crossCount = new List<CrossCount>();
            string tempStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime tempDate = Convert.ToDateTime(tempStart);

            for (int i = 0; i < 24; i++)
            {
                CrossCount count = new CrossCount();

                DateTime _start = tempDate.AddHours(i);
                DateTime _end = _start.AddHours(1);
                long startTime = ToolsDoc.Tools.GetJsTimeStamp(_start);
                long endTime = ToolsDoc.Tools.GetJsTimeStamp(_end);
                string sql = "select count(1)  from traffic_vehicle_pass_pic where pass_time >= " + startTime + " ";
                sql += "and pass_time <= " + endTime + " ";
                sql += "and crossing_id = " + id + " ";

                RetModel rm = OracleHelper.ESQuery(sql);
                count.num = rm.rows[0][0];
                count.type = i.ToString();
                crossCount.crossCount.Add(count);
            }

            return crossCount;
            




            ////根据设备名取对应路口的表名
            //string sql1 = "select local_id from PASS_ID_CORRESPOND where hik_id like '%" + id + "%'";
            //DataSet ds = OracleHelper.QueryU_Dev(sql1);
            //if (Tools.DSisNull(ds))
            //{
            //    DataTable dt = ds.Tables[0];
            //    string crossing_id = dt.Rows[0][0].ToString();
            //    string tableName = "I" + crossing_id;//测试数据
            //    string sqlCount = "";
            //    string dtDate = "";
            //    string dtDD = "2019-01-01";//DateTime.Now.ToString("yyyy-MM-dd");       测试数据
            //    CrossCount_Entity CrossCount = new CrossCount_Entity();
            //    CrossCount.crossCount = new List<CrossCount>();
            //    switch (type)
            //    {
            //        case "day":
            //            sqlCount = "select count(pass_id)num ,TO_CHAR (pass_time, 'hh24') hour " +
            //                    "from " + tableName + " where " +
            //                    "pass_time >= to_timestamp('" + dtDD + " 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "GROUP BY TO_CHAR (pass_time, 'hh24') " +
            //                    "order by TO_CHAR (pass_time, 'hh24')";
            //            break;
            //        case "month":
            //            dtDate = "2019-01";//DateTime.Now.ToString("yyyy-MM");
            //            sqlCount = "select count(pass_id)num ,TO_CHAR (pass_time, 'dd') dd " +
            //                    "from " + tableName + " where " +
            //                    "pass_time >= to_timestamp('" + dtDate + "-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "GROUP BY TO_CHAR (pass_time, 'dd') " +
            //                    "order by TO_CHAR (pass_time, 'dd')";
            //            break;
            //        case "year":
            //            dtDate = DateTime.Now.ToString("yyyy");
            //            sqlCount = "select count(pass_id)num ,TO_CHAR (pass_time, 'MM') MM " +
            //                    "from " + tableName + " where  " +
            //                    "pass_time >= to_timestamp('" + dtDate + "-01-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                    "GROUP BY TO_CHAR (pass_time, 'MM') " +
            //                    "order by TO_CHAR (pass_time, 'MM')";
            //            break;
            //        default:
            //            break;
            //    }
            //    DataSet dsCount = OracleHelper.QueryU_Dev(sqlCount);
            //    if (Tools.DSisNull(dsCount))
            //    {
            //        DataTable dtCount = dsCount.Tables[0];
            //        for (int i = 0; i < dtCount.Rows.Count; i++)
            //        {
            //            CrossCount CC = new CrossCount();
            //            CC.num = dtCount.Rows[i][0].ToString();
            //            CC.type = dtCount.Rows[i][1].ToString();
            //            CrossCount.crossCount.Add(CC);
            //        }
            //        CrossCount.id = id;
            //        return CrossCount;
            //    }
            //}
            //return null; 
        }
        /// <summary>
        /// 取路口车流数据按路口方向分别统计蓝，黄及其他车牌
        /// </summary>
        /// <param name="crossName">电警设备名</param>
        /// <param name="type">day/month/year</param>
        [WebMethod]
        public Cross_Dirction_Count GetCrossCountByDirction(string id, string type)
        {
            Cross_Dirction_Count crossCount = new Cross_Dirction_Count();
            crossCount.id = id;
            crossCount.dirction_count = new List<Dirction_Count>();
            List<Temp_Dirction_Count> list = new List<Temp_Dirction_Count>();

            string tempStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime _start = Convert.ToDateTime(tempStart);
            string tempEnd = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            DateTime _end = Convert.ToDateTime(tempEnd);




            long startTime = ToolsDoc.Tools.GetJsTimeStamp(_start);
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(_end);
            string sql = "select direction, count(plate_color),plate_color from traffic_vehicle_pass_pic where pass_time >= " + startTime + " ";
            sql += "and pass_time <= " + endTime + " ";
            sql += "and crossing_id = " + id + " ";
            sql += "group by direction,plate_color";
            RetModel rm = OracleHelper.ESQuery(sql);
            foreach (string[] item in rm.rows)
            {
                Temp_Dirction_Count count = new Temp_Dirction_Count();
                string _dire = Tools.ReturnDirectionIndex(item[0]);
                count.direction = _dire;
                switch (item[2])
                {
                    case "1":
                        count.yellow = Int16.Parse(item[1]);
                        break;
                    case "2":
                        count.blue = Int16.Parse(item[1]);
                        break;
                    case "5":
                        count.green = Int16.Parse(item[1]);
                        break;
                    case "0":
                        count.white = Int16.Parse(item[1]);
                        break;
                    case "3":
                        count.black = Int16.Parse(item[1]);
                        break;
                    case "4":
                        count._other = Int16.Parse(item[1]);
                        break;
                    case "6":
                        count._black = Int16.Parse(item[1]);
                        break;
                    default:
                        //count.other = Int16.Parse(item[1]);
                        break;
                }
                list.Add(count);
            }
            Dirction_Count _X = new Dirction_Count();
            Dirction_Count _N = new Dirction_Count();
            Dirction_Count _B = new Dirction_Count();
            Dirction_Count _D = new Dirction_Count();
            foreach (Temp_Dirction_Count item in list)
            {
                switch (item.direction)
                {
                    case "由东向西":
                        _D.direction = item.direction;
                        _D.blue += item.blue;
                        _D.yellow += item.yellow;
                        _D.other += item.green;
                        _D.other += item.white;
                        _D.other += item.black;
                        _D.other += item._other;
                        _D.other += item._black;
                        break;
                    case "由西向东":
                        _X.direction = item.direction;
                        _X.blue += item.blue;
                        _X.yellow += item.yellow;
                        _X.other += item.green;
                        _X.other += item.white;
                        _X.other += item.black;
                        _X.other += item._other;
                        _X.other += item._black;
                        break;
                    case "由南向北":
                        _N.direction = item.direction;
                        _N.blue += item.blue;
                        _N.yellow += item.yellow;
                        _N.other += item.green;
                        _N.other += item.white;
                        _N.other += item.black;
                        _N.other += item._other;
                        _N.other += item._black;
                        break;
                    case "由北向南":
                        _B.direction = item.direction;
                        _B.blue += item.blue;
                        _B.yellow += item.yellow;
                        _B.other += item.green;
                        _B.other += item.white;
                        _B.other += item.black;
                        _B.other += item._other;
                        _B.other += item._black;
                        break;
                    default:
                        break;
                }

            }
            if (_X.direction != null)
            {
                crossCount.dirction_count.Add(_X);
            }
            if (_B.direction != null)
            {
                crossCount.dirction_count.Add(_B);
            }
            if (_D.direction != null)
            {
                crossCount.dirction_count.Add(_D);
            }
            if (_N.direction != null)
            {
                crossCount.dirction_count.Add(_N);
            }
            return crossCount;

            


            ////根据海康路口ID取本地对应关系表中的ID
            //string sql = "select local_id from PASS_ID_CORRESPOND where hik_id like '%" + id + "%'";
            //DataSet ds = OracleHelper.QueryU_Dev(sql);
            //if (Tools.DSisNull(ds))
            //{
            //    DataTable dt = ds.Tables[0];
            //    string crossing_id = dt.Rows[0][0].ToString();
            //    string tableName = "I" + crossing_id;//测试数据
            //    string sqlCount = "";
            //    string dtDate = "";
            //    string dtDD = "2019-01-01";//DateTime.Now.ToString("yyyy-MM-dd");//测试数据
            //    Cross_Dirction_Count CDC = new Cross_Dirction_Count();
            //    CDC.dirction_count = new List<Dirction_Count>();
            //    switch (type)
            //    {
            //        case "day":
            //            sqlCount = "select direction ,sum(blue),sum(yellow),sum(other) " +
            //                        "from " + tableName + " where " +
            //                        "pass_time >= to_timestamp('" + dtDD + " 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "GROUP BY direction";
            //            break;
            //        case "month":
            //            dtDate = DateTime.Now.ToString("yyyy-MM");//测试数据
            //            sqlCount = "select direction ,sum(blue),sum(yellow),sum(other) " +
            //                        "from " + tableName + " where " +
            //                        "pass_time >= to_timestamp('" + dtDate + "-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "GROUP BY direction";
            //            break;
            //        case "year":
            //            dtDate = DateTime.Now.ToString("yyyy");//测试数据
            //            sqlCount = "select direction ,sum(blue),sum(yellow),sum(other) " +
            //                        "from " + tableName + " where " +
            //                        "pass_time >= to_timestamp('" + dtDate + "-01-01 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "and pass_time <= to_timestamp('" + dtDD + " 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //                        "GROUP BY direction";
            //            break;
            //        default:
            //            break;
            //    }
            //    DataSet dsCount = OracleHelper.QueryU_Dev(sqlCount);
            //    if (Tools.DSisNull(dsCount))
            //    {
            //        DataTable dtCount = dsCount.Tables[0];
            //        for (int i = 0; i < dtCount.Rows.Count; i++)
            //        {
            //            Dirction_Count DC = new Dirction_Count();
            //            DC.direction = dtCount.Rows[i][0].ToString();
            //            DC.blue = short.Parse(dtCount.Rows[i][1].ToString());
            //            DC.yellow = short.Parse(dtCount.Rows[i][2].ToString());
            //            DC.other = short.Parse(dtCount.Rows[i][3].ToString());
            //            CDC.dirction_count.Add(DC);
            //        }
            //        CDC.id = id;
            //        return CDC;
            //    }
            //}
            //return null; 
        }
        /// <summary>
        /// 根据路口名，取每个方向的左直右过车数据
        /// </summary>
        /// <param name="Cross_Name"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Direction_Flow_Entity> Get_Traffic_Direction_Data(string Cross_ID)
        {
            //模拟时间
            string morning = "";
            string night = "";
            string allof = "";

            string[] ids = GetCrossid(Cross_ID);
            if (ids == null)
            {
                return null;
            }
            TimepLog timepLog = new TimepLog();
            DateTime now = DateTime.Now;
            DateTime morning_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 08:30:00");//早高峰
            TimeSpan morning_Span = now - morning_time;
            double ms = morning_Span.TotalSeconds;
            if (ms > 0) //早高峰时间段已过
            {
                DateTime night_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 18:30:00");//晚高峰
                TimeSpan night_Span = now - night_time;
                double ns = night_Span.TotalSeconds;
                if (ns > 0) //晚高峰时间段已过
                {
                    string temp = morning_time.ToString("yyyy-MM-dd");
                    timepLog.morning_date_start = temp + " 07:30:00";
                    timepLog.morning_date_end = temp + " 08:30:00";
                    timepLog.night_date_start = temp + " 17:00:00";
                    timepLog.night_date_end = temp + " 18:30:00";
                    timepLog.allof_date_start = temp + " 00:00:00";
                    timepLog.allof_date_end = temp + " 23:59:59";

                    morning = DateTime.Now.ToString("MM-dd");
                    night = DateTime.Now.ToString("MM-dd");
                    allof = DateTime.Now.ToString("MM-dd");
                }
                else//早高峰时间段已过，晚高峰时间段未到
                {
                    //当天的早高峰
                    string temp = morning_time.ToString("yyyy-MM-dd");
                    timepLog.morning_date_start = temp + " 07:30:00";
                    timepLog.morning_date_end = temp + " 08:30:00";

                    morning = DateTime.Now.ToString("MM-dd");

                    //前一天的晚高峰和整天数据
                    temp = morning_time.AddDays(-1).ToString("yyyy-MM-dd");
                    timepLog.night_date_start = temp + " 17:00:00";
                    timepLog.night_date_end = temp + " 18:30:00";
                    timepLog.allof_date_start = temp + " 00:00:00";
                    timepLog.allof_date_end = temp + " 23:59:59";

                    night = DateTime.Now.AddDays(-1).ToString("MM-dd");
                    allof = DateTime.Now.AddDays(-1).ToString("MM-dd");
                }
            }
            else//未到早高峰时间段
            {
                string temp = morning_time.AddDays(-1).ToString("yyyy-MM-dd");
                timepLog.morning_date_start = temp + " 07:30:00";
                timepLog.morning_date_end = temp + " 08:30:00";
                timepLog.night_date_start = temp + " 17:00:00";
                timepLog.night_date_end = temp + " 18:30:00";
                timepLog.allof_date_start = temp + " 00:00:00";
                timepLog.allof_date_end = temp + " 23:59:59";

                morning = DateTime.Now.AddDays(-1).ToString("MM-dd");
                night = DateTime.Now.AddDays(-1).ToString("MM-dd");
                allof = DateTime.Now.AddDays(-1).ToString("MM-dd");
            }
            
            List<Direction_Flow_Entity> list = new List<Direction_Flow_Entity>();
            Direction_Flow_Entity flow;
            List<Traffic_Data> td_morning = Get_Traffic_Data(timepLog.morning_date_start, timepLog.morning_date_end, ids);
            List<Traffic_Data> td_night = Get_Traffic_Data(timepLog.night_date_start, timepLog.night_date_end, ids);
            List<Traffic_Data> td_allof_date = Get_Traffic_Data(timepLog.allof_date_start, timepLog.allof_date_end, ids);
            if (td_morning != null)
            {
                flow = new Direction_Flow_Entity();
                flow.section = "morning";
                flow.date = morning;//Convert.ToDateTime(timepLog.morning_date_start).ToString("MM-dd");
                flow.data = td_morning;
                list.Add(flow);
            }
            if (td_night != null)
            {
                flow = new Direction_Flow_Entity();
                flow.section = "night";
                flow.date = night;// Convert.ToDateTime(timepLog.night_date_start).ToString("MM-dd");
                flow.data = td_night;
                list.Add(flow);
            }
            if (td_allof_date != null)
            {
                flow = new Direction_Flow_Entity();
                flow.section = "allOfDay";
                flow.date = allof;// Convert.ToDateTime(timepLog.allof_date_start).ToString("MM-dd");
                flow.data = td_allof_date;
                list.Add(flow);
            }
            if (list.Count > 0)
            {
                return list;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据路口、表名，取每个方向的左直右过车数据
        /// </summary>
        /// <param name="start_time"></param>
        /// <param name="end_time"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private List<Traffic_Data> Get_Traffic_Data(string start_time, string end_time, string[] ids)
        {
            //string sql = "select a.direction,sum(a.yellow),sum(a.blue),sum(a.other),c.flow_no from TRAFFIC_LANE_INFO b,FLOW_DICT c," + ids[0] + " a " +
            //            "where b.crossing_id in (" + ids[1] + ") " +
            //            "and a.road_lane = substr(b.lane_name,8,1) " +
            //            "and a.direction = substr(b.lane_name,0,4) " +
            //            "and a.pass_time >= to_timestamp('" + start_time + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //            "and a.pass_time <= to_timestamp('" + end_time + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
            //            "and (c.flow_no like '%1%' or c.flow_no like '%2%' or c.flow_no like '%3%') " +
            //            "and c.crossing_id = b.crossing_id " +
            //            "group by a.direction,c.flow_no";
            

            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(start_time));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(end_time));

            string sql = "select crossing_id,direction,lane_no,count(crossing_id)  from traffic_vehicle_pass_pic where pass_time >= " + startTime + " ";
            sql += "and pass_time <= " + endTime + " ";
            sql += "and crossing_id in (" + ids[1] + ") ";
            sql += "and lane_no != '0' ";
            sql += "group by crossing_id,direction,lane_no";

            RetModel rm = OracleHelper.ESQuery(sql);

            string sqlDev = "select FLOW_NAME,CROSSING_ID,LANE_NO,DIRECTION from FLOW_DICT where crossing_id in (" + ids[1] + ")";
            DataSet ds = OracleHelper.QueryU_Dev(sqlDev);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0];
                List<Traffic_Data> list = new List<Traffic_Data>();
                foreach (string[] item in rm.rows)
                {
                    string m_crossing_id = item[0];
                    string m_direction = item[1];
                    string n_lane_no = item[2];
                    string count = item[3];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string FLOW_NAME = dt.Rows[i][0].ToString();
                        string CROSSING_ID = dt.Rows[i][1].ToString();
                        string LANE_NO = dt.Rows[i][2].ToString();
                        string DIRECTION = dt.Rows[i][3].ToString();
                        if (CROSSING_ID == m_crossing_id && Tools.ReturnDirection(DIRECTION) == m_direction && LANE_NO == n_lane_no)
                        {
                            Traffic_Data d = new Traffic_Data();
                            d.direction = DIRECTION;
                            switch (FLOW_NAME)
                            {
                                case "左转":
                                case "调头":
                                    d.left = Int16.Parse(count);
                                    break;
                                case "直行":
                                    d.forward = Int16.Parse(count);
                                    break;
                                case "右转":
                                    d.right = Int16.Parse(count);
                                    break;
                                default:
                                    break;
                            }

                            list.Add(d);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    Traffic_Data _X = null;
                    Traffic_Data _N = null;
                    Traffic_Data _B = null;
                    Traffic_Data _D = null;
                    foreach (Traffic_Data item in list)
                    {
                        switch (item.direction)
                        {
                            case "由东向西":
                                if (_D == null)
                                { 
                                    _D = new Traffic_Data();
                                }
                                _D.direction = item.direction;
                                _D.forward += item.forward;
                                _D.left += item.left;
                                _D.right += item.right;
                                break;
                            case "由西向东":
                                if (_X == null)
                                {
                                    _X = new Traffic_Data();
                                }
                                _X.direction = item.direction;
                                _X.forward += item.forward;
                                _X.left += item.left;
                                _X.right += item.right;
                                break;
                            case "由南向北":
                                if (_N == null)
                                {
                                    _N = new Traffic_Data();
                                }
                                _N.direction = item.direction;
                                _N.forward += item.forward;
                                _N.left += item.left;
                                _N.right += item.right;
                                break;
                            case "由北向南":
                                if (_B == null)
                                {
                                    _B = new Traffic_Data();
                                }
                                _B.direction = item.direction;
                                _B.forward += item.forward;
                                _B.left += item.left;
                                _B.right += item.right;
                                break;
                            default:
                                break;
                        }
                    }
                    List<Traffic_Data> TrafficList = new List<Traffic_Data>();
                    if (_X != null)
                    {
                        _X.count = _X.left + _X.right + _X.forward;
                        TrafficList.Add(_X);
                    }
                    if (_N != null)
                    {
                        _N.count = _N.left + _N.right + _N.forward;
                        TrafficList.Add(_N);
                    }
                    if (_B != null)
                    {
                        _B.count = _B.left + _B.right + _B.forward;
                        TrafficList.Add(_B);
                    }
                    if (_D != null)
                    {
                        _D.count = _D.left + _D.right + _D.forward;
                        TrafficList.Add(_D);
                    }
                    return TrafficList;
                }
            }
            return null;
        }
        /// <summary>
        /// 查询本地数据所对应的表名和海康id
        /// </summary>
        /// <param name="CN"></param>
        /// <returns>0：表名(弃用)，1：海康ID</returns>
        private string[] GetCrossid(string crossid)
        {

            string sql = "select local_id,hik_id from PASS_ID_CORRESPOND where UTCS_ID = '" + crossid + "'";
            DataSet ds = OracleHelper.QueryU_Dev(sql);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0];
                string[] res = new string[2];
                res[0] = Tools.ReTableTitle(DateTime.Now.Year) + dt.Rows[0][0].ToString();
                res[1] = dt.Rows[0][1].ToString();
                return res;
            }
            return null;
        }

        [WebMethod]
        public void SetTrafficData(VehicleInfo vehicleInfo)
        {
            
            string sql = string.Format("insert into traffic_speeding values(SEQ_SPD.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}')", vehicleInfo.plate_no, vehicleInfo.plate_color, vehicleInfo.vehicle_type, vehicleInfo.vehicle_speed, vehicleInfo.pass_time, vehicleInfo.lane_no, vehicleInfo.address);
            //Tools.WriteErrLog(sql);
            OracleHelper.ExecuteU_Dev_212(sql);
        }
    }
}
