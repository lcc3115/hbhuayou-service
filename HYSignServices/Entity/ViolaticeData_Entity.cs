using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class ViolaticeData_Entity
    {
        public string id;
        public List<ViolaticeData> violaticeData;
    }

    public struct ViolaticeData
    {
        public string num;
        public string type;
    }
}