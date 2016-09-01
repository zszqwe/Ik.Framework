using IBatisNet.DataMapper;
using Ik.Framework.Common.Paging;
using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Ik.Framework.DataAccess.DataMapping
{
    internal class DataAccessContext
    {
        private static ILog _logger = LogManager.GetLogger("数据访问服务");
        private DataMappingContext _context = null;
        private DataAccessType _dataAccessType = DataAccessType.SelectReadOnly;
        private MethodInfo _method = null;
        private List<string> _parameterList = new List<string>();
        private bool _isHashtableParameter = false;
        private bool _isSelectList = false;
        private bool _isIPagedList = false;
        private string _statementName = null;
        private Type _pagedListType = null;
        private DataAccessOutSqlTextConfig _outTypeConfig = null;
        private bool _isNotCanNullReturnValue = false;

        public DataAccessContext(DataMappingContext context, MethodInfo method, DataAccessOutSqlTextConfig outTypeConfig)
        {
            this._context = context;
            this._method = method;
            this._statementName = method.DeclaringType.FullName + "." + method.Name;
            var outSqlType = method.GetCustomAttribute<DataAccessOutSqlTextAttribute>();
            if (outSqlType != null)
            {
                this._outTypeConfig = outSqlType.Create();
            }
            else if (this._context.OutTypeConfig != null)
            {
                this._outTypeConfig = this._context.OutTypeConfig;
            }
            else if (outTypeConfig != null)
            {
                this._outTypeConfig = outTypeConfig;
            }
            var type = method.GetCustomAttribute<DataAccessTypeAttribute>();
            var methodName = method.Name.ToLower();
            if (type != null)
            {
                this._dataAccessType = type.AccessType;
            }
            else if (methodName.StartsWith("get"))
            {
                this._dataAccessType = DataAccessType.SelectReadOnly;
            }
            else if (methodName.StartsWith("update"))
            {
                this._dataAccessType = DataAccessType.Update;
            }
            else if (methodName.StartsWith("delete"))
            {
                this._dataAccessType = DataAccessType.Delete;
            }
            else if (methodName.StartsWith("insert"))
            {
                this._dataAccessType = DataAccessType.Insert;
            }
            if (this._dataAccessType == DataAccessType.SelectReadOnly && context.Reader == null)
            {
                this._dataAccessType = DataAccessType.SelectReadWrite;
                _logger.Warn("数据源配置未指定只读库连接，该方法自动转为读写库连接，方法名：{0}", method.Name);
            }
            var interfaces = this._method.ReturnType.GetInterfaces();
            foreach (var item in interfaces)
            {
                if (item == typeof(IPagedList))
                {
                    this._isIPagedList = true;
                    break;
                }
            }
            if (!this._isIPagedList)
            {
                foreach (var item in interfaces)
                {
                    if (item == typeof(IEnumerable))
                    {
                        this._isSelectList = true;
                        break;
                    }
                }
            }
            if (this._isIPagedList && this._method.ReturnType.IsGenericType)
            {
                var genericArgs = this._method.ReturnType.GetGenericArguments();
                if (genericArgs.Length > 1)
                {
                    throw new DataAccessException("分页查询方法返回值必须为PagedList<T>接口的对象");
                }
                var returnType = genericArgs[0];
                this._pagedListType = typeof(PagedList<>).MakeGenericType(returnType);
            }

            if (!this._isIPagedList && !this._isSelectList && !IsNullableType(this._method.ReturnType) && this._method.ReturnType.IsValueType)
            {
                this._isNotCanNullReturnValue = true;
            }

            var parameters = this._method.GetParameters();
            if (parameters.Length > 1)
            {
                if (this._isIPagedList)
                {
                    throw new DataAccessException("分页查询方法参数必须且只能是一个IPageParameter接口的对象");
                }
                foreach (var item in parameters)
                {
                    _parameterList.Add(item.Name);
                }
                _isHashtableParameter = true;
            }
            else if (parameters.Length == 0 && this._isIPagedList)
            {
                throw new DataAccessException("分页查询方法参数必须且只能是一个IPageParameter接口的对象");
            }
            else if (parameters.Length == 1 && this._isIPagedList)
            {
                var parameterInterfaces = parameters[0].ParameterType.GetInterfaces();
                bool flag = false;
                foreach (var item in parameterInterfaces)
                {
                    if (item == typeof(IPageParameter))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new DataAccessException("分页查询方法参数必须且只能是一个IPageParameter接口的对象");
                }
            }
        }

        private bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }

        public object Execute(object[] Arguments)
        {
            object value = null;
            var parameter = WarpParameter(Arguments);
            switch (this._dataAccessType)
            {
                case DataAccessType.Insert:
                    value = this._context.Writer.Insert(this._statementName, parameter);
                    break;
                case DataAccessType.Delete:
                    value = this._context.Writer.Delete(this._statementName, parameter);
                    break;
                case DataAccessType.Update:
                    value = this._context.Writer.Update(this._statementName, parameter);
                    break;
                case DataAccessType.SelectReadOnly:
                case DataAccessType.SelectReadWrite:
                    if (this._isIPagedList)
                    {
                        if (this._context.HasDataAccessTypeContext)
                        {
                            value = QueryPageList(this._context.ContextMapper, parameter);
                        }
                        else if (this._dataAccessType == DataAccessType.SelectReadOnly)
                        {
                            value = QueryPageList(this._context.Reader, parameter);
                        }
                        else
                        {
                            value = QueryPageList(this._context.Writer, parameter);
                        }
                    }
                    else if (this._isSelectList)
                    {
                        if (this._context.HasDataAccessTypeContext)
                        {
                            value = this._context.ContextMapper.QueryForList(this._statementName, parameter);
                        }
                        else if (this._dataAccessType == DataAccessType.SelectReadOnly)
                        {
                            value = this._context.Reader.QueryForList(this._statementName, parameter);
                        }
                        else
                        {
                            value = this._context.Writer.QueryForList(this._statementName, parameter);
                        }
                    }
                    else
                    {
                        if (this._context.HasDataAccessTypeContext)
                        {
                            value = this._context.ContextMapper.QueryForObject(this._statementName, parameter);
                        }
                        else if (this._dataAccessType == DataAccessType.SelectReadOnly)
                        {
                            value = this._context.Reader.QueryForObject(this._statementName, parameter);
                        }
                        else
                        {
                            value = this._context.Writer.QueryForObject(this._statementName, parameter);
                        }
                        if (value == null && this._isNotCanNullReturnValue)
                        {
                            throw new DataAccessException("返回值是一个不可空的对象，但是返回值为空，请使用可空对象定义返回值");
                        }
                    }
                    break;
            }
            if (this._outTypeConfig != null)
            {
                bool isOut = false;
                if (this._outTypeConfig.IgnoreEnvironment)
                {
                    isOut = true;
                }
                else if (!AppInfo.DeployEnvInfo.IsFormal)
                {
                    isOut = true;
                }
                if (isOut)
                {
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
                        var statement = this._context.Writer.GetMappedStatement(this._statementName);
                        var scope = statement.Statement.Sql.GetRequestScope(statement, parameter, session);
                        statement.PreparedCommand.Create(scope, session, statement.Statement, parameter);
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
                    if (!string.IsNullOrEmpty(commandText))
                    {
                        if (this._outTypeConfig.OutType == OutSqlTextType.AutoLog)
                        {
                            _logger.Info(commandText);
                        }
                        else
                        {
                            Debug.WriteLine(commandText);
                            Console.WriteLine(commandText);
                        }
                    }
                }
            }
            return value;
        }

        private  string GetDbParameterValue(DbParameter parameter)
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

        private object QueryPageList(ISqlMapper mapper, object parameter)
        {
            object value = null;
            var list = mapper.QueryForList(this._statementName, parameter);
            IPageParameter pageParameter = (IPageParameter)parameter;
            if (pageParameter.RequireTotalCount)
            {
                int count = (int)mapper.QueryForObject(this._statementName + "_Count", parameter);
                value = Activator.CreateInstance(this._pagedListType, list, pageParameter.PageIndex, pageParameter.PageSize, count);
            }
            else
            {
                value = Activator.CreateInstance(this._pagedListType, list, pageParameter.PageIndex, pageParameter.PageSize, pageParameter.TotalCount);
            }
            return value;
        }

        private object WarpParameter(object[] args)
        {
            if (this._isHashtableParameter)
            {
                if (args.Length != this._parameterList.Count)
                {
                    throw new DataAccessException("参数匹配错误");
                }
                Hashtable ht = new Hashtable();
                for (int i = 0; i < args.Length; i++)
                {
                    ht.Add(this._parameterList[i], args[i]);
                }
                return ht;
            }
            return args.Length == 0 ? null : args[0];
        }
    }

}
