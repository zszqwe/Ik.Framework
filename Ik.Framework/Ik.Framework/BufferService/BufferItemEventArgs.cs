using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public class BufferItemEventArgs : EventArgs
    {
        private BufferItem _item = null;

        public BufferItemEventArgs(BufferItem item)
        {
            this._item = item;
        }
        public  BufferItem Item
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
