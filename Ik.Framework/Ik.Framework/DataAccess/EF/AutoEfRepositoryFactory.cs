using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Ik.Framework.DataAccess.EF
{
    public static class AutoEfRepositoryFactory 
    {
        private static ConcurrentDictionary<string, string> _dataAccessList = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, IEfRepositoryFactory> _dataAccessFactoryList = new ConcurrentDictionary<string, IEfRepositoryFactory>();

        public static IEfRepositoryFactory GetEfRepositoryFactory<T>() where T : BaseEntity
        {
            return GetEfRepositoryFactory(typeof(T));
        }

        public static IRepository<T> GetEfRepository<T>() where T : BaseEntity
        {
            IEfRepositoryFactory factory = GetEfRepositoryFactory<T>();
            return factory.GetEfRepository<T>();
        }

        public static object GetEfRepository(Type entityType)
        {
            IEfRepositoryFactory factory = GetEfRepositoryFactory(entityType);
            return factory.GetEfRepository(entityType);
        }

        public static IEfRepositoryFactory GetEfRepositoryFactory(Type entityType)
        {
            string dataSourceName = _dataAccessList.GetOrAdd(entityType.AssemblyQualifiedName, (key) =>
            {
                DataSourceAttribute da = entityType.GetCustomAttribute<DataSourceAttribute>();
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

            return GetEfRepositoryFactory(dataSourceName);
        }

        public static IEfRepositoryFactory GetEfRepositoryFactory(string dataSourceName)
        {
            IEfRepositoryFactory factory = _dataAccessFactoryList.GetOrAdd(dataSourceName, (key) =>
            {
                return new EfRepositoryFactory(key);
            });
            return factory;
        }
    }
}
