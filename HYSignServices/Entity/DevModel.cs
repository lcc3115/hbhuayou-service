using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class DevModel
    {
        public string Name { get; set; }
        public List<DevDtl> Device { get; set; }
    }
}