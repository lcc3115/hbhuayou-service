using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using HYSignServices.HYService;
using System.Drawing;
using HYSignServices.Entity;
using System.Net;
using System.IO;
using System.Text;

namespace HYSignServices
{
    /// <summary>
    /// LngLatChange 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class LngLatChange : System.Web.Services.WebService
    {

        [WebMethod]
        public int HelloWorld()
        {
            string sql = "select id,lng,lat from UNITY_DEVICE_INFO where ip like '192.168.32%' and ip not in ('192.168.32.11','192.168.32.15','192.168.32.16','192.168.32.19','192.168.32.20','192.168.32.23','192.168.32.24','192.168.32.27','192.168.32.28','192.168.32.31','192.168.32.32','192.168.32.35','192.168.32.36','192.168.32.39','192.168.32.40','192.168.32.43','192.168.32.44','192.168.32.55','192.168.32.56','192.168.32.59','192.168.32.60','192.168.32.63','192.168.32.64')";
            DataSet ds = OracleHelper.QueryU_Dev(sql);
            if (Tools.DSisNull(ds))
            {
                List<string> sqlList = new List<string>();
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string id = dt.Rows[i][0].ToString();
                    string lng = dt.Rows[i][1].ToString();
                    string lat = dt.Rows[i][2].ToString();
                    double lngBD = Convert.ToDouble(lng);
                    double latBD = Convert.ToDouble(lat);
                    Points point = ToolsDoc.Tools.GCJ02ToWGS84(lngBD, latBD);
                    string sqlEx = "update UNITY_DEVICE_INFO set lng = '" + point.x + "',lat = '" + point.y + "' where id = " + id + "";
                    sqlList.Add(sqlEx);
                }
                int res = OracleHelper.ExecuteArray(sqlList);
                return res;
            }
            return 0;
        }

        [WebMethod]
        public void TESTD()
        {
            WebProxy proxyObject = new WebProxy("120.78.201.239", 808);//str为IP地址 port为端口号 代理类
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http://www.whatismyip.com.tw"); // 访问这个网站 ，返回的就是你发出请求的代理ip 这个做代理ip测试非常方便，可以知道代理是否成功

            //HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http://www.baidu.com"); // 61.183.192.5
            Req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";

            Req.Proxy = proxyObject; //设置代理
            Req.Method = "GET";
            HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
            string StringSub = "";
            string OkStr = "";
            Encoding code = Encoding.GetEncoding("UTF-8");
            using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), code))
            {

                string str = sr.ReadToEnd();//获取得到的网址html返回数据，这里就可以使用某些解析html的dll直接使用了,比如htmlpaser 

            }




            ////System.Net.WebRequest webRequest = null;
            //WebProxy defaultProxy = new WebProxy();
            //defaultProxy.Address = new Uri("http://120.78.201.239:808");
            //defaultProxy.Credentials = new NetworkCredential("huayou", "Huayou1008611");
            //System.Net.WebRequest.DefaultWebProxy = defaultProxy;

            //try
            //{
            //    string url = "https://oapi.dingtalk.com/gettoken?appkey=dingygnpijklydmnku8j&appsecret=bOw2FjiM8KDEzuRdS_uP7469a7qNNTojirHKJiODAcyaBP6826VcSqXaxdZTUk72";
            //    string data = "";
            //    HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            //    reqest.Proxy = defaultProxy;
            //    reqest.Method = "POST";
            //    reqest.ContentType = "application/json";
            //    Stream stream = reqest.GetRequestStream();
            //    byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
            //    stream.Write(bs, 0, bs.Length);
            //    stream.Flush();
            //    stream.Close();
            //    HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
            //    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //    string value = sr.ReadToEnd();
            //    response.Close();
            //}
            //catch (Exception)
            //{
                
            //    throw;
            //}
        }

        [WebMethod]
        public void InsertUtcs(string cross_id, string time,string content)
        {
            string sql = "insert into UTCS_TEMP_DATA values('" + cross_id + "','" + time + "','" + content + "')";
            OracleHelper.Execute_local249(sql);
        }

        [WebMethod]
        public List<UTCS_DATA> LoadData(string id)
        {
            string sql = "select time,content from UTCS_TEMP_DATA where cross_id = '"+id+"' order by time";
            DataSet ds = OracleHelper.Query_local249(sql);
            if (Tools.DSisNull(ds))
            {
                List<UTCS_DATA> utcsList = new List<UTCS_DATA>();
                DataTable dt = ds.Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UTCS_DATA utcs = new UTCS_DATA();
                    utcs.time = dt.Rows[i][0].ToString();
                    utcs.content = dt.Rows[i][1].ToString();
                    utcsList.Add(utcs);
                }

                return utcsList;
            }
            return null;
        }

        public struct UTCS_DATA
        {
            public string time { get; set; }
            public string content { get; set; }
        }
    }
}
