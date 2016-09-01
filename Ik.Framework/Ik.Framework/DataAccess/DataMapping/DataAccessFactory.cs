using IkCastle.DynamicProxy;
using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using IBatisNet.DataMapper;
using System.Collections;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Ik.Framework.DataAccess.DataMapping
{
    public class DataAccessFactory :IDataAccessFactory
    {
        private static ILog _logger = LogManager.GetLogger("数据访问服务");
        private DataSource _dataSourceConfig = null;
        private DataMappingContext _context = new DataMappingContext();
        private Dictionary<string, object> _dataAccessList = new Dictionary<string, object>();
        private SqlMapLoader _mapLoader = null;
        private DataAccessConfigManager configManager = new DataAccessConfigManager();
        private static readonly object lockObj = new object();


        public DataAccessFactory(string dataSourceName)
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
            var resourceName = "Ik.Framework.DataAccess.DataMapping.Map.SQLServer.config";
            if (_dataSourceConfig.Provider == DataSourceProvider.MySQL)
            {
                resourceName = "Ik.Framework.DataAccess.DataMapping.Map.MySQL.config";
            }
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(resourceName);
            XmlDocument config = new XmlDocument();
            config.Load(stream);
            this._mapLoader = new SqlMapLoader(config, _context.WriteConnectionString, _context.ReadConnectionString);
            this._context.Reader = _mapLoader.ReadOnlyMapper;
            this._context.Writer = _mapLoader.WriteMapper;
        }

        public DataAccessFactory(string dataSourceName, DataAccessOutSqlTextConfig outTypeConfig)
            : this(dataSourceName)
        {
            this._context.OutTypeConfig = outTypeConfig;
        }
        public T GetDataAccess<T>()
        {
            Type type = typeof(T);
            T value = (T)GetDataAccess(type);
            return value;
        }

        public object GetDataAccess(Type dataAccessType)
        {
            string key = dataAccessType.AssemblyQualifiedName;
            object value = null;
            if (_dataAccessList.ContainsKey(key))
            {
                value = _dataAccessList[key];
            }
            else
            {
                lock (lockObj)
                {
                    if (_dataAccessList.ContainsKey(key))
                    {
                        value = _dataAccessList[key];
                    }
                    else
                    {
                        value = CreateDataAccessObject(dataAccessType);
                        _dataAccessList.Add(key, value);
                    }
                }
            }
            return value;
        }

        private object CreateDataAccessObject(Type type)
        {
            try
            {
                string xmlTemplete = "<sqlMap embedded=\"{0},{1}\" xmlns=\"http://ibatis.apache.org/dataMapper\" />";
                XmlDocument config = new XmlDocument();
                config.LoadXml(string.Format(xmlTemplete, type.FullName + ".sql.xml", type.Assembly.FullName));
                if (this._context.Reader != null)
                {
                    _mapLoader.ReadOnlyBuilder.ConfigureSqlMap(config.DocumentElement);
                }
                _mapLoader.WriteBuilder.ConfigureSqlMap(config.DocumentElement);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(string.Format("创建数据访问对象失败，对象类型：{0}", type.FullName), ex);
            }
            DefaultProxyBuilder builder = new DefaultProxyBuilder();
            ProxyGenerator generator = new ProxyGenerator(builder);
            DataAccessOutSqlTextConfig outTypeConfig = null;
            var outSqlType = type.GetCustomAttribute<DataAccessOutSqlTextAttribute>();
            if (outSqlType != null)
            {
                outTypeConfig = outSqlType.Create();
            }
            return generator.CreateInterfaceProxyWithoutTarget(type, new DataAccess(this._context, outTypeConfig));
        }


        public void BeginTransaction()
        {
            this._context.Writer.BeginTransaction();
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            this._context.Writer.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            this._context.Writer.LocalSession.Complete();
        }

        public void RollBackTransaction()
        {
            this._context.Writer.LocalSession.Dispose();
        }


        public DataMappingTransactionScope CreateTransactionScope()
        {
            return new DataMappingTransaction(this);
        }


        public DataMappingTransactionScope CreateTransactionScope(System.Data.IsolationLevel isolationLevel)
        {
            return new DataMappingTransaction(this, isolationLevel);
        }


        public void SqlBulkCopyInsert(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            bool isSessionLocal = false;
            var session = this._context.Writer.LocalSession;
            if (session == null)
            {
                session = this._context.Writer.CreateSqlMapSession();
                isSessionLocal = true;
            }
            SqlBulkCopy copy = null;
            if (!isSessionLocal && session.IsTransactionStart)
            {
                copy = new SqlBulkCopy((SqlConnection)session.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)session.Transaction);
            }
            else
            {
                copy = new SqlBulkCopy((SqlConnection)session.Connection);
            }
            copy.DestinationTableName = dt.TableName;
            copy.BatchSize = dt.Rows.Count;
            try
            {
                if (isSessionLocal)
                {
                    session.OpenConnection();
                }
                copy.WriteToServer(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                copy.Close();
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            
        }


        public void SqlBulkCopyInsert(DataTable dt, IEnumerable<SqlBulkCopyColumnMapping> mapping)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            bool isSessionLocal = false;
            var session = this._context.Writer.LocalSession;
            if (session == null)
            {
                session = this._context.Writer.CreateSqlMapSession();
                isSessionLocal = true;
            }
            SqlBulkCopy copy = null;
            if (!isSessionLocal && session.IsTransactionStart)
            {
                copy = new SqlBulkCopy((SqlConnection)session.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)session.Transaction);
            }
            else
            {
                copy = new SqlBulkCopy((SqlConnection)session.Connection);
            }
            foreach (var item in mapping)
            {
                copy.ColumnMappings.Add(item);
            }
            copy.DestinationTableName = dt.TableName;
            copy.BatchSize = dt.Rows.Count;
            try
            {
                if (isSessionLocal)
                {
                    session.OpenConnection();
                }
                copy.WriteToServer(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                copy.Close();
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
        }


        public DataAccessContextScope CreateReadOnlyContextScope()
        {
            return new ReadOnlyContextScope(this._context); ;
        }

        public DataAccessContextScope CreateReadWriteContextScope()
        {
            return new ReadWriteContextScope(this._context);
        }


        public void BeginReadOnlyContext()
        {
            this._context.StartReadOnlyContext();
        }

        public void BeginReadWriteContex()
        {
            this._context.StartReadWriteContext();
        }

        public void EndContext()
        {
            this._context.EndContext();
        }

        public string GetRunString<T>(string methodName, object parameters)
        {
            Type type = typeof(T);
            var statementName = type.FullName + "." + methodName;
            string commandText = string.Empty;
            var isSessionLocal = false;
            ISqlMapSession session = this._context.Writer.LocalSession;
            if (session == null)
            {
                session = this._context.Writer.CreateSqlMapSession();
                isSessionLocal = true;
            }
            try
            {
                var statement = this._context.Writer.GetMappedStatement(statementName);
                var scope = statement.Statement.Sql.GetRequestScope(statement, parameters, session);
                statement.PreparedCommand.Create(scope, session, statement.Statement, parameters);
                commandText = scope.IDbCommand.CommandText;
                foreach (DbParameter pt in scope.IDbCommand.Parameters)
                {
                    Regex regex = new Regex(pt.ParameterName + @"\D");
                    commandText = regex.Replace(commandText, GetDbParameterValue(pt));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return commandText;
        }

        private string GetDbParameterValue(DbParameter parameter)
        {
            string value = "";
            if (parameter.Value != null)
            {
                value = parameter.Value.ToString();
            }
            if (parameter.DbType == DbType.String || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.Guid || parameter.DbType == DbType.AnsiString || parameter.DbType == DbType.DateTime2 || parameter.DbType == DbType.DateTimeOffset)
            {
                value = value.Replace("'", "''");
                value = "'" + value + "'";
            }
            return value;
        }
    }
}
