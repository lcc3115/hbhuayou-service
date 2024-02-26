using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class FlowDict
    {
		#region Model
		private int? _flow_id;
		private int? _flow_no;
		private string _flow_name;
		private int? _crossing_id;
		private int? _lane_no;
		private int? _percent;
		/// <summary>
		/// 
		/// </summary>
		public int? Flow_Id
		{
			set { _flow_id = value; }
			get { return _flow_id; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Flow_No
		{
			set { _flow_no = value; }
			get { return _flow_no; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string Flow_Name
		{
			set { _flow_name = value; }
			get { return _flow_name; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Crossing_Id
		{
			set { _crossing_id = value; }
			get { return _crossing_id; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Lane_No
		{
			set { _lane_no = value; }
			get { return _lane_no; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Percent
		{
			set { _percent = value; }
			get { return _percent; }
		}
		#endregion Model
	}
}