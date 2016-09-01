using IkCastle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.DataMapping
{
    internal class DataAccess : IInterceptor
    {
        private DataMappingContext _context = null;
        private ConcurrentDictionary<string, DataAccessContext> _dataAccessList = new ConcurrentDictionary<string, DataAccessContext>();
        private DataAccessOutSqlTextConfig _outTypeConfig = null;

        public DataAccess(DataMappingContext context, DataAccessOutSqlTextConfig outTypeConfig)
        {
            this._context = context;
            this._outTypeConfig = outTypeConfig;
        }

        public void Intercept(IInvocation invocation)
        {
            var key = invocation.Method.DeclaringType.Assembly.FullName + "." + invocation.Method.Name;
            var context = this._dataAccessList.GetOrAdd(key, k => new DataAccessContext(this._context, invocation.Method, this._outTypeConfig));
            invocation.ReturnValue = context.Execute(invocation.Arguments);
        }
    }
}
