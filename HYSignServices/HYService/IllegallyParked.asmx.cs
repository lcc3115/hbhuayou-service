using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HYSignServices.Entity;
using System.Data;
using Illegallyparked.Entity;

namespace HYSignServices.HYService
{
    /// <summary>
    /// IllegallyParked 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class IllegallyParked : System.Web.Services.WebService
    {

        [WebMethod]
        public List<ScreenInfo> GetScreenInfo()
        {
            try
            {
                string sql = "select * from SCREEN_INFO where screen_type = '5' and lng is not null and lat is not null";
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
                        //screen.AUTO = dt.Rows[i][12].ToString();
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
        /// 取违停数据(查询海康的违停视图)
        /// </summary>
        /// <param name="rn">1-最后一条  5-最后五条</param>
        [WebMethod]
        public List<LlegallyEntity> GetillegallyData(string rn)
        {
            //查询同一路口车牌被抓次数
            List<TempCount> countList = ResearchIllegalCount();

            string sql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(PARTITION BY wfddbh ORDER BY wfsj DESC) rn, wfddbh,hphm,wfsj FROM illegallyparked where wfxw = '10390' order by wfsj desc) ";
            
            sql += "WHERE rn < "+(int.Parse(rn)+1)+"  order by wfddbh,rn";
            
            try
            {
                DataSet ds = OracleHelper.Query(sql);
                if (Tools.DSisNull(ds))
                {
                    DataTable dt = ds.Tables[0];
                    List<LlegallyEntity> llegalList = new List<LlegallyEntity>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        LlegallyEntity llegal = new LlegallyEntity();
                        llegal.rn = dt.Rows[i][0].ToString();
                        llegal.crossName = dt.Rows[i][1].ToString();
                        llegal.plate = dt.Rows[i][2].ToString();
                        llegal.llegalTime = dt.Rows[i][3].ToString();
                        LlegallyEntity temp = llegalList.FirstOrDefault(o => o.crossName == llegal.crossName && o.plate == llegal.plate);
                        if (temp != null)
                            continue;
                        //同一设备同一车牌一天只抓拍一次
                        string llegalTime = Convert.ToDateTime(llegal.llegalTime).ToString("yyyy-MM-dd");
                        llegal.count = "0";
                        TempCount tempCount = countList.FirstOrDefault(o => o.crossName == llegal.crossName &&
                                                                                                    o.plate == llegal.plate &&
                                                                                                    o.llegalTime == llegalTime);
                        if (tempCount != null)
                        {
                            llegal.count = tempCount.count;
                        }
                        llegalList.Add(llegal);
                    }
                    return llegalList;
                }
                return null;
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("GetillegallyData err:" + ex.Message);
                return null;
            }
        }

        private List<TempCount> ResearchIllegalCount()
        {
            //DateTime nowTime = DateTime.Now;
            //string starTime = nowTime.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
            //string endTime = nowTime.ToString("yyyy-MM-dd HH:mm:ss");
            string plateNumSql = "select wfddbh, hphm,to_char(wfsj,'yyyy-MM-dd'),count(hphm) from illegallyparked ";
            //plateNumSql += "where wfsj >= to_timestamp('" + starTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            //plateNumSql += "and wfsj <= to_timestamp('" + endTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            plateNumSql += "where  wfxw = '10390' ";
            plateNumSql += "group by wfddbh,hphm,to_char(wfsj,'yyyy-MM-dd')";

            List<TempCount> countList = new List<TempCount>();
            try
            {
                DataSet plateNumds = OracleHelper.Query(plateNumSql);
                if (Tools.DSisNull(plateNumds))
                {
                    DataTable plateNumdt = plateNumds.Tables[0];

                    for (int i = 0; i < plateNumdt.Rows.Count; i++)
                    {
                        TempCount tempCount = new TempCount();

                        tempCount.crossName = plateNumdt.Rows[i][0].ToString();
                        tempCount.plate = plateNumdt.Rows[i][1].ToString();
                        tempCount.count = plateNumdt.Rows[i][3].ToString();
                        tempCount.llegalTime = plateNumdt.Rows[i][2].ToString();
                        countList.Add(tempCount);
                    }
                    return countList;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("GetCountList err:" + ex.Message);
                return null;
            }
            return null;
        }

        //增/改
        [WebMethod]
        public string UpdateScreenInfo(ScreenEntity Screen)
        {
            string sql = "";
            if (string.IsNullOrEmpty(Screen.SCREEN_NO))
            {
                //新增
                sql = "insert into SCREEN_INFO(SCREEN_NO,SCREEN_NAME,DIRECTION,SCREEN_TYPE,SCREEN_IP_LINE1,SCREEN_IP_LINE2,LNG,LAT,NEWCARD)";
                string sqlValue = string.Format(" VALUES(seq_scr.nextval,'{0}','{1}','5','{2}','{3}','{4}','{5}','{6}')", Screen.SCREEN_NAME, Screen.DIRECTION, Screen.IPAddress, Screen.BallIPAddress, Screen.LNG, Screen.LAT, Screen.NEWCARD);
                sql += sqlValue;
                
            }
            else//(SCREEN_NO,SCREEN_NAME,DIRECTION,SCREEN_TYPE,SCREEN_IP_LINE1,SCREEN_IP_LINE2,LNG,LAT,NEWCARD)";
            {
                string sqlValue = "update SCREEN_INFO set SCREEN_NAME = '{0}',DIRECTION = '{1}',SCREEN_IP_LINE1 = '{2}',SCREEN_IP_LINE2 = '{3}',LNG = '{4}',LAT = '{5}',NEWCARD = '{6}' where SCREEN_NO = '{7}'";
                sql = string.Format(sqlValue, Screen.SCREEN_NAME, Screen.DIRECTION, Screen.IPAddress, Screen.BallIPAddress, Screen.LNG, Screen.LAT, Screen.NEWCARD, Screen.SCREEN_NO);
                //Tools.WriteErrLog("sqlInfo:" + sql);
            }
            try
            {
                string res = OracleHelper.ExecuteU_Dev_212(sql);
                return res;
            }
            catch (Exception ex)
            {
                Tools.WriteErrLog("UpdateScreenInfo err:" + ex.Message);
                return "fail";
            }
        }

    }
}
