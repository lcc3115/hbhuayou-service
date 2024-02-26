using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class RquestParm
    {
        public List<QueryModel> queryArray { get; set; }
        public List<QueryModel> compareArray { get; set; }
    }
}