using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class Crossing_Info
    {
        public int Crossing_Id { get; set; }

        public string Crossing_Name { get; set; }

        public List<Lane_Info> Lane { get; set; }

    }
}