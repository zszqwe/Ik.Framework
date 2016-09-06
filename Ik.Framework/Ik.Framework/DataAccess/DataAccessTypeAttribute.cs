using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DataAccessTypeAttribute : Attribute
    {
        public DataAccessType AccessType { get; private set; }

        public DataAccessTypeAttribute(DataAccessType accessType)
        {
            this.AccessType = accessType;
        }
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
    public class DataSourceAttribute : Attribute
    {
        public string Name { get; private set; }

        public DataSourceAttribute(string name)
        {
            this.Name = name;
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
