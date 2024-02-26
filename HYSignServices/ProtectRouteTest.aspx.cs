using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using blqw;
using System.Data;
using System.Text;
using HYSignServices.ToolsDoc;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using HYSignServices.Entity;
using System.Xml;

namespace HYSignServices
{
    public partial class ProtectRouteTest : System.Web.UI.Page
    {
        static string m_lng;
        static string m_lat;
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax2.Register(this);
            
        }

        public void SetMaker(string lonLat)
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "loadDeviceFromTree", "<script>loadDeviceFromTree('" + lonLat + "')</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('" + lonLat + "')</script>");
        }

        [blqw.AjaxMethod]
        public string GetUTCSInfo()
        {
            //DirectoryInfo cro_files = new DirectoryInfo(@"C:\Users\Administrator\Desktop\PROTECTROUTE\bin\conf\cross_info.csv");
            //System.Data.DataTable cro_dt = Tools.CsvToDataTable(cro_files.FullName, 1);
            //if (cro_dt.Rows.Count > 0)
            //{
            //    //StartConnect();
            //    return DataTableToJson(cro_dt);
            //}
            //return null;
            //string sql = "select * from cross_info where cross_no in(420100021193,420100021194,420100021195,420100023072,420100023073,420100023074,420100023081,420100023114,420100023115,420100023116,420100023117,420100023118,420100023119,420100023120,420100023122,420100023123,420100023124,420100023125,420100023126,420100023127,420100023128,420100023129,420100023130,420100023131,420100023132,420100023133,420100023134,420100023135,420100023136,420100023137,420100023138,420100023139,420100023140,420100023153,420100023154,420100023155,420100023156,420100023157,420100023158,420100023159,420100023160,420100023161,420100023179,420100023180,420100023181,420100023184,420100023185,420100023186,420100023192,420100105183,420100105187,420100105188,420100105189,420100105190,420100105191,420100105196,420100105197,420100105198,420100105199,420100105200)";
            string sql = "select ID,DEV_NAME,LNG,LAT from UNITY_DEVICE_INFO where LNG is not null";
            DataSet ds = OracleHelper.QueryU_Dev(sql);
            DataTable dt = ds.Tables[0];
            return DataTableToJson(dt);

        }

        [blqw.AjaxMethod]
        public string GetScreenInfo()
        {
            string sql = "select SCREEN_NO,lng,lat from SCREEN_INFO";
            DataSet ds = OracleHelper.QueryU_Dev(sql);
            DataTable dt = ds.Tables[0];
            return DataTableToJson(dt);
            
            //return null;
        }
        [blqw.AjaxMethod]
        public string SetDirt(string no, string dirt1, string dirt2)
        {
            try
            {
                string sql = "insert into SCREEN_DIRT values(" + no + "," + dirt1 + "," + dirt2 + ")";
                string res = OracleHelper.ExecuteU_Dev(sql);
                if (res == "success")
                {
                    return "成功";
                }
                return "失败";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        [blqw.AjaxMethod]
        public string SetScreenInfo(string data)
        {
            try
            {
                string[] temp = data.Split(',');
                string no = temp[0];
                string lng = temp[1];
                string lat = temp[2];

                string sql = "update SCREEN_INFO set lng = '" + lng + "',lat = '" + lat + "' where SCREEN_NO = " + no + "";
                string res = OracleHelper.ExecuteU_Dev(sql);
                if (res == "success")
                {
                    return "成功";
                }
                return "失败";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            

            
        }

        [blqw.AjaxMethod]
        public void SetLngLatByID(string id, string lng, string lat)
        {
            string sql = "update UNITY_DEVICE_INFO set LNG = '" + lng + "',LAT = '" + lat + "' where ID = '" + id + "'";
            OracleHelper.ExecuteU_Dev(sql);
        }

        [blqw.AjaxMethod]
        public string getLngLat()
        {
            return m_lng + "," + m_lat;
        }
        public void ShowMessage(string msg)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('" + msg + "')</script>");
        }

        private String DataTableToJson(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "";
            }

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        [blqw.AjaxMethod]
        public string GetDirt(string lng1,string lat1,string lng2,string lat2)
        {

            return null;
        }

        Socket ClientSocket;
        Thread thread;
        public void StartConnect()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //ClientSocket.Connect("113.57.86.182", 8058);
                ClientSocket.Connect("192.168.0.236", 8058);
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                Console.WriteLine("初始化错误消息" + ss);
                return;
            }

            thread = new Thread(new ParameterizedThreadStart(ReceiveData));
            thread.IsBackground = true;
            thread.Start(ClientSocket);

        }

        private void ReceiveData(object obj)
        {
            Socket Socket = obj as Socket;
            byte[] data = new byte[1024 * 1024];

            while (true)
            {
                int readlen = 0;
                try
                {
                    readlen = Socket.Receive(data, 0, data.Length, SocketFlags.None);
                    string txt = Encoding.UTF8.GetString(data, 0, readlen);
                    if (txt.Substring(0, 3) == "GIS")
                    {
                        string[] array = txt.Split('_');
                        string[] gisarray = array[1].Split(',');
                        //var dirt = array[1];
                        //string lng = gisarray[0];
                        //string lat = gisarray[1];
                        //SetMaker(lng + "," + lat);
                        m_lng = gisarray[0];
                        m_lat = gisarray[1];
                    }
                }
                catch (Exception ex)
                {
                    //ReadExcel.WriteErrLog(ex.Message);
                    //异常退出时  
                    //StopConnetct();
                    return;
                }
            }
        }

        public void Send_Msg(string msg)
        {

            if (ClientSocket != null && ClientSocket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                ClientSocket.Send(data, 0, data.Length, SocketFlags.None);
            }
        }

        [blqw.AjaxMethod]
        public void Search_Devices(string no,string lnglat, string dirt1, string dirt2)
        {
            try
            {
                //114.1556237891091,30.49683291155828 CL003
                //114.1557870041621,30.49673121698459
                //@"C:\Users\Administrator\Desktop\匝道屏.xml"
                XmlDocument doc = new XmlDocument();
                doc.Load(@"C:\Users\Administrator\Desktop\匝道屏1.xml");

                XmlNodeList Folder = doc.SelectNodes("Document/Folder");
                XmlNodeList FolderList = Folder[0].ChildNodes;
                foreach (XmlNode item in FolderList)
                {
                    string name = item.Name;
                    if (name == "Placemark")
                    {
                        XmlNodeList PlacemarkList = item.ChildNodes;
                        foreach (XmlNode item1 in PlacemarkList)
                        {
                            string name1 = item1.Name;
                            if (name1 == "Point")
                            {
                                XmlNodeList PointList = item1.ChildNodes;
                                XmlNode point = PointList[0];
                                string name2 = point.Name;
                                string v2 = point.InnerText;
                                string[] v2_xy = v2.Split(',');
                                double lng = double.Parse(v2_xy[0]);
                                double lat = double.Parse(v2_xy[1]);
                                Points p = Tools.WGS84ToGCJ02(new Points(lng, lat));
                                string lnt_t = p.x.ToString("F6");
                                string lat_t = p.y.ToString("F6");
                                string sql = "insert into SCREEN_INFO(SCREEN_NO,lng,lat) values(seq_scr.nextval,'" + lnt_t + "','" + lat_t + "')";
                                OracleHelper.ExecuteU_Dev(sql);
                            }
                        }
                    }
                }
                //Points o = Tools.WGS84ToGCJ02(new Points(114.1557870041621, 30.49673121698459));
                //string p = o.x + "," + o.y;
                //string[] temp = lnglat.Split(',');
                //string lng = temp[0];
                //string lat = temp[1];
                //string sql = "insert into SCREEN_INFO(SCREEN_NO,lng,lat) values('" + no + "','" + lng + "','" + lat + "')";
                //OracleHelper.ExecuteU_Dev(sql);
                //string sql1 = "insert into SCREEN_DIRT values('" + no + "','" + dirt1 + "','" + dirt2 + "')";
                //OracleHelper.ExecuteU_Dev(sql1);
            }
            catch (Exception ex)
            {
                
            }
            
        }

        [blqw.AjaxMethod]
        public string testjson()
        {
            try
            {
                
                string sql = "select to_char(check_time,'yyyy-MM-dd hh24:mi')passtime,check_type,DIRECTION,IP_ADDR,PIC_NAME from EVENT_DATA " +
                            " where CHECK_TIME >= to_timestamp('2019-11-26 12:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff') and " +
                            " CHECK_TIME <= to_timestamp('2019-11-26 16:00:00.000000', 'yyyy-mm-dd hh24:mi:ss.ff')";

                DataSet ds = OracleHelper.QueryU_Dev(sql);
                
                DataTable dt = ds.Tables[0];
                string dataJson = HYSignServices.HYService.Tools.DataTableToJson(dt);

                return dataJson;
            }
            catch (Exception ex)
            {

            }
            return null ;
        }
    }
}