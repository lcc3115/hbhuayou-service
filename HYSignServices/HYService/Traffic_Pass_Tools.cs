using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace HYSignServices.HYService
{
    public class Traffic_Pass_Tools
    {
        /// <summary>
        /// 自定义查询参数
        /// </summary>
        /// <param name="type">1.按天计算  2.按小时计算  3.按分钟计算(查询前一分钟)</param>
        /// <param name="range">时间范围(type为1和2时生效)</param>
        /// <param name="crossidlist">路口编号，多个用','隔开</param>
        /// <returns></returns>
        public static string GetVehicleNumByCustom_tool(string type, string range, string crossidlist)
        {
            int num = 0;
            if (string.IsNullOrEmpty(type))
            { return "参数type不可为空"; }
            if (type == "1" || type == "2")
            {
                if (string.IsNullOrEmpty(range) || range == "0")
                { return "按天计算和按小时计算时时间范围不能为空或0"; }
                num = int.Parse(range);
            }

            string sql = "";
            string crolist = " 1=1";
            if (!string.IsNullOrEmpty(crossidlist)) crolist = " crossing_id in (" + crossidlist + ")";
            switch (type)
            {
                case "1":
                    sql = "select day_time,count(*) num from (select  to_char(pass_time,'dd') day_time from TRAFFIC_VEHICLE_PASS" +
                          " where pass_time>sysdate - " + num + " and " + crolist + ") group by day_time order by day_time";
                    break;
                case "2":
                    sql = "select hour_time,count(*) num from (select  to_char(pass_time,'hh24') hour_time from TRAFFIC_VEHICLE_PASS" +
                          " where pass_time>sysdate -" + num + "/24 and " + crolist + ") group by hour_time order by hour_time";
                    break;
                case "3":
                    string startTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    string endTime = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-dd HH:mm:ss");
                    sql = "select count(*) num from TRAFFIC_VEHICLE_PASS where pass_time >= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                          " and pass_time <= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') and" + crolist + "";
                    break;
                default:
                    break;
            }
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (type == "1" || type == "2") dt.Rows.RemoveAt(0);
                return Tools.DataTableToJson(dt);
            }
            return null;
        }
        /// <summary>
        /// 获取前一个小时每分钟的车流量
        /// </summary>
        /// <param name="crossidlist">路口编号，多个用','隔开，为空时查询所有路口</param>
        /// <returns></returns>
        public static string GetCrossVehicleNumPerMi_tool(string crossidlist)
        {
            string order = "to_char(sysdate-59/1440,'mi'), 1";
            for (int i = 58; i >= 0; i--)
            {
                order += ",to_char(sysdate-" + i + "/1440,'mi'), " + (60 - i) + "";
            }
            string crolist = " 1=1";
            if (!string.IsNullOrEmpty(crossidlist)) crolist = " crossing_id in (" + crossidlist + ")";
            string sql = "select mi_time,count(*) num from (select  to_char(pass_time,'mi') mi_time from TRAFFIC_VEHICLE_PASS" +
                        " where pass_time>sysdate -1/24 and " + crolist + ") group by mi_time order by decode(mi_time, " + order + ")";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                DataTable dt = ds.Tables[0];
                //Tools.ReDataTable(dt);

                //dt.Rows[0].
                return Tools.DataTableToJson(dt);

            }
            return null;
        }

        /// <summary>
        /// 获取前一分钟的车流量
        /// </summary>
        /// <param name="crossidlist"></param>
        /// <returns></returns>
        public static string GetVehicleNumByLastMi_tool( string crossidlist)
        {
            string sql = "";
            string crolist = " 1=1";
            if (!string.IsNullOrEmpty(crossidlist)) crolist = " crossing_id in (" + crossidlist + ")";
            string startTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-dd HH:mm:ss");
            sql = "select to_char(sysdate,'mi') MI_TIME, count(*) num from TRAFFIC_VEHICLE_PASS where pass_time >= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff')" +
                  " and pass_time <= to_timestamp('" + startTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') and" + crolist + "";
            DataSet ds = OracleHelper.Query(sql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                return Tools.DataTableToJson(dt);
            }
            return null;
        }
    }
}