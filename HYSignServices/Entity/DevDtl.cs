using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class DevDtl
    {
        public string DeviceName { get; set; }
        public List<ImgDataModel> Img { get; set; }
    }
}