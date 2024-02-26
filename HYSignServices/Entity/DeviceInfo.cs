using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HYSignServices.Entity
{
    public class DeviceInfo
    {
        public string cross_id { get; set; }
        public string dev_name { get; set; }
        public string dev_ip { get; set; }
        public Int16 port { get; set; }
        public string dev_user { get; set; }
        public string dev_psw { get; set; }
        public string direction { get; set; }
    }
}
