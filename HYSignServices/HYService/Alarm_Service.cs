using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HYSignServices.Entity;
using System.Data;
using System.Threading;
using HYSignServices.ToolsDoc;

namespace HYSignServices.HYService
{
    public class Alarm_Service
    {
        public static string lastAlarmDate = "";
        public static string nowAlarmDate = "";
        public static bool threadLoad = true;
        static Thread thread = null;
        /// <summary>
        /// 布控报警信息
        /// </summary>
        /// <returns>布控车辆结构体</returns>
        public List<Disposition_alarm_Entity> Disposition_Alarm(string rn)
        {
            int rownum = int.Parse(rn);
            List<Disposition_alarm_Entity> list = new List<Disposition_alarm_Entity>();
            string sql = "SELECT * FROM " +
                        "(SELECT B.*, rownum r  FROM " +
                        "(select a.plate_no , " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1006 and sysdict_code = a.disposition_reason)disposition_reason, " +
                        "to_char(a.pass_time,'yyyy-mm-dd hh24:mi:ss') passtime, " +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)lane_name, " +
                        "(select crossing_name from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id)crossing_name, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1003 and sysdict_code = a.plate_color)plate_color, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1004 and sysdict_code = a.vehicle_color)vehicle_color, " +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1002 and sysdict_code = a.vehicle_type)vehicle_type " +
                        "from TRAFFIC_DISPOSITION_ALARM a order by a.pass_time desc " +
                        ") B WHERE rownum <= " + Tools.ROW_LIMIT * rownum + ") C WHERE r > " + (Tools.ROW_LIMIT * rownum - Tools.ROW_LIMIT) + "";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Disposition_alarm_Entity alarm = new Disposition_alarm_Entity();
                    alarm.plate_no = dt.Rows[i][0].ToString();
                    alarm.disposition_reason = dt.Rows[i][1].ToString();
                    alarm.pass_time = dt.Rows[i][2].ToString();
                    alarm.lane_name = dt.Rows[i][3].ToString();
                    alarm.crossing_name = dt.Rows[i][4].ToString();
                    alarm.plate_color = dt.Rows[i][5].ToString();
                    alarm.vehicle_color = dt.Rows[i][6].ToString();
                    alarm.vehicle_type = dt.Rows[i][7].ToString();
                    list.Add(alarm);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 违法过车信息查询
        /// </summary>
        /// <param name="plate_no"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param> 可为空
        public List<Violative_alarm_Entity> Violative_alarm(string plate_no, string startTime, string endTime)
        {
            List<Violative_alarm_Entity> list = new List<Violative_alarm_Entity>();
            string sql = "select plate_no,pass_time,crossing_id," +
                        "(select crossing_name from TRAFFIC_CROSSING_INFO where a.crossing_id = crossing_id)crossing_name," +
                        "(select lane_id from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)lane_id," +
                        "(select lane_name from TRAFFIC_LANE_INFO where lane_no = a.lane_no and crossing_id = a.crossing_id)lane_name," +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 9007 and sysdict_code = a.direction_index)dirction," +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1004 and sysdict_code = a.vehicle_color)vehicle_color," +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1002 and sysdict_code = a.vehicle_type)vehicle_type," +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1001 and sysdict_code = a.plate_type)plate_type," +
                        "(select sysdict_name from TRAFFIC_SYSDICT where sysdict_type = 1003 and sysdict_code = a.plate_color)plate_color " +
                        "from TRAFFIC_VIOLATIVE_ALARM a " +
                        "where plate_no = '" + plate_no + "' and a.pass_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')";

            if (!string.IsNullOrEmpty(endTime))
            {
                sql += " and a.pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Violative_alarm_Entity violative_alarm = new Violative_alarm_Entity();
                    violative_alarm.plate_no = dt.Rows[i][0].ToString();
                    violative_alarm.pass_time = dt.Rows[i][1].ToString();
                    violative_alarm.crossing_id = dt.Rows[i][2].ToString();
                    violative_alarm.crossing_name = dt.Rows[i][3].ToString();
                    violative_alarm.lane_id = dt.Rows[i][4].ToString();
                    violative_alarm.lane_name = dt.Rows[i][5].ToString();
                    violative_alarm.dirction = dt.Rows[i][6].ToString();
                    violative_alarm.vehicle_color = dt.Rows[i][7].ToString();
                    violative_alarm.vehicle_type = dt.Rows[i][8].ToString();
                    violative_alarm.plate_type = dt.Rows[i][9].ToString();
                    violative_alarm.plate_color = dt.Rows[i][10].ToString();
                    list.Add(violative_alarm);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 区间报警接口
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>可为空
        /// <param name="plate_no"></param>
        /// <returns></returns>
        public List<Section_alarm_Entity> Section_alarm(string startTime, string endTime, string plate_no)
        {
            List<Section_alarm_Entity> list = new List<Section_alarm_Entity>();
            string sql = "select plate_no,pass_time_in,pass_time_out,avg_speed,speed_limit," +
                        "(select section_name from TRAFFIC_SECTION_INFO where section_id = a.section_id)section_name from TRAFFIC_SECTION_ALARM a" +
                        " where alarm_time >= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                        " and plate_no = '" + plate_no + "'";
            if (!string.IsNullOrEmpty(endTime))
            {
                sql += " and alarm_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            //if(!string.IsNullOrEmpty(plate_no))
            //{
            //    sql += " and plate_no = '" + plate_no + "'";
            //}
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Section_alarm_Entity section_alarm = new Section_alarm_Entity();
                    section_alarm.plate_no = dt.Rows[i][0].ToString();
                    section_alarm.pass_time_in = dt.Rows[i][1].ToString();
                    section_alarm.pass_time_out = dt.Rows[i][2].ToString();
                    section_alarm.avg_speed = dt.Rows[i][3].ToString();
                    section_alarm.speed_limit = dt.Rows[i][4].ToString();
                    section_alarm.section_name = dt.Rows[i][5].ToString();
                    list.Add(section_alarm);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 判断是否有新的报警消息
        /// </summary>
        /// <returns>bool</returns>
        public static void HasAlarmData()
        {
            while (threadLoad)
            {
                string sql = "SELECT * FROM (SELECT B.*, rownum r FROM " +
                        "( select to_char(pass_time,'yyyy-mm-dd hh24:mi:ss') passtime from TRAFFIC_DISPOSITION_ALARM order by pass_time desc) B  " +
                        "WHERE rownum <= 1) C WHERE r > 0";
                DataSet ds = OracleHelper.Query(sql);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    nowAlarmDate = dt.Rows[0][0].ToString();
                    if (!string.IsNullOrEmpty(lastAlarmDate))
                    {
                        if (!lastAlarmDate.Equals(nowAlarmDate))
                        {
                            //lastAlarmDate = nowAlarmDate;
                            //SocketClint sc = new SocketClint();
                            //sc.SendMessage("ShowAlarmData");
                            //sc.StopConnetct();
                            //sc = null;
                        }
                    }
                    lastAlarmDate = nowAlarmDate;
                }
                Thread.Sleep(10000);
            }  
        }

        public static void StartAlarmThread()
        {
            thread = new Thread(HasAlarmData);
            thread.IsBackground = true;
            thread.Start();
        }

        public static void StopAlarmThread()
        {
            if (thread != null && threadLoad)
            {
                threadLoad = false;
                thread.Abort();
            }
        }
    }
}