using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using HYSignServices.Entity;

namespace HYSignServices.HYService
{
    /// <summary>
    /// ScreenService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ScreenService : System.Web.Services.WebService
    {
        #region 车道屏
        /// <summary>
        /// 查询龙门架信息
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<ScreenInfo> GetScreenInfo()
        {
            try
            {
                string sql = "select * from SCREEN_INFO";
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<ScreenInfo> list = new List<ScreenInfo>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ScreenInfo screen = new ScreenInfo();
                        screen.SCREEN_NO = dt.Rows[i][0].ToString();
                        screen.SCREEN_NAME = dt.Rows[i][7].ToString();
                        screen.DIRECTION = dt.Rows[i][8].ToString();
                        screen.SCREEN_TYPE = dt.Rows[i][9].ToString();
                        screen.LNG = dt.Rows[i][5].ToString();
                        screen.LAT = dt.Rows[i][6].ToString();
                        screen.SCREEN_IP_LINE1 = dt.Rows[i][10].ToString();
                        screen.SCREEN_IP_LINE2 = dt.Rows[i][1].ToString();
                        screen.SCREEN_IP_LINE3 = dt.Rows[i][2].ToString();
                        screen.SCREEN_IP_LINE4 = dt.Rows[i][3].ToString();
                        screen.SCREEN_IP_LINE5 = dt.Rows[i][4].ToString();
                        screen.NEWCARD = dt.Rows[i][11].ToString();
                        list.Add(screen);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("GetScreenInfo err:" + ex.Message);
                return null;
            }
            return null;
        }
        /// <summary>
        /// 修改屏幕雨天自动发布/手动发布
        /// </summary>
        /// <param name="status">1-手动， 2-自动</param>
        /// <returns></returns>
        [WebMethod]
        public string SetScreenStatus(string status, string user_id)
        {
            string sql = "update manuallanescreen set status = " + status + " where screen_no is not null";
            try
            {
                string res = OracleHelper.ExecuteU_Dev_212(sql);
                if (res == "success")
                {
                    string actionSql = "insert into ACTION_LOG values('快速路','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','2','修改车道屏发布状态为" + status + "','" + user_id + "')";
                    string ss = OracleHelper.ExecuteU_Dev_212(actionSql);
                }
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 查询车道屏执行状态
        /// </summary>
        /// <returns>0-未查询到数据 1-手动 2-自动 default-Exception</returns>
        [WebMethod]
        public string GetScreenStatus()
        {
            string sql = "select status from manuallanescreen where screen_no is not null group by status";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    string status = dt.Rows[0][0].ToString();
                    return status;
                }
                return "0";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 查询每个车道屏的执行状态
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<ScreenInfo> GetScreenStatusDetial()
        {
            string sql = "select ip,status from MANUALLANESCREEN where screen_no is not null";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<ScreenInfo> list = new List<ScreenInfo>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ScreenInfo screen = new ScreenInfo();
                        screen.SCREEN_IP_LINE1 = dt.Rows[0][0].ToString();
                        screen.NEWCARD = dt.Rows[0][1].ToString();
                        list.Add(screen);
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
        #endregion

        #region 登录信息

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [WebMethod]
        public string RegistRoadManager(string user,string psw)
        {
            try
            {
                string sql_query = "select * from USERINFO where username = '" + user + "'";
                DataSet ds = OracleHelper.Query212(sql_query);
                if (Tools.DSisNull(ds))
                {
                    //用户名存在
                    return "UserIsExist";
                }
                string sql_execute = "insert into userinfo values(seq_user.nextval,'" + user + "','" + psw + "','1003','4003','','','1')";
                string res = OracleHelper.ExecuteU_Dev_212(sql_execute);
                return res;
            }
            catch (System.Data.OracleClient.OracleException ex)
            {
                return ex.Message;
            }
            
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        /// <returns>登录成功后返回用户ID</returns>
        [WebMethod]
        public string LogionRoadManager(string user, string psw)
        {
            try
            {
                string sql = "select * from USERINFO where username = '" + user + "'";
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    string psw_table = dt.Rows[0][2].ToString();
                    
                    if (psw_table == psw)
                    {
                        string userid = dt.Rows[0][0].ToString();
                        return "success," + userid;
                    }
                    else
                    {
                        return "fail,";
                    }
                }
                else
                {
                    return "NotRegist,";
                }
            }
            catch (System.Data.OracleClient.OracleException ex)
            {
                return ex.Message + ",";
            }
        }
        #endregion

        #region 事件检测
        /// <summary>
        /// 查询事件检测相机信息
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<CheckDeviceInfo> GetCheckDevice()
        {
            try
            {
                string sql = "select dev_name,ip,port,user_name,password,lng,lat from UNITY_DEVICE_INFO where ip like '%192.168.32%'";
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<CheckDeviceInfo> list = new List<CheckDeviceInfo>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CheckDeviceInfo cdi = new CheckDeviceInfo();
                        cdi.dev_name = dt.Rows[i][0].ToString();
                        cdi.ip = dt.Rows[i][1].ToString();
                        cdi.port = dt.Rows[i][2].ToString();
                        cdi.user_name = dt.Rows[i][3].ToString();
                        cdi.password = dt.Rows[i][4].ToString();
                        cdi.lng = dt.Rows[i][5].ToString();
                        cdi.lat = dt.Rows[i][6].ToString();

                        list.Add(cdi);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("GetCheckDevice err:" + ex.Message);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 查询事件检测数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="rn">分页码</param>
        [WebMethod]
        public List<CheckData> CheckEvenData(string roadName, string startTime, string endTime,string type, string direction,string rn)
        {
            int rownum = int.Parse(rn);

            string sql = "select * from( " +
                        "select b.*, rownum r from (" +
                        "select x.check_type,x.check_time,y.dev_name,x.direction,x.pic_name from EVENT_DATA x ,UNITY_DEVICE_INFO y where x.ip_addr = y.ip";
            #region 拼接SQL语句
            if (!string.IsNullOrEmpty(type))
            {
                sql += " and check_type = '" + type + "'";
            }
            if (!string.IsNullOrEmpty(direction))
            {
                sql += " and direction = '" + direction + "'";
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                //startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM-dd HH:mm"); ;
                sql += " and check_time > to_timestamp('" + startTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sql += " and check_time <= to_timestamp('" + endTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            
            if (!string.IsNullOrEmpty(roadName))
            {
                sql += " and ip_addr in (select ip from UNITY_DEVICE_INFO where ip like '192.168.32%' and dev_name like '%" + roadName + "%')";
            }
            sql += " order by check_time";
            #endregion
            sql += ") b where rownum <= " + Tools.ROW_LIMIT * rownum + ") c where r > " + (Tools.ROW_LIMIT * rownum - Tools.ROW_LIMIT) + "";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    List<CheckData> list = new List<CheckData>();
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CheckData cdi = new CheckData();
                        string temptype = dt.Rows[i][0].ToString();
                        cdi.check_type = Tools.GetCheckType(temptype);
                        cdi.check_time = dt.Rows[i][1].ToString();
                        cdi.dev_name = dt.Rows[i][2].ToString();
                        string direc = dt.Rows[i][3].ToString();
                        cdi.direction = Tools.GetCheckDire(direc);
                        cdi.pic_name = dt.Rows[i][4].ToString();
                        list.Add(cdi);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("CheckEvenData err:" + ex.Message);
                return null;
            }
            return null;            
        }

        /// <summary>
        /// 查询事件检测数据所有数据
        /// </summary>List<CheckData>
        /// <param name="type">类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        [WebMethod]
        public DataTable CheckEcentAll(string roadName, string startTime, string endTime, string type, string direction)
        {
            string sql = "select x.check_type 类型,to_char(x.check_time,'yyyy-mm-dd hh24:mi:ss') 时间,y.dev_name 路口,x.direction 方向 from EVENT_DATA x ,UNITY_DEVICE_INFO y where x.ip_addr = y.ip";
            if (!string.IsNullOrEmpty(type))
            {
                sql += " and check_type = '" + type + "'";
            }
            if (!string.IsNullOrEmpty(direction))
            {
                sql += " and direction = '" + direction + "'";
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                sql += " and x.check_time > to_timestamp('" + startTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sql += " and x.check_time <= to_timestamp('" + endTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }

            if (!string.IsNullOrEmpty(roadName))
            {
                sql += " and ip_addr in (select ip from UNITY_DEVICE_INFO where ip like '192.168.32%' and dev_name like '%" + roadName + "%')";
            }
            sql += " order by check_time";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    //List<CheckData> list = new List<CheckData>();
                    DataTable dt = ds.Tables[0];
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    CheckData cdi = new CheckData();
                    //    string temptype = dt.Rows[i][0].ToString();
                    //    cdi.check_type = Tools.GetCheckType(temptype);
                    //    cdi.check_time = dt.Rows[i][1].ToString();
                    //    cdi.dev_name = dt.Rows[i][2].ToString();
                    //    string direc = dt.Rows[i][3].ToString();
                    //    cdi.direction = Tools.GetCheckDire(direc);
                    //    list.Add(cdi);
                    //}
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("CheckEvenAll err:" + ex.Message);
                return null;
            }
            return null;     
        }

        /// <summary>
        /// 查询事件检测数据总条数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="rn">分页码</param>
        [WebMethod]
        public string CheckEvenCount(string roadName, string startTime, string endTime,string type, string direction)
        {
            string sql = "select count(*) from EVENT_DATA  where 1=1 ";
                        
            #region 拼接SQL语句
            if (!string.IsNullOrEmpty(type))
            {
                sql += " and check_type = '" + type + "'";
            }
            if (!string.IsNullOrEmpty(direction))
            {
                sql += " and direction = '" + direction + "'";
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                //startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM-dd HH:mm"); ;
                sql += " and check_time > to_timestamp('" + startTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sql += " and check_time <= to_timestamp('" + endTime + ":00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";
            }
            if (!string.IsNullOrEmpty(roadName))
            {
                sql += " and ip_addr in (select ip from UNITY_DEVICE_INFO where ip like '192.168.32%' and dev_name like '%" + roadName + "%')";
            }
            #endregion
            
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    string count = dt.Rows[0][0].ToString();
                    return count;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("CheckEvenData err:" + ex.Message);
                return "0";
            }
            return "0";
        }


        /// <summary>
        /// 查询最新N条数据
        /// </summary>
        /// <param name="num">条目数</param>
        [WebMethod]
        public string QueryEvenData(string num)
        {
            string sql = "select * from ( select to_char(a.check_time,'yyyy-MM-dd hh24:mi')passtime,a.check_type,a.DIRECTION,a.IP_ADDR,a.PIC_NAME,b.dev_name,b.lng,b.lat";
            sql += " from EVENT_DATA a,UNITY_DEVICE_INFO b where a.ip_addr = b.ip order by check_time desc ) where  rownum <= " + num;
            DataSet ds = OracleHelper.Query212(sql);
            if (Tools.DSisNull(ds))
            {
                DataTable dt = ds.Tables[0];
                string dataJson = Tools.DataTableToJson(dt);
                return dataJson;
            }
            return "";
        }

        #endregion

        #region 管制屏

        /// <summary>
        /// 添加管制屏计划
        /// </summary>
        /// <param name="_list"></param>
        /// <returns>返回被添加条目数</returns>
        [WebMethod]
        public string AddControlPlanArray(List<Control_Plan> _list)
        {
            if (_list != null && _list.Count > 0)
            {
                List<string> sqlList = new List<string>();
                foreach (Control_Plan plan in _list)
                {
                    string sql = "insert into control_plan values('" + plan.id + "','" + plan.name + "','" + plan.ip + "','" + plan.pic_name + "','" + plan.cross_name + "','" + plan.type + "')";
                    sqlList.Add(sql);
                }
                if (sqlList.Count > 0)
                {
                    try
                    {
                        int resCount = OracleHelper.ExecuteArray212(sqlList);
                        return resCount.ToString();
                    }
                    catch (Exception ex)
                    {
                        //Tools.WriteErrLog(sqlList[0]);
                        return ex.Message;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 添加单条管制屏计划
        /// </summary>
        /// <param name="_list"></param>
        /// <returns>success/fail/Exception</returns>
        [WebMethod]
        public string AddControlPlanSingel(Control_Plan plan)
        {
            string sql = "insert into control_plan values('" + plan.id + "','" + plan.name + "','" + plan.ip + "','" + plan.pic_name + "','" + plan.cross_name + "','" + plan.type + "')";
            try
            {
                string resCount = OracleHelper.ExecuteU_Dev_212(sql);
                return resCount;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 删除管制屏计划
        /// </summary>
        /// <param name="_list"></param>
        /// <returns>success/fail/Exception</returns>
        [WebMethod]
        public string DelControlPlanSingel(List<string> _id)
        {
            List<string> sqlList = new List<string>();
            foreach (string id in _id)
            {
                string sql = "delete from control_plan where plan_no = '" + id + "'";
                sqlList.Add(sql);
            }
            try
            {
                int resCount = OracleHelper.ExecuteArray212(sqlList);
                return resCount.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 删除车道屏计划
        /// </summary>
        /// <param name="_list"></param>
        /// <returns>success/fail/Exception</returns>
        [WebMethod]
        public string DelRoadPlanSingel(string PlanName)
        {
            string sql = "delete from control_plan where plan_name = '" + PlanName + "'";
            string resCount = OracleHelper.ExecuteU_Dev_212(sql);
            return resCount;
        }
        /// <summary>
        /// 查询计划名是否存在
        /// </summary>
        /// <param name="planName"></param>
        /// <returns>1-存在  0-不存在</returns>
        [WebMethod]
        public string SelectPlanName(string planName)
        {
            string sql = "select plan_name from control_plan where plan_name = '" + planName + "'";
            DataSet ds = OracleHelper.Query212(sql);
            if (Tools.DSisNull(ds))
            {
                return "1";
            }
            return "0";
        }

        
        /// <summary>
        /// group by plan_name
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<string> QueryControlPlanArray(string planType)
        {
            string sql = "select plan_name from control_plan where plan_type = '" + planType + "' group by plan_name";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<string> list = new List<string>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string name = dt.Rows[i][0].ToString();
                        list.Add(name);
                    }
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("QueryControlPlanArray err:" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据计划名查询计划详细
        /// </summary>
        /// <param name="planName"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Control_Plan> QueryControlPlanDetail(string planName)
        {
            if (!string.IsNullOrEmpty(planName))
            {
                string sql = "select * from control_plan where plan_name = '" + planName + "'";
                try
                {
                    DataSet ds = OracleHelper.Query212(sql);
                    if (Tools.DSisNull(ds))
                    {
                        DataTable dt = ds.Tables[0];
                        List<Control_Plan> list = new List<Control_Plan>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Control_Plan plan = new Control_Plan();
                            plan.id = dt.Rows[i][0].ToString();
                            plan.name = dt.Rows[i][1].ToString();
                            plan.ip = dt.Rows[i][2].ToString();
                            plan.pic_name = dt.Rows[i][3].ToString();
                            plan.cross_name = dt.Rows[i][4].ToString();
                            plan.type = dt.Rows[i][5].ToString();
                            list.Add(plan);
                        }
                        return list;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Tools.WriteErrLog("QueryControlPlanDetail err:" + ex.Message);
                    return null;
                }
            }
            return null;
        }


        /// <summary>
        /// 查询上下匝道屏计划
        /// </summary>
        /// <param name="planName"></param>
        /// <returns></returns>
        [WebMethod]
        public List<Control_Plan> QueryZaodaoPlanDetail()
        {
            
            string sql = "select * from control_plan where plan_type = '2' order by plan_pic";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<Control_Plan> list = new List<Control_Plan>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Control_Plan plan = new Control_Plan();
                        plan.id = dt.Rows[i][0].ToString();
                        plan.name = dt.Rows[i][1].ToString();
                        plan.ip = dt.Rows[i][2].ToString();
                        plan.pic_name = dt.Rows[i][3].ToString();
                        plan.cross_name = dt.Rows[i][4].ToString();
                        list.Add(plan);
                    }
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("QueryZaodaoPlanDetail err:" + ex.Message);
                return null;
            }
        }
        #endregion

        #region 龙灵山
        [WebMethod]
        public List<NovaScreenModel> GetLLSInfo()
        {
            string sql = "select screenname,direction,screenip,deviceip,roadnum,screentype from novascreen";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<NovaScreenModel> list = new List<NovaScreenModel>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NovaScreenModel screen = new NovaScreenModel();
                        screen.ScreenName = dt.Rows[i][0].ToString();
                        screen.Direction = dt.Rows[i][1].ToString();
                        screen.ScreenIP = dt.Rows[i][2].ToString();
                        screen.DeviceIP = dt.Rows[i][3].ToString();
                        screen.RoadNum = dt.Rows[i][4].ToString();
                        screen.ScreenType = dt.Rows[i][5].ToString();
                        list.Add(screen);
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

        #endregion
    }
}
