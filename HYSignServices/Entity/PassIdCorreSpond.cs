using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class PassIdCorreSpond
    {
		#region Model
		private int? _localId;
		private string _synCrossName;
		private string _synCrossCoordLat;
		private string _synCrossCoordLng;
		private string _utcsId;
		/// <summary>
		/// 
		/// </summary>
		public int? SynCrossNO
		{
			set { _localId = value; }
			get { return _localId; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string SynCrossName
		{
			set { _synCrossName = value; }
			get { return _synCrossName; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string SynCrossCoordLng
		{
			set { _synCrossCoordLat = value; }
			get { return _synCrossCoordLat; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string SynCrossCoordLat
		{
			set { _synCrossCoordLng = value; }
			get { return _synCrossCoordLng; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string UtcsId
		{
			set { _utcsId = value; }
			get { return _utcsId; }
		}
		#endregion Model
	}
}