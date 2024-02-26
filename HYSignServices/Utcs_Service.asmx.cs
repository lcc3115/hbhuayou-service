using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using HYSignServices.ToolsDoc;
using HYSignServices.Entity;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace HYSignServices
{
    /// <summary>
    /// Utcs_Service 华友接口服务
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Utcs_Service : System.Web.Services.WebService
    {
        //private static string cross_ids = "'420100023423','420100023151','420100023167','420100023166','420100023148','420100023112','420100023165','420100001016','420100001017','420100001201','420100001018','420100001015','420100023177','420100023113','420100023302','420100023303'";
        private string lastID = "";
        /// <summary>
        /// 查询现项目内存在的路口
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<string> GetCrossInfo()
        {
            List<string> list = new List<string>();
            string sql = "select cross_no,cross_name from CROSS_INFO";// where cross_no in(" + cross_ids + ")";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string cross_id = dt.Rows[i][0].ToString();
                        string cross_name = dt.Rows[i][1].ToString();
                        list.Add(cross_id + "-" + cross_name);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        [WebMethod]
        public List<Utcs_DevInfo> GetCrossInfoWithLngLat()
        {
            List<Utcs_DevInfo> list = new List<Utcs_DevInfo>();
            string sql = "select cross_no,cross_name,position_x,position_y from cross_info";// where cross_no in(" + cross_ids + ")";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    Utcs_DevInfo ud;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ud = new Utcs_DevInfo();
                        ud.cross_no = dt.Rows[i][0].ToString();
                        ud.cross_name = dt.Rows[i][1].ToString();
                        ud.lng = dt.Rows[i][2].ToString();
                        ud.lat = dt.Rows[i][3].ToString();
                        list.Add(ud);
                        ud = null;
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }




        /// <summary>
        /// 查询配时计划
        /// </summary>
        /// <param name="cross_id"></param>
        /// <returns></returns>
        [WebMethod]
        public List<string> ResearchCrossSchema(string cross_id)
        {

            //string CrossPlanStr = Tools.ThreadLock("CrossPlan", cross_id, "0");
            
            string CrossPlanStr = Tools.ThreadLock("CrossPlan", cross_id, "0");
            Thread.Sleep(2000);
            XmlNodeList OperationList = Tools.LoadXmlStr(CrossPlanStr);
            if (OperationList == null)
            {
                return null;
            }
            string PlanNo = "0";
            foreach (XmlNode item in OperationList)
            {
                foreach (XmlNode CrossPlan in item.ChildNodes)
                {
                    switch (CrossPlan.Name)
                    {
                        case "PlanNo":
                            PlanNo = CrossPlan.InnerText;
                            break;
                        default:
                            break;
                    }
                }
            }
            //string PlanParamStr = Tools.ThreadLock("PlanParam", cross_id, "0");
            
            string PlanParamStr = Tools.ThreadLock("PlanParam", cross_id, "0");

            XmlNodeList Operation = Tools.LoadXmlStr(PlanParamStr);
            if (Operation == null)
            {
                return null;
            }
            List<string> list = new List<string>();
            foreach (XmlNode item in Operation)
            {
                string pNO = string.Empty;
                string pName = string.Empty;
                foreach (XmlNode PlanParam in item.ChildNodes)
                {
                    switch (PlanParam.Name)
                    {
                        case "PlanNo":
                            pNO = PlanParam.InnerText;
                            break;
                        case "PlanName":
                            pName = PlanParam.InnerText;
                            break;
                        default:
                            break;
                    }
                }
                string msgStr = pNO + "-" + pName;
                if (pNO == PlanNo)
                {
                    msgStr += "-0";
                }
                list.Add(msgStr);
            }
            return list;

            #region 数据库查询模式
            //List<string> list = new List<string>();
            //string sql = "select a.scheme_no,a.scheme_name,b.control_scheme_no from UTCS_CROSS_SCHEMA a, UTCS_RUN_PHASE b where a.cross_no = '" + cross_id + "' and a.cross_no = b.cross_no";
            //try
            //{
            //    DataSet ds = OracleHelper.Query_utcs(sql);
            //    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //    {
            //        DataTable dt = ds.Tables[0];
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            string scheme_no = dt.Rows[i][0].ToString();
            //            string scheme_name = dt.Rows[i][1].ToString();
            //            string control_scheme_no = dt.Rows[i][2].ToString();
            //            if (scheme_no == control_scheme_no)
            //            {
            //                list.Add(scheme_no + "-" + scheme_name + "-0");
            //            }
            //            else
            //            {
            //                list.Add(scheme_no + "-" + scheme_name);
            //            }
            //        }
            //        return list;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            //return null;
            #endregion
        }

        /// <summary>
        /// 查询相位
        /// </summary>
        /// <param name="cross_id"></param>
        /// <param name="scheme_no">配时计划编号</param>
        /// <returns>bas64编码相位图</returns>
        [WebMethod]
        public List<string> GetPhaseInfo(string cross_id, string scheme_no)
        {
            string PlanParamStr = Tools.ThreadLock("PlanParam", cross_id, scheme_no);
            

            XmlNodeList OperationList = Tools.LoadXmlStr(PlanParamStr);
            if (OperationList == null)
            {
                return null;
            }
            List<string> StageList = new List<string>();
            foreach (XmlNode item in OperationList)
            {
                foreach (XmlNode PlanParam in item.ChildNodes)
                {
                    if (PlanParam.Name == "StageNoList")
                    {
                        foreach (XmlNode StageNoList in PlanParam.ChildNodes)
                        {
                            string StageNo = StageNoList.InnerText;
                            StageList.Add(StageNo);
                        }
                    }
                }
            }
            string StageParamStr = Tools.ThreadLock("StageParam", cross_id, "0");
            
            XmlNodeList Operation = Tools.LoadXmlStr(StageParamStr);
            if (Operation == null)
            {
                return null;
            }
            List<string> list = new List<string>();
            foreach (XmlNode item in Operation)
            {
                string Stageid = string.Empty;
                string StageName = string.Empty;
                string Green = string.Empty;
                foreach (XmlNode StageParam in item.ChildNodes)
                {
                    switch (StageParam.Name)
                    {
                        case "StageNo":
                            Stageid = StageParam.InnerText;
                            break;
                        case "StageName":
                            StageName = StageParam.InnerText;
                            break;
                        case "Green":
                            Green = StageParam.InnerText;
                            break;
                        default:
                            break;
                    }
                }
                if (StageList.Contains(Stageid))
                {
                    StageName += "";
                    int phase_green = int.Parse(Green) + 3;
                    Bitmap bmp = Image_Tools.CreateImg(StageName, phase_green.ToString(), cross_id);
                    string imgBase64 = Tools.ImgToBase64String(bmp);
                    list.Add(imgBase64);
                }
            }
            return list;
            #region 数据库查询
            //List<string> list = new List<string>();
            //string sql = "select timing_name,phase_green from utcs_cross_phasetiming where cross_no = '" + cross_id + "' and scheme_no = " + scheme_no + "";
            //try
            //{
            //    DataSet ds = OracleHelper.Query_utcs(sql);
            //    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //    {
            //        DataTable dt = ds.Tables[0];
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            string timing_name = dt.Rows[i][0].ToString();
            //            string temp_phase = dt.Rows[i][1].ToString();
            //            int phase_green = int.Parse(temp_phase) + 3;
            //            Bitmap bmp = Image_Tools.CreateImg(timing_name, phase_green.ToString(), cross_id);
            //            string imgBase64 = Tools.ImgToBase64String(bmp);
            //            list.Add(imgBase64);
            //        }
            //        return list;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            //return null;
            #endregion
        }

        
        /// <summary>
        /// 绑定相位
        /// </summary>
        /// 
        /// <returns></returns>
        [WebMethod]
        public List<PhaseInfo> BindPhase()
        {
            //string receiveData = Tools.ThreadLock("PhaseParam", "", "0");
            //Tools.WriteLog(receiveData, "D:/mLog.log");
            //Tools.WriteLog(tempStr, "D:/mLog.log");
            string receiveData = System.IO.File.ReadAllText(@"D:/PhaseParam.txt");
            if (string.IsNullOrEmpty(receiveData))
            {
                return null;
            }
            XmlNodeList OperationList = Tools.LoadXmlStr(receiveData);
            if (OperationList == null)
            {
                return null;
            }

            PhaseInfo phaseInfo = new PhaseInfo();
            List<PhaseInfo> list = new List<PhaseInfo>();
            foreach (XmlNode item in OperationList)
            {

                Phasedetial detial = new Phasedetial();

                foreach (XmlNode PhaseParam in item.ChildNodes)
                {

                    switch (PhaseParam.Name)
                    {
                        case "CrossID":
                            string cID = PhaseParam.InnerText;
                            if (lastID != cID)
                            {
                                if (phaseInfo.phase_list.Count > 0)
                                {
                                    list.Add(phaseInfo);
                                }
                                phaseInfo = new PhaseInfo();
                                phaseInfo.phase_list = new List<Phasedetial>();
                                phaseInfo.cross_id = cID;
                            }
                            lastID = cID;
                            break;
                        case "PhaseNo":
                            detial.phase_no = PhaseParam.InnerText;
                            break;
                        case "PhaseName":
                            detial.phase_name = PhaseParam.InnerText;
                            break;
                        default:
                            break;
                    }

                }
                phaseInfo.phase_list.Add(detial);
            }
            list.Add(phaseInfo);
            return list;

            #region 数据库查询模式
            //string ids = "";
            //string sqlLocal = "select cross_no from CROSS_INFO";
            //DataSet dsLocal = OracleHelper.QueryU_Dev(sqlLocal);
            //DataTable dtLocal = dsLocal.Tables[0];
            //for (int i = 0; i < dtLocal.Rows.Count; i++)
            //{
            //    ids += dtLocal.Rows[i][0].ToString() + ",";
            //}
            //string cross_ids = ids.TrimEnd(',');
            //List<PhaseInfo> list = new List<PhaseInfo>();
            //string sql = "select cross_no,phase_no,phase_name from UTCS_CROSS_PHASE where cross_no in(" + cross_ids + ") and phase_name not like '%行人灯%'";
            //try
            //{
            //    DataSet ds = OracleHelper.Query_utcs(sql);
            //    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //    {
            //        DataTable dt = ds.Tables[0];
            //        string[] crossArray = cross_ids.Split(',');
            //        for (int j = 0; j < crossArray.Length; j++)
            //        {
            //            string pass_cross_id = crossArray[j];
                        
            //            PhaseInfo phaseInfo = new PhaseInfo();
            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                string crossid = dt.Rows[i][0].ToString();
            //                Phasedetial phase_detial = new Phasedetial();
            //                if (crossid == pass_cross_id)
            //                {
            //                    phase_detial.phase_no = dt.Rows[i][1].ToString();
            //                    phase_detial.phase_name = dt.Rows[i][2].ToString();
            //                    phaseInfo.phase_list.Add(phase_detial);
            //                }
            //            }
            //            if (phaseInfo.phase_list.Count > 0)
            //            {
            //                phaseInfo.cross_id = pass_cross_id;
            //                list.Add(phaseInfo);
            //            }
            //        }
            //        return list;
            //    }
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            #endregion
        }

        /// <summary>
        /// 绑定相位
        /// </summary>
        /// <param name="cross_ids">路口ID 多个ID用‘,’隔开</param>
        /// <returns></returns>
        [WebMethod]
        public List<PhaseInfo> BindPhase_Single(string cross_ids)
        {

            string receiveData = Tools.ThreadLock("PhaseParam", cross_ids, "0");

            Thread.Sleep(3000);
            if (string.IsNullOrEmpty(receiveData))
            {
                return null;
            }
            XmlNodeList OperationList = Tools.LoadXmlStr(receiveData);
            if (OperationList == null)
            {
                return null;
            }

            PhaseInfo phaseInfo = new PhaseInfo();
            List<PhaseInfo> list = new List<PhaseInfo>();
            foreach (XmlNode item in OperationList)
            {

                Phasedetial detial = new Phasedetial();

                foreach (XmlNode PhaseParam in item.ChildNodes)
                {

                    switch (PhaseParam.Name)
                    {
                        case "CrossID":
                            string cID = PhaseParam.InnerText;
                            if (lastID != cID)
                            {
                                if (phaseInfo.phase_list.Count > 0)
                                {
                                    list.Add(phaseInfo);
                                }
                                phaseInfo = new PhaseInfo();
                                phaseInfo.phase_list = new List<Phasedetial>();
                                phaseInfo.cross_id = cID;
                            }
                            lastID = cID;
                            break;
                        case "PhaseNo":
                            detial.phase_no = PhaseParam.InnerText;
                            break;
                        case "PhaseName":
                            detial.phase_name = PhaseParam.InnerText;
                            break;
                        default:
                            break;
                    }

                }
                phaseInfo.phase_list.Add(detial);
            }
            list.Add(phaseInfo);
            return list;
        }
        

        
        /// <summary>
        /// 查询所有相位周期
        /// </summary>
        /// <param name="cross_ids">路口ID，多个路口用','隔开</param>
        /// <returns>is_cur=0:当前运行相位,is_cur=1:非当前运行相位</returns>
        [WebMethod]
        public List<PhasetimingInfo> GetCross_Phasetiming_CUR()
        {
            //string receiveData = Tools.ThreadLock("StageParam", "", "0");
            string receiveData = System.IO.File.ReadAllText(@"D:/StageParam.txt");
            List<string> StageNoList = new List<string>() { "101","102","103","104","105","106","107","108","109"};

            if (string.IsNullOrEmpty(receiveData))
            {
                return null;
            }
            XmlNodeList OperationList = Tools.LoadXmlStr(receiveData);
            if (OperationList == null)
            {
                return null;
            }

            PhasetimingInfo Phasetiming = new PhasetimingInfo();
            List<PhasetimingInfo> list = new List<PhasetimingInfo>();
            foreach (XmlNode item in OperationList)
            {

                PhasetimingDetial detial = new PhasetimingDetial();

                foreach (XmlNode StageParam in item.ChildNodes)
                {

                    switch (StageParam.Name)
                    {
                        case "CrossID":
                            string cID = StageParam.InnerText;
                            if (lastID != cID)
                            {
                                if (Phasetiming.phase_list.Count > 0)
                                {
                                    list.Add(Phasetiming);
                                }
                                Phasetiming = new PhasetimingInfo();
                                detial.is_cur = "0";
                                Phasetiming.phase_list = new List<PhasetimingDetial>();
                                Phasetiming.cross_id = cID;
                            }
                            else
                            {
                                detial.is_cur = "1";
                            }
                            lastID = cID;
                            break;
                        case "StageNo":
                            detial.timing_no = StageParam.InnerText;
                            break;
                        case "StageName":
                            detial.timing_name = StageParam.InnerText;
                            break;
                        case "Green":
                            detial.phaseGreen = StageParam.InnerText;
                            break;
                        default:
                            break;
                    }

                }
                if (!StageNoList.Contains(detial.timing_no))
                {
                    continue;
                }
                Phasetiming.phase_list.Add(detial);
            }
            list.Add(Phasetiming);
            return list;
        }
        #region GetCross_Phasetiming_CUR数据库查询模式
        //public List<PhasetimingInfo> GetCross_Phasetiming_CUR()
        //{
        //    string ids = "";
        //    string sqlLocal = "select cross_no from CROSS_INFO";
        //    DataSet dsLocal = OracleHelper.QueryU_Dev(sqlLocal);
        //    DataTable dtLocal = dsLocal.Tables[0];
        //    for (int i = 0; i < dtLocal.Rows.Count; i++)
        //    {
        //        ids += dtLocal.Rows[i][0].ToString() + ",";
        //    }
        //    string cross_ids = ids.TrimEnd(',');
        //    List<PhasetimingInfo> list = new List<PhasetimingInfo>();
        //    string sql = "select b.cross_no,b.timing_no,b.timing_name,b.phase_green,b.allred_time,a.cur_stage_no from UTCS_RUN_PHASE a ,utcs_cross_phasetiming b " +
        //                "where a.control_scheme_no = b.scheme_no and a.cross_no = b.cross_no and a.cross_no in(" + cross_ids + ")";
        //    try
        //    {
        //        DataSet ds = OracleHelper.Query_utcs(sql);
        //        if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            DataTable dt = ds.Tables[0];
        //            string[] crossArray = cross_ids.Split(',');
        //            for (int j = 0; j < crossArray.Length; j++)
        //            {
        //                string pass_cross_id = crossArray[j];

        //                PhasetimingInfo phasetiming = new PhasetimingInfo();
        //                double angle = 0;
        //                double cur = 0;
                        
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    string crossid = dt.Rows[i][0].ToString();
        //                    PhasetimingDetial phase_detial = new PhasetimingDetial();
        //                    if (crossid == pass_cross_id)
        //                    {
        //                        phase_detial.timing_no = dt.Rows[i][1].ToString();
        //                        phase_detial.timing_name = dt.Rows[i][2].ToString();
                                
        //                        int allred = int.Parse(dt.Rows[i][4].ToString()) / 10;
        //                        phase_detial.phaseGreen = (int.Parse(dt.Rows[i][3].ToString()) + 3 + allred).ToString();
        //                        angle += int.Parse(phase_detial.phaseGreen);
        //                        string isCur = dt.Rows[i][5].ToString();
        //                        phase_detial.is_cur = phase_detial.timing_no == isCur ? "0" : "1";

        //                        if (phase_detial.is_cur == "0")
        //                        {
        //                            cur = angle;
                                    
        //                        }
        //                        phasetiming.phase_list.Add(phase_detial);
        //                    }
        //                }
        //                if (phasetiming.phase_list.Count > 0)
        //                {
        //                    phasetiming.cross_id = pass_cross_id;
                            
        //                    //每秒运行的角度
        //                    double sa = 360 / angle;
        //                    //当前角度:已运行过的周期(含本身) / 总周期 * 360
        //                    double ac = cur / angle * 360;
        //                    phasetiming.second_angle = (int)Math.Round(sa);
        //                    phasetiming.angle_cur = (int)Math.Round(ac);
        //                    for (int i = 0; i < phasetiming.phase_list.Count; i++)
        //                    {
        //                        if (phasetiming.phase_list[i].is_cur == "0")
        //                        {
        //                            if (i == phasetiming.phase_list.Count - 1)
        //                            {
        //                                phasetiming.angle_cur = 0;
        //                            }
        //                            break;
        //                        }
        //                    }
        //                    list.Add(phasetiming);
        //                }
        //            }
        //            return list;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        #endregion
        /// <summary>
        /// 查询单个相位周期
        /// </summary>
        /// <param name="cross_ids">路口ID，多个路口用','隔开</param>
        /// <returns>is_cur=0:当前运行相位,is_cur=1:非当前运行相位</returns>
        [WebMethod]
        public List<PhasetimingInfo> GetCross_Phasetiming_CUR_Single(string cross_ids)
        {

            string receiveData = Tools.ThreadLock("StageParam", cross_ids, "0");
            

            List<string> StageNoList = new List<string>() { "101", "102", "103", "104", "105", "106", "107", "108", "109" };

            if (string.IsNullOrEmpty(receiveData))
            {
                return null;
            }
            XmlDocument doc = new XmlDocument();
            XmlNodeList OperationList = Tools.LoadXmlStr(receiveData);
            if (OperationList == null)
            {
                return null;
            }

            PhasetimingInfo Phasetiming = new PhasetimingInfo();
            List<PhasetimingInfo> list = new List<PhasetimingInfo>();
            foreach (XmlNode item in OperationList)
            {

                PhasetimingDetial detial = new PhasetimingDetial();

                foreach (XmlNode StageParam in item.ChildNodes)
                {

                    switch (StageParam.Name)
                    {
                        case "CrossID":
                            string cID = StageParam.InnerText;
                            if (lastID != cID)
                            {
                                if (Phasetiming.phase_list.Count > 0)
                                {
                                    list.Add(Phasetiming);
                                }
                                Phasetiming = new PhasetimingInfo();
                                detial.is_cur = "0";
                                Phasetiming.phase_list = new List<PhasetimingDetial>();
                                Phasetiming.cross_id = cID;
                            }
                            else
                            {
                                detial.is_cur = "1";
                            }
                            lastID = cID;
                            break;
                        case "StageNo":
                            detial.timing_no = StageParam.InnerText;
                            break;
                        case "StageName":
                            detial.timing_name = StageParam.InnerText;
                            break;
                        case "Green":
                            detial.phaseGreen = StageParam.InnerText;
                            break;
                        default:
                            break;
                    }

                }
                if (!StageNoList.Contains(detial.timing_no))
                {
                    continue;
                }
                Phasetiming.phase_list.Add(detial);
            }
            list.Add(Phasetiming);
            return list;
        }

        [WebMethod]
        public List<Column_Info> GetColumnInfo()
        {
            string sql = "select * from UNITY_COLUMN_INFO";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (HYSignServices.HYService.Tools.DSisNull(ds))
                {
                    List<Column_Info> list = new List<Column_Info>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Column_Info dev = new Column_Info();
                        dev.ColumnID = dt.Rows[i][0].ToString();
                        dev.ColumnName = dt.Rows[i][1].ToString();
                        dev.Lng = dt.Rows[i][2].ToString();
                        dev.Lat = dt.Rows[i][3].ToString();
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

        [WebMethod]
        public List<Crossing_Info> GetCrossingInfo()
        {
            string sql = "select * from UNITY_CROSSING_INFO";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (HYSignServices.HYService.Tools.DSisNull(ds))
                {
                    List<Crossing_Info> list = new List<Crossing_Info>();
                    DataTable dt = ds.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Crossing_Info dev = new Crossing_Info();
                        dev.CrossingID = dt.Rows[i][0].ToString();
                        dev.Lng = dt.Rows[i][1].ToString();
                        dev.Lat = dt.Rows[i][2].ToString();
                        dev.Rot = dt.Rows[i][3].ToString();
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

        
    }
    


    public class Crossing_Info
    {
        public string CrossingID { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string Rot { get; set; }
    }

    public class Column_Info
    {
        public string ColumnID { get; set; }
        public string ColumnName { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
    }

    public class PhaseInfo
    {
        public string cross_id;
        public List<Phasedetial> phase_list = new List<Phasedetial>();
    }

    public class Phasedetial
    {
        public string phase_no;
        public string phase_name;
    }

    public class PhasetimingInfo
    {
        public string cross_id;
        public int angle_cur = 0;
        public int second_angle = 0;
        public List<PhasetimingDetial> phase_list = new List<PhasetimingDetial>();
    }

    public class PhasetimingDetial
    {
        public string timing_no;
        public string timing_name;
        public string phaseGreen;
        public string is_cur;
    }
}
