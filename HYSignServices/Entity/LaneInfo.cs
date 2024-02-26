using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class LaneInfo
    {
        public int LaneNO { get; set; }
        //public int LaneId { get; set; }
        public string LaneName { get; set; }
        [JsonIgnore]
        public int CrossingId { get; set; }
        public string FlowName { get; set; }
        public int Perct { get; set; }
    }
}