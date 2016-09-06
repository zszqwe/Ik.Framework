using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.DataMapping
{
    public abstract class DataMappingTransactionScope : IDisposable
    {
        /// <summary>
        /// 设置事务为完成状态
        /// </summary>
        public abstract void Complete();

        /// <summary>
        /// 事务回收
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// 事务回收
        /// </summary>
        public void Dispose()
        {
            this.Rollback();
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
