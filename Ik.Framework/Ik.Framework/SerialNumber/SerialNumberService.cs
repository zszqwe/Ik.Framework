using Ik.Framework.Caching;
using Ik.Framework.DataAccess.DataMapping;
using Ik.Framework.Events;
using Ik.Framework.Logging;
using Ik.Framework.SerialNumber.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ik.Framework.SerialNumber
{
    public enum SerialNumberCreateType
    {
        DataBase, Cache
    }

    [EventAutoSubscribeAttribute(false)]
    [DistributedEvent(Framework.Events.HandleType.Asyn, SerialNumberEventTopics.Topic_SerialNumber_Operator)]
    public class SerialNumberService: IConsumer<SerialNumberUpdatedEvent>, ISerialNumberService
    {
        private static readonly ILog logger = LogManager.GetLogger("编号服务");
        private ISerialNumberDao _serialNumberDao = null;
        private IDataAccessFactory _dataAccessFactory = null;
        private string _key = null;
        private SerialNumberCreateType _createType = SerialNumberCreateType.DataBase;
        private SerialNumberDto _config = null;
        public const string KEY_SEQUENCE_MAXVALUE = "max_value_{key}_v1";
        public const string KEY_SEQUENCE_NEXTVALUE = "next_value_{key}_v1";
        public const string KEY_SEQUENCE_MAXVALUE_SETFLAG = "flag_set_maxvalue_{key}_v1";
        private MemcachedManager _cacheManager = null;
        private ulong _maxValueCas = 0;
        private bool _isInited = false;
        private ulong _initNextValueMaxValue = ulong.MaxValue - 10000000;
        private static object _lockObj = new object();
        private static ConcurrentDictionary<string, SerialNumberDto> _configDict = new ConcurrentDictionary<string, SerialNumberDto>();

        public SerialNumberService()
        {
 
        }

        public SerialNumberService(string key, IDataAccessFactory dataAccessFactory)
            : this(key, SerialNumberCreateType.DataBase, dataAccessFactory)
        {

        }

        public SerialNumberService(string key, SerialNumberCreateType createType, IDataAccessFactory dataAccessFactory)
        {
            this._key = key;
            this._createType = createType;
            this._dataAccessFactory = dataAccessFactory;
            this._serialNumberDao = dataAccessFactory.GetDataAccess<ISerialNumberDao>();
            this._config = _configDict.GetOrAdd(this._key, (k) => this._serialNumberDao.GetSerialNumberConfigByKey(this._key));
            this._cacheManager = new MemcachedManager(IKCacheFactory.CacheKeyFormat);
        }

        public string GetSerialNumber()
        {
            string number = "";
            if (this._createType == SerialNumberCreateType.DataBase)
            {
                var nextValue = this._serialNumberDao.GetIncrementSerialNumberByKey(this._key, this._config.LengthMaxValue);
                number = BuildNumber(nextValue);
            }
            else
            {
                Init();

                ulong nextValue = this.NextValue;
                ulong maxValue = this.MaxValue;

                if (nextValue >= maxValue || (int)(maxValue - nextValue) < this._config.CheckThreshold)
                {
                    SetSequenceStateValue(false);
                    maxValue = this.MaxValue;
                }
                ulong value = this._cacheManager.Increment(KEY_SEQUENCE_NEXTVALUE, this._initNextValueMaxValue, 1, this._key);
                if (value == this._initNextValueMaxValue || value > this._initNextValueMaxValue || value == 0)//如果还没有初始化自增序号
                {
                    Thread.Sleep(500);
                    string ex = "自增序号失败，缓存失效或者出现无效序号，编号服务Key:" + this._key;
                    logger.Error(ex);
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                }
                if (value > maxValue)
                {
                    Thread.Sleep(500);
                    string ex = "超过了编号缓冲段最大边界，编号服务Key:" + this._key;
                    logger.Error(ex);
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                }
                number = BuildNumber((long)value);
            }
            return number;
        }

        public IList<string> GetSerialNumber(int count)
        {
            if (count < 1)
            {
                throw new InkeyException(InkeyErrorCodes.CommonFailure, "批量获取个数超出范围申请范围，编号服务Key:" + this._key);
            }
            List<string> numberList = new List<string>();
            if (this._createType == SerialNumberCreateType.DataBase)
            {
                var nextValue = this._serialNumberDao.GetIncrementCountSerialNumberByKey(this._key, count, this._config.LengthMaxValue);
                for (long i = nextValue - count + 1; i <= nextValue; i++)
                {
                    numberList.Add(BuildNumber(i));
                }
            }
            else
            {
                Init();

                ulong nextValue = this.NextValue;
                ulong maxValue = this.MaxValue;

                if (nextValue >= maxValue || ((nextValue + (ulong)count) >= maxValue) || (int)(maxValue - nextValue - (ulong)count) < this._config.CheckThreshold)
                {
                    SetSequenceStateValue(false);
                    maxValue = this.MaxValue;
                }
                ulong value = this._cacheManager.Increment(KEY_SEQUENCE_NEXTVALUE, this._initNextValueMaxValue, (ulong)count, this._key);
                if (value == this._initNextValueMaxValue || value > this._initNextValueMaxValue || value == 0)//如果还没有初始化自增序号
                {
                    Thread.Sleep(500);
                    string ex = "自增序号失败，缓存失效或者出现无效序号，编号服务Key:" + this._key;
                    logger.Error(ex);
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                }
                if (value > maxValue)
                {
                    Thread.Sleep(500);
                    string ex = "超过了编号缓冲段最大边界，编号服务Key:" + this._key;
                    logger.Error(ex);
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                }
                for (ulong i = value - (ulong)count + 1; i <= value; i++)
                {
                    numberList.Add(BuildNumber((long)i));
                }
            }
            return numberList;
        }

        protected virtual string BuildNumber(long nextValue)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this._config.PrefixValue))
            {
                sb.Append(this._config.PrefixValue);
            }

            if (!string.IsNullOrEmpty(this._config.DateFormat))
            {
                DateTime newTime = DateTime.Now;
                sb.Append(newTime.ToString(this._config.DateFormat));
            }
            if (this._config.FormatLength > 0)
            {
                sb.Append(string.Format(this._config.LengthTemplete, nextValue));
            }
            else
            {
                sb.Append(nextValue);
            }
            return sb.ToString();
        }

        protected ulong MaxValue
        {
            get
            {
                int checkCasCount = 0;
                CasValue<ulong> casValue = null;
                while (true)
                {
                    casValue = GetMaxValueCasValue();
                    if (casValue != null)//缓存如果掉了，直接重来，避免在高并发的方法中初始化
                    {
                        break;
                    }
                    if (checkCasCount > 2)
                    {
                        string ex = "缓存丢失，参数：MaxValue，编号服务Key:" + this._key;
                        logger.Error(ex);
                        this._isInited = false;
                        throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                    }
                    Thread.Sleep(500);
                    checkCasCount++;
                }
                if (_maxValueCas != casValue.Cas)
                {
                    int checkCasCount2 = 0;
                    //如果序号状态发生了变化
                    while (true)
                    {
                        if (checkCasCount2 > 2)
                        {
                            string ex = "缓存异常，参数：MaxValue，编号服务Key:" + this._key;
                            logger.Error(ex);
                            throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                        }
                        casValue = GetMaxValueCasValue();
                        if (casValue == null)
                        {
                            string ex = "缓存丢失，参数：MaxValue，变化检查，编号服务Key:" + this._key;
                            logger.Error(ex);
                            this._isInited = false;
                            throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                        }
                        if (_maxValueCas == casValue.Cas)
                        {
                            break;
                        }
                        _maxValueCas = casValue.Cas;
                        checkCasCount2++;
                        Thread.Sleep(1000);
                    }
                }
                return casValue.Result;
            }
        }

        
        protected ulong NextValue
        {
            get
            {
                int checkCasCount = 0;
                while (true)
                {
                    var stateValue = GetNextValueState();
                    if (stateValue != null)
                    {
                        return stateValue.Result;
                    }
                    if (checkCasCount > 2)
                    {
                        string ex = "获取自增序号失败，初始化重置，参数：NextValue，编号服务Key:" + this._key;
                        logger.Error(ex);
                        this._isInited = false;
                        throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                    }
                    Thread.Sleep(500);
                    checkCasCount++;
                }

            }
        }

        private CasValue<ulong> GetMaxValueCasValue()
        {
            return this._cacheManager.GetWithCas<ulong>(KEY_SEQUENCE_MAXVALUE, this._key);
        }

        private CasValue<ulong> GetNextValueCasValue()
        {
            return this._cacheManager.GetWithCas<ulong>(KEY_SEQUENCE_NEXTVALUE, this._key);
        }

        private StateValue<ulong> GetMaxValueState()
        {
            return this._cacheManager.GetWithState<ulong>(KEY_SEQUENCE_MAXVALUE, this._key);
        }

        private StateValue<ulong> GetNextValueState()
        {
            return this._cacheManager.GetWithState<ulong>(KEY_SEQUENCE_NEXTVALUE, this._key);
        }

        private void Init()
        {
            if (!_isInited)
            {
                lock (_lockObj)
                {
                    if (!_isInited)
                    {
                        SetSequenceStateValue(true);
                        _isInited = true;
                    }
                }
            }
        }

        private void SetSequenceStateValue(bool isInit)
        {

            //设置群集同步标志
            var flag = this._cacheManager.Synchronous(KEY_SEQUENCE_MAXVALUE_SETFLAG, () =>
            {
                //获取当前最大序号
                var lastNextValue = this._serialNumberDao.GetSerialNumberNextValueByKey(this._key);

                if (isInit)
                {
                    var nextValueState = GetNextValueCasValue();
                    var maxValueState = GetMaxValueCasValue();
                    if (nextValueState != null
                        && maxValueState != null
                        && maxValueState.Result == (ulong)this._config.NextValue
                        && (int)(maxValueState.Result - nextValueState.Result) > this._config.CheckThreshold)
                    {
                        this._maxValueCas = maxValueState.Cas;
                        return;
                    }
                }
                using (var scope = this._dataAccessFactory.CreateTransactionScope())
                {
                    try
                    {
                        //使用最近的序号去设置最大序号，确保是本服务设置成功
                        var setState = this._serialNumberDao.GetApplyCacheSerialNumberCapacityByKey(this._key, lastNextValue);
                        if (setState.SetState)
                        {
                            ulong lastSequenceStateCas = 0;
                            int checkCasCount = 0;
                            while (true)
                            {
                                var setCasValue = this._cacheManager.SetWithCas(KEY_SEQUENCE_MAXVALUE, setState.NextValue, this._key);
                                if (setCasValue != null && setCasValue.Result)
                                {
                                    lastSequenceStateCas = setCasValue.Cas;
                                    break;
                                }
                                if (checkCasCount > 2)
                                {
                                    string ex = "申请编号缓存失败，编号服务Key:" + this._key;
                                    logger.Error(ex, 9000);
                                    throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                                }
                                Thread.Sleep(500);
                                checkCasCount++;
                            }
                            StateValue<ulong> nextValueState = null;
                            int checkCasCount2 = 0;
                            while (checkCasCount2 < 2)
                            {
                                nextValueState = GetNextValueState();
                                if (nextValueState != null)
                                {
                                    break;
                                }
                                Thread.Sleep(500);
                                checkCasCount2++;
                            }

                            if (nextValueState == null || isInit)
                            {
                                int checkCasCount3 = 0;
                                while (true)
                                {
                                    var setFlag = this._cacheManager.Set(KEY_SEQUENCE_NEXTVALUE, lastNextValue, this._key);
                                    if (setFlag)
                                    {
                                        break;
                                    }
                                    if (checkCasCount3 > 2)
                                    {
                                        string ex = "设置序列号缓存失败，初始化重置，编号服务Key:" + this._key;
                                        logger.Error(ex, 9000);
                                        throw new InkeyException(InkeyErrorCodes.CommonFailure, ex);
                                    }
                                    Thread.Sleep(500);
                                    checkCasCount3++;
                                }

                            }
                            this._maxValueCas = lastSequenceStateCas;
                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this._cacheManager.Remove(KEY_SEQUENCE_MAXVALUE_SETFLAG, this._key);
                        throw ex;
                    }
                }
            },this._key);

            if (!flag && isInit)
            {
                Thread.Sleep(3000);
            }
        }

        public void HandleEvent(string lable, SerialNumberUpdatedEvent eventMessage)
        {
            SerialNumberDto config;
            if (string.IsNullOrEmpty(eventMessage.Key))
            {
                _configDict.Clear();
            }
            else
            {
                _configDict.TryRemove(eventMessage.Key, out config);
            }
        }

        public static void SubscriberSerialNumberEvent()
        {
            if (SubscriptionManager.IsSubscriberContextInitialize)
            {
                SubscriptionManager.Subscriber.RegisterConsumerType(typeof(SerialNumberService));
            }
        }

        public void UpdateNotification()
        {
            UpdateNotificationByKey(this._key);
        }

        public static void UpdateNotificationByKey(string key)
        {
            SubscriptionManager.Publisher.PublishDistributed<SerialNumberUpdatedEvent>(SerialNumberEventTopics.Topic_SerialNumber_Operator, new SerialNumberUpdatedEvent { Key = key });
        }
    }

    public class SerialNumberUpdatedEvent : IEventTrack
    {
        private Guid eventId = Guid.NewGuid();
        public Guid EventId
        {
            get
            {
                return eventId;
            }
        }

        public string Key { get; set; }
    }

    public class SerialNumberEventTopics
    {
        public const string Topic_SerialNumber_Operator = "SerialNumber_Operator";
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
