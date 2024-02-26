using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HYSignServices.Entity;
using System.Data;
using Newtonsoft.Json;
using HYSignServices.ToolsDoc;

namespace HYSignServices.HYService
{
    /// <summary>
    /// VehicleService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class VehicleService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<DeviceInfo> Get_U_DevInfoWithLngLat()
        {

            string sql = "select id,dev_name,ip,port,user_name,password,direction from UNITY_DEVICE_INFO where remark = '电警' and direction is not null";
            
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    List<DeviceInfo> list = new List<DeviceInfo>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DeviceInfo dev = new DeviceInfo();
                        dev.cross_id = dt.Rows[i][0].ToString();
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.dev_ip = dt.Rows[i][2].ToString();
                        dev.port = Int16.Parse(dt.Rows[i][3].ToString());
                        dev.dev_user = dt.Rows[i][4].ToString();
                        dev.dev_psw = dt.Rows[i][5].ToString();
                        dev.direction = dt.Rows[i][6].ToString();
                        list.Add(dev);
                    }
                    return list;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取路口信息
        /// </summary>
        /// <param name="crossing_id"></param>
        /// <returns></returns>
        [WebMethod]
        public List<AlarmEntity> GetCrossInfo(string crossing_id)
        {
            string sql = "select crossing_id,crossing_name from TRAFFIC_CROSSING_INFO";
            if (!string.IsNullOrEmpty(crossing_id))
            {
                sql += " where crossing_id = " + crossing_id;
            }
            DataSet ds = OracleHelper.QueryU_Dev(sql);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0]; 
                List<AlarmEntity> list = new List<AlarmEntity>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AlarmEntity dev = new AlarmEntity();
                    dev.num = dt.Rows[i][0].ToString();
                    dev.CrossName = dt.Rows[i][1].ToString();
                    list.Add(dev);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 取车道
        /// </summary>
        /// <param name="crossing_id"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Lane_Info> GetLaneInfo(string crossing_id)
        {
            //string sql = "select substr(lane_name,0,4)lanename,substr(lane_name,8,9)laneno from TRAFFIC_LANE_INFO where crossing_id = " + crossing_id + " order by lanename,laneno";
            
            string sql = "select substr(a.lane_name,0,4)lanename,substr(a.lane_name,8,9)laneno,b.flow_no from TRAFFIC_LANE_INFO a,FLOW_DICT b  ";
            sql += "where a.crossing_id = " + crossing_id + " ";
            sql += "and a.crossing_id = b.crossing_id and substr(a.lane_name,0,4) = b.direction ";
            sql += "and substr(a.lane_name,8,9) = b.lane_no ";
            sql += "order by lanename,laneno";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<Lane_Info> list = new List<Lane_Info>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Lane_Info fd = new Lane_Info();
                        fd.Lane_Name = Tools.GetDirection(dt.Rows[i][0].ToString());
                        fd.Lane_NO = int.Parse(dt.Rows[i][1].ToString());
                        fd.Flow_Name = Tools.GetFlowIndex(dt.Rows[i][2].ToString());
                        bool isInSide = true;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].Flow_Name.Contains(fd.Flow_Name) && fd.Lane_NO == list[j].Lane_NO && fd.Lane_Name == list[j].Lane_Name)
                            {
                                isInSide = false;
                                break;
                            }
                            if (fd.Flow_Name != list[j].Flow_Name && fd.Lane_NO == list[j].Lane_NO && fd.Lane_Name == list[j].Lane_Name)
                            {
                                list[j].Flow_Name += "," + fd.Flow_Name;
                                isInSide = false;
                                break;
                            }
                        }
                        if (isInSide)
                        {
                            list.Add(fd);
                        }
                    }
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 取过车数据  1天以内
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Violative_alarm_Entity> GetVehiclePass(string start, string end,string crossing_id)
        {
            DateTime dateStart;
            DateTime dateEnd;
            try
            {
                dateStart = Convert.ToDateTime(start);
                dateEnd = Convert.ToDateTime(end);
            }
            catch (Exception)
            {
                return null;
            }
            
            TimeSpan ts = dateEnd.Subtract(dateStart);
            if (ts.TotalDays > 1)
            {
                return null;
            }

            long startTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(dateStart));
            long endTime = ToolsDoc.Tools.GetJsTimeStamp(Convert.ToDateTime(dateEnd));
            HYSignServices.ToolsDoc.QueryModel query = new HYSignServices.ToolsDoc.QueryModel();
            query.query = "select crossing_id,direction,lane_no,pass_time,plate_color,vehicle_type from traffic_vehicle_pass_pic where crossing_id in(" + crossing_id + ") and pass_time >= " + startTime + " and pass_time <= " + endTime + " order by pass_time";
            string queryJson = JsonConvert.SerializeObject(query);
            string postModel = HttpHelper.HttpPost(ToolsDoc.Tools.ESUrlStr, queryJson);
            RetModel retModel = JsonConvert.DeserializeObject<RetModel>(postModel);
            try
            {
                List<Violative_alarm_Entity> list = new List<Violative_alarm_Entity>();
                foreach (string[] item in retModel.rows)
                {
                    Violative_alarm_Entity dev = new Violative_alarm_Entity();
                    dev.crossing_id = item[0];
                    dev.lane_id = item[2];
                    dev.lane_name = item[1].Substring(3,1);
                    dev.pass_time = ToolsDoc.Tools.GetJsTime(long.Parse(item[3]));
                    dev.vehicle_type = item[5];
                    dev.plate_color = item[4];
                    list.Add(dev);
                    
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //取路口车道数据(自用)
        [WebMethod]
        public List<CrossLaneMode> GetCrossDirection()
        {
            string sql = "select b.crossing_id,b.lane_no,b.lane_name from TRAFFIC_LANE_INFO b  order by  b.crossing_id";
            try
            {
                DataSet ds = OracleHelper.Query(sql);
                if (Tools.DSisNull(ds))
                {
                    List<CrossLaneMode> list = new List<CrossLaneMode>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CrossLaneMode dev = new CrossLaneMode();
                        dev.cross_id = dt.Rows[i][0].ToString();
                        dev.lane_no = dt.Rows[i][1].ToString();
                        dev.lane_name = dt.Rows[i][2].ToString();
                        list.Add(dev);
                    }
                    return list;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 取事件检测相机数据
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<CheckDeviceInfo> GetEventInfo()
        {
            string sql = "select ip,dev_name,lng,lat from UNITY_DEVICE_INFO where ip like '192.168.32%' and remark = '球机' order by ip";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    List<CheckDeviceInfo> list = new List<CheckDeviceInfo>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CheckDeviceInfo dev = new CheckDeviceInfo();
                        dev.ip = dt.Rows[i][0].ToString();
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.lng = dt.Rows[i][2].ToString();
                        dev.lat = dt.Rows[i][3].ToString();

                        list.Add(dev);
                    }
                    return list;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private string ConvertDirectionIndex(string directionStr)
        {
            switch (directionStr)
            {
                case "由东向西":
                    return "1";
                case "由西向东":
                    return "2";
                case "由南向北":
                    return "3";
                case "由北向南":
                    return "4";
                default:
                    return "0";
            }
        }


    }
}
