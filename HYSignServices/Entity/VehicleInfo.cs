using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class VehicleInfo
    {
        public string plate_no { get; set; }
        public string plate_color { get; set; }
        public string vehicle_type { get; set; }
        public string vehicle_speed { get; set; }
        public string pass_time { get; set; }
        public string lane_no { get; set; }
        public string address { get; set; }
    }
}