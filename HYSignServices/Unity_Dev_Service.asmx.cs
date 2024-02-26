using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using HYSignServices.Entity;
using System.Net.NetworkInformation;
using System.Threading;
using HYSignServices.HYService;

namespace HYSignServices
{
    /// <summary>
    /// Unity_Dev_Service 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Unity_Dev_Service : System.Web.Services.WebService
    {
        [WebMethod]
        public List<u_dev_class> Get_U_DevInfoWithLngLat()
        {
            //string ip = Context.Request.UserHostAddress;
            //List<string> ips = new List<string>() { "192.168.10.105", "192.168.10.103", "192.168.10.106", "192.168.10.102" };
            //string tempSql = string.Empty;
            //if (!ips.Contains(ip))
            //{
            //    return null;
            //}
            string sql = "select * from UNITY_DEVICE_INFO where lng is not null";// and port = 8000";// and ip in('192.168.81.7','192.168.81.20','192.168.81.100','192.168.81.101','192.168.80.183')
            
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (Tools.DSisNull(ds))
                {
                    
                    List<u_dev_class> list = new List<u_dev_class>();
                    DataTable dt = ds.Tables[0];
                    
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        
                        u_dev_class dev = new u_dev_class();
                        dev.id = int.Parse(dt.Rows[i][0].ToString());
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.idtype = dt.Rows[i][2].ToString();
                        dev.entityGroupName = dt.Rows[i][3].ToString();
                        dev.monitorType = dt.Rows[i][4].ToString();
                        dev.ip = dt.Rows[i][5].ToString();
                        dev.prot = int.Parse(dt.Rows[i][6].ToString());
                        dev.user_name = dt.Rows[i][7].ToString();
                        dev.password = dt.Rows[i][8].ToString();
                        dev.position = dt.Rows[i][9].ToString();
                        dev.rotation = dt.Rows[i][10].ToString();
                        dev.remark = dt.Rows[i][11].ToString();
                        dev.initRotation = dt.Rows[i][12].ToString();
                        dev.belong = dt.Rows[i][13].ToString();
                        dev.lng = dt.Rows[i][14].ToString();
                        dev.lat = dt.Rows[i][15].ToString();
                        dev.tag = i.ToString();
                        list.Add(dev);

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
        public List<u_dev_class> Get_U_DevInfo249()
        {
            string sql = "select * from UNITY_DEVICE_INFO where lng is not null";// and port = 8000";// and ip in('192.168.81.7','192.168.81.20','192.168.81.100','192.168.81.101','192.168.80.183')
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (Tools.DSisNull(ds))
                {

                    List<u_dev_class> list = new List<u_dev_class>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        u_dev_class dev = new u_dev_class();
                        dev.id = int.Parse(dt.Rows[i][0].ToString());
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.idtype = dt.Rows[i][2].ToString();
                        dev.entityGroupName = dt.Rows[i][3].ToString();
                        dev.monitorType = dt.Rows[i][4].ToString();
                        dev.ip = dt.Rows[i][5].ToString();
                        dev.prot = int.Parse(dt.Rows[i][6].ToString());
                        dev.user_name = dt.Rows[i][7].ToString();
                        dev.password = dt.Rows[i][8].ToString();
                        dev.position = dt.Rows[i][9].ToString();
                        dev.rotation = dt.Rows[i][10].ToString();
                        dev.remark = dt.Rows[i][11].ToString();
                        dev.initRotation = dt.Rows[i][12].ToString();
                        dev.belong = dt.Rows[i][13].ToString();
                        dev.lng = dt.Rows[i][14].ToString();
                        dev.lat = dt.Rows[i][15].ToString();
                        dev.tag = i.ToString();
                        list.Add(dev);

                    }
                    if (list.Count > 0)
                    {
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        [WebMethod]
        public string Update_U_DevInfoWithLngLat(string lng,string lat ,string ip)
        {
            string sql = "update UNITY_DEVICE_INFO set lng = '" + lng + "',lat = '" + lat + "' where ip = '"+ip+"'";
            try
            {
                string res = OracleHelper.ExecuteU_Dev(sql);
                return res;
            }
            catch (Exception ex)
            {
                return "fial";
            }
        }

        private void ExcutePingArray(List<u_dev_class> list)
        {
            tagArray = new List<string>();
            foreach (u_dev_class dev in list)
            {
                if (!dev.ip.Contains("192.168.32"))
                {
                    Thread thread = new Thread(PingHost);
                    thread.IsBackground = true;
                    thread.Start(dev.ip + "," + dev.tag);
                }
                
            } 
        }
        //存放ping不通的IP的tag
        private List<string> tagArray;
        /// <summary>
        /// ping每个地址，将ping不通的地址tag存进tagArray中
        /// </summary>
        /// <param name="ip"></param>
        private void PingHost(object ip)
        {
            try
            {
                string obj = ip.ToString();
                string[] objStr = obj.Split(',');
                string device_ip = objStr[0];
                string index = objStr[1];
                Ping ping = new Ping();
                PingReply pingreply = ping.Send(device_ip);
                if (pingreply.Status != IPStatus.Success)
                {
                    tagArray.Add(index);
                }
            }
            catch (Exception ex)
            {

            }
        }

        [WebMethod]
        public List<u_dev_class> Get_U_DevInfo()
        {
            string sql = "select * from UNITY_DEVICE_INFO";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<u_dev_class> list = new List<u_dev_class>();
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        u_dev_class dev = new u_dev_class();
                        dev.id = int.Parse(dt.Rows[i][0].ToString());
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.idtype = dt.Rows[i][2].ToString();
                        dev.entityGroupName = dt.Rows[i][3].ToString();
                        dev.monitorType = dt.Rows[i][4].ToString();
                        dev.ip = dt.Rows[i][5].ToString();
                        dev.prot = int.Parse(dt.Rows[i][6].ToString());
                        dev.user_name = dt.Rows[i][7].ToString();
                        dev.password = dt.Rows[i][8].ToString();
                        dev.position = dt.Rows[i][9].ToString();
                        dev.rotation = dt.Rows[i][10].ToString();
                        dev.remark = dt.Rows[i][11].ToString();
                        dev.initRotation = dt.Rows[i][12].ToString();
                        dev.belong = dt.Rows[i][13].ToString();
                        
                        list.Add(dev);
                    }
                    if (list.Count > 0)
                    {
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }
        //http://192.168.0.236:8011/Unity_Dev_Service.asmx/Add_Dev?dev_name=路口名&assetName=监控名称&entityGroupName=实体名称&monitorType=监控类型&ip=192.x.x.x&port=8000&user_name=admin&password=kfqdd212&position=位置&rotation=角度
        [WebMethod]
        public string Add_Dev(string dev_name, string idtype, string entityGroupName, string monitorType, string ip, string port, string user_name, string password, string position, string rotation, string remark, string initRotation)
        {
            string sql = "insert into UNITY_DEVICE_INFO(ID,DEV_NAME,IDTYPE,ENTITYGROUPNAME,ENTITYGROUPNAME,IP,PORT,USER_NAME,PASSWORD,POSITION,ROTATION,REMARK,INITROTATION) "+
                        "values(seq_udev.nextval,'" + dev_name + "','" + idtype + "','" + entityGroupName + "','" + monitorType + "','" + ip + "'," + int.Parse(port) + ",'" + user_name + "','" + password + "','" + position + "','" + rotation + "','" + remark + "')";
            string res = OracleHelper.ExecuteU_Dev(sql);
            return res;
        }

        [WebMethod]
        public string Del_Dev(string dev_id)
        {
            string sql = "delete from UNITY_DEVICE_INFO where id = " + dev_id + "";
            string res = OracleHelper.ExecuteU_Dev(sql);
            return res;
        }

        [WebMethod]
        public List<u_dev_class> Get_U_DevInfoByBelong(string belong)
        {
            string sql = "select * from UNITY_DEVICE_INFO where belong = '" + belong + "'";
            try
            {
                DataSet ds = OracleHelper.QueryU_Dev(sql);
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<u_dev_class> list = new List<u_dev_class>();
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        u_dev_class dev = new u_dev_class();
                        dev.id = int.Parse(dt.Rows[i][0].ToString());
                        dev.dev_name = dt.Rows[i][1].ToString();
                        dev.idtype = dt.Rows[i][2].ToString();
                        dev.entityGroupName = dt.Rows[i][3].ToString();
                        dev.monitorType = dt.Rows[i][4].ToString();
                        dev.ip = dt.Rows[i][5].ToString();
                        dev.prot = int.Parse(dt.Rows[i][6].ToString());
                        dev.user_name = dt.Rows[i][7].ToString();
                        dev.password = dt.Rows[i][8].ToString();
                        dev.position = dt.Rows[i][9].ToString();
                        dev.rotation = dt.Rows[i][10].ToString();
                        dev.remark = dt.Rows[i][11].ToString();
                        dev.initRotation = dt.Rows[i][12].ToString();
                        dev.belong = dt.Rows[i][13].ToString();
                        dev.lng = dt.Rows[i][14].ToString();
                        dev.lat = dt.Rows[i][15].ToString();
                        list.Add(dev);
                    }
                    if (list.Count > 0)
                    {
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        
    }
}
