using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class AlarmJson
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string roadName { get; set; }
    }

    public class Data
    {
        public string cross_id { get; set; }
        public string cross_name { get; set; }
        public string plate_no { get; set; }
        public string plate_color { get; set; }
        public string passtime { get; set; }
        public string lane_name { get; set; }
        public string sysdict_code { get; set; }
        public string sysdict_name { get; set; }
        public string dev_ip { get; set; }
        public string dev_code { get; set; }
        
    }

    public class ExcelData
    {
        public string cross_name { get; set; }
        public string dev_ip { get; set; }
        public string direction { get; set; }
        public string dire_index { get; set; }
        public string dev_code { get; set; }
        public string[] roadNum { get; set; }
    }
    public class ResouersData
    {
        public string cross_id { get; set; }

        public string alarm_id { get; set; }
        public string cross_name { get; set; }
        public string alarm_time { get; set; }
        public string amarm_road { get; set; }
        public string plate_no { get; set; }
        public string dev_ip { get; set; }
        public string dev_code { get; set; }
        public string sysdict_code { get; set; }
        public string sysdict_name { get; set; }
        public string operation { get; set; }
    }
    public class HH24Data
    {
        public string cross_name { get; set; }
        public string lane_name { get; set; }
        public string dev_code { get; set; }
        public string sysdict_code { get; set; }
        public string HH00 { get; set; }
        public string HH01 { get; set; }
        public string HH02 { get; set; }
        public string HH03 { get; set; }
        public string HH04 { get; set; }
        public string HH05 { get; set; }
        public string HH06 { get; set; }
        public string HH07 { get; set; }
        public string HH08 { get; set; }
        public string HH09 { get; set; }
        public string HH10 { get; set; }
        public string HH11 { get; set; }
        public string HH12 { get; set; }
        public string HH13 { get; set; }
        public string HH14 { get; set; }
        public string HH15 { get; set; }
        public string HH16 { get; set; }
        public string HH17 { get; set; }
        public string HH18 { get; set; }
        public string HH19 { get; set; }
        public string HH20 { get; set; }
        public string HH21 { get; set; }
        public string HH22 { get; set; }
        public string HH23 { get; set; }
    }
    public class AnalyseDayData
    {
        public string cross_name { get; set; }
        public string lane_name { get; set; }
        public string dev_code { get; set; }
        public string sysdict_code { get; set; }
        public string day1 { get; set; }
        public string day2 { get; set; }
        public string today { get; set; }
        public string persenDay1 { get; set; }
        public string persenDay2 { get; set; }
    }

    public class PlateCheck
    {
        public string plate { get; set; }
        public string color { get; set; }
        public string date { get; set; }
        public string count { get; set; }
        
    }

    public class CrossCheck
    {
        public string plate { get; set; }
        public string s_time { get; set; }
        public string e_time { get; set; }
        public string cross_name { get; set; }
        public string direction { get; set; }
        public string count { get; set; }
    }
}