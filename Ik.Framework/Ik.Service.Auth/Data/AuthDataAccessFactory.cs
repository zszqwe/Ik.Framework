using Ik.Framework.DataAccess.DataMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Data
{
    public class AuthDataAccessFactory
    {
        private static IDataAccessFactory factory = null;
        private static object lockObj = new object();
        public const string DataSourceName = AuthDataSources.DataSource_AuthService;

        public static IDataAccessFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lockObj)
                    {
                        if (factory == null)
                        {
                            factory = new DataAccessFactory(DataSourceName);
                        }
                    }
                }
                return factory;
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
