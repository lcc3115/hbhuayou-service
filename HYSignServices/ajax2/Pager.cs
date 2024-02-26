using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public class Pager
    {
        public Pager()
            :this("pager")
        {

        }

        public Pager(string name)
        {
            Name = name;
        }

        #region 字段
        private int _pageindex;
        private int _pagenumber;
        private int _pagecount;
        private int _itemcount;
        private int _pagesize; 
        #endregion

        public string Name
        {
            get;
            private set;
        }

        public int pageindex
        {
            get
            {
                return Math.Min(_pageindex, _pagecount - 1);
            }
            set
            {
                _pageindex = value;
                _pagenumber = value + 1;
                Calc();
            }
        }

        public int pagenumber
        {
            get
            {
                return Math.Min(_pagenumber, _pagecount);
            }
            set
            {
                _pagenumber = value;
                _pageindex = value - 1;
                Calc();
            }
        }

        public int pagesize
        {
            get
            {
                return _pagesize;
            }
            set
            {
                _pagesize = value;
                Calc();
            }
        }

        public int pagecount
        {
            get
            {
                return _pagecount;
            }
            set
            {
                _pagecount = value;
                _itemcount = -1;
                Calc();
            }
        }

        public int itemcount
        {
            get
            {
                return _itemcount;
            }
            set
            {
                _itemcount = value;
                Calc();
            }
        }

        private void Calc()
        {
            if (_pageindex < 0)
            {
                _pageindex = 0;
                _pagenumber = 1;
            }
            if (_pagesize > 0)
            {
                if (_itemcount != -1)
                {
                    _pagecount = (_itemcount + _pagesize - 1) / _pagesize;
                }
            }
        }
    }
}
