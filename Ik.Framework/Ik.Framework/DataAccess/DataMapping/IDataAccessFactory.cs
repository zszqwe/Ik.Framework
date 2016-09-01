using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.DataMapping
{
    public interface IDataAccessFactory
    {
        /// <summary>
        /// 获取默认数据访问接口
        /// </summary>
        /// <param name="daKey">数据访问接口Key</param>
        /// <returns>接口实现</returns>
        T GetDataAccess<T>();

        object GetDataAccess(Type dataAccessType);

        /// <summary>
        /// 打开事物
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 打开事物
        /// </summary>
        /// <param name="isolationLevel">事物级别</param>
        void BeginTransaction(System.Data.IsolationLevel isolationLevel);

        /// <summary>
        /// 提交事物
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 回滚事物
        /// </summary>
        void RollBackTransaction();

        DataMappingTransactionScope CreateTransactionScope();

        DataMappingTransactionScope CreateTransactionScope(System.Data.IsolationLevel isolationLevel);

        void SqlBulkCopyInsert(DataTable dt);

        void SqlBulkCopyInsert(DataTable dt, IEnumerable<SqlBulkCopyColumnMapping> mapping);

        DataAccessContextScope CreateReadOnlyContextScope();
        DataAccessContextScope CreateReadWriteContextScope();

        void BeginReadOnlyContext();

        void BeginReadWriteContex();

        void EndContext();

        string GetRunString<T>(string methodName, object parameters);
    }
}
