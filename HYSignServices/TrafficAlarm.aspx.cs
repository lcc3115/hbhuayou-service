using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HYSignServices.Entity;
using System.Data;
using HYSignServices.HYService;
using System.IO;
using BigmapSite.Tools;

namespace HYSignServices
{
    public partial class TrafficAlarm : System.Web.UI.Page
    {
        private static int LIMIT = 20;
        private static List<Data> GridViewData = null;
        //public delegate void UpdateDelegate();
        //private static AlarmJson alarmJson = null;
        private static List<HH24Data> GridViewHH24 = null;
        private static List<AnalyseDayData> GridViewSyncDay = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
            
        }
        //开始时间
        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            txtStartTime.Text = calStartTime.SelectedDate.ToString("yyyy-MM-dd");
        }
        //结束时间
        protected void Calendar2_SelectionChanged(object sender, EventArgs e)
        {
            txtEndTime.Text = calEndTime.SelectedDate.ToString("yyyy-MM-dd");
        }

        #region 违法查询
        //查询
        protected void btnSerh_OnClick(object sender, EventArgs e)
        {
            EnableUI(false,"请稍后...");

            string startTime = txtStartTime.Text + " 00:00:00";
            string endTime = txtEndTime.Text + " 23:59:59";
            string roadName = txtRoadName.Text;

            GridViewData = GetAlarmCount(startTime, endTime, roadName);
            if (GridViewData == null)
            {
                return;
            }
            HH24GridView.Visible = false;
            AnalyseDayGridView.Visible = false;

            AlarmGridView.DataSource = GridViewData;
            AlarmGridView.DataBind();

            AlarmGridView.Visible = true;

            EnableUI(true, "");
        }
        //分页事件
        protected void PageChanging(object sender, GridViewPageEventArgs e)
        {
            AlarmGridView.PageIndex = e.NewPageIndex;

            //125  6 20
            //总数-分页*行数 >= 要显示的行数
            if (GridViewData.Count - AlarmGridView.PageIndex * LIMIT >= LIMIT)
            {
                List<Data> data = GridViewData.GetRange(AlarmGridView.PageIndex * LIMIT, LIMIT);
            }
            else
            {
                List<Data> data = GridViewData.GetRange(AlarmGridView.PageIndex * LIMIT, GridViewData.Count - AlarmGridView.PageIndex * LIMIT);
            }

            AlarmGridView.DataSource = GridViewData;
            AlarmGridView.DataBind();
        }
        //导出
        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            string[] title = new string[] { "路口编号", "路口名称", "车牌", "车牌颜色", "违法时间", "方向车道", "违法代码", "违法行为", "设备IP", "设备编码" };
            DataTable dataTable = Tools.ListToDataTable(GridViewData, title);
            ExportExcel(dataTable, txtEndTime.Text);
            
        }
        #endregion

        #region 违法统计
        //统计
        protected void btnAlarm_OnClick(object sender, EventArgs e)
        {
            string startTime = txtStartTime.Text + " 00:00:00";
            string endTime = txtEndTime.Text + " 23:59:59";
            string roadName = txtRoadName.Text;

            GridViewHH24 = SetDevCode(startTime, endTime, roadName);
            if (GridViewHH24 == null)
            {
                return;
            }
            List<HH24Data> list = new List<HH24Data>();

            HH24Data hh24Data = null;
            for (int i = 0; i < GridViewHH24.Count; i++)
            {
                hh24Data = GridViewHH24[i];
                
                if (list.FirstOrDefault(o => o.cross_name == hh24Data.cross_name && o.lane_name == hh24Data.lane_name && o.sysdict_code == hh24Data.sysdict_code && o.dev_code == hh24Data.dev_code) != null)
                {
                    //跳过集合中已添加过的对象
                    continue;
                }

                for (int j = 0; j < GridViewHH24.Count; j++)
                {
                    if (i == j) //跳过父循环中的对象
                        continue;
                    if (GridViewHH24[i].cross_name == GridViewHH24[j].cross_name &&
                       GridViewHH24[i].lane_name == GridViewHH24[j].lane_name &&
                       GridViewHH24[i].sysdict_code == GridViewHH24[j].sysdict_code &&
                       GridViewHH24[i].dev_code == GridViewHH24[j].dev_code)
                    {
                        #region 合并时间段的值
                        int HH00 = (string.IsNullOrEmpty(GridViewHH24[j].HH00) ? 0 : int.Parse(GridViewHH24[j].HH00));
                        hh24Data.HH00 = string.IsNullOrEmpty(hh24Data.HH00) ? HH00.ToString() : (HH00 + int.Parse(hh24Data.HH00)).ToString();

                        int HH01 = (string.IsNullOrEmpty(GridViewHH24[j].HH01) ? 0 : int.Parse(GridViewHH24[j].HH01));
                        hh24Data.HH01 = string.IsNullOrEmpty(hh24Data.HH01) ? HH01.ToString() : (HH01 + int.Parse(hh24Data.HH01)).ToString();
                        int HH02 = (string.IsNullOrEmpty(GridViewHH24[j].HH02) ? 0 : int.Parse(GridViewHH24[j].HH02));
                        hh24Data.HH02 = string.IsNullOrEmpty(hh24Data.HH02) ? HH02.ToString() : (HH02 + int.Parse(hh24Data.HH02)).ToString();
                        int HH03 = (string.IsNullOrEmpty(GridViewHH24[j].HH03) ? 0 : int.Parse(GridViewHH24[j].HH03));
                        hh24Data.HH03 = string.IsNullOrEmpty(hh24Data.HH03) ? HH03.ToString() : (HH03 + int.Parse(hh24Data.HH03)).ToString();
                        int HH04 = (string.IsNullOrEmpty(GridViewHH24[j].HH04) ? 0 : int.Parse(GridViewHH24[j].HH04));
                        hh24Data.HH04 = string.IsNullOrEmpty(hh24Data.HH04) ? HH04.ToString() : (HH04 + int.Parse(hh24Data.HH04)).ToString();
                        int HH05 = (string.IsNullOrEmpty(GridViewHH24[j].HH05) ? 0 : int.Parse(GridViewHH24[j].HH05));
                        hh24Data.HH05 = string.IsNullOrEmpty(hh24Data.HH05) ? HH05.ToString() : (HH05 + int.Parse(hh24Data.HH05)).ToString();
                        int HH06 = (string.IsNullOrEmpty(GridViewHH24[j].HH06) ? 0 : int.Parse(GridViewHH24[j].HH06));
                        hh24Data.HH06 = string.IsNullOrEmpty(hh24Data.HH06) ? HH06.ToString() : (HH06 + int.Parse(hh24Data.HH06)).ToString();
                        int HH07 = (string.IsNullOrEmpty(GridViewHH24[j].HH07) ? 0 : int.Parse(GridViewHH24[j].HH07));
                        hh24Data.HH07 = string.IsNullOrEmpty(hh24Data.HH07) ? HH07.ToString() : (HH07 + int.Parse(hh24Data.HH07)).ToString();
                        int HH08 = (string.IsNullOrEmpty(GridViewHH24[j].HH08) ? 0 : int.Parse(GridViewHH24[j].HH08));
                        hh24Data.HH08 = string.IsNullOrEmpty(hh24Data.HH08) ? HH08.ToString() : (HH08 + int.Parse(hh24Data.HH08)).ToString();
                        int HH09 = (string.IsNullOrEmpty(GridViewHH24[j].HH09) ? 0 : int.Parse(GridViewHH24[j].HH09));
                        hh24Data.HH09 = string.IsNullOrEmpty(hh24Data.HH09) ? HH09.ToString() : (HH09 + int.Parse(hh24Data.HH09)).ToString();
                        int HH10 = (string.IsNullOrEmpty(GridViewHH24[j].HH10) ? 0 : int.Parse(GridViewHH24[j].HH10));
                        hh24Data.HH10 = string.IsNullOrEmpty(hh24Data.HH10) ? HH10.ToString() : (HH10 + int.Parse(hh24Data.HH10)).ToString();
                        int HH11 = (string.IsNullOrEmpty(GridViewHH24[j].HH11) ? 0 : int.Parse(GridViewHH24[j].HH11));
                        hh24Data.HH11 = string.IsNullOrEmpty(hh24Data.HH11) ? HH11.ToString() : (HH11 + int.Parse(hh24Data.HH11)).ToString();
                        int HH12 = (string.IsNullOrEmpty(GridViewHH24[j].HH12) ? 0 : int.Parse(GridViewHH24[j].HH12));
                        hh24Data.HH12 = string.IsNullOrEmpty(hh24Data.HH12) ? HH12.ToString() : (HH12 + int.Parse(hh24Data.HH12)).ToString();
                        int HH13 = (string.IsNullOrEmpty(GridViewHH24[j].HH13) ? 0 : int.Parse(GridViewHH24[j].HH13));
                        hh24Data.HH13 = string.IsNullOrEmpty(hh24Data.HH13) ? HH13.ToString() : (HH13 + int.Parse(hh24Data.HH13)).ToString();
                        int HH14 = (string.IsNullOrEmpty(GridViewHH24[j].HH14) ? 0 : int.Parse(GridViewHH24[j].HH14));
                        hh24Data.HH14 = string.IsNullOrEmpty(hh24Data.HH14) ? HH14.ToString() : (HH14 + int.Parse(hh24Data.HH14)).ToString();
                        int HH15 = (string.IsNullOrEmpty(GridViewHH24[j].HH15) ? 0 : int.Parse(GridViewHH24[j].HH15));
                        hh24Data.HH15 = string.IsNullOrEmpty(hh24Data.HH15) ? HH15.ToString() : (HH15 + int.Parse(hh24Data.HH15)).ToString();
                        int HH16 = (string.IsNullOrEmpty(GridViewHH24[j].HH16) ? 0 : int.Parse(GridViewHH24[j].HH16));
                        hh24Data.HH16 = string.IsNullOrEmpty(hh24Data.HH16) ? HH16.ToString() : (HH16 + int.Parse(hh24Data.HH16)).ToString();
                        int HH17 = (string.IsNullOrEmpty(GridViewHH24[j].HH17) ? 0 : int.Parse(GridViewHH24[j].HH17));
                        hh24Data.HH17 = string.IsNullOrEmpty(hh24Data.HH17) ? HH17.ToString() : (HH17 + int.Parse(hh24Data.HH17)).ToString();
                        int HH18 = (string.IsNullOrEmpty(GridViewHH24[j].HH18) ? 0 : int.Parse(GridViewHH24[j].HH18));
                        hh24Data.HH18 = string.IsNullOrEmpty(hh24Data.HH18) ? HH18.ToString() : (HH18 + int.Parse(hh24Data.HH18)).ToString();
                        int HH19 = (string.IsNullOrEmpty(GridViewHH24[j].HH19) ? 0 : int.Parse(GridViewHH24[j].HH19));
                        hh24Data.HH19 = string.IsNullOrEmpty(hh24Data.HH19) ? HH19.ToString() : (HH19 + int.Parse(hh24Data.HH19)).ToString();
                        int HH20 = (string.IsNullOrEmpty(GridViewHH24[j].HH20) ? 0 : int.Parse(GridViewHH24[j].HH20));
                        hh24Data.HH20 = string.IsNullOrEmpty(hh24Data.HH20) ? HH20.ToString() : (HH20 + int.Parse(hh24Data.HH20)).ToString();
                        int HH21 = (string.IsNullOrEmpty(GridViewHH24[j].HH21) ? 0 : int.Parse(GridViewHH24[j].HH21));
                        hh24Data.HH21 = string.IsNullOrEmpty(hh24Data.HH21) ? HH21.ToString() : (HH21 + int.Parse(hh24Data.HH21)).ToString();
                        int HH22 = (string.IsNullOrEmpty(GridViewHH24[j].HH22) ? 0 : int.Parse(GridViewHH24[j].HH22));
                        hh24Data.HH22 = string.IsNullOrEmpty(hh24Data.HH22) ? HH22.ToString() : (HH22 + int.Parse(hh24Data.HH22)).ToString();
                        int HH23 = (string.IsNullOrEmpty(GridViewHH24[j].HH23) ? 0 : int.Parse(GridViewHH24[j].HH23));
                        hh24Data.HH23 = string.IsNullOrEmpty(hh24Data.HH23) ? HH23.ToString() : (HH23 + int.Parse(hh24Data.HH23)).ToString();
                        #endregion
                    }
                }
                list.Add(hh24Data);
            }
            
            #region 筛选阈值范围内的数据
            GridViewHH24.Clear();
            if (!string.IsNullOrEmpty(txtAlarm.Text))
            {
                int threshold = int.Parse(txtAlarm.Text);
                foreach (HH24Data item in list)
                {
                    if (!string.IsNullOrEmpty(item.HH00) && int.Parse(item.HH00) >= threshold)
                    { GridViewHH24.Add(item); continue; }
                    if (!string.IsNullOrEmpty(item.HH01) && int.Parse(item.HH01) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH02) && int.Parse(item.HH02) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH03) && int.Parse(item.HH03) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH04) && int.Parse(item.HH04) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH05) && int.Parse(item.HH05) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH06) && int.Parse(item.HH06) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH07) && int.Parse(item.HH07) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH08) && int.Parse(item.HH08) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH09) && int.Parse(item.HH09) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH10) && int.Parse(item.HH10) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH11) && int.Parse(item.HH11) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH12) && int.Parse(item.HH12) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH13) && int.Parse(item.HH13) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH14) && int.Parse(item.HH14) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH15) && int.Parse(item.HH15) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH16) && int.Parse(item.HH16) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH17) && int.Parse(item.HH17) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH18) && int.Parse(item.HH18) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH19) && int.Parse(item.HH19) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH20) && int.Parse(item.HH20) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH21) && int.Parse(item.HH21) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH22) && int.Parse(item.HH22) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                    if (!string.IsNullOrEmpty(item.HH23) && int.Parse(item.HH23) >= threshold)
                        {GridViewHH24.Add(item);continue;}
                }
            }
            else
            {
                GridViewHH24 = list;
            }
            #endregion

            AlarmGridView.Visible = false;
            AnalyseDayGridView.Visible = false;

            HH24GridView.DataSource = GridViewHH24;//GridViewHH24;
            HH24GridView.DataBind();

            HH24GridView.Visible = true;

            EnableUI(true, "");
            ChangeCellColor();
        }

        //改变页面列表中的数值颜色
        private void ChangeCellColor()
        {
            int threshold = -1;
            if (!string.IsNullOrEmpty(txtAlarm.Text))
            {
                threshold = int.Parse(txtAlarm.Text);
            }
            for (int i = 0; i <= HH24GridView.Rows.Count - 1; i++)
            {
                for (int j = 0; j < HH24GridView.Rows[i].Cells.Count; j++)
			    {
                    string val = HH24GridView.Rows[i].Cells[j].Text;
                    if (val.Length <= 2)
                    {
                        int numb = int.Parse(val);
                        if (threshold != -1 && numb >= threshold)
                        {
                            HH24GridView.Rows[i].Cells[j].ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    if (val == "0")
                    {
                        HH24GridView.Rows[i].Cells[j].Text = "";
                    }
			    }
            }
        }
        //统计分页事件
        protected void HH24PageChanging(object sender, GridViewPageEventArgs e)
        {
            HH24GridView.PageIndex = e.NewPageIndex;

            //125  6 20
            //总数-分页*行数 >= 要显示的行数
            if (GridViewHH24.Count - HH24GridView.PageIndex * LIMIT >= LIMIT)
            {
                List<HH24Data> data = GridViewHH24.GetRange(HH24GridView.PageIndex * LIMIT, LIMIT);
            }
            else
            {
                List<HH24Data> data = GridViewHH24.GetRange(HH24GridView.PageIndex * LIMIT, GridViewHH24.Count - HH24GridView.PageIndex * LIMIT);
            }

            HH24GridView.DataSource = GridViewHH24;
            HH24GridView.DataBind();
            ChangeCellColor();
        }
        protected void btnExportAlarm_OnClick(object sender, EventArgs e)
        {
            string[] title = new string[] { "路口名称", "方向车道", "设备编码", "违法代码", "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"};
            DataTable dataTable = Tools.ListToDataTable(GridViewHH24, title);
            ExportExcel(dataTable, "统计"+txtEndTime.Text);
        }
        #endregion

        #region 同比
        protected void AnalyseDayData_OnClick(object sender, EventArgs e)
        {
            string startTime = txtStartTime.Text + " 00:00:00";
            string endTime = txtEndTime.Text + " 23:59:59";
            string roadName = txtRoadName.Text;

            GridViewSyncDay = SetDevCodeAnyc(endTime, roadName);

            if (GridViewSyncDay == null)
            {
                return;
            }

            AnalyseDayData lastData = null;
            List<AnalyseDayData> list = new List<AnalyseDayData>();
            foreach (AnalyseDayData dataPrent in GridViewSyncDay)
            {
                if (lastData == null)
                {
                    lastData = dataPrent;
                    continue;
                }
                if (dataPrent.cross_name == lastData.cross_name &&
                   dataPrent.lane_name == lastData.lane_name &&
                   dataPrent.sysdict_code == lastData.sysdict_code &&
                   dataPrent.dev_code == lastData.dev_code)
                {
                    #region 合并时间段的值
                    int today = (string.IsNullOrEmpty(dataPrent.today) ? 0 : int.Parse(dataPrent.today)) + (string.IsNullOrEmpty(lastData.today) ? 0 : int.Parse(lastData.today));
                    lastData.today = today == 0 ? "" : today.ToString();
                    int day1 = (string.IsNullOrEmpty(dataPrent.day1) ? 0 : int.Parse(dataPrent.day1)) + (string.IsNullOrEmpty(lastData.day1) ? 0 : int.Parse(lastData.day1));
                    lastData.day1 = day1 == 0 ? "" : day1.ToString();
                    int day2 = (string.IsNullOrEmpty(dataPrent.day2) ? 0 : int.Parse(dataPrent.day2)) + (string.IsNullOrEmpty(lastData.day2) ? 0 : int.Parse(lastData.day2));
                    lastData.day2 = day2 == 0 ? "" : day2.ToString();
                    #endregion
                }
                else
                {
                    list.Add(lastData);
                    //记录初始dataPrent
                    lastData = dataPrent;
                }
            }

            //计算同比,改变前端同比样式
            foreach (AnalyseDayData data in list)
            {
                if (string.IsNullOrEmpty(data.today))
                    continue;
                if (!string.IsNullOrEmpty(data.day1))
                {
                    int day1 = int.Parse(data.day1);
                    int today = int.Parse(data.today);
                    //double persenDay = ((today - day1) / day1);
                    int persenDay = today - day1;
                    data.persenDay1 = ConvertpersenStr(persenDay);
                }
                if (!string.IsNullOrEmpty(data.day2))
                {
                    int day2 = int.Parse(data.day2);
                    int today = int.Parse(data.today);
                    //double persenDay = ((today - day2) / day2);
                    int persenDay = today - day2;
                    data.persenDay2 = ConvertpersenStr(persenDay);
                }
            }
            

            GridViewSyncDay.Clear();
            GridViewSyncDay = list;

            AlarmGridView.Visible = false;
            HH24GridView.Visible = false;

            AnalyseDayGridView.DataSource = GridViewSyncDay;
            AnalyseDayGridView.DataBind();

            AnalyseDayGridView.Visible = true;

            EnableUI(true, "");
            ChangeAnycColor();
        }

        //同比分页事件
        protected void SyncDayPageChanging(object sender, GridViewPageEventArgs e)
        {
            AnalyseDayGridView.PageIndex = e.NewPageIndex;

            //125  6 20
            //总数-分页*行数 >= 要显示的行数
            if (GridViewSyncDay.Count - AnalyseDayGridView.PageIndex * LIMIT >= LIMIT)
            {
                List<AnalyseDayData> data = GridViewSyncDay.GetRange(AnalyseDayGridView.PageIndex * LIMIT, LIMIT);
            }
            else
            {
                List<AnalyseDayData> data = GridViewSyncDay.GetRange(AnalyseDayGridView.PageIndex * LIMIT, GridViewSyncDay.Count - AnalyseDayGridView.PageIndex * LIMIT);
            }

            AnalyseDayGridView.DataSource = GridViewSyncDay;
            AnalyseDayGridView.DataBind();
            ChangeAnycColor();
        }
        //同比导出
        protected void btnExportAnyc_OnClick(object sender, EventArgs e)
        {
            string[] title = new string[] { "路口名称", "方向车道", "设备编码", "违法代码", "day-1", "day-2", "当日", "同比day-1", "同比day-2"};
            DataTable dataTable = Tools.ListToDataTable(GridViewSyncDay, title);
            ExportExcel(dataTable, "同比" + txtEndTime.Text);
        }
        private void ChangeAnycColor()
        {
            for (int i = 0; i <= AnalyseDayGridView.Rows.Count - 1; i++)
            {
                for (int j = 7; j < 9; j++)
                {
                    string val = AnalyseDayGridView.Rows[i].Cells[j].Text;
                    if (string.IsNullOrEmpty(val) || val == "&nbsp;")
                    {
                        AnalyseDayGridView.Rows[i].Cells[j].Text = "—";
                        continue;
                    }
                        
                    //int persion = int.Parse(val);
                    if (val.Contains("+"))
                    {
                        //string anyc = "↑&nbsp;" + Math.Abs(persion) + "%";
                        //AnalyseDayGridView.Rows[i].Cells[j].Text = anyc;
                        AnalyseDayGridView.Rows[i].Cells[j].ForeColor = System.Drawing.Color.Red;
                    }
                    if (val.Contains("-"))
                    {
                        //string anyc = "↓&nbsp;" + Math.Abs(persion) + "%";
                        //AnalyseDayGridView.Rows[i].Cells[j].Text = anyc;
                        AnalyseDayGridView.Rows[i].Cells[j].ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// 比对两张表添加ID车道字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_OnClick(object sender, EventArgs e)
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('正在查询，请稍后')</script>");
            EnableUI(false, "");
            string tempath = "C:\\TempFile\\";
            if (false == System.IO.Directory.Exists(tempath))
            {
                System.IO.Directory.CreateDirectory(tempath);
            }
            tempath += FileUpload1.PostedFile.FileName;
            FileUpload1.SaveAs(tempath);
            //原始表
            DataTable dt = Tools.ExcelToDataTable(tempath, true);
            List<ResouersData> serousList = new List<ResouersData>();
            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ResouersData rd = new ResouersData();
                rd.alarm_time = dt.Rows[i][0].ToString().Trim();
                rd.plate_no = dt.Rows[i][1].ToString().Trim();
                rd.dev_code = dt.Rows[i][2].ToString().Trim();
                rd.sysdict_code = dt.Rows[i][3].ToString().Trim();
                rd.operation = dt.Rows[i][4].ToString().Trim();

                DateTime datetime = DateTime.Parse(rd.alarm_time);
                dates.Add(datetime);
                serousList.Add(rd);
            }
            string endTime = dates.Max().ToString("yyyy-MM-dd HH:mm:ss");
            string startTime = dates.Min().ToString("yyyy-MM-dd HH:mm:ss");
            List<Data> dataList = GetAlarmCount(startTime, endTime, null);
            if (dataList == null)
            {
                EnableUI(true, "未查询到违法数据");
                return;
            }
            foreach (ResouersData rd in serousList)
            {
                string alarm_time = rd.alarm_time;
                string dev_code = rd.dev_code;
                if (alarm_time == "2022-04-09 14:04:47")
                {
                    alarm_time.ToString();
                }
                foreach (Data data in dataList)
                {
                    if (string.IsNullOrEmpty(data.dev_code))
                        continue;
                    string sysdict_time = data.passtime;
                    string sysdict_code = data.dev_code.Substring(data.dev_code.Length - 5, 5);
                    if (sysdict_time == "2022-04-09 14:04:47")
                    {
                        sysdict_time.ToString();
                    }
                    //420108000000011341
                    
                    if (alarm_time == sysdict_time && dev_code == sysdict_code)
                    {
                        rd.amarm_road = data.lane_name;
                        rd.alarm_id = DateTime.Parse(sysdict_time).ToString("yyyymmddHHmmss") + dev_code;
                        rd.cross_id = data.cross_id;
                        rd.cross_name = data.cross_name;
                        rd.dev_ip = data.dev_ip;
                        rd.sysdict_name = data.sysdict_name;
                        break;
                    }
                }
            }
            
            EnableUI(true, "");

            string[] title = new string[] { "路口编号", "违法编码", "路口名称", "违法时间", "方向车道", "车辆号牌", "设备IP", "设备编码", "违法编号", "违法行为", "录入人员" };
            DataTable dataTable = Tools.ListToDataTable(serousList, title);
            string[] tempName = FileUpload1.PostedFile.FileName.Split('.');
            ExportExcel(dataTable, tempName[0]);
            //ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('比对数据已保存到本地文档')</script>");
        }

        /// <summary>
        /// 违停查重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCheck_OnClick(object sender, EventArgs e)
        {
            //导出对象集合
            List<PlateCheck> plateList = new List<PlateCheck>();
            //服务器存储客户端上传的文件夹路径
            string tempath = "C:\\TempFile\\";
            if (false == System.IO.Directory.Exists(tempath))
            {
                System.IO.Directory.CreateDirectory(tempath);
            }
            tempath += FileUpload2.PostedFile.FileName;
            //客户端上传文件保存到服务器临时文件夹
            FileUpload2.SaveAs(tempath);
            //读取数据表到DataTable
            DataTable dt = Tools.ExcelToDataTable(tempath, true);
            //上一次读取车牌
            string sPlate_no = "";
            //上一次读取时间
            string last_alarmTimeStr = "";
            //遍历数据表
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //车牌
                string plate_no = dt.Rows[i][0].ToString().Trim();
                //时间
                string alarmTime = dt.Rows[i][1].ToString().Trim();
                //转换字符串
                string date = DateTime.FromOADate(double.Parse(alarmTime)).ToString();
                string alarmTimeStr = DateTime.Parse(date).ToString("yyyy-MM-dd HH:mm:ss");
                //新建对象存储每行数据
                PlateCheck pc = new PlateCheck();
                pc.plate = plate_no;
                pc.date = alarmTimeStr;
                pc.count = "";

                if (plate_no == sPlate_no)
                {
                    string startTime = last_alarmTimeStr;
                    string endTime = alarmTimeStr;
                    //从海康过车数据库取过车记录
                    pc = SqlHelper.GetPassCount(plate_no, startTime, endTime);

                }
                //存储到集合
                plateList.Add(pc);
                //更新上一次数据记录
                last_alarmTimeStr = alarmTimeStr;

                sPlate_no = plate_no;
            }

            //导出
            string[] title = new string[] { "车牌","车牌颜色", "违停时间", "过车次数" };
            DataTable dataTable = Tools.ListToDataTable(plateList, title);
            ExportExcel(dataTable, "违停查重" + DateTime.Now.ToString("yyyyMMddHHmmss"));

        }

        /// <summary>
        /// 过车筛查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPass_OnClick(object sender, EventArgs e)
        {
            //导出对象集合
            List<CrossCheck> plateList = new List<CrossCheck>();
            //服务器存储客户端上传的文件夹路径
            string tempath = "C:\\TempFile\\";
            if (false == System.IO.Directory.Exists(tempath))
            {
                System.IO.Directory.CreateDirectory(tempath);
            }
            tempath += FileUploadPass.PostedFile.FileName;
            //客户端上传文件保存到服务器临时文件夹
            FileUploadPass.SaveAs(tempath);
            //读取数据表到DataTable
            DataTable dt = Tools.ExcelToDataTable(tempath, true);

            //遍历数据表
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CrossCheck crossCheck = new CrossCheck();
                //车牌
                crossCheck.plate = dt.Rows[i][0].ToString().Trim();
                //路口
                crossCheck.cross_name = dt.Rows[i][3].ToString().Trim();
                //方向
                crossCheck.direction = dt.Rows[i][4].ToString().Trim();
                //时间
                string s_time = dt.Rows[i][1].ToString().Trim();
                string e_time = dt.Rows[i][2].ToString().Trim();
                //时间格式转换
                string s_date = DateTime.FromOADate(double.Parse(s_time)).ToString();
                crossCheck.s_time = DateTime.Parse(s_date).ToString("yyyy-MM-dd HH:mm:ss");
                string e_date = DateTime.FromOADate(double.Parse(e_time)).ToString();
                crossCheck.e_time = DateTime.Parse(e_date).ToString("yyyy-MM-dd HH:mm:ss");
                //数据库结果集
                DataSet ds = SqlHelper.CheckPassCross(crossCheck);

                if (Tools.DSisNull(ds))
                {
                    //有车道结果
                    DataTable c_dt = ds.Tables[0];
                    for (int j = 0; j < c_dt.Rows.Count; j++)
                    {
                        crossCheck.direction = c_dt.Rows[j][2].ToString().Trim();
                        crossCheck.count = c_dt.Rows[j][3].ToString().Trim();
                        plateList.Add(crossCheck);
                    }
                }
                else
                {
                    //无车道结果
                    crossCheck.count = "0";
                    plateList.Add(crossCheck);
                }
            }
            //导出
            string[] title = new string[] { "车牌", "开始时间", "结束时间", "卡口/路口", "方向/车道", "过车次数" };
            DataTable dataTable = Tools.ListToDataTable(plateList, title);
            ExportExcel(dataTable, "过车筛查" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// 导出excel到客户端
        /// </summary>
        /// <param name="dataTable"></param>
        private void ExportExcel(DataTable dataTable,string fileName)
        {
            
            System.IO.MemoryStream file = Tools.ExportDataSetToExcel(dataTable);
            if (file != null)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xls"));
                Response.Clear();
                file.WriteTo(Response.OutputStream);
                Response.End();
            }
            else
            {
                txtHold.Text = "导出异常";
            }
        }
        /// <summary>
        /// 查询违法记录
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="roadName"></param>
        /// <returns></returns>
        private List<Data> GetAlarmCount(string start, string end, string roadName)
        {
            //设备总表实体
            List<ExcelData> excelList = LoadExcel();
            //违法记录实体
            List<Data> dataList = GetTrafficAlarmExcel(start, end,roadName);

            if (dataList == null || excelList == null)
            {
                return null;
            }
            foreach (Data data in dataList)
            {
                //违法表路口名
                string dName = data.cross_name;
                //存放IP和对应的路口名相似度值
                Dictionary<string, float> SimilIPArray = new Dictionary<string, float>();

                foreach (ExcelData excelData in excelList)
                {
                    //设备总表路口名
                    string eName = excelData.cross_name;
                    //计算两个路口名的相似度
                    float similar = Tools.LevenShtein(dName, eName);

                    SimilIPArray.Add(excelData.dev_ip, similar);

                    if (similar == 1)
                    {
                        break;
                    }
                }
                //取最大相似度
                float max = SimilIPArray.Max(kvp => kvp.Value);
                //最大相似度小于70%丢掉数据
                if (max < Tools.simil) continue;  
                //根据最大相似度找到对应IP
                KeyValuePair<string, float> result = SimilIPArray.FirstOrDefault(o => o.Value == max);
                //根据IP找到对应设备总表中的实体
                ExcelData finalExcel = excelList.FirstOrDefault(dl => dl.dev_ip == result.Key);
                string[] item = data.lane_name.Split('-');
                string direction = item[0];
                string lane_no = item[1].Substring(2, 1);
                //根据设备总表中的实体取违法记录实体，并赋值IP和CODE字段
                foreach (ExcelData excelData in excelList)
                {
                    if (excelData.cross_name == finalExcel.cross_name //路口名相同
                        && direction == excelData.direction           //方向相同
                        && excelData.roadNum.Contains(lane_no))       //车道包含
                    {
                        data.dev_ip = excelData.dev_ip;
                        data.dev_code = excelData.dev_code;
                    }
                }
            }

            return dataList;
        }

        private List<HH24Data> SetDevCode(string start, string end, string roadName)
        {
            //设备总表实体
            List<ExcelData> excelList = LoadExcel();
            //违法记录实体
            List<HH24Data> dataList = GetGroupHH24Alarm(start, end, roadName);

            if (dataList == null || excelList == null)
            {
                return null;
            }
            foreach (HH24Data data in dataList)
            {
                //违法表路口名
                string dName = data.cross_name;

                //存放IP和对应的路口名相似度值
                Dictionary<string, float> SimilIPArray = new Dictionary<string, float>();

                foreach (ExcelData excelData in excelList)
                {
                    //设备总表路口名
                    string eName = excelData.cross_name;

                    //计算两个路口名的相似度
                    float similar = Tools.LevenShtein(dName, eName);

                    SimilIPArray.Add(excelData.dev_ip, similar);

                    if (similar == 1)
                    {
                        break;
                    }
                }
                //取最大相似度
                float max = SimilIPArray.Max(kvp => kvp.Value);
                //最大相似度小于70%丢掉数据
                if (max < Tools.simil) continue;
                //根据最大相似度找到对应IP
                KeyValuePair<string, float> result = SimilIPArray.FirstOrDefault(o => o.Value == max);
                //根据IP找到对应设备总表中的实体
                ExcelData finalExcel = excelList.FirstOrDefault(dl => dl.dev_ip == result.Key);
                string[] item = data.lane_name.Split('-');
                string direction = item[0];
                string lane_no = item[1].Substring(2, 1);
                //根据设备总表中的实体取违法记录实体，并赋值CODE字段
                foreach (ExcelData excelData in excelList)
                {
                    if (excelData.cross_name == finalExcel.cross_name //路口名相同
                        && direction == excelData.direction           //方向相同
                        && excelData.roadNum.Contains(lane_no))       //车道包含
                    {
                        data.dev_code = excelData.dev_code;
                    }
                }
                data.lane_name = direction;
            }
            return dataList;
        }
        //统计
        private List<HH24Data> GetGroupHH24Alarm(string start, string end, string roadName)
        {
            int threshold = -1;
            bool setFlag = false;
            if (!string.IsNullOrEmpty(txtAlarm.Text))
            {
                threshold = int.Parse(txtAlarm.Text);
            }
            List<HH24Data> dataList = new List<HH24Data>();
            //需要过滤的路口ID
            DataTable dtContID = Tools.ExcelToDataTable(@"D:\HYSignService\海康卡口id表.xlsx", true);
            List<string> idList = new List<string>();
            
            for (int i = 0; i < dtContID.Rows.Count; i++)
            {
                string id = dtContID.Rows[i][0].ToString().Trim();
                idList.Add(id);
            }
            string alarmSql = "select a.crossing_id,b.crossing_name,c.lane_name,a.violative_action,to_char(a.pass_time,'HH24') passtime,count(a.crossing_id) numb  ";
            alarmSql += "from TRAFFIC_VIOLATIVE_ALARM a ,TRAFFIC_CROSSING_INFO b ,TRAFFIC_LANE_INFO c ";
            alarmSql += "where a.crossing_id = b.crossing_id and a.lane_no = c.lane_no and c.crossing_id = b.crossing_id ";
            alarmSql += "and a.violative_action in('13730','11163','60951','13010','13447','13570','12081','16251','1301','1208','1625') ";
            alarmSql += "and a.pass_time >= to_timestamp('" + start + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            alarmSql += "and a.pass_time <= to_timestamp('" + end + ".999999', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            if (!string.IsNullOrEmpty(roadName))
            {
                alarmSql += "and b.crossing_name like '%" + roadName + "%' ";
            }
            alarmSql += "group by b.crossing_name,c.lane_name,a.violative_action,to_char(a.pass_time,'HH24'),a.crossing_id ";
            alarmSql += "order by b.crossing_name,c.lane_name,a.violative_action,passtime ";
            
            DataSet dsCount = OracleHelper.Query(alarmSql);
            if (Tools.DSisNull(dsCount))
            {
                DataTable dt = dsCount.Tables[0];
                HH24Data data = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (data == null)
                        data = new HH24Data();

                    string cross_id = dt.Rows[i][0].ToString();
                    if (idList.Contains(cross_id))
                    {
                        //卡口ID表里对应的违法数据不要
                        continue;
                    }
                    string cross_name = dt.Rows[i][1].ToString()
                        .Replace("（", "(")
                        .Replace("）", ")");
                    string lane_name = dt.Rows[i][2].ToString();
                    string sysdict_code = dt.Rows[i][3].ToString();

                    string passtime = dt.Rows[i][4].ToString();

                    if (data.cross_name != cross_name || data.lane_name != lane_name || data.sysdict_code != sysdict_code)
                    {
                        if (data.cross_name != null)
                        {
                            dataList.Add(data);
                            data = new HH24Data();
                        }
                        data.cross_name = cross_name;
                        data.lane_name = lane_name;
                        data.sysdict_code = sysdict_code;
                    }
                    
                #region 插入时间段
                    switch (passtime)
                    {
                        case "00":
                            data.HH00 = dt.Rows[i][5].ToString();
                            break;
                        case "01":
                            data.HH01 = dt.Rows[i][5].ToString();
                            break;
                        case "02":
                            data.HH02 = dt.Rows[i][5].ToString();
                            break;
                        case "03":
                            data.HH03 = dt.Rows[i][5].ToString();
                            break;
                        case "04":
                            data.HH04 = dt.Rows[i][5].ToString();
                            break;
                        case "05":
                            data.HH05 = dt.Rows[i][5].ToString();
                            break;
                        case "06":
                            data.HH06 = dt.Rows[i][5].ToString();
                            break;
                        case "07":
                            data.HH07 = dt.Rows[i][5].ToString();
                            break;
                        case "08":
                            data.HH08 = dt.Rows[i][5].ToString();
                            break;
                        case "09":
                            data.HH09 = dt.Rows[i][5].ToString();
                            break;
                        case "10":
                            data.HH10 = dt.Rows[i][5].ToString();
                            break;
                        case "11":
                            data.HH11 = dt.Rows[i][5].ToString();
                            break;
                        case "12":
                            data.HH12 = dt.Rows[i][5].ToString();
                            if (int.Parse(data.HH12) >= threshold)
                                setFlag = true;
                            break;
                        case "13":
                            data.HH13 = dt.Rows[i][5].ToString();
                            break;
                        case "14":
                            data.HH14 = dt.Rows[i][5].ToString();
                            break;
                        case "15":
                            data.HH15 = dt.Rows[i][5].ToString();
                            break;
                        case "16":
                            data.HH16 = dt.Rows[i][5].ToString();
                            break;
                        case "17":
                            data.HH17 = dt.Rows[i][5].ToString();
                            break;
                        case "18":
                            data.HH18 = dt.Rows[i][5].ToString();
                            break;
                        case "19":
                            data.HH19 = dt.Rows[i][5].ToString();
                            break;
                        case "20":
                            data.HH20 = dt.Rows[i][5].ToString();
                            break;
                        case "21":
                            data.HH21 = dt.Rows[i][5].ToString();
                            break;
                        case "22":
                            data.HH22 = dt.Rows[i][5].ToString();
                            break;
                        case "23":
                            data.HH23 = dt.Rows[i][5].ToString();
                            break;
                        default:
                            break;
                    }
                #endregion
                    if (i == dt.Rows.Count - 1)// && setFlag
                    {
                        dataList.Add(data);
                    }
                        
                }
            }
            if (dataList.Count > 0)
            {
                return dataList;
            }
            EnableUI(true, "无法获取违法数据");
            return null;
        }
        //同比
        private List<AnalyseDayData> DayOnDya(string end, string roadName)
        {
            string val = txtFloat.Text;
            int numb = 0;
            if (!string.IsNullOrEmpty(val))
            {
                numb = int.Parse(val);
            }
            
            DateTime temp = DateTime.Parse(end).AddDays(-2);

            string start = temp.ToString("yyyy-MM-dd") + "00:00:00";
            string startTime = temp.ToString("yyyy-MM-dd");
            string middelTime = DateTime.Parse(end).AddDays(-1).ToString("yyyy-MM-dd");
            string endTime = DateTime.Parse(end).ToString("yyyy-MM-dd");
            //AnalyseDayData
            List<AnalyseDayData> dataList = new List<AnalyseDayData>();
            //需要过滤的路口ID
            DataTable dtContID = Tools.ExcelToDataTable(@"D:\HYSignService\海康卡口id表.xlsx", true);
            List<string> idList = new List<string>();
            
            for (int i = 0; i < dtContID.Rows.Count; i++)
            {
                string id = dtContID.Rows[i][0].ToString().Trim();
                idList.Add(id);
            }
            string alarmSql = "select a.crossing_id,b.crossing_name,c.lane_name,a.violative_action,to_char(a.pass_time,'yyyy-mm-dd') passtime,count(a.crossing_id) numb ";
            alarmSql += "from TRAFFIC_VIOLATIVE_ALARM a ,TRAFFIC_CROSSING_INFO b ,TRAFFIC_LANE_INFO c ";
            alarmSql += "where a.crossing_id = b.crossing_id and a.lane_no = c.lane_no and c.crossing_id = b.crossing_id ";
            alarmSql += "and a.violative_action in('13730','11163','60951','13010','13447','13570','12081','16251','1301','1208','1625') ";
            alarmSql += "and a.pass_time >= to_timestamp('" + start + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            alarmSql += "and a.pass_time <= to_timestamp('" + end + ".999999', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            if (!string.IsNullOrEmpty(roadName))
            {
                alarmSql += "and b.crossing_name like '%" + roadName + "%' ";
            }
            alarmSql += "group by b.crossing_name,c.lane_name,a.violative_action,to_char(a.pass_time,'yyyy-mm-dd'),a.crossing_id ";
            alarmSql += "order by b.crossing_name,c.lane_name,a.violative_action,passtime";
            
            DataSet dsCount = OracleHelper.Query(alarmSql);
            if (Tools.DSisNull(dsCount))
            {
                DataTable dt = dsCount.Tables[0];
                AnalyseDayData data = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (data == null)
                        data = new AnalyseDayData();

                    string cross_id = dt.Rows[i][0].ToString();
                    if (idList.Contains(cross_id))
                    {
                        //卡口ID表里对应的违法数据不要
                        continue;
                    }
                    string cross_name = dt.Rows[i][1].ToString()
                        .Replace("（", "(")
                        .Replace("）", ")");
                    string lane_name = dt.Rows[i][2].ToString();
                    string sysdict_code = dt.Rows[i][3].ToString();

                    string passtime = dt.Rows[i][4].ToString();
                    if (data.cross_name != cross_name || data.lane_name != lane_name || data.sysdict_code != sysdict_code)
                    {
                        if (data.cross_name != null)
                        {
                            if (numb > 0)
                            {
                                //杨森说只取前一天的浮动值
                                if (!string.IsNullOrEmpty(data.day1) && !string.IsNullOrEmpty(data.today))
                                {
                                    int today = int.Parse(data.today);
                                    int day1 = int.Parse(data.day1);
                                    int difference = today - day1;
                                    int abs = Math.Abs(difference);
                                    if (abs >= numb)
                                    {
                                        dataList.Add(data);
                                    }
                                }
                            }
                            else
                            {
                                dataList.Add(data);
                            }
                            
                            data = new AnalyseDayData();
                        }
                        data.cross_name = cross_name;
                        data.lane_name = lane_name;
                        data.sysdict_code = sysdict_code;
                    }
                    if (passtime == startTime)
                    {
                        data.day2 = dt.Rows[i][5].ToString();
                    }
                    else if (passtime == middelTime)
                    {
                        data.day1 = dt.Rows[i][5].ToString();
                    }
                    else if (passtime == endTime)
                    {
                        data.today = dt.Rows[i][5].ToString();
                    }
                    if (i == dt.Rows.Count - 1)
                        dataList.Add(data);
                }
            }
            if (dataList.Count > 0)
            {
                return dataList;
            }
            EnableUI(true, "无法获取违法数据");
            return null;
        }
        private List<AnalyseDayData> SetDevCodeAnyc(string end, string roadName)
        {
            //设备总表实体
            List<ExcelData> excelList = LoadExcel();
            //违法记录实体
            List<AnalyseDayData> dataList = DayOnDya(end, roadName);
            
            if (dataList == null || excelList == null)
            {
                return null;
            }
            foreach (AnalyseDayData data in dataList)
            {
                //违法表路口名
                string dName = data.cross_name;
                //存放IP和对应的路口名相似度值
                Dictionary<string, float> SimilIPArray = new Dictionary<string, float>();

                foreach (ExcelData excelData in excelList)
                {
                    //设备总表路口名
                    string eName = excelData.cross_name;
                    //计算两个路口名的相似度
                    float similar = Tools.LevenShtein(dName, eName);

                    SimilIPArray.Add(excelData.dev_ip, similar);

                    if (similar == 1)
                    {
                        break;
                    }
                }
                //取最大相似度
                float max = SimilIPArray.Max(kvp => kvp.Value);
                //最大相似度小于70%丢掉数据
                if (max < Tools.simil) continue;
                //根据最大相似度找到对应IP
                KeyValuePair<string, float> result = SimilIPArray.FirstOrDefault(o => o.Value == max);
                //根据IP找到对应设备总表中的实体
                ExcelData finalExcel = excelList.FirstOrDefault(dl => dl.dev_ip == result.Key);
                string[] item = data.lane_name.Split('-');
                string direction = item[0];
                string lane_no = item[1].Substring(2, 1);
                //根据设备总表中的实体取违法记录实体，并赋值CODE字段
                foreach (ExcelData excelData in excelList)
                {
                    if (excelData.cross_name == finalExcel.cross_name //路口名相同
                        && direction == excelData.direction           //方向相同
                        && excelData.roadNum.Contains(lane_no))       //车道包含
                    {
                        data.dev_code = excelData.dev_code;
                    }
                }

                data.lane_name = direction;
            }
            return dataList;
        }
        /// <summary>
        /// 根据数值改变前端显示样式
        /// </summary>
        /// <param name="persen"></param>
        /// <returns></returns>
        private string ConvertpersenStr(int persen)
        {
            string persenDay = string.Empty;
            //两位小数
            //double persenDou = Math.Round(persen, 2) * 100;
            //double persenStr = Math.Abs(persenDou);
            if (persen > 0)
            {

                persenDay = "+ " + persen.ToString();
            }
            else if (persen < 0)
            {
                persenDay = "- " + Math.Abs(persen);
            }
            else if (persen == 0)
            {
                persenDay = persen.ToString();
            }
            return persenDay;
        }
        /// <summary>
        /// 获取违法记录
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="roadName"></param>
        /// <returns></returns>
        public List<Data> GetTrafficAlarmExcel(string start,string end,string roadName)
        { 
            List<Data> dataList = new List<Data>();
            //需要过滤的路口ID
            DataTable dtContID = Tools.ExcelToDataTable(@"D:\HYSignService\海康卡口id表.xlsx", true);
            List<string> idList = new List<string>();
            for (int i = 0; i < dtContID.Rows.Count; i++)
            {
                string id = dtContID.Rows[i][0].ToString().Trim();
                idList.Add(id);
            }
            string alarmSql = "select c.crossing_id,c.crossing_name,a.plate_no,a.plate_color,to_char(a.pass_time,'yyyy-mm-dd HH24:mi:ss') passtime,e.lane_name,b.sysdict_code,b.sysdict_name ";
            //alarmSql += "CONCAT('http://192.168.92.235:6120',d.vehiclepicurl) pic ";
            alarmSql += "from TRAFFIC_VIOLATIVE_ALARM a, TRAFFIC_SYSDICT b ,TRAFFIC_CROSSING_INFO c,picurl_info d , TRAFFIC_LANE_INFO e ";
            alarmSql += "where b.sysdict_type = 1017 ";//a.plate_no = '鄂H3L520' and 
            alarmSql += "and b.sysdict_code in('13730','11163','60951','13010','13447','13570','12081','16251','1301','1208','1625') ";
            alarmSql += "and b.sysdict_code = a.violative_action ";
            alarmSql += "and c.crossing_id = a.crossing_id ";
            alarmSql += "and d.vehiclelsh = a.violative_alarm_id ";
            alarmSql += "and e.crossing_id = c.crossing_id ";
            alarmSql += "and e.lane_no = a.lane_no ";
            alarmSql += "and d.picurltype = 1 ";
            alarmSql += "and a.pass_time >= to_timestamp('" + start + ".000000', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            alarmSql += "and a.pass_time <= to_timestamp('" + end + ".999999', 'yyyy-mm-dd hh24:mi:ss.ff') ";
            if (!string.IsNullOrEmpty(roadName))
            {
                alarmSql += "and c.crossing_name like '%" + roadName + "%' ";
            }
            //alarmSql += " and a.crossing_id = 101696";
            alarmSql += "order by a.pass_time desc";
            //alarmSql += ") x WHERE rownum <= " + rn + ") y WHERE r >" + (20 * rn - 20) + "";
            DataSet dsCount = OracleHelper.Query(alarmSql);
            if (Tools.DSisNull(dsCount))
            {
                DataTable dt = dsCount.Tables[0];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Data data = new Data();
                    data.cross_id = dt.Rows[i][0].ToString();
                    if (idList.Contains(data.cross_id))
                    {
                        //卡口ID表里对应的违法数据不要
                        continue;
                    }
                    data.cross_name = dt.Rows[i][1].ToString()
                        .Replace("（", "(")
                        .Replace("）", ")");
                    data.plate_no = dt.Rows[i][2].ToString();
                    string plate_color = dt.Rows[i][3].ToString();
                    data.plate_color = Tools.ConvertColor(plate_color);
                    data.passtime = dt.Rows[i][4].ToString();
                    data.lane_name = dt.Rows[i][5].ToString();
                    data.sysdict_name = dt.Rows[i][7].ToString();
                    data.sysdict_code = dt.Rows[i][6].ToString();
                    dataList.Add(data);
                }
            }
            if (dataList.Count > 0)
            {
                return dataList;
            }
            EnableUI(true, "无法获取违法数据");
            return null;
        }
        /**
        private T MashCrossCode<T>(List<ExcelData> excelList ,T data1)
        {
            
            //Type type = data1.GetType();
            ////System.Reflection.PropertyInfo[] ropertyInfo = type.GetProperties();
            //string name = type.Name;
            
            //if (name.Equals("AnalyseDayData"))
            //{
            //    data = data1 as AnalyseDayData;
            //}
            //return data1;

            //违法表路口名
            //string dName = data.cross_name;
            ////存放IP和对应的路口名相似度值
            //Dictionary<string, float> SimilIPArray = new Dictionary<string, float>();

            //foreach (ExcelData excelData in excelList)
            //{
            //    //设备总表路口名
            //    string eName = excelData.cross_name;
            //    //计算两个路口名的相似度
            //    float similar = Tools.LevenShtein(dName, eName);

            //    SimilIPArray.Add(excelData.dev_ip, similar);

            //    if (similar == 1)
            //    {
            //        break;
            //    }
            //}
            ////取最大相似度
            //float max = SimilIPArray.Max(kvp => kvp.Value);
            ////最大相似度小于70%丢掉数据
            //if (max < 0.7) return data;
            ////根据最大相似度找到对应IP
            //KeyValuePair<string, float> result = SimilIPArray.FirstOrDefault(o => o.Value == max);
            ////根据IP找到对应设备总表中的实体
            //ExcelData finalExcel = excelList.FirstOrDefault(dl => dl.dev_ip == result.Key);
            //string[] item = data.lane_name.Split('-');
            //string direction = item[0];
            //string lane_no = item[1].Substring(2, 1);
            ////根据设备总表中的实体取违法记录实体，并赋值CODE字段
            //foreach (ExcelData excelData in excelList)
            //{
            //    if (excelData.cross_name == finalExcel.cross_name //路口名相同
            //        && direction == excelData.direction           //方向相同
            //        && excelData.roadNum.Contains(lane_no))       //车道包含
            //    {
            //        data.dev_code = excelData.dev_code;
            //    }
            //}
        }
         */
        /// <summary>
        /// 获取设备总表静态数据
        /// </summary>
        /// <returns></returns>
        private List<ExcelData> LoadExcel()
        {
            List<ExcelData> eList = new List<ExcelData>();
            String strpath = System.IO.Path.GetFullPath(@"D:\HYSignService\电警设备编码.xlsx");

            DataTable dt = new DataTable();

            dt = Tools.ExcelToDataTable(strpath, false);
            if (dt == null)
            {
                EnableUI(true, "无法获取字典数据");
                return null;
            }
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ExcelData excelData = new ExcelData();
                    string cross_name = dt.Rows[i][0].ToString().Trim();

                    if (cross_name.Contains("前拍"))
                    {
                        continue;
                    }
                    string dire = dt.Rows[i][2].ToString().Trim();
                    if (dire.Contains("前拍"))
                    {
                        continue;
                    }
                    string roadNum = dt.Rows[i][4].ToString().Trim();
                    if (string.IsNullOrEmpty(roadNum) || roadNum.Equals("/") || roadNum.Equals("无"))
                    {
                        continue;
                    }
                    excelData.dev_ip = dt.Rows[i][1].ToString().Trim();

                    ExcelData temp = eList.FirstOrDefault(o => o.dev_ip == excelData.dev_ip);
                    if (temp != null)
                    {
                        continue;
                    }
                    excelData.cross_name = cross_name
                        .Replace("（", "(")
                        .Replace("）", ")");
                    
                    
                    string direction = "";
                    if (dire.Length >= 2)
                    {
                        direction = dire.Substring(0, 1);
                        excelData.dire_index = dire.Substring(1, 1);
                    }
                    else if (dire.Length == 1)
                    {
                        direction = dire;
                        excelData.dire_index = "1";
                    }
                    excelData.direction = Tools.ConvertDirection(direction);

                    excelData.dev_code = dt.Rows[i][3].ToString().Trim();
                    
                    excelData.roadNum = roadNum.Split(',');
                    
                    eList.Add(excelData);
                }

            }
            if (eList.Count > 0)
            {
                return eList;
            }
            return null;
        }



        
        /// <summary>
        /// 禁用/启用UI
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="text"></param>
        private void EnableUI(bool flag,string text)
        {
            System.Threading.Thread.Sleep(500);
            btnSerh.Enabled = flag;
            btnExport.Enabled = flag;
            txtHold.Text = text;
            btnImport.Enabled = flag;
            FileUpload1.Enabled = flag;
        }
        
        
    }
}