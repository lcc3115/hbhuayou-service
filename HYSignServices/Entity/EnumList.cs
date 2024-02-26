using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccidentInvestigation.Model
{
    public class EnumList
    {
        public enum DangerType
        {
            施工打围不规范 = 0,
            路口视距不足 = 1,
            临水或临崖=2,
            陡坡=3,
            急弯=4,
            车速过快=5,
            人车混行=6,
            标线磨损=7,
            行人横穿=8,
            路面破损=9,
            路中障碍物=10,
            照明隐患=11,
            转弯半径不够=12,
            雨雪天气湿滑=13,
            标志标牌设备不全=14,
            其他隐患=15
        }

        public enum StreetName
        {
            纱帽街=0,
            邓南街=1,
            东荆街=2,
            湘口街=3
        }

        public enum DangerLevel
        {
            紧急=0,
            普通=1
        }
    }
}