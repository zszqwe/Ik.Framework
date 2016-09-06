using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Ik.Framework.DataAccess.DataMapping
{
    public static class AutoDataAccessFactory 
    {
        private static ConcurrentDictionary<string, string> _dataAccessList = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, IDataAccessFactory> _dataAccessFactoryList = new ConcurrentDictionary<string, IDataAccessFactory>();

        public static IDataAccessFactory GetDataAccessFactory<T>()
        {
            return GetDataAccessFactory(typeof(T));
        }

        public static T GetDataAccess<T>()
        {
            IDataAccessFactory factory = GetDataAccessFactory<T>();
            return factory.GetDataAccess<T>();
        }

        public static object GetDataAccess(Type dataAccessType)
        {
            IDataAccessFactory factory = GetDataAccessFactory(dataAccessType);
            return factory.GetDataAccess(dataAccessType);
        }

        public static IDataAccessFactory GetDataAccessFactory(Type dataAccessType)
        {
            string dataSourceName = _dataAccessList.GetOrAdd(dataAccessType.AssemblyQualifiedName, (key) =>
            {
                DataSourceAttribute da = dataAccessType.GetCustomAttribute<DataSourceAttribute>();
                string name = null;
                if (da != null && !string.IsNullOrEmpty(da.Name))
                {
                    name = da.Name;
                }
                if (name == null)
                {
                    throw new DataAccessException("该接口未标识为数据访问类型");
                }
                return name;
            });

            IDataAccessFactory factory = GetDataAccessFactory(dataSourceName);
            return factory;
        }

        public static IDataAccessFactory GetDataAccessFactory(string dataSourceName)
        {
            IDataAccessFactory factory = _dataAccessFactoryList.GetOrAdd(dataSourceName, (key) =>
            {
                return new DataAccessFactory(key);
            });
            return factory;
        }

        public static string GetRunString<T>(string methodName, object parameters)
        {
            IDataAccessFactory factory = GetDataAccessFactory<T>();
            return factory.GetRunString<T>(methodName, parameters);
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
