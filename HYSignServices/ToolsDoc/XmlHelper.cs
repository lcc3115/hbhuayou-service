using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccidentInvestigation.Model;
using System.Xml;
namespace HYSignServices.ToolsDoc
{
    public class XmlHelper
    {
        private string xmlsrc;
        private XmlDocument doc;
        private MarkerList markerList;
        private XmlNode markerListNode;
        private XmlNode currentID;

        public XmlHelper(string xmlPath)
        {
            xmlsrc = xmlPath;
            doc = new XmlDocument();
            doc.Load(xmlPath);
            ReturnMarkerList();
        }

        private void ReturnMarkerList()
        {
            markerList=new MarkerList();
            var root = doc.SelectSingleNode("Root");
            markerListNode=root.SelectSingleNode("MarkerList");
            var MarkerList = markerListNode.SelectNodes("Marker");
            if (MarkerList != null)
            {
                currentID = root.SelectSingleNode("CurrentID");
                markerList.CurrentID = Convert.ToInt32(currentID.InnerText);
                foreach (XmlNode item in MarkerList)
                {
                    MarkerData marker = new MarkerData();
                    var serialNumber = item.Attributes["SerialNumber"];
                    int id = Convert.ToInt32(serialNumber.InnerText);
                    marker.SerialNumber = id;
                    var lng = item.SelectSingleNode("Lng");
                    marker.Lng = lng.InnerText;
                    var lat = item.SelectSingleNode("Lat");
                    marker.Lat = lat.InnerText;
                    var name = item.SelectSingleNode("Title");
                    marker.Title = name.InnerText;
                    var streetName = item.SelectSingleNode("StreetName");
                    marker.StreetName = streetName.InnerText;
                    var roadName = item.SelectSingleNode("RoadName");
                    marker.RoadName = roadName.InnerText;
                    var dangerType = item.SelectSingleNode("DangerType");
                    EnumList.DangerType dt;
                    Enum.TryParse<EnumList.DangerType>(dangerType.InnerText, out dt);
                    marker.DangerType = dt;
                    var dangerLevel = item.SelectSingleNode("DangerLevel");
                    marker.DangerLevel = Convert.ToInt32(dangerLevel.InnerText);
                    var time = item.SelectSingleNode("Time");
                    marker.Time = DateTime.Parse(time.InnerText);
                    var content = item.SelectSingleNode("Content");
                    marker.Content = content.InnerText;
                    var images = item.SelectSingleNode("images").SelectNodes("ImgSrc");
                    List<string> imgsrc = new List<string>();
                    foreach (XmlNode src in images)
                    {
                        imgsrc.Add(src.InnerText);
                    }
                    marker.ImageList = imgsrc;
                    markerList.DataList.Add(marker);
                }
            }
        }


        public MarkerList GetList()
        {
            return markerList;
        }


        //添加一个标注
        public string AddMaker(MarkerData data)
        {
            XmlElement marker = doc.CreateElement("Marker");;
            XmlNode lng = doc.CreateElement("Lng");
            XmlNode lat = doc.CreateElement("Lat");
            XmlNode name = doc.CreateElement("Title");

            XmlNode streetName = doc.CreateElement("StreetName");
            XmlNode roadName = doc.CreateElement("RoadName");
            XmlNode dangerType = doc.CreateElement("DangerType");
            XmlNode dangerLevel = doc.CreateElement("DangerLevel");
            XmlNode time = doc.CreateElement("Time");
            XmlNode content = doc.CreateElement("Content");
            try
            {
                lng.InnerText = data.Lng;
                lat.InnerText = data.Lat;
                name.InnerText = data.Title;
                streetName.InnerText = data.StreetName;
                roadName.InnerText = data.RoadName;
                dangerType.InnerText = ((int)data.DangerType).ToString();
                dangerLevel.InnerText = data.DangerLevel.ToString();
                time.InnerText = data.Time.ToString();
                content.InnerText = data.Content;

                marker.AppendChild(lng);
                marker.AppendChild(lat);
                marker.AppendChild(name);
                marker.AppendChild(streetName);
                marker.AppendChild(roadName);
                marker.AppendChild(dangerType);
                marker.AppendChild(streetName);
                marker.AppendChild(dangerLevel);
                marker.AppendChild(time);
                marker.AppendChild(content);


                XmlNode images = doc.CreateElement("images");
                for (int i = 0; i < data.ImageList.Count; i++)
                {
                    XmlNode src = doc.CreateElement("ImgSrc");
                    src.InnerText = data.ImageList[i];
                    images.AppendChild(src);
                }
                marker.AppendChild(images);
                marker.SetAttribute("SerialNumber", (++markerList.CurrentID).ToString());
                markerListNode.AppendChild(marker);
                currentID.InnerText = markerList.CurrentID.ToString();
                data.SerialNumber = markerList.CurrentID;
                markerList.DataList.Add(data);
                doc.Save(xmlsrc);
                return markerList.CurrentID.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public int EditMaker(string content, string name, List<string> imgList, int id, string vstreetName, string vroadName, string vdangerType,string vdangerLevel)
        {
            XmlNode thisMarker = markerListNode.SelectSingleNode("Marker[@SerialNumber='" + id + "']");
            if (thisMarker != null)
            {
                var nameNode = thisMarker.SelectSingleNode("Title");
                var contentNode = thisMarker.SelectSingleNode("Content");
                var streetName = thisMarker.SelectSingleNode("StreetName");
                var roadName = thisMarker.SelectSingleNode("RoadName");
                var dangerType = thisMarker.SelectSingleNode("DangerType");
                var dangerLevel = thisMarker.SelectSingleNode("DangerLevel");

                nameNode.InnerText = name;
                contentNode.InnerText = content;
                streetName.InnerText = vstreetName;
                roadName.InnerText = vroadName;
                dangerType.InnerText = vdangerType;
                dangerLevel.InnerText = vdangerLevel;

                var images = thisMarker.SelectSingleNode("images");
                images.RemoveAll();
                for (int i = 0; i < imgList.Count; i++)
                {
                    XmlNode src = doc.CreateElement("ImgSrc");
                    int index = imgList[i].IndexOf("/Upload");
                    var srcStr= index>=0 ? imgList[i].Substring(index,imgList[i].Length-index) : imgList[i];
                    src.InnerText = srcStr;
                    images.AppendChild(src);
                }
                var marker = markerList.DataList.FirstOrDefault(o => o.SerialNumber == id);
                marker.Content = content;
                marker.Title = name;
                doc.Save(xmlsrc);
                return 0;
            }
            else
            {
                return -1;
            }

        }

        public MarkerList searchMakers(int dtype, int dlevel, string startTime, string endTime, string strretVal)
        {
            if (dtype == -1 && dlevel == -1 && startTime == null && endTime == null && strretVal == "全选")
            {
                return GetList();
            }
            else
            {
                var tempMarkeList=markerList.DataList;
                if(dtype!=-1)
                {
                    if (tempMarkeList!=null)
                        tempMarkeList = tempMarkeList.Where(o => (int)o.DangerType == dtype).ToList();
                }
                if (dlevel != -1)
                {
                    if (tempMarkeList != null)
                        tempMarkeList = tempMarkeList.Where(o => o.DangerLevel == dlevel).ToList();
                }
                if (!string.IsNullOrEmpty(startTime))
                {
                    if (tempMarkeList != null)
                    {
                        DateTime dateStart = DateTime.Parse(startTime);
                        tempMarkeList = tempMarkeList.Where(o => o.Time >= dateStart).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    if (tempMarkeList != null)
                    {
                        DateTime dateEnd = DateTime.Parse(endTime);
                        tempMarkeList = tempMarkeList.Where(o => o.Time <= dateEnd).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(strretVal))
                {
                    if (tempMarkeList != null)
                    {
                        tempMarkeList = tempMarkeList.Where(o => o.StreetName == strretVal).ToList();
                    }
                }



                if (tempMarkeList == null || tempMarkeList.Count==0)
                {
                    MarkerList newList = new MarkerList();
                    newList.CurrentID = -1;
                    return newList;
                }

                markerList.DataList=tempMarkeList;
                return markerList;
            }
        }



    }
}