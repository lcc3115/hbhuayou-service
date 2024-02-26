using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AccidentInvestigation.Model;
using HYSignServices.ToolsDoc;
using System.IO;
using HYSignServices.Entity;
using System.Data;

namespace HYSignServices.HYService
{
    /// <summary>
    /// UpLoadData 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class UpLoadData : System.Web.Services.WebService
    {

        [WebMethod]
        public string AddMaker(string img, string lng, string lat, string dangerType, string dangerLevel, string streetName, string roadName,string title,string content)
        {
            //Tools.WriteErrLog(img + "\r\n" + lng + "\r\n" + lat + "\r\n" + dangerType + "\r\n" + dangerLevel + "\r\n" + streetName + "\r\n" + roadName + "\r\n" + title + "\r\n" + content);
            MarkerData data = new MarkerData ();
            XmlHelper xh = new XmlHelper(@"D:\HYAccidentInvestigation\XML\Maker.xml");

            data.Lng = lng;
            data.Lat = lat;
            EnumList.DangerType danger;
            Enum.TryParse(dangerType, out danger);
            data.DangerType = danger;
            data.DangerLevel = int.Parse(dangerLevel);
            data.StreetName = streetName;
            data.RoadName = roadName;
            data.Time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Title = title;
            data.Content = content;
            data.ImageList = new List<string>();
            string path = Base64ToImg(img);
            data.ImageList.Add(path);
            string res = xh.AddMaker(data);
            return res;
        }

        private string Base64ToImg(string imgStr)
        {
            byte[] arr2 = Convert.FromBase64String(imgStr);
            string filePath = "D:\\HYAccidentInvestigation\\Upload\\Image\\";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            using (MemoryStream ms2 = new MemoryStream(arr2))
            {
                System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms2);
                bmp2.Save(filePath + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp2.Save(filePath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bmp2.Save(filePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                //bmp2.Save(filePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            return "/Upload/Image/" + fileName;
        }

        #region 气象数据

        /// <summary>
        /// 气象数据插入数据库
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [WebMethod]
        public string InsertModbusData(ModbusModel item)
        {
            string sql = "INSERT ";
            try
            {
                string sqlContent = string.Format("INTO METEOROLOGICAL (IP, WINDSPEED, WINDDIRECTION, TEMPERATURE,HUMIDITY,PRESSURE,RAINFALL,VISIBILITY,RECEIVETIME,ROAD_TEMPERATURE,ROAD_PONDING,ROAD_ICEACCRETION,ROAD_SNOWCOVER,SLIPPERY)\n");
                string sqlValue = string.Format(" VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}',{7},to_date('{8}','yyyy-mm-dd hh24:mi:ss'),'{9}','{10}','{11}','{12}','{13}')\n", item.IP, item.WindSpeed, item.WindDirection, item.Temperature, item.Humidity, item.Pressure, item.Rainfall, item.Visibility, item.Time, item.Road_Temperature, item.Road_Ponding, item.Road_IceAccretion, item.Road_SnowCover, item.Slippery);
                sql += sqlContent;
                sql += sqlValue;
                string result = OracleHelper.ExecuteU_Dev_212(sql);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


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
        /// 查询每个车道屏的执行状态
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<ScreenInfo> GetScreenStatusDetial()
        {
            string sql = "select ip,status from MANUALLANESCREEN";
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
                        screen.SCREEN_IP_LINE1 = dt.Rows[i][0].ToString();
                        screen.NEWCARD = dt.Rows[i][1].ToString();
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
