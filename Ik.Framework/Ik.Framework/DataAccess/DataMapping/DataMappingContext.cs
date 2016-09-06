using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ik.Framework.DataAccess.DataMapping
{
    public class DataMappingContext
    {
        private const string _contextName = "_IBATIS_LOCAL_CONTEXT_{0}";
        private string _sessionName = null;

        public DataMappingContext()
        {
            this._sessionName = string.Format(_contextName, this.GetHashCode());
        }

        public ISqlMapper Writer { set; get; }

        public string WriteConnectionString { get; set; }

        public ISqlMapper Reader { get; set; }

        public string ReadConnectionString { get; set; }

        public DataAccessOutSqlTextConfig OutTypeConfig { get; set; }

        public bool HasDataAccessTypeContext
        {
            get
            {
                return ContextMapper != null;
            }
        }

        public ISqlMapper ContextMapper
        {
            get
            {
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    return CallContext.GetData(_sessionName) as ISqlMapper;
                }
                return currentContext.Items[_sessionName] as ISqlMapper;
            }
        }

        public void StartReadOnlyContext()
        {

            if (this.HasDataAccessTypeContext)
            {
                throw new DataAccessException("不能重复开启数据访问上下文");
            }
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                CallContext.SetData(_sessionName, this.Reader);
            }
            else
            {
                currentContext.Items[_sessionName] = this.Reader;
            }
        }

        public void StartReadWriteContext()
        {

            if (this.HasDataAccessTypeContext)
            {
                throw new DataAccessException("不能重复开启数据访问上下文");
            }
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                CallContext.SetData(_sessionName, this.Writer);
            }
            else
            {
                currentContext.Items[_sessionName] = this.Writer;
            }
        }

        public void EndContext()
        {
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                CallContext.SetData(_sessionName, null);
            }
            else
            {
                currentContext.Items.Remove(_sessionName);
            }
        }
    }

    public class ReadOnlyContextScope : DataAccessContextScope
    {
        public ReadOnlyContextScope(DataMappingContext context) : base(context) { 
            
        }


        protected override DataAccessContextType ContextType
        {
            get { return DataAccessContextType.Normal; }
        }
    }


    public class ReadWriteContextScope : DataAccessContextScope
    {
        public ReadWriteContextScope(DataMappingContext context) : base(context) { }



        protected override DataAccessContextType ContextType
        {
            get { return DataAccessContextType.ReadWrite; }
        }
    }


    public abstract class DataAccessContextScope : IDisposable
    {
        protected DataMappingContext _context = null;

        public DataAccessContextScope(DataMappingContext context)
        {
            this._context = context;
            if (ContextType == DataAccessContextType.Normal)
            {
                this._context.StartReadOnlyContext();
            }
            else if (ContextType == DataAccessContextType.ReadWrite)
            {
                this._context.StartReadWriteContext();
            }
            else
            {
                throw new DataAccessException("不支持的上下文参数");
            }
        }

        protected abstract DataAccessContextType ContextType
        {
            get;
        }

        public void Dispose()
        {
            this._context.EndContext();
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
