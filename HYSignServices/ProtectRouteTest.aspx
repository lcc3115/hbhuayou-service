<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProtectRouteTest.aspx.cs" Inherits="HYSignServices.ProtectRouteTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width initial-scale=1.0 maximum-scale=1.0 user-scalable=0"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link rel="stylesheet" href="http://cache.amap.com/lbs/static/main1119.css"/>
    <%--<script type="text/javascript" src="http://webapi.amap.com/maps?v=1.4.5&key=9e01a63f0420dd7aff38aadbe457e953&plugin=Map3D,AMap.ControlBar,AMap.MouseTool"></script>--%>
    
    <script src="//webapi.amap.com/maps?v=1.4.14&key=9e01a63f0420dd7aff38aadbe457e953&plugin=Map3D"></script>
    <script type="text/javascript" src="http://cache.amap.com/lbs/static/addToolbar.js"></script>

	<script type="text/javascript" src="http://a.amap.com/jsapi_demos/static/demo-center/model/js/three.js"></script>
	<script type="text/javascript" src="http://a.amap.com/jsapi_demos/static/demo-center/model/js/loaders/MTLLoader.js"></script>
	<script type="text/javascript" src="http://a.amap.com/jsapi_demos/static/demo-center/model/js/loaders/LoaderSupport.js"></script>
	<script type="text/javascript" src="http://a.amap.com/jsapi_demos/static/demo-center/model/js/loaders/OBJLoader2.js"></script>
    <style type="text/css">
        .reserch
        {
            position: absolute;
	        top:10px;
	        left:100px;
	        padding: 10px;  
        }
        </style>
</head>

<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            /***
            **************************************************************
            *                                                            *
            *                         BUG退散！                          *
            *   .=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-.       *
            *    |                     ______                     |      *
            *    |                  .-"      "-.                  |      *
            *    |                 /            \                 |      *
            *    |     _          |              |          _     |      *
            *    |    ( \         |,  .-.  .-.  ,|         / )    |      *
            *    |     > "=._     | )(__/  \__)( |     _.=" <     |      *
            *    |    (_/"=._"=._ |/     /\     \| _.="_.="\_)    |      *
            *    |           "=._"(_     ^^     _)"_.="           |      *
            *    |               "=\__|IIIIII|__/="               |      *
            *    |              _.="| \IIIIII/ |"=._              |      *
            *    |    _     _.="_.="\          /"=._"=._     _    |      *
            *    |   ( \_.="_.="     `--------`     "=._"=._/ )   |      *
            *    |    > _.="                            "=._ <    |      *
            *    |   (_/                                    \_)   |      *
            *    |                                                |      *
            *    '-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-='      *
            *                                                            *
            *                   BUG ELIMINATE PREPARATION                *
            **************************************************************
            */
            //从设备列表定位到地图页面
            function loadDeviceFromTree(data) {
                //console.log(data);
                setMapCenter(data);
            }
            function Search_point() {
//                var jsonData = testjson();
//                if (jsonData != null) {
//                    var deviceInfo = JSON.parse(jsonData);
//                    for (var i = 0; i < deviceInfo.length; i++) {
//                        console.log(deviceInfo[i].PASSTIME);
//                        console.log(deviceInfo[i].CHECK_TYPE);
//                        console.log(deviceInfo[i].DIRECTION);
//                        console.log(deviceInfo[i].IP_ADDR);
//                        console.log(deviceInfo[i].PIC_NAME);
//                    }
//                }

                var no = document.getElementById('scr_no').value;
                for (var i = 0; i < deviceInfo.length; i++) {
                    if (no == deviceInfo[i].CROSS_NO) {
                        map.setZoomAndCenter(17, [deviceInfo[i].POSITION_X, deviceInfo[i].POSITION_Y]);

                        var content = '<div>ID：' + deviceInfo[i].CROSS_NO + '</div>';
                        content += '<div>NAME：' + deviceInfo[i].CROSS_NAME + '</div>';
                        var infoWindow = new AMap.InfoWindow({ offset: new AMap.Pixel(0, -30) });
                        infoWindow.setContent(content);
                        infoWindow.open(map, [deviceInfo[i].POSITION_X, deviceInfo[i].POSITION_Y]);
                        break;
                    }
                }
            }
            
        </script>
    </form>
    <div id="container"></div>
    <div class="reserch" runat="server" visible="true" id="Research_div">
        NO:
        <input type="text" runat="server" value="" id="scr_no" />
        <%--dirt1:
        <input type="text" runat="server" value="" id="dirt1" />
        dirt2:
        <input type="text" runat="server" value="" id="dirt2" />--%>
        <input type="button" id="serch_btn" value="查询" onclick="Search_point()"/>
    </div>
    <script type="text/javascript">
        var map;
        var geolocation;
        //var marker;
        var markero;
        var lon, lat;
        map = new AMap.Map('container', {
            resizeEnable: true,
            rotateEnable: true,
            pitchEnable: true,
            zoom: 17,
            pitch: 80,
            rotation: -15,
            viewMode: '3D', //开启3D视图,默认为关闭
            buildingAnimation: false, //楼块出现是否带动画
            expandZoomRange: true,
            zooms: [3, 20],
            center: [114.1889709, 30.5106787]
            
            //layers: [new AMap.TileLayer.Satellite()]
        });
        //map.on('click', showInfoClick);

        // 添加到地图上
        //map.add(layer1);
        function setMapCenter(data) {
            console.log(data);
            var info = JSON.parse(data);
            var lng = info[0].LON;
            var lat = info[0].LAT;
            //map.setZoomAndCenter(17, [lng, lat]);
            if (markero != null) {
                map.remove(markero);
            }


            markero = new AMap.Marker({
                position: [lng, lat],
                title: info[0].POINTNAME, //鼠标停留显示的信息
                //icon: src, //自定义标记图片
                imageSize: new AMap.Size(2, 2),
                clickable: false, //可点击
                draggable: false, //可拖拽
                map: map

            });
        }

        //setInterval("showTime()", 2000);
        function showTime() {
            var lnglat = getLngLat();
            //console.log(lnglat);
            var gisarray = lnglat.split(',');
            //var dirt = array[1];
            var lng = gisarray[0];
            var lat = gisarray[1];
            //map.setZoomAndCenter(17, [lng, lat]);
            //console.log("lng:" + lng + ",lat:" + lat + ",dir:" + dirt);
            if (markero != null) {
                map.remove(markero);
            }

            markero = new AMap.Marker({
                position: [lng, lat],
                //title: info[0].POINTNAME, //鼠标停留显示的信息
                //icon: src, //自定义标记图片
                imageSize: new AMap.Size(2, 2),
                clickable: false, //可点击
                draggable: false, //可拖拽
                map: map
            });
        }

        var op = 0;
        var lng;
        var lat;
        var markero1;
        function showInfoClick(e) {
            op++;
//            lng = e.lnglat.getLng();
//            lat = e.lnglat.getLat();
//            console.log(lng + "," + lat);
            if (op == 1) {
                if (markero != null) {
                    map.remove(markero);
                }
                if (markero1 != null) {
                    map.remove(markero1);
                }
                lng = e.lnglat.getLng();
                lat = e.lnglat.getLat();
                console.log("点1：" + lng + "," + lat);
                markero1 = new AMap.Marker({
                    position: [lng, lat],

                    imageSize: new AMap.Size(1, 1),
                    clickable: false, //可点击
                    draggable: false, //可拖拽
                    map: map
                });
            }
            if (op == 2) {
                var lng1 = e.lnglat.getLng();
                var lat1 = e.lnglat.getLat();
                console.log("点2：" + lng1 + "," + lat1);
                var dis = AMap.GeometryUtil.distance([lng, lat], [lng1, lat1]);
                console.log("距离:" + dis);
                op = 0;
                markero = new AMap.Marker({
                    position: [lng1, lat1],

                    imageSize: new AMap.Size(0.3, 0.3),
                    clickable: false, //可点击
                    draggable: false, //可拖拽
                    map: map
                });
                
            }

        }
        //GETSCREEN();
        function GETSCREEN() {
            var jsonData = GetScreenInfo();
            if (jsonData != null) {
                var deviceInfo = JSON.parse(jsonData);
                var icon = new AMap.Icon({
                    size: new AMap.Size(20, 25),
                    image: "../images/icon-lb02.png",
                    imageSize: new AMap.Size(20, 25)
                    //imageOffset: new AMap.Pixel(-95, -3)
                });
                for (var i = 0; i < deviceInfo.length; i++) {
                    var marker = new AMap.Marker({
                        position: [deviceInfo[i].LNG, deviceInfo[i].LAT],
                        title: deviceInfo[i].SCREEN_NO, //鼠标停留显示的信息
                        
                        icon: icon, //自定义标记图片
                        size: new AMap.Size(5, 5),
                        imageSize: new AMap.Size(5, 5),
                        clickable: true, //可点击
                        draggable: true, //可拖拽
                        extData: deviceInfo[i].SCREEN_NO,
                        map: map
                    });
                    marker.content = '<div class="info-title">点位信息</div><br>';
                    marker.content += '<div>项目名称：' + deviceInfo[i].SCREEN_NO + '</div>';
                    
                    marker.on('click', markerClick);
                }
            }
        }
        var deviceInfo;
        GetAllUTCS();
        function GetAllUTCS() {
            var jsonData = GetUTCSInfo();
            if (jsonData != null) {
                deviceInfo = JSON.parse(jsonData);
                var icon = new AMap.Icon({
                    size: new AMap.Size(20, 25),
                    image: "../images/icon-lb02.png",
                    imageSize: new AMap.Size(20, 25)
                    //imageOffset: new AMap.Pixel(-95, -3)
                });
                for (var i = 0; i < deviceInfo.length; i++) {
                    var marker = new AMap.Marker({
                        position: [deviceInfo[i].LNG, deviceInfo[i].LAT],
                        title: deviceInfo[i].DEV_NAME, //鼠标停留显示的信息
                        //offset: new AMap.Pixel(10, 10),
                        icon: icon, //自定义标记图片
                        size: new AMap.Size(5, 5),
                        imageSize: new AMap.Size(5, 5),
                        clickable: true, //可点击
                        draggable: true, //可拖拽
                        extData: deviceInfo[i].ID,
                        map: map
                    });
                    marker.content = '<div class="info-title">点位信息</div><br>';
                    marker.content += '<div>ID：' + deviceInfo[i].ID + '</div>';
                    marker.content += '<div>NAME：' + deviceInfo[i].DEV_NAME + '</div>';
                    marker.on('click', markerClick);
                    AMap.event.addListener(marker, 'dragend', _isondragend); //添加标记拖动结束时触发的事件
                    
                }
            }
        }
        function _isondragend(e) {
//            var infoWindow = new AMap.InfoWindow({ offset: new AMap.Pixel(0, -30) });
//            infoWindow.setContent(e.target.content);
            //            infoWindow.open(map, e.target.getPosition());
            var lng = e.lnglat.getLng();
            var lat = e.lnglat.getLat();
            var id = e.target.B.extData;
            console.log(id);
            console.log(lng);
            console.log(lat);
            SetLngLatByID(id, lng, lat);
        }
        

        function markerClick(e) {
            //console.log(e);
            var content = e.target.content;
//            var NO = e.target.B.extData;
//            content += '<div>' + e.lnglat.lng + ',' + e.lnglat.lat + '</div><br>';
//            content += "&nbsp;<a href='javascript:void(0)' onclick='changeOP(\"" + NO + "," + e.lnglat.lng + "," + e.lnglat.lat + "\")'>保存</a>";
            var infoWindow = new AMap.InfoWindow({ offset: new AMap.Pixel(0, -30) });
            infoWindow.setContent(content);
            infoWindow.open(map, e.target.getPosition());
        }
        function changeOP(data) {
            var res = SetScreenInfo(data);
            alert(res);
        }
        

//        var ws = new WebSocket("ws://111.47.4.163:8058/");
// 
//        ws.onopen = function(evt) {
//	        console.log("Connection open ...");
//	        ws.send("Hello WebSockets!");
//        };
//        
//        ws.onmessage = function (evt) {
//            //console.log("Received Message: " + evt.data);

//            if (evt.data.substring(0, 3) == "GIS") {
//                var array = evt.data.split('_');
//                var gisarray = array[1].split(',');
//                //var dirt = array[1];
//                var lng = gisarray[0];
//                var lat = gisarray[1];
//                //console.log("lng:" + lng + ",lat:" + lat + ",dir:" + dirt);
//                if (markero != null) {
//                    map.remove(markero);
//                }

//                markero = new AMap.Marker({
//                    position: [lng, lat],
//                    //title: info[0].POINTNAME, //鼠标停留显示的信息
//                    //icon: src, //自定义标记图片
//                    imageSize: new AMap.Size(2, 2),
//                    clickable: false, //可点击
//                    draggable: false, //可拖拽
//                    map: map
//                });
//            }

//        };
// 
//        ws.onclose = function(evt) {
//	        console.log("Connection closed.");
//        };

//        ws.onerror = function (evt) {
//            console.log(evt.data);
        //        }


//        var polyline = new AMap.Polyline({
//            path: [
//                new AMap.LngLat(114.099488, 30.456294),
//                new AMap.LngLat(114.104493, 30.457615)
//            ],
//            borderWeight: 100, // 线条宽度，默认为 1
//            strokeColor: 'green', // 线条颜色
//            lineJoin: 'round' // 折线拐点连接处样式
//            
//        });
//        map.add(polyline);
//        polyline.on('click', clickHandler);

//        
//        function clickHandler(e) {
//            //            e.target.B.strokeColor = 'red';
//            //            e.target.He.strokeColor = 'red';
//            e.target.$e.altitude[0].Re.L.style.strokeColor = 'red';
//        }

        
        
    </script>
</body>
</html>
