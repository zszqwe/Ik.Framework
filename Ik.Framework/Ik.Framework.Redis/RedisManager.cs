using Ik.Framework.Caching.CacheKeyManager;
using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using Ik.Framework.Caching;
using Ik.Framework.Common.Serialization;

namespace Ik.Framework.Redis
{
    public class RedisManager
    {
        public const string LogModelName_RedisService = "分布式缓存服务(Redis)";
        private static ILog logger = LogManager.GetLogger(RedisManager.LogModelName_RedisService);
        private ICacheKeyFormat cacheKeyFormat = null;
        private static int defaultCacheMinutes = 60 * 24 * 28;
        private static ConnectionMultiplexer _redis = null;
        private static object _locker = new object();
        private static string _defaultConnectionString = null;

        public RedisManager()
        {
            this.cacheKeyFormat = DefaultKeyFormat.Instance;
        }

        public RedisManager(ICacheKeyFormat cacheKeyFormat)
        {
            this.cacheKeyFormat = cacheKeyFormat;
        }

        private string CheckKey(string key, params object[] values)
        {
            return this.cacheKeyFormat.FormatKey(key, values);
        }

        private static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;

                        _redis = GetManager();
                        return _redis;
                    }
                }

                return _redis;
            }
        }

        public static string DefaultConnectionString
        {
            get
            {
                return _defaultConnectionString;
            }
            set
            {
                _defaultConnectionString = value;
            }
        }

        private static ConnectionMultiplexer GetManager()
        {
            if (string.IsNullOrEmpty(_defaultConnectionString))
            {
                _defaultConnectionString = ConfigManager.Instance.Get<string>("redis_connection");
            }
            return ConnectionMultiplexer.Connect(_defaultConnectionString);
        }

        private string JsonSerialize(object value)
        {
            return JsonSerializerManager.JsonSerializer(value);
        }

        private T JsonDeserialize<T>(string data)
        {
            return JsonSerializerManager.JsonDeserialize<T>(data);
        }

        private byte[] ByteSerialize(object value)
        {
            return BinarySerializerManager.BinarySerialize(value);
        }

        private object ByteDeserialize(byte[] data)
        {
            return BinarySerializerManager.BinaryDeSerialize(data);
        }

        #region 公共

        public bool Persist(string key, params object[] values)
        {
            return InternalPersist(CheckKey(key, values));
        }

        internal bool InternalPersist(string key)
        {
            var db = Manager.GetDatabase();
            return db.KeyDelete(key);
        }

        public bool Remove(string key, params object[] values)
        {
            return InternalRemove(CheckKey(key, values));
        }

        internal bool InternalRemove(string key)
        {
            var db = Manager.GetDatabase();
            return db.KeyDelete(key);
        }

        public bool Exists(string key, params object[] values)
        {
            return InternalExists(CheckKey(key, values));
        }

        internal bool InternalExists(string key)
        {
            var db = Manager.GetDatabase();
            return db.KeyExists(key);
        }

        #endregion

        #region 普通用法

        public bool Add(string key, object data, params object[] values)
        {
            return InternalAdd(CheckKey(key, values), data);
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
            var db = Manager.GetDatabase();
            var flag = db.StringSet(key, JsonSerialize(data));
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalAdd(string key, object data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, JsonSerialize(data), expireTime - DateTime.Now);
        }

        internal bool InternalAdd(string key, object data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, JsonSerialize(data), validFor);
        }

        internal bool InternalAdd(string key, object data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, JsonSerialize(data), TimeSpan.FromMinutes(cacheMinutes));
        }

        public bool IsAdd(string key, params object[] values)
        {
            object result;
            return InternalIsAdd<object>(CheckKey(key, values), out result);
        }

        public bool IsAdd<T>(string key, out T result, params object[] values)
        {
            return InternalIsAdd<T>(CheckKey(key, values), out result);
        }

        internal bool InternalIsAdd<T>(string key, out T result)
        {
            result = default(T);
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            var cResult = tran.AddCondition(Condition.KeyExists(key));
            var taskResult = tran.StringGetAsync(key);
            tran.Execute();
            if (cResult.WasSatisfied)
            {
                var value = (string)taskResult.Result;
                result = JsonDeserialize<T>(value);
                return true;
            }
            return false;
        }


        public T Get<T>(string key, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values));
        }

        internal T InternalGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var value = (string)db.StringGet(key);
            if (string.IsNullOrEmpty(value))
                return default(T);
            return JsonDeserialize<T>(value);
        }


        public T Get<T>(string key, Func<T> acquire, params object[] values)
        {
            return InternalGet<T>(CheckKey(key, values), acquire);
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
            if (!InternalIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalAdd(key, result);
            }
            return result;
        }

        internal T InternalGet<T>(string key, DateTime expireTime, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalAdd(key, result, expireTime);
            }
            return result;
        }

        internal T InternalGet<T>(string key, int cacheMinutes, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalAdd(key, result, cacheMinutes);
            }
            return result;
        }

        internal T InternalGet<T>(string key, TimeSpan validFor, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalAdd(key, result, validFor);
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
            if (InternalIsAdd<T>(key, out result))
            {
                return new StateValue<T> { Result = result };
            }
            return null;
        }

        public string GetOriginalValue(string key, params object[] values)
        {
            var db = Manager.GetDatabase();
            return (string)db.StringGet(CheckKey(key, values));
        }

        public bool OriginalValueAdd(string key, string dataJsonString, params object[] values)
        {
            if (string.IsNullOrEmpty(dataJsonString))
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.StringSet(CheckKey(key, values), dataJsonString);
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        #region 二进制序列化



        public bool RawIsAdd(string key, params object[] values)
        {
            object result;
            return InternalRawIsAdd<object>(CheckKey(key, values), out result);
        }

        /// <summary>
        /// 检查缓存是否存在，存在则返回对象（原始对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawIsAdd<T>(string key, out T result, params object[] values)
        {
            return InternalRawIsAdd<T>(CheckKey(key, values), out result);
        }

        internal bool InternalRawIsAdd<T>(string key, out T result)
        {
            result = default(T);
            object obj = null;
            var flag = false;
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            var cResult = tran.AddCondition(Condition.KeyExists(key));
            var taskResult = tran.StringGetAsync(key);
            tran.Execute();
            try
            {
                if (cResult.WasSatisfied)
                {
                    var data = (byte[])taskResult.Result;
                    obj = ByteDeserialize(data);
                }
            }
            catch
            {
                InternalRemove(key);
                flag = false;
            }
            if (flag)
            {
                if (obj is T)
                {
                    result = (T)obj;
                }
                else
                {
                    InternalRemove(key);
                    flag = false;
                }
            }
            return flag;
        }

        public T RawGet<T>(string key)
        {
            T result;
            return InternalRawIsAdd<T>(key, out result) ? result : default(T);
        }

        /// <summary>
        /// 设置缓存（原始对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawAdd<T>(string key, T data, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheMinutes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, cacheMinutes);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheMinutes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, validFor);
        }

        /// <summary>
        /// 设置缓存值，按照原始对象设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expireTime"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool RawAdd<T>(string key, T data, DateTime expireTime, params object[] values)
        {
            return InternalRawAdd<T>(CheckKey(key, values), data, expireTime);
        }


        internal bool InternalRawAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.StringSet(key, ByteSerialize(data));

            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, ByteSerialize(data), expireTime - DateTime.Now);
        }

        internal bool InternalRawAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, ByteSerialize(data), TimeSpan.FromMinutes(cacheMinutes));
        }

        internal bool InternalRawAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.StringSet(key, ByteSerialize(data), validFor);
        }

        public T RawGet<T>(string key, Func<T> acquire, params object[] values)
        {
            return InternalRawGet<T>(CheckKey(key, values), acquire);
        }

        public T RawGet<T>(string key, int cacheMinutes, Func<T> acquire, params object[] values)
        {
            return InternalRawGet<T>(CheckKey(key, values), cacheMinutes, acquire);
        }

        public T RawGet<T>(string key, TimeSpan validFor, Func<T> acquire, params object[] values)
        {
            return InternalRawGet<T>(CheckKey(key, values), validFor, acquire);
        }

        internal T InternalRawGet<T>(string key, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalRawIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalRawAdd(key, result);
            }
            return result;
        }

        internal T InternalRawGet<T>(string key, DateTime expireTime, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalRawIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalRawAdd(key, result, expireTime);
            }
            return result;
        }

        internal T InternalRawGet<T>(string key, int cacheMinutes, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalRawIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalRawAdd(key, result, cacheMinutes);
            }
            return result;
        }

        internal T InternalRawGet<T>(string key, TimeSpan validFor, Func<T> acquire)
        {
            T result = default(T);
            if (!InternalRawIsAdd<T>(key, out result))
            {
                result = acquire();
                InternalRawAdd(key, result, validFor);
            }
            return result;
        }

        public StateValue<T> RawGetWithState<T>(string key, params object[] values)
        {
            return InternalRawGetWithState<T>(CheckKey(key, values));
        }

        internal StateValue<T> InternalRawGetWithState<T>(string key)
        {
            T result = default(T);
            if (InternalRawIsAdd<T>(key, out result))
            {
                return new StateValue<T> { Result = result };
            }
            return null;
        }

        public byte[] RawGetOriginalValue(string key, params object[] values)
        {
            var db = Manager.GetDatabase();
            return (byte[])db.StringGet(CheckKey(key, values));
        }

        public bool RawOriginalValueAdd(string key, byte[] dataBytes, params object[] values)
        {
            if (dataBytes == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.StringSet(CheckKey(key, values), dataBytes);
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        #endregion

        #endregion

        #region List操作

        public bool ListCreateAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalListCreateAdd(CheckKey(key, values), dataList);
        }

        public bool ListCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalListCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool ListCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalListCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalListCreateAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.Execute();

            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalListCreateAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalListCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalListCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0; ;
        }



        public bool ListPushAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), dataList);
        }

        public bool ListPushAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool ListPushAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalListPushAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;

            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool ListPushAdd<T>(string key, T data, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), data);
        }

        public bool ListPushAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool ListPushAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalListPushAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalListPushAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)JsonSerialize(data)) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)JsonSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)JsonSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalListPushAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)JsonSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }


        public IEnumerable<T> ListGet<T>(string key, params object[] values)
        {
            return InternalListGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<T> InternalListGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.ListRange(key);
            return list.Select(v => JsonDeserialize<T>((string)v)).ToList();
        }

        public IEnumerable<T> ListGetRange<T>(string key, long start, long end, params object[] values)
        {
            return InternalListGetRange<T>(CheckKey(key, values), start, end);
        }

        internal IEnumerable<T> InternalListGetRange<T>(string key, long start, long end)
        {
            var db = Manager.GetDatabase();
            var list = db.ListRange(key, start, end);
            return list.Select(v => JsonDeserialize<T>((string)v)).ToList();
        }

        public T ListPopGet<T>(string key, params object[] values)
        {
            return InternalListPopGet<T>(CheckKey(key, values));
        }

        internal T InternalListPopGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var value = (string)db.ListRightPop(key);
            return JsonDeserialize<T>(value);
        }

        public T ListGetByIndex<T>(string key, long index, params object[] values)
        {
            return InternalListGetByIndex<T>(CheckKey(key, values), index);
        }

        internal T InternalListGetByIndex<T>(string key, long index)
        {
            var db = Manager.GetDatabase();
            var value = (string)db.ListGetByIndex(key, index);
            return JsonDeserialize<T>(value);
        }

        public bool ListSetByIndex<T>(string key, long index, T data, params object[] values)
        {
            return InternalListSetByIndex<T>(CheckKey(key, values), index, data);
        }

        internal bool InternalListSetByIndex<T>(string key, long index, T data)
        {
            var db = Manager.GetDatabase();
            db.ListSetByIndex(key, index, JsonSerialize(data));
            return true;
        }

        public bool ListRemove<T>(string key, T data, params object[] values)
        {
            return InternalListRemove(CheckKey(key, values), data);
        }

        internal bool InternalListRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.ListRemove(key, JsonSerialize(data)) > 0;
        }

        #region 二进制序列化
        public bool RawListCreateAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawListCreateAdd(CheckKey(key, values), dataList);
        }

        public bool RawListCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawListCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawListCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawListCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawListCreateAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.Execute();
            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawListCreateAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawListCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawListCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.ListRightPushAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0;
        }



        public bool RawListPushAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), dataList);
        }

        public bool RawListPushAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawListPushAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawListPushAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool RawListPushAdd<T>(string key, T data, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), data);
        }

        public bool RawListPushAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool RawListPushAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalRawListPushAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalRawListPushAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)ByteSerialize(data)) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)ByteSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)ByteSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawListPushAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.ListRightPush(key, (RedisValue)ByteSerialize(data)) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }


        public IEnumerable<T> RawListGet<T>(string key, params object[] values)
        {
            return InternalRawListGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<T> InternalRawListGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var length = db.ListLength(key);
            var list = db.ListRange(key);
            return list.Select(v => (T)ByteDeserialize((byte[])v)).ToList();
        }

        public T RawListPopGet<T>(string key, params object[] values)
        {
            return InternalRawListPopGet<T>(CheckKey(key, values));
        }

        internal T InternalRawListPopGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var value = (byte[])db.ListRightPop(key);
            return (T)ByteDeserialize(value);
        }

        public T RawListGetByIndex<T>(string key, long index, params object[] values)
        {
            return InternalRawListGetByIndex<T>(CheckKey(key, values), index);
        }

        internal T InternalRawListGetByIndex<T>(string key, long index)
        {
            var db = Manager.GetDatabase();
            var value = (byte[])db.ListGetByIndex(key, index);
            return (T)ByteDeserialize(value);
        }

        public bool RawListSetByIndex<T>(string key, long index, T data, params object[] values)
        {
            return InternalListSetByIndex<T>(CheckKey(key, values), index, data);
        }

        internal bool InternalRawListSetByIndex<T>(string key, long index, T data)
        {
            var db = Manager.GetDatabase();
            db.ListSetByIndex(key, index, ByteSerialize(data));
            return true;
        }

        public bool RawListRemove<T>(string key, T data, params object[] values)
        {
            return InternalRawListRemove(CheckKey(key, values), data);
        }

        internal bool InternalRawListRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.ListRemove(key, ByteSerialize(data)) > 0;
        }
        #endregion
        #endregion

        #region Set 操作
        public bool SetCreateAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalSetCreateAdd(CheckKey(key, values), dataList);
        }

        public bool SetCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalSetCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool SetCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalSetCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalSetCreateAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.Execute();
            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSetCreateAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalSetCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalSetCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0;
        }

        public bool SetAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), dataList);
        }

        public bool SetAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool SetAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalSetAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)JsonSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool SetAdd<T>(string key, T data, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), data);
        }

        public bool SetAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool SetAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalSetAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalSetAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)JsonSerialize(data));
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)JsonSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)JsonSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalSetAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)JsonSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<T> SetGet<T>(string key, params object[] values)
        {
            return InternalSetGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<T> InternalSetGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.SetMembers(key);
            return list.Select(v => JsonDeserialize<T>(v)).ToList();
        }

        public T SetPopGet<T>(string key, params object[] values)
        {
            return InternalSetPopGet<T>(CheckKey(key, values));
        }

        internal T InternalSetPopGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var value = (string)db.SetPop(key);
            return JsonDeserialize<T>(value);
        }

        public bool SetRemove<T>(string key, T data, params object[] values)
        {
            return InternalSetRemove<T>(CheckKey(key, values), data);
        }

        internal bool InternalSetRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SetRemove(key, JsonSerialize(data));
        }

        public bool SetRemove<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalSetRemove<T>(CheckKey(key, values), dataList);
        }

        internal bool InternalSetRemove<T>(string key, IEnumerable<T> dataList)
        {
            var db = Manager.GetDatabase();
            return db.SetRemove(key, dataList.Select(v => (RedisValue)v.ToJsonString()).ToArray()) > 0;
        }

        public bool SetContains<T>(string key, T data, params object[] values)
        {
            return InternalSetContains<T>(CheckKey(key, values), data);
        }

        internal bool InternalSetContains<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SetContains(key, JsonSerialize(data));
        }

        #region 二进制序列表
        public bool RawSetCreateAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawSetCreateAdd(CheckKey(key, values), dataList);
        }

        public bool RawSetCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawSetCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawSetCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawSetCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawSetCreateAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.Execute();
            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSetCreateAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawSetCreateAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawSetCreateAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SetAddAsync(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0;
        }

        public bool RawSetAdd<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), dataList);
        }

        public bool RawSetAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawSetAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawSetAdd<T>(string key, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, IEnumerable<T> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, IEnumerable<T> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, IEnumerable<T> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, dataList.Select(d => (RedisValue)ByteSerialize(d)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool RawSetAdd<T>(string key, T data, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), data);
        }

        public bool RawSetAdd<T>(string key, T data, int cacheMinutes, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool RawSetAdd<T>(string key, T data, TimeSpan validFor, params object[] values)
        {
            return InternalRawSetAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalRawSetAdd<T>(string key, T data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)ByteSerialize(data));
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, T data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)ByteSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, T data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)ByteSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawSetAdd<T>(string key, T data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SetAdd(key, (RedisValue)ByteSerialize(data));
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<T> RawSetGet<T>(string key, params object[] values)
        {
            return InternalRawSetGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<T> InternalRawSetGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.SetMembers(key);
            return list.Select(v => ((T)ByteDeserialize(v))).ToList();
        }

        public T RawSetPopGet<T>(string key, params object[] values)
        {
            return InternalRawSetPopGet<T>(CheckKey(key, values));
        }

        internal T InternalRawSetPopGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var value = (byte[])db.SetPop(key);
            return (T)ByteDeserialize(value);
        }

        public bool RawSetRemove<T>(string key, T data, params object[] values)
        {
            return InternalRawSetRemove<T>(CheckKey(key, values), data);
        }

        internal bool InternalRawSetRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SetRemove(key, ByteSerialize(data));
        }

        public bool RawSetRemove<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawSetRemove<T>(CheckKey(key, values), dataList);
        }

        internal bool InternalRawSetRemove<T>(string key, IEnumerable<T> dataList)
        {
            var db = Manager.GetDatabase();
            return db.SetRemove(key, dataList.Select(v => (RedisValue)ByteSerialize(v)).ToArray()) > 0;
        }

        public bool RawSetContains<T>(string key, T data, params object[] values)
        {
            return InternalRawSetContains<T>(CheckKey(key, values), data);
        }

        internal bool InternalRawSetContains<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SetContains(key, ByteSerialize(data));
        }
        #endregion
        #endregion

        #region SortedSet 操作
        public bool SortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, params object[] values)
        {
            return InternalSortedSetCreateAdd(CheckKey(key, values), dataList);
        }

        public bool SortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalSortedSetCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool SortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalSortedSetCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray());
            tran.Execute();
            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0;
        }

        public bool SortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), dataList);
        }

        public bool SortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool SortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(JsonSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool SortedSetAdd<T>(string key, SortedSetValue<T> data, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), data);
        }

        public bool SortedSetAdd<T>(string key, SortedSetValue<T> data, int cacheMinutes, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool SortedSetAdd<T>(string key, SortedSetValue<T> data, TimeSpan validFor, params object[] values)
        {
            return InternalSortedSetAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalSortedSetAdd<T>(string key, SortedSetValue<T> data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, JsonSerialize(data), data.Score);
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, SortedSetValue<T> data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, JsonSerialize(data), data.Score);
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, SortedSetValue<T> data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, JsonSerialize(data), data.Score);
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalSortedSetAdd<T>(string key, SortedSetValue<T> data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, JsonSerialize(data), data.Score);
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<SortedSetValue<T>> SortedSetGet<T>(string key, params object[] values)
        {
            return InternalSortedSetGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<SortedSetValue<T>> InternalSortedSetGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.SortedSetRangeByScoreWithScores(key);
            return list.Select(v => new SortedSetValue<T>(JsonDeserialize<T>(v.Element), v.Score)).ToList();
        }

        public IEnumerable<SortedSetValue<T>> SortedSetGetByScore<T>(string key, double start, double end, params object[] values)
        {
            return InternalSortedSetGetByScore<T>(CheckKey(key, values), start, end);
        }

        internal IEnumerable<SortedSetValue<T>> InternalSortedSetGetByScore<T>(string key, double start, double end)
        {
            var db = Manager.GetDatabase();
            var list = db.SortedSetRangeByScoreWithScores(key, start, end);
            return list.Select(v => new SortedSetValue<T>(JsonDeserialize<T>(v.Element), v.Score)).ToList();
        }

        public bool SortedSetRemove<T>(string key, T data, params object[] values)
        {
            return InternalSortedSetRemove<T>(CheckKey(key, values), data);
        }

        internal bool InternalSortedSetRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SortedSetRemove(key, JsonSerialize(data));
        }

        public bool SortedSetRemove<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalSortedSetRemove<T>(CheckKey(key, values), dataList);
        }

        internal bool InternalSortedSetRemove<T>(string key, IEnumerable<T> dataList)
        {
            var db = Manager.GetDatabase();
            return db.SortedSetRemove(key, dataList.Select(v => (RedisValue)v.ToJsonString()).ToArray()) > 0;
        }

        #region 二进制序列化
        public bool RawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, params object[] values)
        {
            return InternalRawSortedSetCreateAdd(CheckKey(key, values), dataList);
        }

        public bool RawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawSortedSetCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawSortedSetCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray());
            tran.Execute();
            var flag = result.Result > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return result.Result > 0;
        }

        internal bool InternalRawSortedSetCreateAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            var result = tran.SortedSetAddAsync(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return result.Result > 0;
        }

        public bool RawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), dataList);
        }

        public bool RawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, IEnumerable<SortedSetValue<T>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, dataList.Select(d => new SortedSetEntry(ByteSerialize(d.Value), d.Score)).ToArray()) > 0;
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public bool RawSortedSetAdd<T>(string key, SortedSetValue<T> data, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), data);
        }

        public bool RawSortedSetAdd<T>(string key, SortedSetValue<T> data, int cacheMinutes, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool RawSortedSetAdd<T>(string key, SortedSetValue<T> data, TimeSpan validFor, params object[] values)
        {
            return InternalRawSortedSetAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalRawSortedSetAdd<T>(string key, SortedSetValue<T> data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, ByteSerialize(data.Value), data.Score);
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, SortedSetValue<T> data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, ByteSerialize(data.Value), data.Score);
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, SortedSetValue<T> data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, ByteSerialize(data.Value), data.Score);
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawSortedSetAdd<T>(string key, SortedSetValue<T> data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.SortedSetAdd(key, ByteSerialize(data.Value), data.Score);
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<SortedSetValue<T>> RawSortedSetGet<T>(string key, params object[] values)
        {
            return InternalRawSortedSetGet<T>(CheckKey(key, values));
        }

        internal IEnumerable<SortedSetValue<T>> InternalRawSortedSetGet<T>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.SortedSetRangeByScoreWithScores(key);
            return list.Select(v => new SortedSetValue<T>((T)ByteDeserialize(v.Element), v.Score)).ToList();
        }

        public IEnumerable<SortedSetValue<T>> RawSortedSetGetByScore<T>(string key, double start, double end, params object[] values)
        {
            return InternalRawSortedSetGetByScore<T>(CheckKey(key, values), start, end);
        }

        internal IEnumerable<SortedSetValue<T>> InternalRawSortedSetGetByScore<T>(string key, double start, double end)
        {
            var db = Manager.GetDatabase();
            var list = db.SortedSetRangeByScoreWithScores(key, start, end);
            return list.Select(v => new SortedSetValue<T>((T)ByteDeserialize(v.Element), v.Score)).ToList();
        }

        public bool RawSortedSetRemove<T>(string key, T data, params object[] values)
        {
            return InternalRawSortedSetRemove<T>(CheckKey(key, values), data);
        }

        internal bool InternalRawSortedSetRemove<T>(string key, T data)
        {
            var db = Manager.GetDatabase();
            return db.SortedSetRemove(key, ByteSerialize(data));
        }

        public bool RawSortedSetRemove<T>(string key, IEnumerable<T> dataList, params object[] values)
        {
            return InternalRawSortedSetRemove<T>(CheckKey(key, values), dataList);
        }

        internal bool InternalRawSortedSetRemove<T>(string key, IEnumerable<T> dataList)
        {
            var db = Manager.GetDatabase();
            return db.SortedSetRemove(key, dataList.Select(v => (RedisValue)ByteSerialize(v)).ToArray()) > 0;
        }
        #endregion
        #endregion

        #region Hash操作
        public bool HashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList);
        }

        public bool HashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool HashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            tran.Execute();
            TimeSpan? time = db.KeyTimeToLive(key);
            if (!time.HasValue)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
            }
            return  true;
        }

        internal bool InternalHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return true;
        }

        internal bool InternalHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            return true;
        }

        internal bool InternalHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return true;
        }



        public bool HashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList);
        }

        public bool HashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool HashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            TimeSpan? time = db.KeyTimeToLive(key);
            if (!time.HasValue)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
            }
            return true;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            db.KeyExpire(key, expireTime - DateTime.Now);
            return true;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            db.KeyExpire(key, validFor);
            return true;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(JsonSerialize(d.Key), JsonSerialize(d.Value))).ToArray());
            db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            return true;
        }

        public bool HashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), data);
        }

        public bool HashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, int cacheMinutes, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool HashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, TimeSpan validFor, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, JsonSerialize(data.Key), JsonSerialize(data.Value));
            if (flag)
            {
                TimeSpan? time = db.KeyTimeToLive(key);
                if (!time.HasValue)
                {
                    db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
                }
            }
            return flag;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, JsonSerialize(data.Key), JsonSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, JsonSerialize(data.Key), JsonSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, JsonSerialize(data.Key), JsonSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<TValue> HashGet<TKey, TValue>(string key, IEnumerable<TKey> hashFieldList, params object[] values)
        {
            return InternalHashGet<TKey, TValue>(CheckKey(key, values), hashFieldList);
        }

        internal IEnumerable<TValue> InternalHashGet<TKey, TValue>(string key, IEnumerable<TKey> hashFieldList)
        {
            var db = Manager.GetDatabase();
            var list = db.HashGet(key, hashFieldList.Select(k => (RedisValue)k.ToJsonString()).ToArray());
            return list.Select(v => JsonDeserialize<TValue>(v)).ToList();
        }

        public TValue HashGet<TKey, TValue>(string key, TKey hashField, params object[] values)
        {
            return InternalHashGet<TKey, TValue>(CheckKey(key, values), hashField);
        }

        internal TValue InternalHashGet<TKey, TValue>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            var value = (string)db.HashGet(key, JsonSerialize(hashField));
            return JsonDeserialize<TValue>(value);
        }

        public IEnumerable<HashObject> HashAllGet(string key, params object[] values)
        {
            return InternalHashAllGet(CheckKey(key, values));
        }

        internal IEnumerable<HashObject> InternalHashAllGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashGetAll(key);
            return list.Select(v => new HashObject(new HashKeyObject((string)v.Name), new HashValueObject((string)v.Value))).ToList();
        }

        public IEnumerable<TKey> HashKeysGet<TKey>(string key, params object[] values)
        {
            return InternalHashKeysGet<TKey>(CheckKey(key, values));
        }

        internal IEnumerable<TKey> InternalHashKeysGet<TKey>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashKeys(key);
            return list.Select(v => JsonDeserialize<TKey>(v)).ToList();
        }

        public IEnumerable<TValue> HashValuesGet<TValue>(string key, params object[] values)
        {
            return InternalHashValuesGet<TValue>(CheckKey(key, values));
        }

        internal IEnumerable<TValue> InternalHashValuesGet<TValue>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashValues(key);
            return list.Select(v => JsonDeserialize<TValue>(v)).ToList();
        }

        public IEnumerable<HashKeyObject> HashKeysGet(string key, params object[] values)
        {
            return InternalHashKeysGet(CheckKey(key, values));
        }

        internal IEnumerable<HashKeyObject> InternalHashKeysGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashKeys(key);
            return list.Select(v => new HashKeyObject((string)v)).ToList();
        }

        public IEnumerable<HashValueObject> HashValuesGet(string key, params object[] values)
        {
            return InternalHashValuesGet(CheckKey(key, values));
        }

        internal IEnumerable<HashValueObject> InternalHashValuesGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashValues(key);
            return list.Select(v => new HashValueObject((string)v)).ToList();
        }

        public bool HashExists<TKey>(string key, TKey hashField, params object[] values)
        {
            return InternalHashExists(CheckKey(key, values), hashField);
        }

        internal bool InternalHashExists<TKey>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            return db.HashExists(key, JsonSerialize(hashField));
        }

        public bool HashDelete<TKey>(string key, TKey hashField, params object[] values)
        {
            return InternalHashDelete(CheckKey(key, values), hashField);
        }

        internal bool InternalHashDelete<TKey>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            return db.HashDelete(key, JsonSerialize(hashField));
        }

        public bool HashDelete<TKey>(string key, IEnumerable<TKey> hashFieldList, params object[] values)
        {
            return InternalHashDelete(CheckKey(key, values), hashFieldList);
        }

        internal bool InternalHashDelete<TKey>(string key, IEnumerable<TKey> hashFieldList)
        {
            var db = Manager.GetDatabase();
            return db.HashDelete(key, hashFieldList.Select(k => (RedisValue)JsonSerialize(k)).ToArray()) > 0;
        }

        #region 二进制序列化
        public bool RawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList);
        }

        public bool RawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalHashCreateAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            tran.Execute();
            TimeSpan? time = db.KeyTimeToLive(key);
            if (!time.HasValue)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(defaultCacheMinutes));
            }
            return true;
        }

        internal bool InternalRawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, expireTime - DateTime.Now);
            tran.Execute();
            return true;
        }

        internal bool InternalRawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, validFor);
            tran.Execute();
            
            return true;
        }

        internal bool InternalRawHashCreateAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var tran = db.CreateTransaction();
            tran.KeyDeleteAsync(key);
            tran.HashSetAsync(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            tran.KeyExpireAsync(key, TimeSpan.FromMinutes(cacheMinutes));
            tran.Execute();
            return true;
        }



        public bool RawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList, defaultCacheMinutes);
        }

        public bool RawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList, cacheMinutes);

        }

        public bool RawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor, params object[] values)
        {
            return InternalHashAdd(CheckKey(key, values), dataList, validFor);
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            return true;
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, DateTime expireTime)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            db.KeyExpire(key, expireTime - DateTime.Now);
            return true;
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, TimeSpan validFor)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            db.KeyExpire(key, validFor);
            return true;
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, IEnumerable<HashValue<TKey, TValue>> dataList, int cacheMinutes)
        {
            if (dataList == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            db.HashSet(key, dataList.Select(d => new HashEntry(ByteSerialize(d.Key), ByteSerialize(d.Value))).ToArray());
            db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            return true;
        }

        public bool RawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, params object[] values)
        {
            return InternalRawHashAdd(CheckKey(key, values), data, defaultCacheMinutes);
        }

        public bool RawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, int cacheMinutes, params object[] values)
        {
            return InternalRawHashAdd(CheckKey(key, values), data, cacheMinutes);

        }

        public bool RawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, TimeSpan validFor, params object[] values)
        {
            return InternalRawHashAdd(CheckKey(key, values), data, validFor);
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            return db.HashSet(key, ByteSerialize(data.Key), ByteSerialize(data.Value));
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, DateTime expireTime)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, ByteSerialize(data.Key), ByteSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, expireTime - DateTime.Now);
            }
            return flag;
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, TimeSpan validFor)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, ByteSerialize(data.Key), ByteSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, validFor);
            }
            return flag;
        }

        internal bool InternalRawHashAdd<TKey, TValue>(string key, HashValue<TKey, TValue> data, int cacheMinutes)
        {
            if (data == null)
            {
                return false;
            }
            var db = Manager.GetDatabase();
            var flag = db.HashSet(key, ByteSerialize(data.Key), ByteSerialize(data.Value));
            if (flag)
            {
                db.KeyExpire(key, TimeSpan.FromMinutes(cacheMinutes));
            }
            return flag;
        }

        public IEnumerable<TValue> RawHashGet<TKey, TValue>(string key, IEnumerable<TKey> hashFieldList, params object[] values)
        {
            return InternalRawHashGet<TKey, TValue>(CheckKey(key, values), hashFieldList);
        }

        internal IEnumerable<TValue> InternalRawHashGet<TKey, TValue>(string key, IEnumerable<TKey> hashFieldList)
        {
            var db = Manager.GetDatabase();
            var list = db.HashGet(key, hashFieldList.Select(k => (RedisValue)ByteSerialize(k)).ToArray());
            return list.Select(v => (TValue)ByteDeserialize(v)).ToList();
        }

        public TValue RawHashGet<TKey, TValue>(string key, TKey hashField, params object[] values)
        {
            return InternalRawHashGet<TKey, TValue>(CheckKey(key, values), hashField);
        }

        internal TValue InternalRawHashGet<TKey, TValue>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            var value = (byte[])db.HashGet(key, ByteSerialize(hashField));
            return (TValue)ByteDeserialize(value);
        }

        public IEnumerable<HashObject> RawHashAllGet(string key, params object[] values)
        {
            return InternalRawHashAllGet(CheckKey(key, values));
        }

        internal IEnumerable<HashObject> InternalRawHashAllGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashGetAll(key);
            return list.Select(v => new HashObject(new HashKeyObject((byte[])v.Name), new HashValueObject((byte[])v.Value))).ToList();
        }

        public IEnumerable<TKey> RawHashKeysGet<TKey>(string key, params object[] values)
        {
            return InternalRawHashKeysGet<TKey>(CheckKey(key, values));
        }

        internal IEnumerable<TKey> InternalRawHashKeysGet<TKey>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashKeys(key);
            return list.Select(v => (TKey)ByteDeserialize(v)).ToList();
        }

        public IEnumerable<TValue> RawHashValuesGet<TValue>(string key, params object[] values)
        {
            return InternalRawHashValuesGet<TValue>(CheckKey(key, values));
        }

        internal IEnumerable<TValue> InternalRawHashValuesGet<TValue>(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashValues(key);
            return list.Select(v => (TValue)ByteDeserialize(v)).ToList();
        }

        public IEnumerable<HashKeyObject> RawHashKeysGet(string key, params object[] values)
        {
            return InternalRawHashKeysGet(CheckKey(key, values));
        }

        internal IEnumerable<HashKeyObject> InternalRawHashKeysGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashKeys(key);
            return list.Select(v => new HashKeyObject((byte[])v)).ToList();
        }

        public IEnumerable<HashValueObject> RawHashValuesGet(string key, params object[] values)
        {
            return InternalRawHashValuesGet(CheckKey(key, values));
        }

        internal IEnumerable<HashValueObject> InternalRawHashValuesGet(string key)
        {
            var db = Manager.GetDatabase();
            var list = db.HashValues(key);
            return list.Select(v => new HashValueObject((byte[])v)).ToList();
        }

        public bool RawHashExists<TKey>(string key, TKey hashField, params object[] values)
        {
            return InternalRawHashExists(CheckKey(key, values), hashField);
        }

        internal bool InternalRawHashExists<TKey>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            return db.HashExists(key, ByteSerialize(hashField));
        }

        public bool RawHashDelete<TKey>(string key, TKey hashField, params object[] values)
        {
            return InternalRawHashDelete(CheckKey(key, values), hashField);
        }

        internal bool InternalRawHashDelete<TKey>(string key, TKey hashField)
        {
            var db = Manager.GetDatabase();
            return db.HashDelete(key, ByteSerialize(hashField));
        }

        public bool RawHashDelete<TKey>(string key, IEnumerable<TKey> hashFieldList, params object[] values)
        {
            return InternalRawHashDelete(CheckKey(key, values), hashFieldList);
        }

        internal bool InternalRawHashDelete<TKey>(string key, IEnumerable<TKey> hashFieldList)
        {
            var db = Manager.GetDatabase();
            return db.HashDelete(key, hashFieldList.Select(k => (RedisValue)ByteSerialize(k)).ToArray()) > 0;
        }
        #endregion
        #endregion
    }

    

    

    

    

    
}
