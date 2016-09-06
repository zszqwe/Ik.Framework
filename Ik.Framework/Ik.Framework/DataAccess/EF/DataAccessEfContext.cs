using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Ik.Framework.DataAccess.EF
{
    public class DataAccessEfContext
    {
        private const string _contextName = "_EF_LOCAL_CONTEXT_{0}";
        private const string _defaultContextName = "_EF_DEFAULT_CONTEXT_{0}";
        private string _sessionName = null;
        private string _defaultSessionName = null;

        public DataAccessEfContext()
        {
            this._sessionName = string.Format(_contextName, this.GetHashCode());
            this._defaultSessionName = string.Format(_defaultContextName, this.GetHashCode());
            
        }

        public EfReaderAndWriterContext DefaultContext
        {
            get
            {
                EfReaderAndWriterContext context = null;
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    context = CallContext.GetData(_defaultContextName) as EfReaderAndWriterContext;
                }
                else
                {
                    context = currentContext.Items[_defaultContextName] as EfReaderAndWriterContext;
                }

                if (context == null)
                {
                    var contextTemp = new EfReaderAndWriterContext();
                    if (!string.IsNullOrEmpty(this.ReadConnectionString))
                    {
                        contextTemp.Reader = new IkObjectContext(this.ReadConnectionString);
                    }
                    contextTemp.Writer = new IkObjectContext(this.WriteConnectionString);
                    contextTemp.ContextType = DataAccessContextType.Normal;
                    if (currentContext == null)
                    {
                        CallContext.SetData(_defaultContextName, contextTemp);
                    }
                    else
                    {
                        currentContext.Items[_defaultContextName] = contextTemp;
                    }
                    context = contextTemp;
                }
                return context;
            }
        }

        public void EndDefaultContext()
        {
            EfReaderAndWriterContext context = null;
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                context = CallContext.GetData(_defaultContextName) as EfReaderAndWriterContext;
            }
            else
            {
                context = currentContext.Items[_defaultContextName] as EfReaderAndWriterContext;
            }
            if (context != null)
            {
                if (context.Reader != null)
                {
                    context.Reader.Dispose();
                }
                context.Writer.Dispose();
            }
            if (currentContext == null)
            {
                CallContext.SetData(_defaultContextName, null);
            }
            else
            {
                currentContext.Items.Remove(_defaultContextName);
            }
        }

        public string WriteConnectionString { get; set; }

        public string ReadConnectionString { get; set; }

        public bool HasDataAccessTypeContext
        {
            get
            {
                return DbContext != null;
            }
        }

        public EfReaderAndWriterContext DbContext
        {
            get
            {
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    return CallContext.GetData(_sessionName) as EfReaderAndWriterContext;
                }
                return currentContext.Items[_sessionName] as EfReaderAndWriterContext;
            }
        }

        public void StartNormalContext()
        {
            if (this.HasDataAccessTypeContext)
            {
                throw new DataAccessException("不能重复开启数据访问上下文");
            }
            var context = new EfReaderAndWriterContext();
            if (!string.IsNullOrEmpty(this.ReadConnectionString))
            {
                context.Reader = new IkObjectContext(this.ReadConnectionString);
            }
            context.Writer = new IkObjectContext(this.WriteConnectionString);
            context.ContextType = DataAccessContextType.Normal;
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                CallContext.SetData(_sessionName, context);
            }
            else
            {
                currentContext.Items[_sessionName] = context;
            }
        }

        public void StartReadWriteContext()
        {

            if (this.HasDataAccessTypeContext)
            {
                throw new DataAccessException("不能重复开启数据访问上下文");
            }
             var context = new EfReaderAndWriterContext();
             if (!string.IsNullOrEmpty(this.ReadConnectionString))
             {
                 context.Reader = new IkObjectContext(this.ReadConnectionString);
             }
             context.Writer = new IkObjectContext(this.WriteConnectionString);
             context.ContextType = DataAccessContextType.ReadWrite;
            HttpContext currentContext = HttpContext.Current;
            if (currentContext == null)
            {
                CallContext.SetData(_sessionName, context);
            }
            else
            {
                currentContext.Items[_sessionName] = context;
            }
        }

        public void EndContext()
        {
            if (HasDataAccessTypeContext)
            {
                if (DbContext.Reader != null)
                {
                    DbContext.Reader.Dispose();
                }
                DbContext.Writer.Dispose();
            }
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

    public class EfReaderAndWriterContext : IDisposable
    {
        private ConcurrentDictionary<string, object> _writerSetList = new ConcurrentDictionary<string, object>();
        private ConcurrentDictionary<string, object> _readerSetList = new ConcurrentDictionary<string, object>();



        public DataAccessContextType ContextType { get; set; }

        public IDbContext Reader { get; set; }

        public IDbContext Writer { get; set; }

        public IDbSet<T> GetWriterSet<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            var value = _writerSetList.GetOrAdd(type.AssemblyQualifiedName, (key) => Writer.Set<T>());
            return (IDbSet<T>)value;
        }

        public IDbSet<T> GetReaderSet<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            var value = _readerSetList.GetOrAdd(type.AssemblyQualifiedName, (key) => Reader.Set<T>());
            return (IDbSet<T>)value;
        }
        public void Dispose()
        {
            if (Reader != null)
            {
                Reader.Dispose();
            }
            if (Writer != null)
            {
                Writer.Dispose();
            }
        }

        ~EfReaderAndWriterContext()
        {
            Dispose();
        }
    }


    public class ReadWriteContextScope : DataAccessContextScope
    {
        public ReadWriteContextScope(DataAccessEfContext context) : base(context) { }



        protected override DataAccessContextType ContextType
        {
            get { return DataAccessContextType.ReadWrite; }
        }
    }

    public class NormalContextScope : DataAccessContextScope
    {
        public NormalContextScope(DataAccessEfContext context) : base(context) { }



        protected override DataAccessContextType ContextType
        {
            get { return DataAccessContextType.Normal; }
        }
    }

   


    public abstract class DataAccessContextScope : IDisposable
    {
        protected DataAccessEfContext _context = null;

        public DataAccessContextScope(DataAccessEfContext context)
        {
            this._context = context;
            if (ContextType == DataAccessContextType.ReadWrite)
            {
                this._context.StartReadWriteContext();
            }
            else if (ContextType == DataAccessContextType.Normal)
            {
                this._context.StartNormalContext();
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

        public int SaveChanges()
        {
            return this._context.DbContext.Writer.SaveChanges();
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
