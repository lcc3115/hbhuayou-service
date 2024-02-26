using HYSignServices.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace HYSignServices.HYService
{
    /// <summary>
    /// TrafficData 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class TrafficData : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetChcekDataList(string requestStr)
        {
            //Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            RetInfo retInfo = new RetInfo();
            try
            {
               
                RquestParm rq = JsonConvert.DeserializeObject<RquestParm>(requestStr);
                var queryArray = rq.queryArray;
                var compareArray = rq.compareArray;
                //var queryArray = new List<QueryModel>() {
                //    new QueryModel{ Index="001",StartTime="2019-05-08 00:00:00",EndTime="2019-05-08 12:00:00"},
                //    new QueryModel{ Index="002",StartTime="2019-05-09 00:00:00",EndTime="2019-05-09 12:00:00"},
                //};
                //var compareArray = new List<QueryModel>() {
                //    new QueryModel{ Index="001",StartTime="2019-05-08 12:00:00",EndTime="2019-05-08 23:59:59"},
                //    new QueryModel{ Index="002",StartTime="2019-05-09 12:00:00",EndTime="2019-05-09 23:59:59"},
                //};
                #region 获取数据
                StringBuilder sqlStr = new StringBuilder();
                for (int i = 0; i < queryArray.Count; i++)
                {
                    sqlStr.Append(@"select t1.*,t2.*,'" + queryArray[i].Index + @"' INDEXSTR,(CASE  WHEN t1.t1crossing_name IS NULL THEN t2.t2crossing_name ELSE t1.t1crossing_name  END ) crossing_name from 
                (select sysdict_name as t1sysdict_name, crossing_name as t1crossing_name, sum(1) as t1sum from
                (select a.violative_action, a.crossing_id, a.pass_time, b.sysdict_name, c.crossing_name from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b, TRAFFIC_CROSSING_INFO c
                where a.violative_action = b.sysdict_code and b.sysdict_type = 1017
                and pass_time >= to_timestamp('" + queryArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and pass_time < to_timestamp('" + queryArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and c.crossing_id = a.crossing_id) t group by sysdict_name, crossing_name) t1
                left join
                (select sysdict_name as t2sysdict_name, crossing_name as t2crossing_name, sum(1) as t2sum from
                (select a.violative_action, a.crossing_id, a.pass_time, b.sysdict_name, c.crossing_name from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b, TRAFFIC_CROSSING_INFO c
                where a.violative_action = b.sysdict_code and b.sysdict_type = 1017
                and pass_time >= to_timestamp('" + compareArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and pass_time < to_timestamp('" + compareArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and c.crossing_id = a.crossing_id) t group by sysdict_name, crossing_name) t2
                on t1.t1sysdict_name = t2.t2sysdict_name and t1.t1crossing_name = t2.t2crossing_name
                union
                select t1.*,t2.*,'" + queryArray[i].Index + @"' INDEXSTR,(CASE  WHEN t2.t2crossing_name IS NULL THEN t1.t1crossing_name ELSE t2.t2crossing_name  END ) crossing_name from
                (select sysdict_name as t2sysdict_name, crossing_name as t2crossing_name, sum(1) as t2sum from
                (select a.violative_action, a.crossing_id, a.pass_time, b.sysdict_name, c.crossing_name from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b, TRAFFIC_CROSSING_INFO c
                where a.violative_action = b.sysdict_code and b.sysdict_type = 1017
                and pass_time >= to_timestamp('" + compareArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and pass_time < to_timestamp('" + compareArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and c.crossing_id = a.crossing_id) t group by sysdict_name, crossing_name) t2
                left join
                (select sysdict_name as t1sysdict_name, crossing_name as t1crossing_name, sum(1) as t1sum from
                (select a.violative_action, a.crossing_id, a.pass_time, b.sysdict_name, c.crossing_name from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b, TRAFFIC_CROSSING_INFO c
                where a.violative_action = b.sysdict_code and b.sysdict_type = 1017
                and pass_time >= to_timestamp('" + queryArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and pass_time < to_timestamp('" + queryArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and c.crossing_id = a.crossing_id) t group by sysdict_name, crossing_name) t1
                on t1.t1sysdict_name = t2.t2sysdict_name and t1.t1crossing_name = t2.t2crossing_name
                ");

                    if (i < queryArray.Count - 1)
                    {
                        sqlStr.Append(" union ");
                    }
                }
                DataSet ds = OracleHelper.Query0249(sqlStr.ToString());
                #endregion
                DataTable dt = ds.Tables[0];
                var roadList = from t in dt.AsEnumerable()
                               group t by new { t1 = t.Field<string>("CROSSING_NAME") } into m
                               select new
                               {
                                   name = m.Key.t1,
                               };
                var roadDataModelList = new List<RoadDataModel>();

                foreach (var road in roadList)
                {
                    var roadDataModel = new RoadDataModel();
                    roadDataModel.Name = road.name;
                    ContrastDataModel contrastDataModel = new ContrastDataModel();
                    var queryTimeDataList = new List<IllegalDataModel>();
                    var cpmpareTimeDataList = new List<IllegalDataModel>();
                    foreach (var query in queryArray)
                    {
                        var queryTimeData = new IllegalDataModel();
                        var cpmpareTimeData = new IllegalDataModel();
                        queryTimeData.Id = query.Index;
                        queryTimeData.StartTime = Convert.ToDateTime(query.StartTime);
                        queryTimeData.EndTime = Convert.ToDateTime(query.EndTime);
                        var compare = compareArray.FirstOrDefault(x => x.Index == query.Index);
                        if (compare != null)
                        {
                            cpmpareTimeData.Id = compare.Index;
                            cpmpareTimeData.StartTime = Convert.ToDateTime(compare.StartTime);
                            cpmpareTimeData.EndTime = Convert.ToDateTime(compare.EndTime);
                        }
                        DataRow[] drs = dt.Select(" CROSSING_NAME = '" + road.name + "' and INDEXSTR='" + query.Index + "'  ");

                        if (drs.Length > 0 && drs != null)
                        {
                            Hashtable qIllHs = new Hashtable();
                            Hashtable cIllHs = new Hashtable();
                            foreach (var dr in drs)
                            {

                                if (dr["T1SYSDICT_NAME"] != null && !string.IsNullOrEmpty(dr["T1SYSDICT_NAME"].ToString()))
                                {
                                    if (dr["T1CROSSING_NAME"].ToString() != road.name)
                                        continue;
                                    qIllHs.Add(dr["T1SYSDICT_NAME"].ToString(), dr["T1SUM"]);
                                }
                                if (dr["T2SYSDICT_NAME"] != null && !string.IsNullOrEmpty(dr["T2SYSDICT_NAME"].ToString()))
                                {
                                    if (dr["T2CROSSING_NAME"].ToString() != road.name)
                                        continue;
                                    cIllHs.Add((dr["T2SYSDICT_NAME"]).ToString(), dr["T2SUM"]);
                                }
                            }
                            queryTimeData.Data = qIllHs;
                            cpmpareTimeData.Data = cIllHs;
                            queryTimeDataList.Add(queryTimeData);
                            cpmpareTimeDataList.Add(cpmpareTimeData);
                        }

                    }
                    contrastDataModel.QueryTimeData = queryTimeDataList;
                    contrastDataModel.CpmpareTimeData = cpmpareTimeDataList;
                    roadDataModel.Data = contrastDataModel;
                    roadDataModelList.Add(roadDataModel);
                }
                retInfo.Code = "20000";
                retInfo.Msg = "";
                retInfo.Data = roadDataModelList;
              //  string jsonStr = JsonConvert.SerializeObject(roadDataModelList);
            }
            catch(Exception ex)
            {
                var exStr= ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length>200? exStr.Substring(0,200): exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;


           
        }

        //[WebMethod]
        //public string GetChcekPicList(string requestStr)
        //{
        //    //Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        //    RetInfo retInfo = new RetInfo();
        //    try
        //    {

        //        RquestParm rq = JsonConvert.DeserializeObject<RquestParm>(requestStr);
        //        var queryArray = rq.queryArray;
        //        //var queryArray = new List<QueryModel>() {
        //        //    new QueryModel{ Index="001",StartTime="2020-04-27 00:00:00",EndTime="2019-04-27 01:00:00"},
        //        //    new QueryModel{ Index="002",StartTime="2020-04-27 13:00:00",EndTime="2020-04-27 14:00:00"},
        //        //};
        //        #region 获取数据
        //        StringBuilder sqlStr = new StringBuilder();
        //        for (int i = 0; i < queryArray.Count; i++)
        //        {

        //            sqlStr.Append(@"select a1.VEHICLEPICURL,a3.crossing_name,'" + queryArray[i].Index + @"' INDEXSTR from (select max(pass_id) as maxid,crossing_id  from TRAFFIC_VEHICLE_PASS 
        //        where pass_time >= to_timestamp('" + queryArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
        //        and pass_time < to_timestamp('" + queryArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
        //        group by crossing_id
        //        ) a2
        //        left join picurl_info a1
        //        on a2.maxid=a1.vehiclelsh 
        //        left join TRAFFIC_CROSSING_INFO a3
        //        on a2.crossing_id=a3.crossing_id
        //        ");
        //            if (i < queryArray.Count - 1)
        //            {
        //                sqlStr.Append(" union ");
        //            }
        //        }
        //        DataSet ds = OracleHelper.Query0249(sqlStr.ToString());
        //        #endregion
        //        DataTable dt = ds.Tables[0];
        //        var roadList = from t in dt.AsEnumerable()
        //                       group t by new { t1 = t.Field<string>("CROSSING_NAME") } into m
        //                       select new
        //                       {
        //                           name = m.Key.t1,
        //                       };
        //        List<DevModel> devList = new List<DevModel>();
        //        foreach (var road in roadList)
        //        {
        //            var devModel = new DevModel();
        //            devModel.Name = road.name;
        //            List<ImgDataModel> imgList = new List<ImgDataModel>();
        //            if (dt != null && dt.Rows.Count > 0)
        //            {

        //                DataRow[] drs = dt.Select(" CROSSING_NAME = '" + road.name + "' ");
        //                foreach (DataRow dr in drs)
        //                {

        //                    if (dr["CROSSING_NAME"].ToString() != road.name)
        //                        continue;
        //                    var query = queryArray.FirstOrDefault(x => x.Index == dr["INDEXSTR"].ToString());
        //                    imgList.Add(new ImgDataModel
        //                    {
        //                        Index = dr["INDEXSTR"].ToString(),
        //                        ImgUrl = dr["VEHICLEPICURL"].ToString(),
        //                        StartTime = Convert.ToDateTime(query.StartTime),
        //                        EndTime = Convert.ToDateTime(query.EndTime)
        //                    });

        //                }
        //            }
        //            devModel.Img = imgList;
        //            devList.Add(devModel);
        //        }
        //        retInfo.Code = "20000";
        //        retInfo.Msg = "";
        //        retInfo.Data = devList;
        //        //string jsonStr = JsonConvert.SerializeObject(devList);
        //        //return jsonStr;
        //    }
        //    catch (Exception ex)
        //    {
        //        var exStr = ex.ToString();
        //        retInfo.Code = "50000";
        //        retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
        //        retInfo.Data = "error";
        //    }
        //    string jsonStr = JsonConvert.SerializeObject(retInfo);
        //    return jsonStr;

        //}


        [WebMethod]
        public string GetChcekPicList(string requestStr)
        {
            //Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            RetInfo retInfo = new RetInfo();
            try
            {

                RquestParm rq = JsonConvert.DeserializeObject<RquestParm>(requestStr);
                var queryArray = rq.queryArray;
                //var queryArray = new List<QueryModel>() {
                //    new QueryModel{ Index="001",StartTime="2020-04-27 00:00:00",EndTime="2019-04-27 01:00:00"},
                //    new QueryModel{ Index="002",StartTime="2020-04-27 13:00:00",EndTime="2020-04-27 14:00:00"},
                //};
                #region 获取数据

                #region   获取所有路口 
                string devSql = @" select distinct t2.crossing_name,t1.crossing_id,substr(t1.lane_name,0, instr(t1.lane_name,'-车道',1,1)-1)  || '设备'||
                  case  substr(t1.lane_name, instr(t1.lane_name, '-车道', 1, 1) + 3) when '1' then 1 when '2' then 1 when '3' then 1  when '4' then 2 when '5' then 2 when '6' then 2 else 3 end as devname
                from TRAFFIC_LANE_INFO t1,TRAFFIC_CROSSING_INFO t2 where t1.crossing_id = t2.crossing_id order by t1.crossing_id";
                DataSet devDs = OracleHelper.Query0249(devSql.ToString());

                #endregion



                StringBuilder sqlStr = new StringBuilder();
                for (int i = 0; i < queryArray.Count; i++)
                {

                    //    sqlStr.Append(@"select a1.VEHICLEPICURL,a3.crossing_name,'" + queryArray[i].Index + @"' INDEXSTR from (select max(pass_id) as maxid,crossing_id  from TRAFFIC_VEHICLE_PASS 
                    //where pass_time >= to_timestamp('" + queryArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                    //and pass_time < to_timestamp('" + queryArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                    //group by crossing_id
                    //) a2
                    //left join picurl_info a1
                    //on a2.maxid=a1.vehiclelsh 
                    //left join TRAFFIC_CROSSING_INFO a3
                    //on a2.crossing_id=a3.crossing_id
                    //");
                    sqlStr.Append(@"        select a1.VEHICLEPICURL,a3.crossing_name,'" + queryArray[i].Index + @"' INDEXSTR,devname from  (select max(pass_id) as maxid, crossing_id,devname || '设备'|| devno as devname from (
                select t1.pass_id,t1.crossing_id,t1.lane_no,
                 substr(t2.lane_name,0, instr(t2.lane_name,'-车道',1,1)-1) as devname,
                case  substr(t2.lane_name, instr(t2.lane_name,'-车道',1,1)+3) when '1' then 1 when '2' then 1 when '3' then 1  when '4' then 2 when '5' then 2 when '6' then 2 else 3 end   as devno
                 from TRAFFIC_VEHICLE_PASS t1,TRAFFIC_LANE_INFO t2 where t1.pass_time >= to_timestamp('" + queryArray[i].StartTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff')
                and t1.pass_time <to_timestamp('" + queryArray[i].EndTime + @"', 'yyyy-mm-dd hh24:mi:ss.ff') and t1.crossing_id=t2.crossing_id and t1.lane_no=t2.lane_no) a1 group by crossing_id,devname,devno) a2
                 left join picurl_info a1
                on a2.maxid=a1.vehiclelsh 
                left join TRAFFIC_CROSSING_INFO a3
                on a2.crossing_id=a3.crossing_id
                ");
                    if (i < queryArray.Count - 1)
                    {
                        sqlStr.Append(" union ");
                    }
                }
                DataSet imgDs = OracleHelper.Query0249(sqlStr.ToString());
                #endregion
                DataTable devDt = devDs.Tables[0];
                DataTable imgDt = imgDs.Tables[0];
                var roadList = from t in devDt.AsEnumerable()
                               group t by new { t1 = t.Field<string>("CROSSING_NAME") } into m
                               select new
                               {
                                   name = m.Key.t1,
                               };
                List<DevModel> devList = new List<DevModel>();
                foreach (var road in roadList)
                {
                    //var devModel = new DevModel();
                    //devModel.Name = road.name;
                    //List<ImgDataModel> imgList = new List<ImgDataModel>();
                    //if (dt != null && dt.Rows.Count > 0)
                    //{

                    //    DataRow[] drs = dt.Select(" CROSSING_NAME = '" + road.name + "' ");
                    //    foreach (DataRow dr in drs)
                    //    {

                    //        if (dr["CROSSING_NAME"].ToString() != road.name)
                    //            continue;
                    //        var query = queryArray.FirstOrDefault(x => x.Index == dr["INDEXSTR"].ToString());
                    //        imgList.Add(new ImgDataModel
                    //        {
                    //            Index = dr["INDEXSTR"].ToString(),
                    //            ImgUrl = dr["VEHICLEPICURL"].ToString(),
                    //            StartTime = Convert.ToDateTime(query.StartTime),
                    //            EndTime = Convert.ToDateTime(query.EndTime)
                    //        });

                    //    }
                    //}
                    //devModel.Img = imgList;
                    //devList.Add(devModel);

                    var devModel = new DevModel();
                    devModel.Name = road.name;
                    List<DevDtl> devDtlList = new List<DevDtl>();
                    if (devDt != null && devDt.Rows.Count > 0)
                    {
                        DataRow[] drs = devDt.Select(" CROSSING_NAME = '" + road.name + "' ");
                        foreach (DataRow dr in drs)
                        {
                            if (dr["CROSSING_NAME"].ToString() != road.name)
                                continue;
                            var devName = dr["DEVNAME"].ToString();
                            List<ImgDataModel> imgList = new List<ImgDataModel>();


                            DataRow[] imgDrs = imgDt.Select(" CROSSING_NAME = '" + road.name + "' and DEVNAME='"+ devName + "' ");
                            foreach (DataRow imgDr in imgDrs)
                            {

                                if (imgDr["CROSSING_NAME"].ToString() != road.name || imgDr["DEVNAME"].ToString() != devName)
                                    continue;
                                var query = queryArray.FirstOrDefault(x => x.Index == imgDr["INDEXSTR"].ToString());
                                imgList.Add(new ImgDataModel
                                {
                                    Index = imgDr["INDEXSTR"].ToString(),
                                    Url = imgDr["VEHICLEPICURL"].ToString(),
                                    StartTime = Convert.ToDateTime(query.StartTime),
                                    EndTime = Convert.ToDateTime(query.EndTime)
                                });

                            }


                            devDtlList.Add(new DevDtl()
                            {
                                DeviceName = devName,
                                Img= imgList
                            }
                            );

                        }
                    }
                    devModel.Device = devDtlList;
                    devList.Add(devModel);
                }
                retInfo.Code = "20000";
                retInfo.Msg = "";
                retInfo.Data = devList;
                //string jsonStr = JsonConvert.SerializeObject(devList);
                //return jsonStr;
            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;

        }


        [WebMethod]
        public string GetDevList()
        {
            //Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            RetInfo retInfo = new RetInfo();
            try
            {
               
                List<UnityDeviceInfo> querList = null;
                List<UnityDeviceInfo> modelList = new List<UnityDeviceInfo>();

                #region 获取数据
                StringBuilder sqlStr = new StringBuilder();
                string whereSql = "";
                if (querList != null && querList.Count > 0)
                {
                    foreach (var model in querList)
                    {
                        whereSql += model.Id.ToString() + ",";
                    }
                    whereSql = "(" + whereSql.Substring(0, whereSql.Length - 1) + ")";
                    sqlStr.Append(" select * from UNITY_DEVICE_INFO where Id in " + whereSql + " ");
                }
                else
                {
                    sqlStr.Append(" select * from UNITY_DEVICE_INFO  ");
                }
                DataSet ds = OracleHelper.QueryU_Dev(sqlStr.ToString());
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    modelList.Add(DataRowToUnityDeviceModel(dr));
                }

                #endregion
                //return JsonConvert.SerializeObject(modelList);
                retInfo.Code = "20000";
                retInfo.Msg = "";
                retInfo.Data = modelList;
               
            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;

        }


        [WebMethod]
        public string UpdateDevInfo(string requestStr)
        {
            //Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            RetInfo retInfo = new RetInfo();
            try
            {
                string retStr = "fail";
                UnityDeviceInfo model = JsonConvert.DeserializeObject<UnityDeviceInfo>(requestStr);
                if (model != null)
                {
                    string sqlStr = " update UNITY_DEVICE_INFO set LAT='" + model.Lat + "',LNG='" + model.Lng + "' where IP='" + model.Ip + "'   ";
                    retStr = OracleHelper.ExecuteU_Dev(sqlStr);
                }

                retInfo.Code = retStr== "fail" ? "50000": "20000";
                retInfo.Msg = "";
                retInfo.Data = "success";

            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }

        public UnityDeviceInfo DataRowToUnityDeviceModel(DataRow row)
        {
            UnityDeviceInfo model = new UnityDeviceInfo();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.Id = int.Parse(row["ID"].ToString());
                }
                if (row["DEV_NAME"] != null)
                {
                    model.DevName = row["DEV_NAME"].ToString();
                }
                if (row["IDTYPE"] != null)
                {
                    model.IdType = row["IDTYPE"].ToString();
                }
                if (row["ENTITYGROUPNAME"] != null)
                {
                    model.EntityGroupName = row["ENTITYGROUPNAME"].ToString();
                }
                if (row["MONITORTYPE"] != null)
                {
                    model.MonitorType = row["MONITORTYPE"].ToString();
                }
                if (row["IP"] != null)
                {
                    model.Ip = row["IP"].ToString();
                }
                if (row["PORT"] != null && row["PORT"].ToString() != "")
                {
                    model.Port = int.Parse(row["PORT"].ToString());
                }
                //if (row["USER_NAME"] != null)
                //{
                //    model.UserName = row["USER_NAME"].ToString();
                //}
                //if (row["PASSWORD"] != null)
                //{
                //    model.Password = row["PASSWORD"].ToString();
                //}
                //if (row["POSITION"] != null)
                //{
                //    model.Position = row["POSITION"].ToString();
                //}
                //if (row["ROTATION"] != null)
                //{
                //    model.Rotation = row["ROTATION"].ToString();
                //}
                if (row["REMARK"] != null)
                {
                    model.Remark = row["REMARK"].ToString();
                }
                if (row["INITROTATION"] != null)
                {
                    model.Initrotation = row["INITROTATION"].ToString();
                }
                if (row["BELONG"] != null)
                {
                    model.Belong = row["BELONG"].ToString();
                }
                if (row["LNG"] != null)
                {
                    model.Lng = row["LNG"].ToString();
                }
                if (row["LAT"] != null)
                {
                    model.Lat = row["LAT"].ToString();
                }
            }
            return model;
        }


        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetCrossList()
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                List<CrossingInfo> crossList = new List<CrossingInfo>();
                List<LaneInfo> laneList = new List<LaneInfo>();
                List<LaneInfo> newLaneList = new List<LaneInfo>();
                string sqlStr1 = @"   select t1.crossing_id,t1.crossing_name,t4.local_id,t4.cross_name local_name,t4.lng,t4.lat  from TRAFFIC_CROSSING_INFO t1 left join FLOW_DICT t2 on t1.crossing_id=t2.crossing_id 
                left join  SYN_CROSS_NO t3 on t1.crossing_id = t3.crossing_id left join PASS_ID_CORRESPOND t4 on t3.local_id = t4.local_id  ";
                DataSet crossDs = OracleHelper.QueryU_Dev(sqlStr1);

                string sqlStr2 = "  select LANE_ID,LANE_NO,LANE_NAME,CROSSING_ID from TRAFFIC_LANE_INFO  ";
                DataSet laneDs = OracleHelper.QueryU_Dev(sqlStr2);

                string sqlStr3 = "  select t1.FLOW_ID,t1.FLOW_NAME,t1.FLOW_NO,t1.PERCENT,t1.CROSSING_ID,t1.LANE_NO,t2.LANE_NAME from FLOW_DICT t1,TRAFFIC_LANE_INFO t2 where t1.LANE_NO=t2.LANE_ID  ";
                DataSet newLaneDs = OracleHelper.QueryU_Dev(sqlStr3);

                foreach (DataRow dr in laneDs.Tables[0].Rows)
                {
                    laneList.Add(DataRowToLaneModel(dr));
                }
                foreach (DataRow dr in newLaneDs.Tables[0].Rows)
                {
                    newLaneList.Add(DataRowToNewLaneModel(dr));
                }
                foreach (DataRow dr in crossDs.Tables[0].Rows)
                {
                    crossList.Add(DataRowToCrossModel(dr, laneList,newLaneList));
                }
                retInfo.Code = "20000";
                retInfo.Data = crossList;
            }
            catch(Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }


        [WebMethod]
        public string UpdateCross(string requestStr)
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                List<CrossingInfo> crossList = JsonConvert.DeserializeObject<List<CrossingInfo>>(requestStr.Replace("\n", ""));
                //List<CrossingInfo> crossList = new List<CrossingInfo>();
                //List<LaneInfo> testLane = new List<LaneInfo>();
                //testLane.Add(new LaneInfo
                //{
                //    LaneNO = 1,
                //    LaneName = "testLane",
                //    CrossingId = 1,
                //    FlowName = "testFlowName",
                //    Perct = 1
                //});
                //crossList.Add(
                //    new CrossingInfo()
                //    {
                //        CrossingId = 1,
                //        CrossingName = "test",
                //        SynCrossNO = 1,
                //        SynCrossName = "testcrossname",
                //        SynCrossCoordLng = "123",
                //        SynCrossCoordLat = "234",
                //        Lane = testLane
                //    });
                StringBuilder sqlStr = new StringBuilder();
                foreach (var crossInfo in crossList)
                {
                    sqlStr.Append(" delete from SYN_CROSS_NO where LOCAL_ID=" + crossInfo.SynCrossNO + " and CROSSING_ID=" + crossInfo.CrossingId + " ; ");
                    sqlStr.Append(" insert into SYN_CROSS_NO(LOCAL_ID,CROSSING_ID) values(" + crossInfo.SynCrossNO + "," + crossInfo.CrossingId + "); ");
                    foreach (var laneInfo in crossInfo.Lane)
                    {
                        //string checkFlowDictStr = " begin select * from FLOW_DICT where CROSSING_ID="+ crossInfo.CrossingId + " and FLOW_NAME='"+ laneInfo .FlowName+ "'; " +
                        //    " select * from FLOW_DICT where LANE_NO=" + laneInfo.LaneNO + " ; " +
                        //    " select max(FLOW_NO) MAXFLOWNO from FLOW_DICT where CROSSING_ID=" + crossInfo.CrossingId + "; end;  ";
                        //DataSet ds = OracleHelper.QueryU_Dev(checkFlowDictStr);
                        string dt1Str = " select * from FLOW_DICT where CROSSING_ID=" + crossInfo.CrossingId + " and FLOW_NAME='" + laneInfo.FlowName + "' ";
                        string dt2Str = " select * from FLOW_DICT where LANE_NO=" + laneInfo.LaneNO + " ";
                        string dt3Str = " select nvl(max(FLOW_NO),0) MAXFLOWNO from FLOW_DICT where CROSSING_ID=" + crossInfo.CrossingId + " ";
                        DataTable dt1 = OracleHelper.QueryU_Dev(dt1Str).Tables[0];
                        DataTable dt2 = OracleHelper.QueryU_Dev(dt2Str).Tables[0];
                        DataTable dt3 = OracleHelper.QueryU_Dev(dt3Str).Tables[0];
                        if (dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0]["LANE_NO"].ToString() != dt2.Rows[0]["LANE_NO"].ToString())
                                sqlStr.Append(" delete from FLOW_DICT where  CROSSING_ID=" + crossInfo.CrossingId + " and FLOW_NAME='" + laneInfo.FlowName + "';  ");

                        }
                        if (dt2.Rows.Count > 0)
                        {
                            sqlStr.Append(" update FLOW_DICT set FLOW_NAME='" + laneInfo.FlowName + "' where  LANE_NO=" + laneInfo.LaneNO + " and  CROSSING_ID=" + crossInfo.CrossingId + " ;  ");
                        }
                        else
                        {
                            int maxId = Convert.ToInt32(dt3.Rows[0]["MAXFLOWNO"].ToString()) + 1;
                            sqlStr.Append(" insert into FLOW_DICT(FLOW_ID,FLOW_NO,FLOW_NAME,CROSSING_ID,LANE_NO,PERCENT) values(FLOWDICT_SEQ.nextval," + maxId + ",'" + laneInfo.FlowName + "'," + laneInfo.CrossingId + "," + laneInfo.LaneNO + ",0); ");
                        }

                        
                    }

                }
                string retStr = OracleHelper.ExecuteU_Tran(" begin "+sqlStr.ToString()+" end;");
                retInfo.Code = retStr == "fail" ? "50000" : "20000";
                retInfo.Data = "success";
            }
            catch(Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }

            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }


        public CrossingInfo DataRowToCrossModel(DataRow row,List<LaneInfo> laneList, List<LaneInfo> newLaneList)
        {
            CrossingInfo model = new CrossingInfo();
            if (row != null)
            {
                if (row["CROSSING_ID"] != null && row["CROSSING_ID"].ToString() != "")
                {
                    model.CrossingId = int.Parse(row["CROSSING_ID"].ToString());
                }
                if (row["CROSSING_NAME"] != null)
                {
                    model.CrossingName = row["CROSSING_NAME"].ToString();
                }
                if (row["LOCAL_ID"] != null && row["LOCAL_ID"].ToString() != "")
                {
                    model.SynCrossNO = int.Parse(row["LOCAL_ID"].ToString());
                }
                if (row["LOCAL_NAME"] != null)
                {
                    model.SynCrossName = row["LOCAL_NAME"].ToString();
                }
                if (row["LNG"] != null)
                {
                    model.SynCrossCoordLng = row["LNG"].ToString();
                }
                if (row["LAT"] != null)
                {
                    model.SynCrossCoordLat = row["LAT"].ToString();
                }
                var newLane= newLaneList.Where(x => x.CrossingId == model.CrossingId).ToList();
                if (newLane != null && newLane.Count() > 0)
                    model.Lane = newLane;
                else
                    model.Lane = laneList.Where(x => x.CrossingId == model.CrossingId).ToList();


            }
            return model;
        }

        public LaneInfo DataRowToLaneModel(DataRow row)
        {
            LaneInfo model = new LaneInfo();
            if (row != null)
            {
                //if (row["LANE_ID"] != null && row["LANE_ID"].ToString() != "")
                //{
                //    model.LaneId = int.Parse(row["LANE_ID"].ToString());
                //}
                if (row["LANE_ID"] != null && row["LANE_ID"].ToString() != "")
                {
                    model.LaneNO = int.Parse(row["LANE_ID"].ToString());
                }
                if (row["LANE_NAME"] != null)
                {
                    model.LaneName = row["LANE_NAME"].ToString();
                }
                if (row["CROSSING_ID"] != null && row["CROSSING_ID"].ToString() != "")
                {
                    model.CrossingId = int.Parse(row["CROSSING_ID"].ToString());
                }

            }
            return model;

        }

        public LaneInfo DataRowToNewLaneModel(DataRow row)
        {
            LaneInfo model = new LaneInfo();
            if (row != null)
            {
                if (row["LANE_NO"] != null && row["LANE_NO"].ToString() != "")
                {
                    model.LaneNO = int.Parse(row["LANE_NO"].ToString());
                }
                if (row["FLOW_NAME"] != null)
                {
                    model.FlowName = row["FLOW_NAME"].ToString();
                }
                if (row["LANE_NAME"] != null)
                {
                    model.LaneName = row["LANE_NAME"].ToString();
                }
                if (row["CROSSING_ID"] != null && row["CROSSING_ID"].ToString() != "")
                {
                    model.CrossingId = int.Parse(row["CROSSING_ID"].ToString());
                }
                if (row["PERCENT"] != null && row["PERCENT"].ToString() != "")
                {
                    model.Perct = int.Parse(row["PERCENT"].ToString());
                }

            }
            return model;

        }

        #region 路口相关操作 

        /// <summary>
        /// 添加路口
        /// </summary>
        /// <param name="requestStr"></param>
        /// <returns></returns>
        [WebMethod]
        public string AddPassIdCorreSpond(string requestStr)
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                string retStr = "fail";
                PassIdCorreSpond model = JsonConvert.DeserializeObject<PassIdCorreSpond>(requestStr);
                int maxId = 1;
                string sqlStr1 = "  select max(LOCAL_ID) maxId from  PASS_ID_CORRESPOND ";
                DataSet ds = OracleHelper.QueryU_Dev(sqlStr1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    maxId = Convert.ToInt32(string.IsNullOrEmpty(dr["maxId"].ToString())?"0": dr["maxId"].ToString());
                    model.SynCrossNO = maxId + 1;
                }

                if (model != null)
                {

                    model.UtcsId = model.UtcsId == null? "": model.UtcsId;
                    string sqlStr = " insert into  PASS_ID_CORRESPOND(LOCAL_ID,CROSS_NAME,LNG,LAT,UTCS_ID) values("+model.SynCrossNO+",'"+ model.SynCrossName+ "','" + model.SynCrossCoordLng + "','" + model.SynCrossCoordLat + "','" + model.UtcsId + "')  ";
                    retStr = OracleHelper.ExecuteU_Dev(sqlStr);
                }

                retInfo.Code = retStr == "fail" ? "50000" : "20000";
                retInfo.Msg = "";
                retInfo.Data = model;

            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }


        /// <summary>
        /// 添加路口
        /// </summary>
        /// <param name="requestStr"></param>
        /// <returns></returns>
        [WebMethod]
        public string UpdatePassIdCorreSpond(string requestStr)
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                string retStr = "fail";
                PassIdCorreSpond model = JsonConvert.DeserializeObject<PassIdCorreSpond>(requestStr);
                if (model != null)
                {

                    model.UtcsId = model.UtcsId == null ? "" : model.UtcsId;
                    string sqlStr = " UPDATE PASS_ID_CORRESPOND SET  CROSS_NAME='"+ model.SynCrossName + "',LNG='"+ model .SynCrossCoordLng+ "',LAT='"+model.SynCrossCoordLat+ "' where LOCAL_ID="+model.SynCrossNO + "  ";
                    retStr = OracleHelper.ExecuteU_Dev(sqlStr);
                }

                retInfo.Code = retStr == "fail" ? "50000" : "20000";
                retInfo.Msg = "";
                retInfo.Data = "success";

            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }


        /// <summary>
        /// 删除路口
        /// </summary>
        /// <param name="requestStr"></param>
        /// <returns></returns>
        [WebMethod]
        public string DelPassIdCorreSpond(string requestStr)
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                string retStr = "fail";
                PassIdCorreSpond model = JsonConvert.DeserializeObject<PassIdCorreSpond>(requestStr);
                string corssNo = model.SynCrossNO.ToString();
                string sqlStr1 = " select CROSSING_ID from SYN_CROSS_NO WHERE LOCAL_ID=" + corssNo + " ";
                DataTable dt= OracleHelper.QueryU_Dev(sqlStr1).Tables[0];
                string whereStr = "";
                foreach(DataRow dr in dt.Rows)
                {
                    whereStr += dr[0].ToString() + ",";
                }
                if (whereStr.Length > 0)
                    whereStr = whereStr.Substring(0, whereStr.Length-1);
                string sqlStr2 = " delete from PASS_ID_CORRESPOND where  LOCAL_ID=" + corssNo + ";delete from SYN_CROSS_NO where LOCAL_ID=" + corssNo + "; ";
                if (whereStr != "")
                {
                    sqlStr2 += "delete from FLOW_DICT where CROSSING_ID in (" + whereStr + ");";
                }
                retStr = OracleHelper.ExecuteU_Tran(" begin "+sqlStr2.Replace("\n","")+" end; ");


                retInfo.Code = retStr == "fail" ? "50000" : "20000";
                retInfo.Msg = "";
                retInfo.Data = "success";

            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }
            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }


        [WebMethod]
        public string GetTrafficCrossingInfoList()
        {
            RetInfo retInfo = new RetInfo();
            try
            {
                List<PassIdCorreSpond> modelList = new List<PassIdCorreSpond>();
                List<LaneInfo> laneList = new List<LaneInfo>();
                string sqlStr1 = "  SELECT LOCAL_ID,CROSS_NAME,LNG,LAT,UTCS_ID FROM  PASS_ID_CORRESPOND  ";
                DataSet crossDs = OracleHelper.QueryU_Dev(sqlStr1);
              
                foreach (DataRow dr in crossDs.Tables[0].Rows)
                {
                    modelList.Add(DataRowToPassIdCorreSpondModel(dr));
                }
                retInfo.Code = "20000";
                retInfo.Data = modelList;
            }
            catch (Exception ex)
            {
                var exStr = ex.ToString();
                retInfo.Code = "50000";
                retInfo.Msg = exStr.Length > 200 ? exStr.Substring(0, 200) : exStr;
                retInfo.Data = "error";
            }

            string jsonStr = JsonConvert.SerializeObject(retInfo);
            return jsonStr;
        }



        /// <summary>
		/// 得到一个对象实体
		/// </summary>
		public PassIdCorreSpond DataRowToPassIdCorreSpondModel(DataRow row)
        {
            PassIdCorreSpond model = new PassIdCorreSpond();
            if (row != null)
            {
                if (row["LOCAL_ID"] != null && row["LOCAL_ID"].ToString() != "")
                {
                    model.SynCrossNO = int.Parse(row["LOCAL_ID"].ToString());
                }
                if (row["CROSS_NAME"] != null)
                {
                    model.SynCrossName = row["CROSS_NAME"].ToString();
                }
                if (row["LNG"] != null)
                {
                    model.SynCrossCoordLng = row["LNG"].ToString();
                }
                if (row["LAT"] != null)
                {
                    model.SynCrossCoordLat = row["LAT"].ToString();
                }
               
            }
            return model;
        }

        #endregion

    }
}
