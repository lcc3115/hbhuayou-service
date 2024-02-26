using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class Cross_Dirction_Count
    {
        public string id { get; set; }
        public List<Dirction_Count> dirction_count { get; set; }
    }

    public struct Dirction_Count
    {
        public string direction { get; set; }
        public Int16 blue { get; set; }
        public Int16 yellow { get; set; }
        //public Int16 green { get; set; }
        public Int16 other { get; set; }
    }
    public struct Temp_Dirction_Count
    {
        public string direction { get; set; }
        public Int16 blue { get; set; }
        public Int16 yellow { get; set; }
        public Int16 green { get; set; }
        public Int16 other { get; set; }
        public Int16 white { get; set; }
        public Int16 black { get; set; }
        public Int16 _black { get; set; }
        public Int16 _other { get; set; }
    }
}