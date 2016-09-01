using IkCaching;
using IkCaching.Configuration;
using IkCaching.Memcached;
using IkCaching.Memcached.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching
{
    internal class LogMonitorMemcachedClient: MemcachedClient
    {
        Ik.Framework.Logging.ILog logger = Ik.Framework.Logging.LogManager.GetLogger("缓存管理");

        public LogMonitorMemcachedClient(MemcachedClientConfiguration memConfig)
            : base(memConfig)
        {
            
        }
        protected override IStoreOperationResult PerformStore(StoreMode mode, string key, object value, uint expires, ref ulong cas, out int statusCode)
        {
            ulong tmp = cas;
            var result = base.PerformStore(mode, key, value, expires, ref cas, out statusCode);
            if (!result.Success)
            {
                StringBuilder message = new StringBuilder();
                StringBuilder errorMessage = new StringBuilder();
                IOperationResult nextResult = result;
                while (nextResult != null)
                {
                    message.Append(nextResult.Message).AppendLine();
                    if (nextResult.Exception != null)
                    {
                        errorMessage.Append(nextResult.Exception.ToString()).AppendLine();
                    }
                    nextResult = nextResult.InnerResult;
                }
                if (errorMessage.Length > 0)
                {
                    logger.Error(string.Format("缓存设置失败，key:{0}，方式：{1}，错误信息：{2}", key, mode.ToString(), message.ToString()), statusCode, errorMessage.ToString());
                }
                else if (mode != StoreMode.Add && tmp == 0)
                {
                    logger.Error(string.Format("缓存设置失败，key:{0}，方式：{1}，错误信息：{2}", key, mode.ToString(), message.ToString()), statusCode, errorMessage.ToString());
                }
            }
            return result;
        }
    }
}
