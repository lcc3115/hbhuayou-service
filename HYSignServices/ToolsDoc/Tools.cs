using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HYSignServices.Entity;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Xml;
namespace HYSignServices.ToolsDoc
{
    public class Tools
    {
        public static double P2PDISTANCE = 80; //行驶方向正前方距离
        public static double RADIUS = 60;        //路口检测半径
        public static double DIRT_RADIUS = 40;        //方向检测半径
        public static string infoLOG_PATH = "E:\\utcs_infoLog.log";
        public static string errLOG_PATH = "E:\\errLog.log";

        private static string serverip = "172.92.1.105";
        private static int serverport = 8010;
        
        public static string localSeq = "";
        public static string tempStr = "";
        //上匝道卡口监控_监测是否上下桥用
        public static string[] UP_BRIDGE = { "192.168.80.112", "192.168.80.111", "192.168.80.113", "192.168.80.114", "192.168.80.134", "192.168.81.31", "192.168.81.30" };// "192.168.81.29", "192.168.81.81"
        //下匝道卡口监控_监测是否上下桥用
        public static string[] DOWN_BRIDGE = { "192.168.80.70", "192.168.80.99", "192.168.80.71", "192.168.80.110", "192.168.80.135", "192.168.81.28" };
        //宇视设备_登录用
        public static string[] ITS_DEVICES = { "192.168.80.134", "192.168.80.135" };
        //海康设备_登录用
        public static string[] HIK_DEVICES = { "192.168.80.112", "192.168.80.111", "192.168.80.113", "192.168.80.114", "192.168.81.31", "192.168.81.30", "192.168.80.70", "192.168.80.99", "192.168.80.71", "192.168.80.110", "192.168.81.28" };//"192.168.81.81","192.168.81.29"重复
        //ES请求地址
        public static string ESUrlStr = "http://192.168.96.209:9200/_xpack/sql";
        /// <summary>
        /// 路口数据 ID,NAME
        /// </summary>
        public static Dictionary<string, string> Crossing_Info;
        /// <summary>
        /// 计算点是否在矩形内
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="buttom"></param>
        /// <param name="pointToCheck"></param>
        /// <returns></returns>
        public static bool isInside(Points left, Points top, Points right, Points buttom, Points pointToCheck)
        {
            
            Line line1=Line.getLine(left, top),
                 line2=Line.getLine(top, right),
                 line3=Line.getLine(right, buttom),
                 line4=Line.getLine(buttom, left);
            if (Line.getValue(line1, pointToCheck) >= 0 &&
               Line.getValue(line2, pointToCheck) >= 0 &&
               Line.getValue(line3, pointToCheck) <= 0 &&
               Line.getValue(line4, pointToCheck) <= 0)
            {
                return true;
            }
            else {
                return false;
            } 
        }

        /// <summary>
        /// 缓存路口数据
        /// </summary>
        public static void GetCrossInfo()
        {
            if (Crossing_Info == null)
            {
                Crossing_Info = new Dictionary<string, string>();
                string sql = "select crossing_id,crossing_name from TRAFFIC_CROSSING_INFO";
                DataSet ds = OracleHelper.Query(sql);
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string id = dt.Rows[i][0].ToString();
                        string name = dt.Rows[i][1].ToString();
                        Crossing_Info.Add(id, name);
                    }
                }
            }
            
        }

        /// <summary>
        /// Js时间戳转换为时间
        /// </summary>
        /// <param name="jsTimeStamp"></param>
        /// <returns></returns>
        public static string GetJsTime(long jsTimeStamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Js时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetJsTimeStamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(time - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double distance(Points p1,Points p2)
        {
            double result = 0;
            result = Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y)*(p1.y - p2.y));
            return result;
        }

        public static string GetGISDirction_test(double m_dirction)
        {
            if (m_dirction != 0)
            {
                try
                {
                    //double dirction = double.Parse(m_dirction);
                    if (m_dirction >= 338 && m_dirction < 23)   //北
                    {
                        return "南进北出";    //南进北出
                    }
                    if (m_dirction >= 23 && m_dirction < 68)
                    {
                        return "西南进东北出";  //西南进东北出
                    }
                    if (m_dirction >= 68 && m_dirction < 113)
                    {
                        return "西进东出";  //西进东出
                    }
                    if (m_dirction >= 113 && m_dirction < 158)
                    {
                        return "西北进东南出";  //西北进东南出
                    }
                    if (m_dirction >= 158 && m_dirction < 203)
                    {
                        return "北进南出";  //北进南出
                    }
                    if (m_dirction >= 203 && m_dirction < 248)
                    {
                        return "东北进西南出";  //东北进西南出
                    }
                    if (m_dirction >= 248 && m_dirction < 293)
                    {
                        return "东进西出";  //东进西出
                    }
                    if (m_dirction >= 293 && m_dirction < 338)
                    {
                        return "东南进西北出";  //东南进西北出
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        ///// <summary>
        ///// 根据GPS方向值返回信号机进出口方向
        ///// </summary>
        ///// <param name="m_dirction"></param>
        ///// <returns>[进，出]</returns>
        //public static string[] GetGISDirction(double m_dirction)
        //{
        //    if (m_dirction != 0)
        //    {
        //        try
        //        {
        //            //double dirction = double.Parse(m_dirction);
        //            if (m_dirction >= 338 && m_dirction < 23)   //北
        //            {
        //                return new string[2] { "4", "0" };    //南进北出
        //            }
        //            if (m_dirction >= 23 && m_dirction < 68)
        //            {
        //                return new string[2] { "5", "1" };  //西南进东北出
        //            }
        //            if (m_dirction >= 68 && m_dirction < 113)
        //            {
        //                return new string[2] { "6", "2" };  //西进东出
        //            }
        //            if (m_dirction >= 113 && m_dirction < 158)
        //            {
        //                return new string[2] { "7", "3" };  //西北进东南出
        //            }
        //            if (m_dirction >= 158 && m_dirction < 203)
        //            {
        //                return new string[2] { "0", "4" };  //北进南出
        //            }
        //            if (m_dirction >= 203 && m_dirction < 248)
        //            {
        //                return new string[2] { "1", "5" };  //东北进西南出
        //            }
        //            if (m_dirction >= 248 && m_dirction < 293)
        //            {
        //                return new string[2] { "2", "6" };  //东进西出
        //            }
        //            if (m_dirction >= 293 && m_dirction < 338)
        //            {
        //                return new string[2] { "3", "7" };  //东南进西北出
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //    }
        //    return null;
        //}

        //public static void GetGISDirction()
        //{
            
        //}
        /*
	     * 大地坐标系资料WGS-84 长半径a=6378137 短半径b=6356752.3142 扁率f=1/298.2572236
	     */
	    /** 长半径a=6378137 */
        private static double a = 6378137;
	    /** 短半径b=6356752.3142 */
        private static double b = 6356752.3142;
	    /** 扁率f=1/298.2572236 */
        private static double f = 1 / 298.2572236;
        /// <summary>
        /// 根据经纬度和方向角计算指定距离外的点坐标
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="brng"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Points computerThatLonLat(double lon, double lat, double brng, double dist) {
 
		    double alpha1 = rad(brng);
		    double sinAlpha1 = Math.Sin(alpha1);
		    double cosAlpha1 = Math.Cos(alpha1);
 
		    double tanU1 = (1 - f) * Math.Tan(rad(lat));
		    double cosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1));
		    double sinU1 = tanU1 * cosU1;
            double sigma1 = Math.Atan2(tanU1, cosAlpha1);
		    double sinAlpha = cosU1 * sinAlpha1;
		    double cosSqAlpha = 1 - sinAlpha * sinAlpha;
		    double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
		    double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
		    double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
 
		    double cos2SigmaM=0;
		    double sinSigma=0;
		    double cosSigma=0;
		    double sigma = dist / (b * A), sigmaP = 2 * Math.PI;
		    while (Math.Abs(sigma - sigmaP) > 1e-12) {
                cos2SigmaM = Math.Cos(2 * sigma1 + sigma);
			    sinSigma = Math.Sin(sigma);
			    cosSigma = Math.Cos(sigma);
			    double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)
					    - B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
			    sigmaP = sigma;
			    sigma = dist / (b * A) + deltaSigma;
		    }
 
		    double tmp = sinU1 * sinSigma - cosU1 * cosSigma * cosAlpha1;
            double lat2 = Math.Atan2(sinU1 * cosSigma + cosU1 * sinSigma * cosAlpha1,
				    (1 - f) * Math.Sqrt(sinAlpha * sinAlpha + tmp * tmp));
            double lambda = Math.Atan2(sinSigma * sinAlpha1, cosU1 * cosSigma - sinU1 * sinSigma * cosAlpha1);
		    double C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
		    double L = lambda - (1 - C) * f * sinAlpha
				    * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
 
		    double revAz = Math.Atan2(sinAlpha, -tmp); // final bearing

            double lngtemp = lon + deg(L);
            double lattemp = deg(lat2);

            double lngnew = double.Parse(lngtemp.ToString("F6"));
            double latnew = double.Parse(lattemp.ToString("F6"));

            return new Points(lngnew, latnew);
	    }
        /**
	     * 度换成弧度
	     * 
	     * @param d
	     *            度
	     * @return 弧度
	     */
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /**
	     * 弧度换成度
	     * 
	     * @param x
	     *            弧度
	     * @return 度
	     */
        private static double deg(double x)
        {
            return x * 180 / Math.PI;
        }

        /// <summary>
        /// 点是否在圆内(在边上也认为在圆内)
        /// </summary>
        /// <param name="cPoint">圆心坐标</param>
        /// <param name="cRadius">圆半径</param>
        /// <param name="point">当前点</param>
        /// <returns></returns>
        public static bool PointInRadius(Points cPoint, double cRadius, Points point)
        {
            double d1 = GetDistance(cPoint.x, cPoint.y, point.x, point.y);
            if (d1 != 0)
            {
                return d1 < cRadius;
            }
            //double distance = Math.Sqrt(Math.Pow(Math.Abs(point.x - cPoint.x), 2) + Math.Pow(Math.Abs(point.y - cPoint.y), 2));
            return false;
        }

        public static bool IsInCircle(
            Points cPoint,
            double r,
            Points point)
        {
            double s = Math.Sqrt((point.x - cPoint.x) * (point.x - cPoint.x) + (point.y - cPoint.y) * (point.y - cPoint.y));
            return Math.Sqrt((point.x - cPoint.x) * (point.x - cPoint.x) + (point.y - cPoint.y) * (point.y - cPoint.y)) <= r;
        }

        private static double Distance(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6378137; //地球半径
            lat1 = lat1 * Math.PI / 180.0;
            lat2 = lat2 * Math.PI / 180.0;
            double sa2 = Math.Sin((lat1 - lat2) / 2.0);
            double sb2 = Math.Sin(((lon1 - lon2) * Math.PI / 180.0) / 2.0);
            return 2 * R * Math.Asin(Math.Sqrt(sa2 * sa2 + Math.Cos(lat1) * Math.Cos(lat2) * sb2 * sb2));
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            try
            {
                double EARTH_RADIUS = 6378137;
                double radLat1 = Rad(lat1);
                double radLng1 = Rad(lng1);
                double radLat2 = Rad(lat2);
                double radLng2 = Rad(lng2);
                double a = radLat1 - radLat2;
                double b = radLng1 - radLng2;
                double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }

        public static string LockPrase(string str, string id, string Entrance, string Exit, string startTime, string Duration)
        {
            localSeq = GetSeq();
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<Message>";
            xml += "<Version>1.1</Version>";
            xml += "<Token>hy_Token</Token>";
            xml += "<From>";
            xml += "<Address><Sys>TICP</Sys>";
            xml += "<SubSys/>";
            xml += "<Instance/>";
            xml += "</Address>";
            xml += "</From>";
            xml += "<To><Address>";
            xml += "<Sys>UTCS</Sys><SubSys/><Instance/>";
            xml += "</Address></To>";
            xml += "<Type>REQUEST</Type>";
            xml += "<Seq>" + localSeq + "</Seq>";
            xml += "<Body><Operation order=\"1\" name=\"Set\">";
            xml += "<" + str + ">";
            xml += "<CrossID>" + id + "</CrossID>";
            xml += "<Type>1</Type>";
            xml += "<Entrance>" + Entrance + "</Entrance>";
            xml += "<Exit>" + Exit + "</Exit>";
            if (!string.IsNullOrEmpty(startTime))
            {
                xml += "<StartTime>" + startTime + "</StartTime>";
                xml += "<Duration>" + Duration + "</Duration>";
            }
            xml += "</" + str + ">";
            xml += "</Operation></Body></Message>";
            return xml;
        }

        public static string SetXMLInfo(string type, string str1, string str2)
        {
            
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<Message>";
            xml += "<Token>hy_Token</Token>";
            xml += "<Body><Operation>";
            xml += "<" + type + ">";
            if (type == "GIS")
            {
                xml += "<LNG>" + str1 + "</LNG>";
                xml += "<LAT>" + str2 + "</LAT>";
            }
            else
            {
                xml += "<STARTIP>" + str1 + "</STARTIP>";
            }
            xml += "</" + type + ">";
            
            xml += "</Operation></Body></Message>";
            return xml;
        }

        
        
        public static string GetSeq()
        {
            string seq = System.DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            return seq;
        }

        static int inData = -1;
        //static string tempXml = "";
        static int inStart = 0;
        public static string ThreadLock(string param, string crossid, string NO)
        {
            string tempXml = string.Empty;
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(serverip, serverport);
            ClientSocket.ReceiveTimeout = 3000;
            byte[] data = new byte[1024 * 256];
            int i = 0;
            if (ClientSocket.Connected)
            {
                byte[] sendData = Encoding.UTF8.GetBytes(Tools.SetUTCSXML(param, crossid, NO));
                ClientSocket.Send(sendData, 0, sendData.Length, SocketFlags.None);
            }

            do
            {
                int readlen = 0;
                try
                {
                     readlen = ClientSocket.Receive(data, 0, data.Length, SocketFlags.None);
                    string txt = Encoding.UTF8.GetString(data, 0, readlen);
                    //Tools.WriteLog(txt,"D:/mLog.log");

                    if (!string.IsNullOrEmpty(Tools.localSeq) && txt.Contains(Tools.localSeq) || inData == 0)
                    {

                        inData = 0;
                        
                        int end = -1;

                        //处理报文头部被上一条报文尾部粘包的情况，inStart控制首次截取
                        if (inStart == 0)
                        {
                            int start = txt.IndexOf(Tools.localSeq);
                            tempXml = txt.Substring(start);
                            tempXml = "<Message><Seq>" + tempXml;
                            inStart = -1;
                        }

                        end = tempXml.IndexOf("</Message>");
                        //截取到尾部，并初始化所有参数
                        if (end > -1)
                        {
                            tempXml = tempXml.Substring(0, end + 10);

                            inData = -1;
                            inStart = 0;
                            if (ClientSocket != null)
                            {
                                if (ClientSocket.Connected)
                                {
                                    ClientSocket.Close();
                                }
                                ClientSocket = null;
                            }
                            return tempXml;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                i++;
            } while (i < 15);

            if (ClientSocket != null)
            {
                if (ClientSocket.Connected)
                {
                    ClientSocket.Close();
                }
                ClientSocket = null;
            }
            inData = -1;
            inStart = 0;
            return null;
        }

        public static XmlNodeList LoadXmlStr(string str)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(str);
            }
            catch (Exception ex)
            {
                return null;
            }
            XmlNodeList nodeList = doc.SelectNodes("Message/Body/Operation");
            XmlNodeList OperationList = nodeList[0].ChildNodes;
            return OperationList;
        }

        public static string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string SetUTCSXML(string info, string id, string NO)
        {
            localSeq = GetSeq();
            Tools.WriteLog(localSeq, "C:/mLog.log");
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<Message>";
            xml += "<Version>1.1</Version>";
            xml += "<Token/>";//hy_Token</Token>
            xml += "<From>";
            xml += "<Address><Sys>TICP</Sys>";
            xml += "<SubSys/>";
            xml += "<Instance/>";
            xml += "</Address>";
            xml += "</From>";
            xml += "<To><Address>";
            xml += "<Sys>UTCS</Sys><SubSys/><Instance/>";
            xml += "</Address></To>";
            xml += "<Type>REQUEST</Type>";
            xml += "<Seq>" + localSeq + "</Seq>";
            xml += "<Body><Operation order=\"1\" name=\"Get\">";
            xml += "<TSCCmd>";
            xml += "<ObjName>" + info + "</ObjName>";
            xml += "<ID>" + id + "</ID>";
            xml += "<No>" + NO + "</No>";
            xml += "</TSCCmd>";
            xml += "</Operation></Body></Message>";
            return xml;
        }

        public static Points GCJ02ToWGS84(double mgLon,double mgLat )
        {
            Points point = Delta(mgLat, mgLon);
            double wgLat = mgLat - point.y;
            double wgLon = mgLon - point.x;
            return new Points(wgLon,wgLat);
        }

        private static Points Delta(double Lat, double Lon)
        {
            const double AXIS = 6378245.0;
            const double EE = 0.00669342162296594323;
            const double PI = 3.1415926535897932384626;
            double dLat = TransformLat(Lon - 105.0, Lat - 35.0);
            double dLon = TransformLon(Lon - 105.0, Lat - 35.0);
            double radLat = Lat / 180.0 * PI;
            double magic = Math.Sin(radLat);
            magic = 1 - EE * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((AXIS * (1 - EE)) / (magic * sqrtMagic) * PI);
            dLon = (dLon * 180.0) / (AXIS / sqrtMagic * Math.Cos(radLat) * PI);
            return new Points(dLon,dLat);
        }

        #region WGS84转GCJ-02

        const double pi = 3.14159265358979324;//Math.PI;//;
        const double e = 6378245.0;
        const double ee = 0.00669342162296594323;
        /// <summary>
        /// WGS84转GCJ-02
        /// </summary>
        /// <param name="wgsPoint"></param>
        /// <returns></returns>
        public static Points WGS84ToGCJ02(Points wgsPoint)
        {

            Points _transPoint = Transform(wgsPoint);
            return new Points(wgsPoint.x + _transPoint.x, wgsPoint.y + _transPoint.y);
        }

        private static Points Transform(Points point)
        {
            
            double _lat = TransformLat(point.x - 105.0, point.y - 35.0);
            double _lon = TransformLon(point.x - 105.0, point.y - 35.0);
            double _radLat = point.y / 180.0 * pi;
            double _magic = Math.Sin(_radLat);
            _magic = 1 - ee * _magic * _magic;
            double _sqrtMagic = Math.Sqrt(_magic);
            _lat = (_lat * 180.0) / ((e * (1 - ee)) / (_magic * _sqrtMagic) * pi);
            _lon = (_lon * 180.0) / (e / _sqrtMagic * Math.Cos(_radLat) * pi);
            //_transPoint.LatY = _lat;
            //_transPoint.LonX = _lon;
            Points _transPoint = new Points(_lon, _lat);
            return _transPoint;
        }

        private static double TransformLat(double lonX, double latY)
        {
            double _ret = -100.0 + 2.0 * lonX + 3.0 * latY + 0.2 * latY * latY + 0.1 * lonX * latY + 0.2 * Math.Sqrt(Math.Abs(lonX));
            _ret += (20.0 * Math.Sin(6.0 * lonX * pi) + 20.0 * Math.Sin(2.0 * lonX * pi)) * 2.0 / 3.0;
            _ret += (20.0 * Math.Sin(latY * pi) + 40.0 * Math.Sin(latY / 3.0 * pi)) * 2.0 / 3.0;
            _ret += (160.0 * Math.Sin(latY / 12.0 * pi) + 320 * Math.Sin(latY * pi / 30.0)) * 2.0 / 3.0;
            return _ret;
        }
        private static double TransformLon(double lonX, double latY)
        {
            double _ret = 300.0 + lonX + 2.0 * latY + 0.1 * lonX * lonX + 0.1 * lonX * latY + 0.1 * Math.Sqrt(Math.Abs(lonX));
            _ret += (20.0 * Math.Sin(6.0 * lonX * pi) + 20.0 * Math.Sin(2.0 * lonX * pi)) * 2.0 / 3.0;
            _ret += (20.0 * Math.Sin(lonX * pi) + 40.0 * Math.Sin(lonX / 3.0 * pi)) * 2.0 / 3.0;
            _ret += (150.0 * Math.Sin(lonX / 12.0 * pi) + 300.0 * Math.Sin(lonX / 30.0 * pi)) * 2.0 / 3.0;
            return _ret;
        }
        #endregion

        //百度转高德
        public static double[] bd09_To_Gcj02(double lat, double lon)
        {
            
            double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
            double x = lon - 0.0065, y = lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            double tempLon = z * Math.Cos(theta);
            double tempLat = z * Math.Sin(theta);
            double[] gps = { tempLat, tempLon };
            return gps;
        }  

        public static void WriteLog(string msg ,string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("{0}\n{1}\r\n", DateTime.Now + " - Clint_Message:", msg);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                //LocationManager.Clint_SendMsg(ex.Message);
                
            }

        }

        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }


        public static System.Data.DataTable CsvToDataTable(string filePath, int n)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            StreamReader reader = new StreamReader(filePath, System.Text.Encoding.Default, false);
            int m = 0;
            while (!reader.EndOfStream)
            {
                m = m + 1;
                string str = reader.ReadLine();
                string[] split = str.Split(',');
                if (m == n)
                {
                    System.Data.DataColumn column; //列名
                    for (int c = 0; c < split.Length; c++)
                    {
                        column = new System.Data.DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = split[c];
                        if (dt.Columns.Contains(split[c]))                 //重复列名处理
                            column.ColumnName = split[c] + c;
                        dt.Columns.Add(column);
                    }
                }
                if (m >= n + 1)
                {
                    System.Data.DataRow dr = dt.NewRow();
                    for (int i = 0; i < split.Length; i++)
                    {
                        dr[i] = split[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            reader.Close();
            return dt;
        }

        
    }
}