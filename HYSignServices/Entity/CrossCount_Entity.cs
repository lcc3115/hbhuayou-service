using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class CrossCount_Entity
    {
        public string id;
        public List<CrossCount> crossCount;
    }

    public struct CrossCount
    {
        public string num;
        public string type;
    }
}