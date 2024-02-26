using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using blqw;
using BigmapSite.Model;
using Newtonsoft.Json;
using BigmapSite.Tools;

namespace BigmapSite.Site
{
    public partial class MapSet : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax2.Register(this);
            new MarkerInfo();
        }

        /// <summary>
        /// 获取静态主类型
        /// </summary>
        /// <param name="content">子类型</param>
        /// <returns></returns>
        [blqw.AjaxMethod]
        public string GetTitle(string subType)
        {
            string title = MarkerInfo.typeSet[subType];
            return title;
        }

        [blqw.AjaxMethod]
        public string GetIcon(string subType)
        {
            string icon = MarkerInfo.iconSet[subType];
            return icon;
        }
        [blqw.AjaxMethod]
        public string GetMarkers()
        {
            List<MarkerInfo> markerInfoArray = SqlHelper.GetMarkers();
            string jsonData = JsonConvert.SerializeObject(markerInfoArray);
            return jsonData;
        }

        [blqw.AjaxMethod]
        public string SavePolyline(string latlngs, string polylineTitle, string mainTitle, string guid,string distance,string color)
        {
            List<AjaxModel> ajaxModelArray = JsonConvert.DeserializeObject<List<AjaxModel>>(latlngs);
            string polylinePostion = "";
            foreach (AjaxModel ajaxModel in ajaxModelArray)
            {
                string lat = Math.Round(double.Parse(ajaxModel.lat), 5).ToString();
                string lng = Math.Round(double.Parse(ajaxModel.lng), 5).ToString();
                polylinePostion += lat + "," + lng + ":";
            }
            
            return SqlHelper.SavePolyline(polylinePostion.TrimEnd(':'), polylineTitle, mainTitle, guid, distance,color);
        }

        [blqw.AjaxMethod]
        public string SaveMarker(string lat,string lng,string markerTitle, string mainTitle, string guid)
        {
            return SqlHelper.SaveMarker(lat, lng, markerTitle, mainTitle, guid);
        }
        [blqw.AjaxMethod]
        public string RemoveObject(string obj_id)
        {
            return SqlHelper.RemoveObject(obj_id);
        }
        [blqw.AjaxMethod]
        public string NewGUID()
        {
            return Guid.NewGuid().ToString();
        }


    }
}