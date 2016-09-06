using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public abstract class BufferItem
    {
        private string typeName;
        public BufferItem()
        {
            this.CreateTime = DateTime.Now;
            this.BufferId = Guid.NewGuid();
            this.typeName = this.GetType().Name;
        }
        public Guid BufferId { get; set; }
        /// <summary>
        /// 日志创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public string TypeName
        {
            get
            {
                return typeName;
            }

            set
            {
                typeName = value;
            }
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
