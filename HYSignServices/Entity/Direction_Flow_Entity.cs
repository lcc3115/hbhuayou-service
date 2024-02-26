using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class Direction_Flow_Entity
    {
        public string section;
        public string date;
        public List<Traffic_Data> data;
    }
    
    public struct TimepLog
    {
        public string morning_date_start;
        public string morning_date_end;
        public string night_date_start;
        public string night_date_end;
        public string allof_date_start;
        public string allof_date_end;
    }
}