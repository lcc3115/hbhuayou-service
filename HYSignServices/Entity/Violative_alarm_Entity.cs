using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    /// <summary>
    /// 违法过车信息结构体
    /// </summary>
    public class Violative_alarm_Entity
    {
        public string plate_no { get; set; }
        public string pass_time { get; set; }
        public string crossing_id { get; set; }
        public string crossing_name { get; set; }
        public string lane_id { get; set; }
        public string lane_name { get; set; }
        public string dirction { get; set; }
        public string vehicle_color { get; set; }
        public string vehicle_type { get; set; }
        public string plate_type { get; set; }
        public string plate_color { get; set; }
        
        
    }
}