using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.DataMapping
{
    internal class DataMappingTransaction : DataMappingTransactionScope
    {
        private IDataAccessFactory _accessFactory = null;
        public DataMappingTransaction(IDataAccessFactory accessFactory)
        {
            this._accessFactory = accessFactory;
            this._accessFactory.BeginTransaction();
        }

        public DataMappingTransaction(IDataAccessFactory accessFactory, System.Data.IsolationLevel isolationLevel)
        {
            this._accessFactory = accessFactory;
            this._accessFactory.BeginTransaction(isolationLevel);
        }

        public override void Complete()
        {
            this._accessFactory.CommitTransaction();
        }

        public override void Rollback()
        {
            this._accessFactory.RollBackTransaction();
        }
    }
}
