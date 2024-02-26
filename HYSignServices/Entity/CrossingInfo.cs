using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class CrossingInfo
    {
        public int CrossingId { get; set; }

        public string CrossingName { get; set; }
        public int? SynCrossNO { get; set; }
        public string SynCrossName { get; set; }
        public string SynCrossCoordLng { get; set; }
        public string SynCrossCoordLat { get; set; }
        public List<LaneInfo> Lane { get; set; }

    }
}