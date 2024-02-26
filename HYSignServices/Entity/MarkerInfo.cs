using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BigmapSite.Model
{
    
    public class MarkerInfo
    {
        public static  Dictionary<String, String> typeSet;

        public static Dictionary<String, String> iconSet;
        public MarkerInfo()
        {
            if (typeSet == null)
            {
                typeSet = new Dictionary<string, string>();
                typeSet.Add("中心线绿化带或植埋式护栏", "中心线隔离设施状况");
                typeSet.Add("普通护栏及软质隔离桩等", "中心线隔离设施状况");
                typeSet.Add("无中心线隔离设施", "中心线隔离设施状况");
                typeSet.Add("主辅道隔离(隔离形式为绿化带或护栏)", "道路状况");
                typeSet.Add("无主辅道隔离", "道路状况");
                typeSet.Add("减速带", "道路状况");
                typeSet.Add("十字形", "主路口");
                typeSet.Add("丁字形", "主路口");
                typeSet.Add("辅道连通道路口(同向)", "路侧辅道连通路口情况");
                typeSet.Add("辅道连通道路口(反向)", "路侧辅道连通路口情况");
                typeSet.Add("主道转辅道口(同向)", "路段开口情况");
                typeSet.Add("主道转辅道口(反向)", "路段开口情况");
                typeSet.Add("辅道转主道口(同向)", "路段开口情况");
                typeSet.Add("辅道转主道口(反向)", "路段开口情况");
                typeSet.Add("高架入口匝道(同向)", "路段开口情况");
                typeSet.Add("高架入口匝道(反向)", "路段开口情况");
                typeSet.Add("高架出口匝道(同向)", "路段开口情况");
                typeSet.Add("高架出口匝道(反向)", "路段开口情况");
                typeSet.Add("单位及小区进出口(同向)", "路段开口情况");
                typeSet.Add("单位及小区进出口(反向)", "路段开口情况");
                typeSet.Add("人行横道", "路段开口情况");
                typeSet.Add("掉头车道", "路段开口情况");
                typeSet.Add("公交站点(同向)", "公交站点情况");
                typeSet.Add("公交站点(反向)", "公交站点情况");
                typeSet.Add("油气站点(同向)", "油气站点情况");
                typeSet.Add("油气站点(反向)", "油气站点情况");
                typeSet.Add("上跨、下跨", "道路两侧山体情况");
                typeSet.Add("山体", "道路两侧山体情况");
                typeSet.Add("监控球机", "监控");
                typeSet.Add("警力1", "保障人员");
                typeSet.Add("警力2", "保障人员");
                typeSet.Add("警力3", "保障人员");
                typeSet.Add("自定义路段", "自定义路段");
                typeSet.Add("自定义区域", "自定义区域");
                typeSet.Add("自定义文字描述", "自定义文字描述");
            }

            if (iconSet == null)
            {
                iconSet = new Dictionary<string, string>();
                iconSet.Add("中心线绿化带或植埋式护栏", "#227700");
                iconSet.Add("普通护栏及软质隔离桩等", "#003377");
                iconSet.Add("无中心线隔离设施", "#000000");
                iconSet.Add("主辅道隔离(隔离形式为绿化带或护栏)", "#FF3333");
                iconSet.Add("无主辅道隔离", "#AA7700");
                iconSet.Add("减速带", "1-1.png");
                iconSet.Add("十字形", "1-2.png");
                iconSet.Add("丁字形", "1-3.png");
                iconSet.Add("辅道连通道路口(同向)", "2-1.png");
                iconSet.Add("辅道连通道路口(反向)", "2-1.png");
                iconSet.Add("主道转辅道口(同向)", "2-2.png");
                iconSet.Add("主道转辅道口(反向)", "2-2.png");
                iconSet.Add("辅道转主道口(同向)", "2-2.png");
                iconSet.Add("辅道转主道口(反向)", "2-2.png");
                iconSet.Add("高架入口匝道(同向)", "2-3.png");
                iconSet.Add("高架入口匝道(反向)", "2-3.png");
                iconSet.Add("高架出口匝道(同向)", "2-3.png");
                iconSet.Add("高架出口匝道(反向)", "2-3.png");
                iconSet.Add("单位及小区进出口(同向)", "2-3.png");
                iconSet.Add("单位及小区进出口(反向)", "2-3.png");
                iconSet.Add("人行横道", "3-1.png");
                iconSet.Add("掉头车道", "3-2.png");
                iconSet.Add("公交站点(同向)", "4-1.png");
                iconSet.Add("公交站点(反向)", "4-1.png");
                iconSet.Add("油气站点(同向)", "4-2.png");
                iconSet.Add("油气站点(反向)", "4-2.png");
                iconSet.Add("上跨、下跨", "5-1.png");
                iconSet.Add("山体", "5-2.png");
                iconSet.Add("监控球机", "6-2.png");
                iconSet.Add("警力1", "6-1.png");
                iconSet.Add("警力2", "6-3.png");
                iconSet.Add("警力3", "6-4.png");
                iconSet.Add("自定义路段", "");
                iconSet.Add("自定义区域", "");
                iconSet.Add("自定义文字描述", "");
            }
        }

        public string markerid { get; set; }
        public string mainType { get; set; }
        public string subType { get; set; }
        public List<double[]> polylinePosition { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string distance { get; set; }
        public string content { get; set; }

    }

    

    public class AjaxModel
    {
        public string lat { get; set; }
        public string lng { get; set; }
        public string equals { get; set; }
        public string toString { get; set; }
        public string distanceTo { get; set; }
        public string wrap { get; set; }
        public string toBounds { get; set; }
        public string clone { get; set; }
    }
}