using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using BigmapSite.Model;
using HYSignServices.Entity;

namespace BigmapSite.Tools
{
    public class SqlHelper
    {
        public static string SaveMarker(string lat, string lng, string markerTitle, string mainTitle, string guid)
        {
            string sql = string.Format("INSERT INTO BIGMAPSITE(MARKERID,MAINTYPE,SUBTYPE,LAT,LNG) VALUES('{0}','{1}','{2}','{3}','{4}')", guid, mainTitle, markerTitle, lat, lng);
            string res = "";
            try
            {
                res = OracleHelper.ExecuteU_Dev_212(sql);
            }
            catch (OracleException ex)
            {
                return ex.Message;
            }
            return res;
        }

        public static string SavePolyline(string linePosition, string polylineTitle, string mainTitle, string guid,string distance,string color)
        {
            string sql = string.Format("INSERT INTO BIGMAPSITE(MARKERID,MAINTYPE,SUBTYPE,POLYLINEPOSTION,DISTANCE,CONTENT) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", guid, mainTitle, polylineTitle, linePosition, distance,color);
            string res = "";
            try
            {
                res = OracleHelper.ExecuteU_Dev_212(sql);
            }
            catch (OracleException ex)
            {
                return ex.Message;
            }
            return res;
        }

        public static List<MarkerInfo> GetMarkers()
        {
            string sql = "SELECT MARKERID,MAINTYPE,SUBTYPE,LAT,LNG,POLYLINEPOSTION,DISTANCE,CONTENT FROM BIGMAPSITE";
            try
            {
                DataSet ds = OracleHelper.Query212(sql);
                if (HYSignServices.HYService.Tools.DSisNull(ds))
                {
                    List<MarkerInfo> list = new List<MarkerInfo>();
                    DataTable dt = ds.Tables[0];
                    MarkerInfo markerInfo;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        markerInfo = new MarkerInfo();
                        markerInfo.markerid = dt.Rows[i][0].ToString();
                        markerInfo.mainType = dt.Rows[i][1].ToString();
                        markerInfo.subType = dt.Rows[i][2].ToString();
                        markerInfo.lat = dt.Rows[i][3].ToString();
                        markerInfo.lng = dt.Rows[i][4].ToString();
                        string polylineStr = dt.Rows[i][5].ToString();
                        if (!string.IsNullOrEmpty(polylineStr))
                        {
                            markerInfo.polylinePosition = new List<double[]>();
                            string[] polylineArray = polylineStr.Split(':');
                            foreach (string item in polylineArray)
                            {
                                string[] tempArray = item.Split(',');
                                double[] latlng = Array.ConvertAll(tempArray, double.Parse);
                                markerInfo.polylinePosition.Add(latlng);
                            }
                            //markerInfo.polylinePosition = Array.ConvertAll(polylineArray, double.Parse);
                        }
                        markerInfo.distance = dt.Rows[i][6].ToString();
                        markerInfo.content = dt.Rows[i][7].ToString();
                        list.Add(markerInfo);
                    }
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
                return null;
            }
        }

        public static string RemoveObject(string obj_id)
        {
            string sql = "DELETE FROM BIGMAPSITE WHERE MARKERID = '"+ obj_id + "'";
            string res = "";
            try
            {
                res = OracleHelper.ExecuteU_Dev_212(sql);
            }
            catch (OracleException ex)
            {
                return ex.Message;
            }
            return res;
        }

        //违停查重
        public static PlateCheck GetPassCount(string plate, string sTime, string eTime)
        {
            string sql = "select count(1),plate_color from TRAFFIC_VEHICLE_PASS where plate_no = '" + plate + "' and pass_time >= to_timestamp('" + sTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') " +
                        "and pass_time <= to_timestamp('" + eTime + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') group by plate_color";
            DataSet ds = OracleHelper.Query0249(sql);
            DataTable dt = ds.Tables[0];
            PlateCheck plateCheck = new PlateCheck();
            plateCheck.plate = plate;
            plateCheck.date = eTime;
            if (HYSignServices.HYService.Tools.DSisNull(ds))
            {
                
                plateCheck.count = dt.Rows[0][0].ToString();
                plateCheck.color = HYSignServices.HYService.Tools.ConvertColor(dt.Rows[0][1].ToString());
            }
            else
            {
                plateCheck.count = "0";
                plateCheck.color = "";
            }
            return plateCheck;//鄂A001ZG
        }

        //过车筛查
        public static DataSet CheckPassCross(CrossCheck crossCheck)
        {
            string sql = "select pass.plate_no,cross.crossing_name,lane.lane_name,count(3) numb from TRAFFIC_VEHICLE_PASS pass, TRAFFIC_CROSSING_INFO cross ,traffic_lane_info lane ";
            sql += "where pass.crossing_id = cross.crossing_id and cross.crossing_id = lane.crossing_id and pass.lane_no = lane.lane_no ";
            sql += "and lane.lane_name like '%" + crossCheck.direction + "%' and pass.plate_no = '" + crossCheck.plate + "' and cross.crossing_name = '" + crossCheck.cross_name + "' ";
            sql += "and pass.pass_time >= to_timestamp('" + crossCheck.s_time + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            sql += "and pass.pass_time <= to_timestamp('" + crossCheck.e_time + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            sql += "group by pass.plate_no,cross.crossing_name,lane.lane_name";
            DataSet ds = OracleHelper.Query0249(sql);
            return ds;
        }

        
    }
}