using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccidentInvestigation.Model
{
    public class MarkerList
    {
        public int CurrentID { get; set; }
        public List<MarkerData> DataList { get; set; }

        public MarkerList()
        {
            CurrentID = -1;
            DataList = new List<MarkerData>();
        }
    }

    public class MarkerData
    {
        public int SerialNumber { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public EnumList.DangerType DangerType { get; set; }
        public int DangerLevel { get; set; }
        public DateTime Time { get; set; }
        public string StreetName { get; set; }
        public string RoadName { get; set; }
        public List<string> ImageList { get; set; }
    }
}