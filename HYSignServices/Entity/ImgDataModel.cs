using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class ImgDataModel
    {
        public string Index { get; set; }
        public string Url { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}