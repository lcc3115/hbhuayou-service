using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class Lane_Info
    {
        public int Lane_NO { get; set; }
        public int Lane_Id { get; set; }
        public string Lane_Name { get; set; }
        [JsonIgnore]
        public int Crossing_Id { get; set; }
        public string Flow_Name { get; set; }
        public int Perct { get; set; }
    }
}