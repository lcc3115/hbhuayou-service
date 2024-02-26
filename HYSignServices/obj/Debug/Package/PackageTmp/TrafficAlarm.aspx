<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrafficAlarm.aspx.cs" Inherits="HYSignServices.TrafficAlarm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>违法记录查询</title>
    <script language="javascript" type="text/javascript" src="Scripts/My97DatePicker4.7.2/WdatePicker.js"></script>
    <style type="text/css">
        .GVTab, .GVTab tr, .GVTab th, .GVTab td
        {
            border: 1px #bbddff Solid;
        }
        /*表头*/
        .GVTab th
        {
            background-color: #E6E6FF;
            color: Navy;
            height: 28px;
            text-align: center;
            vertical-align: middle;
            font-size: 14px;
            padding: 4px 4px 4px 4px;
        }
        /*单元格*/
        .GVTab td, .GVTab td.c
        {
            height: 26px;
            padding: 4px 4px 4px 4px;
        }
        .GVTab td
        {
            text-align: left;
        }
        /*单元·居中*/
        .GVTab td.c
        {
            text-align: center;
        }
        /*偶数行*/
        .GVTab tr.alt
        {
            background-color: #F6F6F6;
            color: #000040;
        }
        /*页脚单元*/
        .GVPager td
        {
            background-color: #E0E0E0;
            height: 26px;
            border: 0; 

            vertical-align: middle;
            padding: 0 5px 0 5px;
        }
        .GVPager td.l
        {
            text-align:left;
        }
        .GVPager td.c
        {
            text-align:center;

        }
        .GVPager td.r
        {
            text-align:right;
        }
        .GVPager a
        {
            text-decoration: none;
            margin-left: 4px;
            margin-right: 4px;
        }
        .GVPager a.b
        {
            text-decoration:underline;
            font-weight: 800;
        }
        /*记录未找到的图标*/
        .nr
        {
            border: 0;
            background: url("img/NoRecord.gif" ) no-repeat center center;
            height: 64px;
            width: 64px;
        }
        .btn
        {
            border: solid 1px #D57E3B;
            background-color: #FFC35E;
            color: Black;
            height: 25px;
            text-align: center;
            margin-left: 30px;
            width:70px;
        }
        .btn1 
        {
            transform: scale(1);
        }
 
        .btn1:active 
        {
            transform: scale(0.97);
        }

        .textbox
        {
            height: 30px;
            font-family: "Verdana";
            font-size: 9pt;
            color: #000000;
            border-color: #D3D3D3;
            border-style: solid;
            border-top-width: 1px;
            border-right-width: 1px;
            border-bottom-width: 1px;
            border-left-width: 1px;
            margin-left: 20px;
        }
    </style>
</head>

<body>
    
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <strong>开始时间:</strong><asp:TextBox ID="txtStartTime" runat="server" ReadOnly="true" CssClass="textbox" > </asp:TextBox>
                    
                </td>
                <td>
                    <strong>结束时间:</strong><asp:TextBox ID="txtEndTime" runat="server" ReadOnly="true" CssClass="textbox"> </asp:TextBox>
                </td>
                <td>
                    <strong>路口名:</strong><asp:TextBox ID="txtRoadName" runat="server" CssClass="textbox"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSerh" runat="server" Text="查询" OnClick="btnSerh_OnClick" CssClass="btn btn1"/>
                    <asp:Button ID="btnExport" runat="server" Text="查询导出" OnClick="btnExport_OnClick" CssClass="btn btn1"/>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td>
                    |&nbsp;&nbsp;
                    <asp:Button ID="btnImport" runat="server" Text="比对" OnClick="btnImport_OnClick" CssClass="btn btn1"/>
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                    <asp:TextBox ID="txtHold" runat="server" ReadOnly="true" BorderWidth="0px" ForeColor="Red" Visible="true" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Calendar ID="calStartTime" runat="server" 
                    BorderWidth="1px" CellPadding="1" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399"
                    Height="150px"  Width="100%" OnSelectionChanged="Calendar1_SelectionChanged">
                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                    <WeekendDayStyle BackColor="#CCCCFF" />
                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                    <OtherMonthDayStyle ForeColor="#999999" />
                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
                        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                    </asp:Calendar>
                </td>
                <td align="center">
                    <asp:Calendar ID="calEndTime" runat="server" 
                    BorderWidth="1px" CellPadding="1" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399"
                    Height="150px"  Width="100%" OnSelectionChanged="Calendar2_SelectionChanged" >
                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                    <WeekendDayStyle BackColor="#CCCCFF" />
                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                    <OtherMonthDayStyle ForeColor="#999999" />
                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
                        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                    </asp:Calendar>
                </td>
                <td>
                    
                    <strong>预警值:</strong><asp:TextBox ID="txtAlarm" runat="server"  CssClass="textbox" />
                    <br/><br/>
                    <strong>浮动值:</strong><asp:TextBox ID="txtFloat" runat="server"  CssClass="textbox" />
                    <br/><br/>
                </td>
                <td>
                    
                    <asp:Button ID="btnAlarm" runat="server" Text="统计" OnClick="btnAlarm_OnClick" CssClass="btn btn1" />
                    <asp:Button ID="Button2" runat="server" Text="统计导出" OnClick="btnExportAlarm_OnClick" CssClass="btn btn1"/>
                    <br/><br/>
                    
                    <asp:Button ID="btnFloat" runat="server" Text="同比" OnClick="AnalyseDayData_OnClick" CssClass="btn btn1" />
                    <asp:Button ID="Button4" runat="server" Text="同比导出" OnClick="btnExportAnyc_OnClick" CssClass="btn btn1" />
                    <br/><br/>

                    
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td>
                    |&nbsp;&nbsp;
                    <asp:Button ID="btnCheck" runat="server" Text="违停查重" OnClick="btnCheck_OnClick" CssClass="btn btn1"/>
                    
                    <asp:FileUpload ID="FileUpload2" runat="server" />
                    <asp:TextBox ID="txtCheck" runat="server" ReadOnly="true" BorderWidth="0px" ForeColor="Red" Visible="true" Text=""></asp:TextBox>
                    <br/><br/>

                    |&nbsp;&nbsp;
                    <asp:Button ID="btnPass" runat="server" Text="过车筛查" OnClick="btnPass_OnClick" CssClass="btn btn1"/>
                    <asp:FileUpload ID="FileUploadPass" runat="server" />
                    <asp:TextBox ID="txtPass" runat="server" ReadOnly="true" BorderWidth="0px" ForeColor="Red" Visible="true" Text=""></asp:TextBox>
                    <br/><br/>
                </td>
            </tr>
        </table>
         

        
        
    </div>
    <div>
        <asp:GridView ID="AlarmGridView" runat="server" AutoGenerateColumns="False" AllowPaging="true" PageSize="20" OnPageIndexChanging="PageChanging"
        CellPadding="0" AllowSorting="True" Width="100%" CssClass="GVTab">
            <PagerSettings FirstPageText="首页" LastPageText="尾页" Mode="NumericFirstLast" NextPageText="下一页" PageButtonCount="10" PreviousPageText="上一页" />
            <PagerStyle CssClass="GVPager" />
            <AlternatingRowStyle CssClass="alt" />
            <Columns>
                <asp:BoundField DataField="cross_id" HeaderText="路口编号" />
                <asp:BoundField DataField="cross_name" HeaderText="路口名称" />
                <asp:BoundField DataField="plate_no" HeaderText="车牌号码" />
                <asp:BoundField DataField="plate_color" HeaderText="车牌颜色" />
                <asp:BoundField DataField="passtime" HeaderText="违法时间" />
                <asp:BoundField DataField="lane_name" HeaderText="方向车道" />
                <asp:BoundField DataField="sysdict_code" HeaderText="违法代码" />
                <asp:BoundField DataField="sysdict_name" HeaderText="违法行为" />
                <asp:BoundField DataField="dev_ip" HeaderText="设备IP" />
                <asp:BoundField DataField="dev_code" HeaderText="设备编码" />
                
            </Columns>
            
        </asp:GridView>
    </div>
    <div>
        <asp:GridView ID="HH24GridView" runat="server" AutoGenerateColumns="False" AllowPaging="true" PageSize="20" OnPageIndexChanging="HH24PageChanging" 
        CellPadding="0" AllowSorting="True" Width="100%" CssClass="GVTab">
            <PagerSettings FirstPageText="首页" LastPageText="尾页" Mode="NumericFirstLast" NextPageText="下一页" PageButtonCount="10" PreviousPageText="上一页" />
            <PagerStyle CssClass="GVPager" />
            <AlternatingRowStyle CssClass="alt" />
            <Columns>
                <asp:BoundField DataField="cross_name" HeaderText="路口名称" />
                <asp:BoundField DataField="lane_name" HeaderText="方向车道" />
                <asp:BoundField DataField="dev_code" HeaderText="设备编码" />
                <asp:BoundField DataField="sysdict_code" HeaderText="违法代码" />
                <asp:BoundField DataField="HH00" HeaderText="00" />
                <asp:BoundField DataField="HH01" HeaderText="01" />
                <asp:BoundField DataField="HH02" HeaderText="02" />
                <asp:BoundField DataField="HH03" HeaderText="03" />
                <asp:BoundField DataField="HH04" HeaderText="04" />
                <asp:BoundField DataField="HH05" HeaderText="05" />
                <asp:BoundField DataField="HH06" HeaderText="06" />
                <asp:BoundField DataField="HH07" HeaderText="07" />
                <asp:BoundField DataField="HH08" HeaderText="08" />
                <asp:BoundField DataField="HH09" HeaderText="09" />
                <asp:BoundField DataField="HH10" HeaderText="10" />
                <asp:BoundField DataField="HH11" HeaderText="11" />
                <asp:BoundField DataField="HH12" HeaderText="12" />
                <asp:BoundField DataField="HH13" HeaderText="13" />
                <asp:BoundField DataField="HH14" HeaderText="14" />
                <asp:BoundField DataField="HH15" HeaderText="15" />
                <asp:BoundField DataField="HH16" HeaderText="16" />
                <asp:BoundField DataField="HH17" HeaderText="17" />
                <asp:BoundField DataField="HH18" HeaderText="18" />
                <asp:BoundField DataField="HH19" HeaderText="19" />
                <asp:BoundField DataField="HH20" HeaderText="20" />
                <asp:BoundField DataField="HH21" HeaderText="21" />
                <asp:BoundField DataField="HH22" HeaderText="22" />
                <asp:BoundField DataField="HH23" HeaderText="23" />
            </Columns>
            
        </asp:GridView>
    </div>
    <div>
        <asp:GridView ID="AnalyseDayGridView" runat="server" AutoGenerateColumns="False" AllowPaging="true" PageSize="20" OnPageIndexChanging="SyncDayPageChanging" 
        CellPadding="0" AllowSorting="True" Width="100%" CssClass="GVTab">
            <PagerSettings FirstPageText="首页" LastPageText="尾页" Mode="NumericFirstLast" NextPageText="下一页" PageButtonCount="10" PreviousPageText="上一页" />
            <PagerStyle CssClass="GVPager" />
            <AlternatingRowStyle CssClass="alt" />
            <Columns>
                <asp:BoundField DataField="cross_name" HeaderText="路口名称" />
                <asp:BoundField DataField="lane_name" HeaderText="方向车道" />
                <asp:BoundField DataField="dev_code" HeaderText="设备编码" />
                <asp:BoundField DataField="sysdict_code" HeaderText="违法代码" />
                <asp:BoundField DataField="day1" HeaderText="day-1" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="day2" HeaderText="day-2" />
                <asp:BoundField DataField="today" HeaderText="当日" />
                <asp:BoundField DataField="persenDay1" HeaderText="同比day-1" />
                <asp:BoundField DataField="persenDay2" HeaderText="同比day-2" />
            </Columns>
            
        </asp:GridView>
    </div>
    </form>
</body>
</html>
