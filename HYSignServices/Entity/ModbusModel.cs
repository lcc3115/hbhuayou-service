using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HYSignServices.Entity
{
    /// <summary>
    /// 气象数据模型
    /// </summary>
    public class ModbusModel
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        public double WindSpeed { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public int WindDirection { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// 气压
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// 雨量
        /// </summary>
        public double Rainfall { get; set; }

        /// <summary>
        /// 能见度
        /// </summary>
        public int Visibility { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 路面温度
        /// </summary>
        public double Road_Temperature { get; set; }

        /// <summary>
        /// 路面积水
        /// </summary>
        public double Road_Ponding { get; set; }


        /// <summary>
        /// 路面积冰
        /// </summary>
        public double Road_IceAccretion { get; set; }


        /// <summary>
        /// 路面积雪
        /// </summary>
        public double Road_SnowCover { get; set; }

        /// <summary>
        /// 湿滑程度
        /// </summary>
        public double Slippery { get; set; }
    }
}



