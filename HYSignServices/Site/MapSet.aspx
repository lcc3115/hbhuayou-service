<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapSet.aspx.cs" Inherits="BigmapSite.Site.MapSet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href='http://192.168.92.212:9000/bigemap.js/v2.1.0/bigemap.css' rel='stylesheet'/>
    <script type="text/javascript" src='http://192.168.92.212:9000/bigemap.js/v2.1.0/bigemap.js'></script>
    <script src="../Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/bm.draw.min.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../Styles/Bigemap.draw.css"/>
    <style type="text/css">
        body { margin: 0; padding: 0; }
        #map { position: absolute; top: 0; bottom: 0; width: 100%; }
        #inputData {
            background-color: rgba(85, 189, 208, 0.37);
            visibility: hidden;
            position: absolute;
            margin:0 auto;
            z-index: 2;
            border-radius:10px;
            top:70px;
            left:20px;
            bottom:50px;
            height:0 auto;
            width:300px;
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
        
    </form>
    <div id='inputData'> 
        <input name="Fruit" type="radio" value="中心线绿化带或植埋式护栏" onclick="radioCheck(this)" />中心线绿化带或植埋式护栏<br/>
        <input name="Fruit" type="radio" value="普通护栏及软质隔离桩等" onclick="radioCheck(this)"/>普通护栏及软质隔离桩等<br/>
        <input name="Fruit" type="radio" value="无中心线隔离设施" onclick="radioCheck(this)"/>无中心线隔离设施<br/>
        <input name="Fruit" type="radio" value="主辅道隔离(隔离形式为绿化带或护栏)" onclick="radioCheck(this)"/>主辅道隔离(隔离形式为绿化带或护栏)<br/>
        <input name="Fruit" type="radio" value="无主辅道隔离" onclick="radioCheck(this)"/>无主辅道隔离<br/>
        <input name="Fruit" type="radio" value="减速带" onclick="radioCheck(this)"/>减速带<br/>
        <input name="Fruit" type="radio" value="十字形" onclick="radioCheck(this)"/>十字形<br/>
        <input name="Fruit" type="radio" value="丁字形" onclick="radioCheck(this)"/>丁字形<br/>
        <input name="Fruit" type="radio" value="辅道连通道路口(同向)" onclick="radioCheck(this)"/>辅道连通道路口(同向)<br/>
        <input name="Fruit" type="radio" value="辅道连通道路口(反向)" onclick="radioCheck(this)"/>辅道连通道路口(反向)<br/>
        <input name="Fruit" type="radio" value="主道转辅道口(同向)" onclick="radioCheck(this)"/>主道转辅道口(同向)<br/>
        <input name="Fruit" type="radio" value="主道转辅道口(反向)" onclick="radioCheck(this)"/>主道转辅道口(反向)<br/>
        <input name="Fruit" type="radio" value="辅道转主道口(同向)" onclick="radioCheck(this)"/>辅道转主道口(同向)<br/>
        <input name="Fruit" type="radio" value="辅道转主道口(反向)" onclick="radioCheck(this)"/>辅道转主道口(反向)<br/>
        <input name="Fruit" type="radio" value="高架入口匝道(同向)" onclick="radioCheck(this)"/>高架入口匝道(同向)<br/>
        <input name="Fruit" type="radio" value="高架入口匝道(反向)" onclick="radioCheck(this)"/>高架入口匝道(反向)<br/>
        <input name="Fruit" type="radio" value="高架出口匝道(同向)" onclick="radioCheck(this)"/>高架出口匝道(同向)<br/>
        <input name="Fruit" type="radio" value="高架出口匝道(反向)" onclick="radioCheck(this)"/>高架出口匝道(反向)<br/>
        <input name="Fruit" type="radio" value="单位及小区进出口(同向)" onclick="radioCheck(this)"/>单位及小区进出口(同向)<br/>
        <input name="Fruit" type="radio" value="单位及小区进出口(反向)" onclick="radioCheck(this)"/>单位及小区进出口(反向)<br/>
        <input name="Fruit" type="radio" value="人行横道" onclick="radioCheck(this)"/>人行横道<br/>
        <input name="Fruit" type="radio" value="掉头车道" onclick="radioCheck(this)"/>掉头车道<br/>
        <input name="Fruit" type="radio" value="公交站点(同向)" onclick="radioCheck(this)"/>公交站点(同向)<br/>
        <input name="Fruit" type="radio" value="公交站点(反向)" onclick="radioCheck(this)"/>公交站点(反向)<br/>
        <input name="Fruit" type="radio" value="油气站点(同向)" onclick="radioCheck(this)"/>油气站点(同向)<br/>
        <input name="Fruit" type="radio" value="油气站点(反向)" onclick="radioCheck(this)"/>油气站点(反向)<br/>
        <input name="Fruit" type="radio" value="上跨、下跨" onclick="radioCheck(this)"/>上跨、下跨<br/>
        <input name="Fruit" type="radio" value="山体" onclick="radioCheck(this)"/>山体<br/>
        <input name="Fruit" type="radio" value="监控球机" onclick="radioCheck(this)"/>监控球机<br/>
        <input name="Fruit" type="radio" value="警力1" onclick="radioCheck(this)"/>警力1
        <input name="Fruit" type="radio" value="警力2" onclick="radioCheck(this)"/>警力2
        <input name="Fruit" type="radio" value="警力3" onclick="radioCheck(this)"/>警力3
        <br/><br/>
        <input name="Fruit" type="radio" value="自定义路段" onclick="radioCheck(this)"/>自定义路段
        <input type="color" id="lineColor" autocomplete="true" style="border: 0; outline:none"/>
        <br/>
        &nbsp;&nbsp;&nbsp;&nbsp;描述:<input type="text" id="txtRoad" value=""/>

        <br/><br/>
        <input name="Fruit" type="radio" value="自定义区域" onclick="radioCheck(this)"/>自定义区域
        <input type="color" id="rangeColor" autocomplete="true" style="border: 0; outline:none"/>
        <br/>
        &nbsp;&nbsp;&nbsp;&nbsp;描述:<input type="text" id="txtRange" value=""/>

        <br/><br/>
        <input name="Fruit" type="radio" value="自定义文字描述" onclick="radioCheck(this)"/>自定义文字描述
        <br/>
        &nbsp;&nbsp;&nbsp;&nbsp;描述:<input type="text" id="txtContent" value=""/>
    </div> 
    <div id='map'>
        
    </div> 
    <script type="text/javascript">
        let latlng;
        let polylineTitle;
        //初始化地图
        BM.Config.HTTP_URL = 'http://192.168.92.212:9000';
        var map = BM.map('map', 'bigemap.71ybpwij', { center: [30.472408294677734, 114.13619232177734], zoom: 15, zoomControl: true });//8swuv2sg,71ybpwij
        map.fitBounds([[30.41877555847168, 114.02143096923828], [30.526039123535156, 114.25094604492188]]);
        //创建一个图形覆盖物的集合来保存点线面
        var drawnItems = new BM.FeatureGroup();
        //添加在地图上
        map.addLayer(drawnItems);
        //右键添加marker事件
        map.on('contextmenu', contextmenu);
        //初始化地图标记点
        initMarkers();
        function initMarkers() {
            //获取所有marker
            var jsonData = GetMarkers();
            //序列化
            var dataArray = JSON.parse(jsonData);
            if (dataArray == null) { return; }
            //遍历添加到地图中
            for (var i = 0; i < dataArray.length; i++) {
                var point = dataArray[i];
                if (point.distance == '0') {
                    //marker
                    var marker;
                    if(point.mainType == '自定义文字描述'){
                        marker = BM.marker([point.lat, point.lng], { riseOnHover: true });
                        marker.bindTooltip(`<div><p><strong>` + point.mainType + `</strong></p><p>` + point.subType + `</p></div>`,{permanent:true,direction:'top'}).openTooltip();
                    } else {
                        var myIcon = BM.icon({
                            iconUrl: '../icon/' + GetIcon(point.subType),
                            iconSize: [25, 25],
                            iconAnchor: [5, 30]
                        });
                        marker = BM.marker([point.lat, point.lng], { icon: myIcon, riseOnHover: true });
                        marker.bindTooltip(`<div><p><strong>` + point.mainType + `</strong></p><p>` + point.subType + `</p></div>`).openTooltip();
                    }

                    
                    //marker.bindTooltip(`<div><p><strong>` + point.mainType + `</strong></p><p>` + point.subType + `</p></div>`,{permanent:true}).openTooltip();
                    //marker.bindTooltip(`<div><p><strong>` + point.mainType + `</strong></p><p>` + point.subType + `</p></div>`).openTooltip();
                    marker.tag = point.markerid;
                    marker.subtype = point.subType;
                    marker.addTo(map);
                    marker.on('click', markClick);
                } else {
                    //polyline
                    var color,polyline,diStr;
                    if(point.mainType == '自定义路段'){
                        color = point.content;
                        polyline = BM.polyline(point.polylinePosition, { color: color, weight: 8, opacity: 0.5 });
                        diStr = '总长度:' + point.distance + '米';
                    }else if(point.mainType == '自定义区域'){
                        color = point.content;
                        polyline = BM.polygon(point.polylinePosition, { color: color, weight: 8, opacity: 0.5 });
                        diStr = '面积:' + point.distance + '㎡';
                    }else{
                        color = GetIcon(point.subType);
                        polyline = BM.polyline(point.polylinePosition, { color: color, weight: 8, opacity: 0.5 });
                        diStr = '总长度:' + point.distance + '米';
                    }
                    //var polyline = BM.polyline(point.polylinePosition, { color: color, weight: 8, opacity: 0.5 });
                    polyline.bindTooltip(`<div><p><strong>` + point.mainType + `</strong></p><p>` + point.subType + `</p><p>` + diStr + `</p></div>`);

                    polyline.tag = point.markerid;
                    polyline.subtype = point.subType;
                    polyline.addTo(map);
                    polyline.on('click', markClick);
                }
            }
        }

        //监听绘制完成事件
        map.on(BM.Draw.Event.CREATED, function (e) {

            var latlngs = e.layer._latlngs;
            
            //计算距离
            var distance = 0;
            if(polylineTitle == '自定义区域'){
                distance = CalculatePolygonArea(latlngs[0]);
            } else {
                for (var i = 0; i < latlngs.length - 1; i++) {
                    distance += map.distance(latlngs[i], latlngs[i + 1]);
                }
            }
            //取距离的整数部分
            var dist = distance.toString().split('.');

            var layer = e.layer;
            
            temp = {
                layer: e.layer,
                type: e.layerType
            };
            
            //设置线条宽度
            layer.options.weight = 8;
            //获取主类型
            var title = GetTitle(polylineTitle);

            var diStr;
            //console.log(title);
            layer.tag = NewGUID();
            if(title == '自定义路段'){
                layer.subtype = document.getElementById('txtRoad').value;
                layer.options.color = document.getElementById('lineColor').value;
                diStr = '总长度:' + dist[0] + '米';
            } else if(title == '自定义区域'){
                layer.subtype = document.getElementById('txtRange').value;
                layer.options.color = document.getElementById('rangeColor').value;
                diStr = '面积:' + dist[0] + '㎡';
                latlngs = e.layer._latlngs[0];
            }else{
                layer.subtype = polylineTitle;
                layer.options.color = GetIcon(polylineTitle);
                diStr = '总长度:' + dist[0] + '米';
            }
            console.log(layer.subtype);
            if(layer.subtype == null || layer.subtype == ""){
                layer.subtype = "该标记未写入描述";
            }
            console.log(layer.subtype);
            //保存线、面数据
            var res = SavePolyline(latlngs, layer.subtype, title,layer.tag,dist[0],layer.options.color);
            console.log(res);
            if (res == 'success') {
                drawnItems.addLayer(layer);
                layer.bindTooltip(`<div><p><strong>`+title+`</strong></p>
                    <p>`+ layer.subtype + `</p><p>` + diStr + `</p></div>`).openTooltip();
                layer.on('click', markClick);
            }
        });
        
        
        //右键增加标记点
        function contextmenu(e) {
            //显示类型栏
            var div = document.getElementById('inputData');
            div.style.visibility = 'visible';
            //全局坐标参，点击类型栏后显示marker用
            latlng = e.latlng;

            
        }
        //标记拖动
        function dragend(e) {

        }
        //左键删除标记点
        function markClick(e) {
            //console.log(e);
            if (confirm('确定删除' + e.target.subtype + '?')) {
                var res = RemoveObject(e.target.tag);
                if (res == "success") {
                    e.target.remove();
                } else {
                    alert(res);
                }
            }
        }

        function radioCheck(e) {

            if (e.value == '中心线绿化带或植埋式护栏' ||
                e.value == '普通护栏及软质隔离桩等' ||
                e.value == '无中心线隔离设施' ||
                e.value == '主辅道隔离(隔离形式为绿化带或护栏)' ||
                e.value == '无主辅道隔离' ||
                e.value == '自定义路段') {
                polylineTitle = e.value;
                var draw = new BM.Draw.Polyline(map);
                draw.enable();
            } else if(e.value == '自定义区域'){
                var draw = new BM.Draw.Polygon(map, {
                    showArea: true, //不显示面积
                    allowIntersection: false, //不允许交叉
                    drawError: {
                        color: '#b00b00',
                        message: '不能绘制交叉的多边形!'
                    }, //绘制错误时的提示信息
                    shapeOptions: {
                        color: 'red',
                    }, //绘制的多边形的样式
                    repeatMode: 0, //是否可以重复绘制
                    beforeAdd: function (latlng, e) {
                        console.log(latlng, e, 96);
                        return true//返回true表示允许添加，false表示不允许添加
                    }
                });
                polylineTitle = e.value;
                draw.enable();
            } else {
                var marker;
                //检查主类型
                var title = GetTitle(e.value);
                var myIcon = BM.icon({
                    iconUrl: '../icon/' + GetIcon(e.value),
                        iconSize: [25, 25],
                        iconAnchor: [10, 25]
                    });
                var subType = e.value;
                if(e.value == '自定义文字描述'){
                    var content = document.getElementById('txtContent').value;
                    if(content == ''){
                        alert('描述不能为空');
                        e.checked = false;
                        var div = document.getElementById('inputData');
                        div.style.visibility = 'hidden';
                        return;
                    }
                    subType = content;
                    marker = BM.marker(latlng);
                    marker.bindTooltip(`<div><p><strong>`+title+`</strong></p>
                    <p>`+subType+`</p></div>`,{permanent:true,direction:'top'}).openTooltip();//绑定提示title
                } else {
                    marker = BM.marker(latlng,
                    {
                        icon: myIcon
                    });
                    marker.bindTooltip(`<div><p><strong>`+title+`</strong></p>
                    <p>`+subType+`</p></div>`).openTooltip();//绑定提示title
                }

                marker.tag = NewGUID();
                marker.subtype = subType;

                //后台保存数据
                var res = SaveMarker(latlng.lat, latlng.lng, subType, title, marker.tag);
                if (res == 'success') {
                    marker.on('dragend', dragend);//绑定拖动事件
                    marker.addTo(map);
                    marker.on('click', markClick);
                } else {
                    alert('添加失败:' + res);
                }
            }
            e.checked = false;
            //隐藏类型栏
            var div = document.getElementById('inputData');
                    div.style.visibility = 'hidden';
        }

        function CalculatePolygonArea (latLngs) {
            var pointsCount = latLngs.length,
                area = 0.0,
                d2r = Math.PI / 180,
                p1, p2;
            console.log(latLngs);
            console.log(pointsCount);
            if (pointsCount > 2) {
                for (var i = 0; i < pointsCount; i++) {
                    p1 = latLngs[i];
                    p2 = latLngs[(i + 1) % pointsCount];
                    area += ((p2.lng - p1.lng) * d2r) *
                        (2 + Math.sin(p1.lat * d2r) + Math.sin(p2.lat * d2r));
                }
                area = area * 6378137.0 * 6378137.0 / 2.0;
            }

            return Math.abs(area);
        }
        
    </script>
</body>
</html>
