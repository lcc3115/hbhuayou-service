using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class SynCrossNo
    {
		#region Model
		private int? _local_id;
		private int? _crossing_id;
		/// <summary>
		/// 
		/// </summary>
		public int? Local_Id
		{
			set { _local_id = value; }
			get { return _local_id; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Crossing_Id
		{
			set { _crossing_id = value; }
			get { return _crossing_id; }
		}
		#endregion Model
	}
}