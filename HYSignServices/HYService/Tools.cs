using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Reflection;

namespace HYSignServices.HYService
{
    public class Tools
    {
        public static float simil = 1;
        public static int ROW_LIMIT = 35; 
        public static string localSeq = "";
        public static string crossids = "100685,100339,100449,100470,100309,101366,101076,101257,101260,101336,102179,100437,100417,100682,102181,102182,101457,100323,100169,100768,100367,100770,100663,100776,100777,100702,100796,100343,100334,100467,100468,100306,100518,100325,100444,100336,100455,100746,101082,100447,102180,102016,100446,102376,102198,100756,100773,100664,100691,100646,100665,102356,100461,101636,100834,100332,100452,100453,100337,100450,100319,100308,100335,101150,102183,100775,100769,100761,101217,101280,101296,101297,101556,100962,100778,100797,101596,101876,100329,100465,100338,100326,100741,101278,100495,100315,102037,102276,100454,101600,101500,102178,100758,100662,100752,100645,100714,100739,100751,101598,101437,101776,102056,100333,102199,100459,100451,100442,102018,100418,101456,102184,100322,100658,100780,100686,100755,100745,101200,101216,101577,101597,101877,101037,100324,100341,100443,100330,100749,100496,100316,100317,100318,100294,100485,102141,100657,100661,100744,100767,100753,101436,101696,101259,101258,100307,100328,100327,100448,100311,100312,100436,101218,101499,102196,100499,100659,100660,100656,100666,100765,100754,101279,102316,100331,101036,100340,100458,100747,101277,100498,100310,100347,101199,101256,100344,101190,101978,100460,100466,100688,101337,100748,101599,101364,101557,101576,101658,100445,102620,100463,102258,102296,101616,102096,102156";
        public static Dictionary<int, string> temp = new Dictionary<int, string>{
                {2018, "H"},
                {2019, "I"},
                {2020, "I"},
                {2021, "I"},
                {2022, "L"},
                {2023, "M"},
                {2024, "N"}
        };
        
        /// <summary>
        /// table转json
        /// </summary>
        public static String DataTableToJson(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "";
            }

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 相位字典转前端字符串
        /// </summary>
        public static string RePhase(string phase)
        {
            switch (phase)
            {
                case "东西直行":
                    return "东直西直";
                case "东西左转":
                    return "东左西左";
                case "南北直行":
                    return "南直北直";
                case "南北左转":
                    return "南左北左";
                default:
                    return phase;
            }
        }

        public static string ReDirection(string direction)
        {
            switch (direction)
            {
                case "w_e":
                    return "西向东";
                case "e_w":
                    return "东向西";
                case "s_n":
                    return "南向北";
                case "n_s":
                    return "北向南";
                default:
                    break;
            }
            return null;
        }

        //判断行车方向
        public static string NextDirection(string direction, string nextDirection)
        {
            //string str = direction.Substring(0, 4);

            if (direction == "由东向西")
            {
                switch (nextDirection)
                {
                    case "由东向西":
                        return "forward";
                    case "由北向南":
                        return "left";
                    case "由南向北":
                        return "right";
                    case "由西向东":
                        return "turnAround";
                    default:
                        break;
                }
            }
            else if (direction == "由西向东")
            {
                switch (nextDirection)
                {
                    case "由西向东":
                        return "forward";
                    case "由北向南":
                        return "right";
                    case "由南向北":
                        return "left";
                    case "由东向西":
                        return "turnAround";
                    default:
                        break;
                }
            }
            else if (direction == "由南向北")
            {
                switch (nextDirection)
                {
                    case "由西向东":
                        return "right";
                    case "由北向南":
                        return "turnAround";
                    case "由南向北":
                        return "forward";
                    case "由东向西":
                        return "left";
                    default:
                        break;
                }
            }
            else if (direction == "由北向南")
            {
                switch (nextDirection)
                {
                    case "由西向东":
                        return "left";
                    case "由北向南":
                        return "forward";
                    case "由南向北":
                        return "turnAround";
                    case "由东向西":
                        return "right";
                    default:
                        break;
                }
            }
            return null;
        }

        static string deviceip;
        static string returnvalue = "";
        static Thread thread;
        public string IsOnline(string ips)
        {
            deviceip = ips;

            thread = new Thread(OnlineMethod);
            thread.IsBackground = true;
            thread.Start();
            // while(thread.ThreadState!=ThreadState.Stopped);

            return (returnvalue);
        }
        public void OnlineMethod()
        {

            
            Ping ping = new Ping();
            PingReply pingreply = ping.Send(deviceip);
            // PingReply pingreply = ping.SendAsync(deviceip,"");
            if (pingreply.Status == IPStatus.Success)
            {
                returnvalue = "在线";
            }
            else
            {
                returnvalue = "离线";
            }
            

        }
        //错误日志
        public static void WriteErrLog(string msg)
        {
            FileStream fs = new FileStream("C:\\TrafficLog.log", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("{0}\n{1}\r\n", DateTime.Now + " - Clint_Message:", msg);
            sw.Flush();
            sw.Close();
        }
        public static void WriteLog(string msg)
        {
            FileStream fs = new FileStream("D:\\TrafficLog.log", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("{0}\n", msg);
            sw.Flush();
            sw.Close();
        }
        public static string GetSeq()
        {
            DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string seq = currentTime.ToString("yyyyMMddHHmmss") + "000001";
            return seq;
        }


        public static string SetXMLInfo(string info, string id, string NO)
        {
            localSeq = GetSeq();
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<Message>";
            xml += "<Version>1.1</Version>";
            xml += "<Token></Token>";
            xml += "<From>";
            xml += "<Address><Sys>TICP</Sys>";
            xml += "<SubSys/>";
            xml += "<Instance/>";
            xml += "</Address>";
            xml += "</From>";
            xml += "<To><Address>";
            xml += "<Sys>UTCS</Sys><SubSys/><Instance/>";
            xml += "</Address></To>";
            xml += "<Type>REQUEST</Type>";
            xml += "<Seq>" + localSeq + "</Seq>";
            xml += "<Body><Operation order=\"1\" name=\"Get\">";
            xml += "<TSCCmd>";
            xml += "<ObjName>" + info + "</ObjName>";
            xml += "<ID>" + id + "</ID>";
            xml += "<No>" + NO + "</No>";
            xml += "</TSCCmd>";
            xml += "</Operation></Body></Message>";
            return xml;
        }

        public static void ReDataTable(DataTable dt)
        {
            DataTable redt = dt;
            for (int i = 0; i < redt.Rows.Count; i++)
            {
                int first = int.Parse(redt.Rows[i][0].ToString());
            }
        }
        /// <summary>
        /// 返回年份所对应的表头
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string ReTableTitle(int title)
        {
            return temp[title];
        }
        /// <summary>
        /// 查询跨年份时返回表头数组
        /// </summary>
        /// <param name="start_year"></param>
        /// <param name="end_year"></param>
        /// <returns></returns>
        public static string[] ReTableTitle(int start_year, int end_year)
        {
            int span = end_year - start_year;
            string[] title = new string[span + 1];
            for (int i = 0; i <= span; i++)
            {
                title[i] = temp[start_year + i];
            }
            return title;
        }
        
        public static bool DSisNull(DataSet ds)
        {
            if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        //十进制转十六进制
        //16384.ToString("x8") ==  0x4000
        public static string GetCheckType(string typeIndex)
        {
            switch (typeIndex)//十进制
            {
                case "0":
                    return "人员聚集";
                case "1":
                    return "拥堵";
                case "2":
                    return "违法停车";
                case "8":
                    return "行人";
                case "16":
                    return "抛洒物";
                case "8192":
                    return "路障";
                case "16384":
                    return "施工";
                default:
                    return "未知";
            }
        }

        public static string GetCheckDire(string typeIndex)
        {
            switch (typeIndex)
            {
                case "4":
                    return "进城";
                case "2":
                    return "出城";
                default:
                    return "未知";
            }
        }

        public static string GetDirection(string typeIndex)
        {
            switch (typeIndex)
            {
                case "由东向西":
                    return "1";
                case "由西向东":
                    return "2";
                case "由南向北":
                    return "3";
                case "由北向南":
                    return "4";
                default:
                    return "0";
            }
        }

        public static string GetFlowIndex(string typeIndex)
        {
            switch (typeIndex)
            {
                case "1":
                    return "3";
                case "3":
                    return "4";
                case "4":
                    return "1";
                case "2":
                    return "2";
                default:
                    return "0";
            }
        }

        public static string ReturnDirection(string typeIndex)
        {
            switch (typeIndex)
            {
                case "由东向西":
                    return "2001";
                case "由西向东":
                    return "2002";
                case "由南向北":
                    return "2003";
                case "由北向南":
                    return "2004";
                default:
                    return "0";
            }
        }

        public static string ReturnDirectionIndex(string typeIndex)
        {
            switch (typeIndex)
            {
                case "2001":
                    return "由东向西";
                case "2002":
                    return "由西向东";
                case "2003":
                    return "由南向北";
                case "2004":
                    return "由北向南";
                default:
                    return "0";
            }
        }

        public static string ConvertDirection(string str)
        {
            switch (str)
            {
                case "东":
                    return "由东向西";
                case "西":
                    return "由西向东";
                case "南":
                    return "由南向北";
                case "北":
                    return "由北向南";
                default:
                    return "";
            }
        }
        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = WorkbookFactory.Create(fs);//new HSSFWorkbook(fs);
                    
                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {

                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            CellType t = cell.CellType;//NUMERIC
                                            if (t == CellType.NUMERIC)
                                            {

                                                column = new DataColumn(cell.NumericCellValue.ToString());
                                                dataTable.Columns.Add(column);

                                            }
                                            if (t == CellType.STRING)
                                            {
                                                if (cell.StringCellValue != null)
                                                {
                                                    column = new DataColumn(cell.StringCellValue);
                                                    dataTable.Columns.Add(column);
                                                }
                                            }
                                            if (t == CellType.BLANK)
                                            {

                                                column = new DataColumn("");
                                                dataTable.Columns.Add(column);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {

                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.BLANK:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.NUMERIC:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.STRING:
                                                case CellType.FORMULA:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// NPOI导出项目详细信息
        /// </summary>
        /// <param name="ds">dataset中的每个datatable都是一页数据</param>
        /// <returns>文件内存</returns>
        public static MemoryStream ExportDataSetToExcel(DataTable table_data)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            
                ISheet sheet1 = hssfworkbook.CreateSheet("违法记录");

                ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                ICellStyle stringStyle = hssfworkbook.CreateCellStyle();
                stringStyle.VerticalAlignment = VerticalAlignment.CENTER;

                //取得列宽
                int columnCount = table_data.Columns.Count;
                int[] arrColWidth = new int[columnCount];
                int width = 10;
                foreach (DataColumn column in table_data.Columns)
                {
                    arrColWidth[column.Ordinal] = width;
                }

                int rowIndex = 0;

                foreach (DataRow row in table_data.Rows)
                {

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet1 = hssfworkbook.CreateSheet();
                        }
                        IRow headerRow = sheet1.CreateRow(0);
                        foreach (DataColumn column in table_data.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        }
                        rowIndex = 1;
                    }

                    int j = 1;
                    IRow dataRow = sheet1.CreateRow(rowIndex);
                    foreach (DataColumn column in table_data.Columns)
                    {
                        ICell newCell = dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                newCell.CellStyle = stringStyle;
                                break;
                            case "System.Double":
                                if (drValue != "")
                                {
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                }
                                else
                                {
                                    newCell.SetCellValue("");
                                }
                                newCell.CellStyle = cellStyle;
                                break;
                        }
                        j++;
                    }

                    rowIndex++;
                }
            


            //冻结窗口  锁定表头和第一列
            //sheet1.CreateFreezePane(1, 1, 1, 1);
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        public static DataTable ListToDataTable<T>(List<T> list, string[] titles)
        {
            DataTable dt = new DataTable();
            Type listType = typeof(T);
            PropertyInfo[] properties = listType.GetProperties();
            //标题行
            if (titles != null && properties.Length == titles.Length)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    dt.Columns.Add(new DataColumn(titles[i], property.PropertyType));
                }
            }
            else
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    dt.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }
            }
            //内容行
            foreach (T item in list)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dr[i] = properties[i].GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static decimal GetSimilarityWith(string sourceString, string str)
        {

            decimal Kq = 2;
            decimal Kr = 1;
            decimal Ks = 1;

            char[] ss = sourceString.ToCharArray();
            char[] st = str.ToCharArray();

            //获取交集数量
            int q = ss.Intersect(st).Count();
            int s = ss.Length - q;
            int r = st.Length - q;

            return Kq * q / (Kq * q + Kr * r + Ks * s);
        }

        public static float LevenShtein(string str1, string str2)
        {
            //计算两个字符串的长度。 
            int len1 = str1.Length;
            int len2 = str2.Length;
            //建立上面说的数组，比字符长度大一个空间 
            int[,] dif = new int[len1 + 1, len2 + 1];
            //赋初值，步骤B。 
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            //计算两个字符是否一样，计算左上的值 
            int temp;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    //取三个值中最小的 
                    dif[i, j] = Math.Min(Math.Min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1), dif[i - 1, j] + 1);
                }
            }
            
            //计算相似度 
            float similarity = 1 - (float)dif[len1, len2] / Math.Max(str1.Length, str2.Length);
            
            return similarity;
        }

        public static string ConvertColor(string code)
        {
            switch (code)
            {
                case "1":
                    return "黄";
                case "2":
                    return "蓝";
                case "3":
                    return "黑";
                case "4":
                    return "其他";
                case "5":
                    return "绿";
                case "6":
                    return "民航黑";
                case "0":
                    return "白";
                default:
                    return "其他";
            }
        }
    }

}