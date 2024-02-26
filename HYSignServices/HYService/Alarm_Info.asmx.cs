using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HYSignServices.Entity;
using System.Data;

namespace HYSignServices.HYService
{
    /// <summary>
    /// Alarm_Info 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Alarm_Info : System.Web.Services.WebService
    {

        Alarm_Service hs = new Alarm_Service();
        /// <summary>
        /// 布控警报  http://192.168.0.236:8008/HYService/Alarm_Info.asmx/Disposition_Alarm
        /// rn --分页码
        /// </summary>
        [WebMethod]
        public List<Disposition_alarm_Entity> Disposition_Alarm(string rn)
        {
            return hs.Disposition_Alarm(rn);
        }
        /// <summary>
        /// 违法过车查询  http://192.168.0.236:8008/HYService/Alarm_Info.asmx/Violative_alarm?plate_no=鄂AN01P7&startTime=2018-05-23&endTime=
        /// </summary>
        [WebMethod]
        public List<Violative_alarm_Entity> Violative_alarm(string plate_no, string startTime, string endTime)
        {
            return hs.Violative_alarm(plate_no, startTime, endTime);
        }
        /// <summary>
        /// 区间报警查询 http://192.168.0.236:8008/HYService/Alarm_Info.asmx/Violative_alarm?startTime=2018-05-23&endTime=&plate_no=鄂AN01P7
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime">可为空</param>
        /// <param name="plate_no"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Section_alarm_Entity> Section_alarm(string startTime, string endTime, string plate_no)
        {
            return hs.Section_alarm(startTime, endTime, plate_no);
        }

        [WebMethod]
        public string StartAlaemData()
        {
            //SocketClint sc = new SocketClint();
            //sc.SendMessage("ShowAlarmData");
            //sc.StopConnetct();
            //sc = null;
            //Alarm_Service.StartAlarmThread();
            return "调用开启接口";
        }
        [WebMethod]
        public string StopAlaemData()
        {
            Alarm_Service.StopAlarmThread();
            return "调用关闭接口";
        }


        [WebMethod]
        public List<NullData> ResearchNullData()
        {
            Dictionary<string, string> list_ids = new Dictionary<string, string>();
            string sqlcross = "select crossing_id,crossing_name from TRAFFIC_CROSSING_INFO";
            DataSet ds = OracleHelper.Query(sqlcross);
            if (Tools.DSisNull(ds))
            {
                
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list_ids.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                }
            }


            string startTime, endTime;
            endTime = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            startTime = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:MM:ss");
            string sql = "select crossing_id from TRAFFIC_VEHICLE_PASS where " +
                        "pass_time > to_timestamp('"+startTime+".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and pass_time <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') group by crossing_id";
            DataSet dsCount = OracleHelper.Query(sql);
            if (Tools.DSisNull(dsCount))
            {
                List<string> list = new List<string>();
                DataTable dt = dsCount.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(dt.Rows[i][0].ToString());
                }
                //string[] idArray = ids.Split(',');
                List<NullData> nullData = new List<NullData>();
                if (list.Count > 0 && list_ids.Count > 0)
                {
                    foreach (var item in list_ids)
                    {
                        if (!list.Contains(item.Key))
                        {
                            NullData n = new NullData();
                            n.cross_info = item.Key + item.Value;
                            nullData.Add(n);
                        }
                        
                    }

                    
                    return nullData;
                    
                }
            }

            return null;
        }
        public struct NullData
        {
            public string cross_info;
        }

        [WebMethod]
        public List<Data> GetTrafficAlarm()
        {
            List<Data> dataList = null;
            string alarmSql = "select c.crossing_id,c.crossing_name,a.plate_no,a.plate_color,to_char(a.pass_time,'yyyy-mm-dd HH24:mi:ss') passtime,e.lane_name,b.sysdict_name ";
            //alarmSql += "CONCAT('http://192.168.92.235:6120',d.vehiclepicurl) pic ";
            alarmSql += "from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b ,TRAFFIC_CROSSING_INFO c,picurl_info d , TRAFFIC_LANE_INFO e ";
            alarmSql += "where b.sysdict_type = 1017 ";//a.plate_no = '鄂鄂H3L520' and 
            alarmSql += "and b.sysdict_code in('13730','11163','60951','13010','13447','13570','12081','16251','1301','1208','1625') ";
            alarmSql += "and b.sysdict_code = a.violative_action ";
            alarmSql += "and c.crossing_id = a.crossing_id ";
            alarmSql += "and d.vehiclelsh = a.violative_alarm_id ";
            alarmSql += "and e.crossing_id = c.crossing_id ";
            alarmSql += "and e.lane_no = a.lane_no ";
            alarmSql += "and d.picurltype = 1 ";
            alarmSql += "and a.pass_time >= to_timestamp('2022-4-8 00:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            alarmSql += "and a.pass_time < to_timestamp('2022-4-8 23:59:59.000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            //alarmSql += " and a.crossing_id = 101696";
            alarmSql += "order by a.pass_time desc";
            DataSet dsCount = OracleHelper.Query(alarmSql);
            if (Tools.DSisNull(dsCount))
            {
                DataTable dt = dsCount.Tables[0];
                dataList = new List<Data>();
                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Data data = new Data();
                    data.cross_id = dt.Rows[i][0].ToString();
                    data.cross_name = dt.Rows[i][1].ToString();
                    data.plate_no = dt.Rows[i][2].ToString();
                    string plate_color = dt.Rows[i][3].ToString();
                    data.plate_color = Tools.ConvertColor(plate_color);
                    data.passtime = dt.Rows[i][4].ToString();
                    data.lane_name = dt.Rows[i][5].ToString();
                    data.sysdict_name = dt.Rows[i][6].ToString();
                    //查询本地数据库IP和设备编码
                    string[] item = data.lane_name.Split('-');
                    string direction = item[0];
                    string lane_no = item[1].Substring(2,1);
                    string devSql = "select a.ip,b.direction,a.devicecode from UNITY_DEVICE_INFO a,FLOW_DICT b ";
                    devSql += "where remark in('电警','卡口') and a.id = b.crossing_id and a.direction = b.direction and a.ip = b.ip_address ";
                    devSql += "and b.lane_no = '" + lane_no + "' and a.id = " + data.cross_id + " and b.direction = '" + direction + "'";
                    
                    DataSet ds = OracleHelper.Query212(devSql);

                    if (Tools.DSisNull(ds))
                    {
                        DataTable dev_dt = ds.Tables[0];
                        for (int j = 0; j < dev_dt.Rows.Count; j++)
                        {
                            string dev_ip = dev_dt.Rows[j][0].ToString();
                            string dev_dire = dev_dt.Rows[j][1].ToString();
                            string code = dev_dt.Rows[j][2].ToString();
                            data.dev_ip = dev_ip;
                            data.dev_code = code;
                            break;
                        }

                    }
                    else
                    {
                        string reSql = "select ip,direction,devicecode from UNITY_DEVICE_INFO  where remark in('电警','卡口') and id = " + data.cross_id + " and direction = '" + direction + "'";
                        DataSet reds = OracleHelper.Query212(reSql);

                        if (Tools.DSisNull(reds))
                        {
                            DataTable dev_dt = reds.Tables[0];
                            for (int j = 0; j < dev_dt.Rows.Count; j++)
                            {
                                string dev_ip = dev_dt.Rows[j][0].ToString();
                                string dev_dire = dev_dt.Rows[j][1].ToString();
                                string code = dev_dt.Rows[j][2].ToString();
                                data.dev_ip = dev_ip;
                                data.dev_code = code;
                                break;
                            }
                        }
                    }
                    dataList.Add(data);
                }
            }
            int op = 0;
            foreach (Data data in dataList)
            {
                if (!string.IsNullOrEmpty(data.dev_code))
                {
                    op++;
                }
                string msg = "路口:" + data.cross_name + ",车牌:" + data.plate_no + ",车牌颜色:" + data.plate_color + ",时间:" + data.passtime + ",车道:" + data.lane_name + ",设备地址:" + data.dev_ip + ",设备编码:" + data.dev_code + ",违法行为:" + data.sysdict_name;
                Tools.WriteLog(msg);
            }
            return dataList;
        }

        
    }
}
