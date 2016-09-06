using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    public class DataAccessOutSqlTextAttribute : Attribute
    {
        public DataAccessOutSqlTextAttribute(bool ignoreEnvironment, OutSqlTextType outType)
        {
            this.IgnoreEnvironment = ignoreEnvironment;
            this.OutType = outType;
        }
        public bool IgnoreEnvironment { get; private set; }

        public OutSqlTextType OutType { get; private set; }

        public DataAccessOutSqlTextConfig Create()
        {
            return new DataAccessOutSqlTextConfig { IgnoreEnvironment = this.IgnoreEnvironment, OutType = this.OutType };
        }
    }

    public class DataAccessOutSqlTextConfig
    {
        public bool IgnoreEnvironment { get; set; }

        public OutSqlTextType OutType { get; set; }
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
