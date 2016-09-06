using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;

namespace Ik.Framework.DataAccess.EF
{
    internal partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private readonly DataAccessEfContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(DataAccessEfContext context)
        {
            this._context = context;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(object id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189

            if (this._context.HasDataAccessTypeContext)
            {
                if (this._context.DbContext.ContextType == DataAccessContextType.ReadWrite)
                {
                    return this._context.DbContext.GetWriterSet<T>().Find(id);
                }
                if (this._context.DbContext.Reader != null)
                {
                    return this._context.DbContext.GetReaderSet<T>().Find(id);
                }
                return this._context.DbContext.GetWriterSet<T>().Find(id);
            }
            else
            {
                return this._context.DefaultContext.GetReaderSet<T>().Find(id);
            }
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (this._context.HasDataAccessTypeContext)
                {
                    var set = this._context.DbContext.GetWriterSet<T>();
                    set.Add(entity);
                }
                else
                {
                    var set = this._context.DefaultContext.GetWriterSet<T>();
                    set.Add(entity);
                    this._context.DefaultContext.Writer.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                if (this._context.HasDataAccessTypeContext)
                {
                    var set = this._context.DbContext.GetWriterSet<T>();
                    foreach (var entity in entities)
                    {
                        set.Add(entity);
                    }
                }
                else
                {
                    var set = this._context.DefaultContext.GetWriterSet<T>();
                    foreach (var entity in entities)
                    {
                        set.Add(entity);
                    }
                    this._context.DefaultContext.Writer.SaveChanges();
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                if (!this._context.HasDataAccessTypeContext)
                {
                    this._context.DefaultContext.Writer.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }
        private static object deleteLockObj = new object();
        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (this._context.HasDataAccessTypeContext)
                {
                    var set = this._context.DbContext.GetWriterSet<T>();
                    var entry = this._context.DbContext.Writer.Entry<T>(entity);
                    if (entry.State == EntityState.Detached)
                        set.Attach(entity);
                    set.Remove(entity);
                }
                else
                {
                    lock (deleteLockObj)
                    {
                        var set = this._context.DefaultContext.GetWriterSet<T>();
                        set.Remove(entity);
                        this._context.DefaultContext.Writer.SaveChanges();
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        private static object deleteAllLockObj = new object();
        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");
                if (this._context.HasDataAccessTypeContext)
                {
                    var set = this._context.DbContext.GetWriterSet<T>();
                    foreach (var entity in entities)
                    {
                        set.Remove(entity);
                    }
                }
                else
                {
                    lock (deleteAllLockObj)
                    {
                        var set = this._context.DefaultContext.GetWriterSet<T>();
                        foreach (var entity in entities)
                        {
                            set.Remove(entity);
                        }
                        this._context.DefaultContext.Writer.SaveChanges();
                    }
                }
                
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                if (this._context.HasDataAccessTypeContext)
                {
                    if (this._context.DbContext.ContextType == DataAccessContextType.ReadWrite)
                    {
                        return this._context.DbContext.GetWriterSet<T>();
                    }
                    if (this._context.DbContext.Reader != null)
                    {
                        return this._context.DbContext.GetReaderSet<T>();
                    }
                    return this._context.DbContext.GetWriterSet<T>();
                }
                if (this._context.DefaultContext.Reader != null)
                {
                    return this._context.DefaultContext.GetReaderSet<T>();
                }
                return this._context.DefaultContext.GetWriterSet<T>();
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {

                if (this._context.HasDataAccessTypeContext)
                {
                    if (this._context.DbContext.ContextType == DataAccessContextType.ReadWrite)
                    {
                        return this._context.DbContext.GetWriterSet<T>().AsNoTracking();
                    }
                    if (this._context.DbContext.Reader != null)
                    {
                        return this._context.DbContext.GetReaderSet<T>().AsNoTracking();
                    }
                    return this._context.DbContext.GetWriterSet<T>().AsNoTracking();
                }
                if (this._context.DefaultContext.Reader != null)
                {
                    return this._context.DefaultContext.GetReaderSet<T>().AsNoTracking();
                }
                return this._context.DefaultContext.GetWriterSet<T>().AsNoTracking();
            }
        }

        #endregion
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
