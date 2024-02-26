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
    /// ARService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ARService : System.Web.Services.WebService
    {
        //删除设备
        [WebMethod]
        public string DeleteDevice(string id)
        {

            string sql = "delete  from HIK_MARK_INFO where id = '" + id + "'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        //删除方向标签
        [WebMethod]
        public string DeleteDevice_Mark(string id)
        {
            string sql = "delete from DIRECTION_MARK_INFO where id = '" + id + "'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        //通过ID查找设备
        [WebMethod]
        public UpdateMonitorWin ResearchDevice(string id)
        {
                          
            string sql = "select MARKADDRESS,PORT,MARKUSERNAME,MARKPW,TYPE,markname ,XIW,XIH,cate,direction from HIK_MARK_INFO where id = '" + id + "'";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    UpdateMonitorWin win = new UpdateMonitorWin();
                    DataTable dt = ds.Tables[0];
                    win.ip = dt.Rows[0][0].ToString();
                    win.port = dt.Rows[0][1].ToString();
                    win.user = dt.Rows[0][2].ToString();
                    win.pw = dt.Rows[0][3].ToString();
                    win.type = dt.Rows[0][4].ToString();
                    win.name = dt.Rows[0][5].ToString();
                    win.xiw = dt.Rows[0][6].ToString();
                    win.xih = dt.Rows[0][7].ToString();
                    win.cate = dt.Rows[0][8].ToString();
                    win.dir = dt.Rows[0][9].ToString();
                    return win;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //通过IP查找设备
        [WebMethod]
        public List<UpdateMonitorWin> ResearchIPAddress(string ip)
        {
            string sql = "select id,markname,cate from HIK_MARK_INFO where belongdev = '" + ip + "'";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<UpdateMonitorWin> list = new List<UpdateMonitorWin>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        UpdateMonitorWin win = new UpdateMonitorWin();
                        win.id = dr["id"].ToString();
                        win.name = dr["markname"].ToString();
                        win.type = dr["cate"].ToString();
                        list.Add(win);
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

        //通过IP查找设备
        [WebMethod]
        public List<DirectionInfo> ResearchDirection(string ip)
        {
            string sql = "select ID,MARK_NAME,P,T,X,Y,ANGLE,BELONG,XIX,XIY from DIRECTION_MARK_INFO where belong = '" + ip + "'";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<DirectionInfo> list = new List<DirectionInfo>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        DirectionInfo DI = new DirectionInfo();
                        DI.id = dr["ID"].ToString();
                        DI.mark_name = dr["MARK_NAME"].ToString();
                        DI.p = dr["p"].ToString();
                        DI.t = dr["t"].ToString();
                        DI.x = dr["x"].ToString();
                        DI.y = dr["y"].ToString();
                        DI.angle = dr["angle"].ToString();
                        DI.belong = dr["belong"].ToString();
                        DI.xiX = dr["XIX"].ToString();
                        DI.xiY = dr["XIY"].ToString();
                        list.Add(DI);
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

        [WebMethod]
        public string InsertDevice(string devID, string devName, string devIP, string devUser, string devPw, string xiX, string xiY, string type, string devPort, string cate, string belongdev, string dir)
        {
            string sql = "insert into HIK_MARK_INFO values('" + devID + "','" + devName + "','" + devIP + "','" + devUser + "','" + devPw + "','" + xiX + "','" + xiY + "','" + type + "'," + devPort + ",'" + cate + "','" + belongdev + "','" + dir + "','0','0','0','0')";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [WebMethod]
        public string InsertDirectionMark(string devID, string devName, string belong, string angle,string xiX,string xiY)
        {
            string sql = "insert into DIRECTION_MARK_INFO values('" + devID + "','" + devName + "','0','0','0','0','" + angle + "','" + belong + "','" + xiX + "','" + xiY + "')";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //通过归属主相机查找设备
        [WebMethod]
        public List<UpdateMonitorWin> ResearchDataByIP(string ip)
        {
            string sql = "select id,markname,markaddress,markusername,markpw,type,port,xiw,xih,cate,DIRECTION,X,Y,P,T from HIK_MARK_INFO where belongdev = '" + ip + "'";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    List<UpdateMonitorWin> list = new List<UpdateMonitorWin>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UpdateMonitorWin win = new UpdateMonitorWin();
                        
                        win.id = dt.Rows[i][0].ToString();
                        win.name = dt.Rows[i][1].ToString();
                        win.ip = dt.Rows[i][2].ToString();
                        win.user = dt.Rows[i][3].ToString();
                        win.pw = dt.Rows[i][4].ToString();
                        win.type = dt.Rows[i][5].ToString();
                        win.port = dt.Rows[i][6].ToString();
                        win.xiw = dt.Rows[i][7].ToString();
                        win.xih = dt.Rows[i][8].ToString();
                        win.cate = dt.Rows[i][9].ToString();
                        win.dir = dt.Rows[i][10].ToString();
                        win.X = dt.Rows[i][11].ToString();
                        win.Y = dt.Rows[i][12].ToString();
                        win.P = dt.Rows[i][13].ToString();
                        win.T = dt.Rows[i][14].ToString();
                        list.Add(win);
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

        [WebMethod]
        public string UpdateDeviceInfo(string devName, string devIP, string devPort, string devUser, string devPw, string type, string cate, string devID)
        {
            string sql = "update HIK_MARK_INFO set markname = '" + devName + "',MARKADDRESS = '" + devIP + "',PORT = " + devPort + ",";
            sql += "MARKUSERNAME = '" + devUser + "',MARKPW = '" + devPw + "',TYPE = '" + type + "',cate = '" + cate + "' where id = '" + devID + "'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //退出或切换主相机时保存当前场景中的设备位置信息
        [WebMethod]
        public string ExitToSaveLocation(string devID,string P,string T,string X,string Y)
        {
            string sql = "update HIK_MARK_INFO set P = '" + P + "',T = '" + T + "',X = " + X + ",";
            sql += "Y = '" + Y + "' where id = '" + devID + "'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string ExitToSaveMark(string devID, string P, string T, string X, string Y)
        {
            string sql = "update DIRECTION_MARK_INFO set P = '" + P + "',T = '" + T + "',X = " + X + ",";
            sql += "Y = '" + Y + "' where id = '" + devID + "'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
