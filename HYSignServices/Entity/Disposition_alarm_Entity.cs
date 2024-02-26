using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    /// <summary>
    /// 布控车辆结构体
    /// </summary>
    public class Disposition_alarm_Entity
    {
        public string lane_id;
        public string lane_name;
        public string crossing_id;
        public string crossing_name;
        public string disposition_reason;
        public string vehicle_color;
        public string vehicle_type;
        public string plate_type;
        public string plate_color;
        public string pass_time;
        public string plate_no;

    }
}