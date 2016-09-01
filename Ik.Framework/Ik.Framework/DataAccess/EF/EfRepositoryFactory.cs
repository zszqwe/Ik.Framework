using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.EF
{
    public class EfRepositoryFactory : IEfRepositoryFactory
    {
        private ConcurrentDictionary<string, object> _dataAccessList = new ConcurrentDictionary<string, object>();
        private DataAccessConfigManager configManager = new DataAccessConfigManager();
        private DataSource _dataSourceConfig = null;
        private DataAccessEfContext _context = new DataAccessEfContext();

        public EfRepositoryFactory(string dataSourceName)
        {
            this._dataSourceConfig = configManager.GetDataSource(dataSourceName);
            if (this._dataSourceConfig == null)
            {
                throw new DataAccessException("获取数据源配置失败，数据源名称：" + dataSourceName);
            }
            if (_dataSourceConfig.ReadOnlyConnectionString != null && !string.IsNullOrEmpty(_dataSourceConfig.ReadOnlyConnectionString.ConnectionString))
            {
                this._context.ReadConnectionString = _dataSourceConfig.ReadOnlyConnectionString.ConnectionString;
            }
            this._context.WriteConnectionString = _dataSourceConfig.ReadWriteConnectionString.ConnectionString;
        }

        public IRepository<T> GetEfRepository<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            IRepository<T> value = (IRepository<T>)GetEfRepository(type);
            return value;
        }

        public object GetEfRepository(Type entityType)
        {
            object value = _dataAccessList.GetOrAdd(entityType.AssemblyQualifiedName, (key) => CreateDataAccessObject(entityType));
            return value;
        }

        public object CreateDataAccessObject(Type entityType)
        {
            var repType = typeof(EfRepository<>).MakeGenericType(entityType);
            var value = Activator.CreateInstance(repType, this._context);
            return value;
        }


        public DataAccessContextScope CreateNormalContextScope()
        {
            return new NormalContextScope(this._context); ;
        }

        public DataAccessContextScope CreateReadWriteContextScope()
        {
            return new ReadWriteContextScope(this._context); 
        }

        public void BeginNormalContext()
        {
            this._context.StartNormalContext();
        }

        public void BeginReadWriteContex()
        {
            this._context.StartReadWriteContext();
        }

        public void EndContext()
        {
            this._context.EndContext();
        }
    }
}
