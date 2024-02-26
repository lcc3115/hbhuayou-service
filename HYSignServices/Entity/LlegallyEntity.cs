using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class LlegallyEntity
    {
        public string rn { get; set; }
        public string crossName { get; set; }
        public string plate { get; set; }
        public string llegalTime { get; set; }
        public string count { get; set; }

        
    }
    public class TempCount
    {
        public string crossName { get; set; }
        public string plate { get; set; }
        public string count { get; set; }
        public string llegalTime { get; set; }
    }

}