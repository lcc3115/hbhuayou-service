using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class UnityDeviceInfo
    {
        private int? _id;
        private string _devname;
        private string _idtype;
        private string _entitygroupname;
        private string _monitortype;
        private string _ip;
        private int? _port;
        private string _username;
        private string _password;
        private string _position;
        private string _rotation;
        private string _remark;
        private string _initrotation;
        private string _belong;
        private string _lng;
        private string _lat;

        public int? Id {get;set;}
        public string DevName {get;set;}
        public string IdType {get;set;}
        public string EntityGroupName {get;set;}
        public string MonitorType {get;set;}
        public string Ip {get;set;}
        public int? Port {get;set;}
        //public string UserName { get => _username; set => _username = value; }
        //public string Password { get => _password; set => _password = value; }
        //public string Position { get => _position; set => _position = value; }
        //public string Rotation { get => _rotation; set => _rotation = value; }
        public string Remark {get;set;}
        public string Initrotation {get;set;}
        public string Belong {get;set;}
        public string Lng {get;set;}
        public string Lat { get; set; }
    }
}