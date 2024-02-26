using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HYSignServices.Entity;

namespace HYSignServices.HYService
{
    /// <summary>
    /// VehicleSpeeding 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class VehicleSpeeding : System.Web.Services.WebService
    {

        [WebMethod]
        public void SetTrafficData(VehicleInfo vehicleInfo)
        {

            string sql = string.Format("insert into traffic_speeding values(SEQ_SPD.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}')", 
                vehicleInfo.plate_no, vehicleInfo.plate_color, vehicleInfo.vehicle_type, vehicleInfo.vehicle_speed, vehicleInfo.pass_time, vehicleInfo.lane_no, vehicleInfo.address);
            //Tools.WriteErrLog(sql);
            OracleHelper.ExecuteU_Dev_212(sql);
            //OracleHelper.Execute_local249(sql);
        }
    }
}
