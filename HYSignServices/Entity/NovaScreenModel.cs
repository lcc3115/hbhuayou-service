using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class NovaScreenModel
    {
        public string ScreenName { get; set; }
        public string Direction { get; set; }
        public string ScreenIP { get; set; }
        public string DeviceIP { get; set; }
        public string RoadNum { get; set; }
        public string ScreenType { get; set; }
    }
}