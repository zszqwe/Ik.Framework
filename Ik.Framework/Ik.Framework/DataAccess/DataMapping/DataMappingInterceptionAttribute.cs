using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.DataMapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DataMappingInterceptionAttribute : Attribute
    {
        public DataMappingInterceptionAttribute(Type intercept)
        {
            this.Intercept = intercept;
        }
        public Type Intercept { get; private set; }
    }

    public interface IDataMappingInterception
    {
        void BeforeExecute(string methodName, object args);

        void EndExecute(string methodName, object args, object value);
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
