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
