using Ik.Framework.Caching.CacheKeyManager;
using Ik.Framework.Configuration;
using IkCaching;
using IkCaching.Configuration;
using IkCaching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Framework.Caching
{
    public class MemcachedManager
    {
        public const string LogModelName_MemcachedService = "分布式缓存服务";
        private static ILog logger = LogManager.GetLogger(MemcachedManager.LogModelName_MemcachedService);
        private ICacheKeyFormat cacheKeyFormat = null;
        private static int defaultCacheMinutes = 60 * 24 * 28;

        public MemcachedManager()
        {
            this.cacheKeyFormat = DefaultKeyFormat.Instance;
        }

        public MemcachedManager(ICacheKeyFormat cacheKeyFormat)
        {
            this.cacheKeyFormat = cacheKeyFormat;
        }

        private string CheckKey(string key, params object[] values)
        {
            return this.cacheKeyFormat.FormatKey(key, values);
        }

        private static MemcachedClient _memClient;
        private static readonly object padlock = new object();

        /// <summary>
        /// 线程安全的单例模式
        /// </summary>
        public static MemcachedClient MemClient
        {
            get
            {
                if (_memClient == null)
                {
                    lock (padlock)
                    {
                        if (_memClient == null)
                        {
                            MemClientInit();
                        }
                    }
                }
                return _memClient;
            }
        }

        private static void MemClientInit()
        {
            logger.Info("开始初始化分布式缓存服务");
            var config = ConfigManager.Instance.Get<MemcachedConfig>(MemcachedConfigurationSectionHandler.ConfigSectionName);

            MemcachedClientConfiguration memConfig = new MemcachedClientConfiguration();
            if(config.Servers == null || config.Servers.Count == 0)
            {
                throw new CachingException("缓存服务器配置不能为空");
            }
            foreach (var item in config.Servers)
            {
                if (item.AddressType == AddressType.Host)
                {
                    var newAddress = Dns.GetHostEntry(item.Address).AddressList.First();
                    var ipEndPoint = new IPEndPoint(newAddress, item.Port);
                    memConfig.Servers.Add(ipEndPoint);
                }
                else if (item.AddressType == AddressType.Ip)
                {
                    var ipEndPoint = new IPEndPoint(IPAddress.Parse(item.Address), item.Port);
                    memConfig.Servers.Add(ipEndPoint);
                }
                else
                {
                    throw new CachingException("未知的地址类型");
                }
                logger.Info(string.Format("添加缓存服务器节点，地址：{0}，端口：{1}", item.Address, item.Port));
            }
            logger.Info(string.Format("已成功添加缓存服务器节点{0}个", config.Servers.Count));
            memConfig.Protocol = MemcachedProtocol.Binary;
            if (config.AuthParameters != null && config.AuthParameters.Count > 0)
            {
                memConfig.Authentication.Type = typeof(PlainTextAuthenticator);
                foreach (var item in config.AuthParameters)
                {
                    memConfig.Authentication.Parameters[item.Key] = item.Value;
                }
            }
            memConfig.SocketPool.MinPoolSize = config.MinPoolSize == 0 ? 2 : config.MinPoolSize;
            memConfig.SocketPool.MaxPoolSize = config.MaxPoolSize == 0 ? 200 : config.MaxPoolSize;
            _memClient = new LogMonitorMemcachedClient(memConfig);
            logger.Info("初始化分布式缓存服务完成");
        }

        #region 普通用法

        public bool Set(string key, object data, params object[] values)
        {
            return InternalSet(CheckKey(key, values), data, defaultCacheMinutes);
        }

        public bool Set(string key, object data, int cacheMinutes, params object[] values)
        {
            return InternalSet(CheckKey(key, values), data, cacheMinutes);

        }

        public bool Set(string key, object data, TimeSpan validFor, params object[] values)
        {
            return InternalSet(CheckKey(key, values), data, validFor);
        }

        internal bool InternalSet(string key, object data)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data.ToJsonString());
        }

        internal bool InternalSet(string key, object data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data.ToJsonString(), expireTime);
        }

        internal bool InternalSet(string key, object data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data.ToJsonString(), validFor);
        }

        internal bool InternalSet(string key, object data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data.ToJsonString(), TimeSpan.FromMinutes(cacheMinutes));
        }

        public bool IsSet(string key, params object[] values)
        {
            object result;
            return InternalIsSet<object>(CheckKey(key, values), out result);
        }

        public bool IsSet<T>(string key, out T result, params object[] values)
        {
            return InternalIsSet<T>(CheckKey(key, values), out result);
        }

        internal bool InternalIsSet<T>(string key, out T result)
        {
            result = default(T);
            object obj;
            var flag = MemClient.TryGet(key, out obj);
            if (flag)
            {
                result = obj.ToString().FromJsonString<T>();
            }
            return flag;
        }

        public bool Remove(string key, params object[] values)
        {
            return InternalRemove(CheckKey(key, values));
        }

        internal bool InternalRemove(string key)
        {
            return MemClient.Remove(key);
        }


        public void Clear()
        {
            MemClient.FlushAll();
        }


        public T Get<T>(string key, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values));
        }

        internal T InternalGet<T>(string key)
        {
            var objStr = MemClient.Get<string>(key);
            if (string.IsNullOrEmpty(objStr))
                return default(T);
            var obj = objStr.FromJsonString<T>();
            return obj;
        }


        public T Get<T>(string key, Func<T> acquire, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values), defaultCacheMinutes, acquire);
        }

        public T Get<T>(string key, int cacheMinutes, Func<T> acquire, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values), cacheMinutes, acquire);
        }

        public T Get<T>(string key, TimeSpan validFor, Func<T> acquire, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values), validFor, acquire);
        }

        internal T InternalGet<T>(string key, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsSet<T>(key, out result))
            {
                result = acquire();
                InternalSet(key, result);
            }
            return result;
        }

        internal T InternalGet<T>(string key, DateTime expireTime, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsSet<T>(key, out result))
            {
                result = acquire();
                InternalSet(key, result, expireTime);
            }
            return result;
        }

        internal T InternalGet<T>(string key, int cacheMinutes, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsSet<T>(key, out result))
            {
                result = acquire();
                InternalSet(key, result, cacheMinutes);
            }
            return result;
        }

        internal T InternalGet<T>(string key, TimeSpan validFor, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsSet<T>(key, out result))
            {
                result = acquire();
                InternalSet(key, result, validFor);
            }
            return result;
        }

        public StateValue<T> GetWithState<T>(string key, params object[] values)
        {
            return InternalGetWithState<T>(CheckKey(key, values));
        }

        internal StateValue<T> InternalGetWithState<T>(string key)
        {
            T result = default(T);
            if (InternalIsSet<T>(key, out result))
            {
                return new StateValue<T> { Result = result };
            }
            return null;
        }

        #endregion

        #region 原子操作

        public ulong Increment(string key, ulong defaultValue, ulong value = 1, params object[] values)
        {
            return MemClient.Increment(CheckKey(key, values), defaultValue, value, TimeSpan.FromMinutes(defaultCacheMinutes));
        }
        

        public ulong Increment(string key, ulong defaultValue, int cacheMinutes, ulong value = 1, params object[] values)
        {
            return MemClient.Increment(CheckKey(key, values), defaultValue, value, TimeSpan.FromMinutes(cacheMinutes));
        }

        public ulong Increment(string key, ulong defaultValue, TimeSpan validFor, ulong value = 1, params object[] values)
        {
            return MemClient.Increment(CheckKey(key, values), defaultValue, value, validFor);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong value = 1, params object[] values)
        {
            return MemClient.Decrement(CheckKey(key, values), defaultValue, value, TimeSpan.FromMinutes(defaultCacheMinutes));
        }
        

        public ulong Decrement(string key, ulong defaultValue, int cacheMinutes, ulong value = 1, params object[] values)
        {
            return MemClient.Decrement(CheckKey(key, values), defaultValue, value, TimeSpan.FromMinutes(cacheMinutes));
        }

        public ulong Decrement(string key, ulong defaultValue, TimeSpan validFor, ulong value = 1, params object[] values)
        {
            return MemClient.Decrement(CheckKey(key, values), defaultValue, value, validFor);
        }

        #endregion

        #region 并发控制相关

        public bool Add(string key, object data, params object[] values)
        {
            return InternalAdd(CheckKey(key, values), data, defaultCacheMinutes);
        }

        public bool Add(string key, object data, int cacheMinutes, params object[] values)
        {
            return InternalAdd(CheckKey(key, values), data, cacheMinutes);
        }

        public bool Add(string key, object data, TimeSpan validFor, params object[] values)
        {
            return InternalAdd(CheckKey(key, values), data, validFor);
        }
        

        internal bool InternalAdd(string key, object data)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data.ToJsonString());
        }

        internal bool InternalAdd(string key, object data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data.ToJsonString(), expireTime);
        }

        internal bool InternalAdd(string key, object data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data.ToJsonString(), TimeSpan.FromMinutes(cacheMinutes));
        }

        internal bool InternalAdd(string key, object data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data.ToJsonString(), validFor);
        }


        public CasValue<T> GetWithCas<T>(string key, params object[] values)
        {
            CasValue<T> result = null;
            var cas = MemClient.GetWithCas<string>(CheckKey(key, values));

            if (cas.StatusCode == 0 && cas.Result != null)
            {
                result = new CasValue<T>();
                result.Cas = cas.Cas;
                result.Result = cas.Result.FromJsonString<T>();
            }
            return result;
        }
        public CasValue<bool> SetWithCas(string key, object data, params object[] values)
        {
            CasValue<bool> result = new CasValue<bool>();
            if (data == null)
            {
                result.Result = false;
                return result;
            }
            var setCas = MemClient.Cas(StoreMode.Set, CheckKey(key, values), data.ToJsonString(), TimeSpan.FromMinutes(defaultCacheMinutes),0);
            if (setCas.StatusCode == 0)
            {
                result = new CasValue<bool>();
                result.Result = setCas.Result;
                result.Cas = setCas.Cas;
            }
            return result;
        }

        public CasValue<bool> SetWithCas(string key, object data, ulong cas, params object[] values)
        {
            CasValue<bool> result = new CasValue<bool>();
            if (data == null)
            {
                result.Result = false;
                return result;
            }
            var setCas = MemClient.Cas(StoreMode.Set, CheckKey(key, values), data.ToJsonString(), TimeSpan.FromMinutes(defaultCacheMinutes), cas);
            if (setCas.StatusCode == 0)
            {
                result = new CasValue<bool>();
                result.Result = setCas.Result;
                result.Cas = setCas.Cas;
            }
            return result;
        }

        public CasValue<bool> SetWithCas(string key, object data, TimeSpan validFor, ulong cas, params object[] values)
        {
            CasValue<bool> result = new CasValue<bool>();
            if (data == null)
            {
                result.Result = false;
                return result;
            }
            var setCas = MemClient.Cas(StoreMode.Set, CheckKey(key, values), data.ToJsonString(), validFor, cas);
            if (setCas.StatusCode == 0)
            {
                result = new CasValue<bool>();
                result.Result = setCas.Result;
                result.Cas = setCas.Cas;
            }
            return result;
        }

        /// <summary>
        /// 群集同步方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acion"></param>
        /// <returns></returns>
        public bool Synchronous(string key, Action acion, params object[] values)
        {
            return InternalRawSynchronous(CheckKey(key, values), acion, 10);
        }

        /// <summary>
        /// 群集同步方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acion"></param>
        /// <param name="lockSeconds"></param>
        /// <returns></returns>
        public bool Synchronous(string key, Action acion, int lockSeconds, params object[] values)
        {
            return InternalRawSynchronous(CheckKey(key, values), acion, lockSeconds);
        }

        /// <summary>
        /// 群集同步方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acion"></param>
        /// <param name="lockSeconds"></param>
        /// <returns></returns>
        internal bool InternalRawSynchronous(string key, Action acion, int lockSeconds)
        {
            if (InternalRawAdd(key, 1, TimeSpan.FromSeconds(lockSeconds)))
            {
                acion();
                return true;
            }
            return false;
        }

        #endregion

        #region 二进制序列化
        /// <summary>
        /// 检查缓存是否存在，存在则返回对象（原始对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawIsSet<T>(string key, out T result, params object[] values) where T : class
        {
            return InternalRawIsSet<T>(CheckKey(key, values), out result);
        }

        internal bool InternalRawIsSet<T>(string key, out T result) where T : class
        {
            result = default(T);
            object obj = null;
            var flag = false;
            try
            {
                flag = MemClient.TryGet(key, out obj);
            }
            catch
            {
                InternalRemove(key);
                flag = false;
            }
            if (flag)
            {
                result = obj as T;
                if (result == null)
                {
                    InternalRemove(key);
                    flag = false;
                }
            }
            return flag;
        }

        internal T InternalRawGet<T>(string key)
        {
            return MemClient.Get<T>(key);
        }

        /// <summary>
        /// 设置缓存（原始对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawSet<T>(string key, T data, params object[] values)
        {
            return InternalRawSet<T>(CheckKey(key, values), data, defaultCacheMinutes);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheMinutes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawSet<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalRawSet<T>(CheckKey(key, values), data, cacheMinutes);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheMinutes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawSet<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalRawSet<T>(CheckKey(key, values), data, validFor);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expireTime"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawSet<T>(string key, T data, DateTime expireTime, params object[] values)
        {
            return InternalRawSet<T>(CheckKey(key, values), data, expireTime);
        }


        internal bool InternalRawSet<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data);
        }

        internal bool InternalRawSet<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data, expireTime);
        }

        internal bool InternalRawSet<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data, TimeSpan.FromMinutes(cacheMinutes));
        }

        internal bool InternalRawSet<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Set, key, data, validFor);
        }


        public bool RawAdd<T>(string key, T data, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, defaultCacheMinutes);
        }

        public bool RawAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, cacheMinutes);
        }

        public bool RawAdd<T>(string key, T data, DateTime expireTime, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, expireTime);
        }

        public bool RawAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, validFor);
        }

        internal bool InternalRawAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data);
        }

        internal bool InternalRawAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data, expireTime);
        }

        internal bool InternalRawAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data, TimeSpan.FromMinutes(cacheMinutes));
        }

        internal bool InternalRawAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            return MemClient.Store(StoreMode.Add, key, data, validFor);
        }

        #endregion
    }
}
