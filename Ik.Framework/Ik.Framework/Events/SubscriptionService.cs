using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Ik.Framework.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        private static ILog logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
        private static Dictionary<Type, List<object>> subList = new Dictionary<Type, List<object>>();
        private static Dictionary<string, Dictionary<string, List<Tuple<HandleType, object>>>> distribSubList = new Dictionary<string, Dictionary<string, List<Tuple<HandleType, object>>>>();
        private static Dictionary<string, Dictionary<string, Type>> distribSubObjectNameList = new Dictionary<string, Dictionary<string, Type>>();
		private static object lockObj = new object();
        private static bool inited = false;
        //private static bool _isAutoSubscribe = false;
        public static void init()
        {
            if (!inited)
            {
                lock (lockObj)
                {
                    if (!inited)
                    {
                        bool? isAutoSubscribe = ConfigManager.Instance.Get<bool?>("event_IsAutoSubscribe");
                        if (isAutoSubscribe.HasValue && !isAutoSubscribe.Value)
                        {
                            inited = true;
                            return;
                        }
                        var typeFinder = new AppDomainTypeFinder();
                        var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
                        foreach (var consumer in consumers)
                        {
                            EventAutoSubscribeAttribute autoEvent = consumer.GetCustomAttribute<EventAutoSubscribeAttribute>();
                            if (autoEvent != null && !autoEvent.IsAutoSubscribe)
                            {
                                continue;
                            }
                            InternalRegisterConsumerType(consumer);
                            
                        }
                        inited = true;
                    }
                }
            }
        }

        private static void InternalRegisterConsumerType(Type consumer)
        {
            logger.Info(string.Format("开始注册订阅对象，类型：{0}", consumer.FullName));
            DistributedEventAttribute dEvent = consumer.GetCustomAttribute<DistributedEventAttribute>();
            var interfaces = consumer.FindInterfaces((type, criteria) =>
            {
                var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                return isMatch;
            }, typeof(IConsumer<>));
            object consumerInstance = null;
            try
            {
                consumerInstance = Activator.CreateInstance(consumer);
            }
            catch (Exception ex)
            {
                throw new EventException(string.Format("订阅对象创建失败，对象类型：{0}", consumer.FullName), ex);
            }
            foreach (var item in interfaces)
            {
                if (subList.ContainsKey(item))
                {
                    subList[item].Add(consumerInstance);
                }
                else
                {
                    subList.Add(item, new List<object> { consumerInstance });
                }
                logger.Info(string.Format("解析订阅类型完成，订阅对象类型：{0}，订阅类型：{1}", consumer.FullName, item.FullName));
                
                if (dEvent != null)
                {
                    var genericArgs = item.GetGenericArguments();
                    var consumerType = genericArgs[0];
                    string objectName = consumerType.FullName;
                    DistributedObjectNameAttribute objectNameAttr = consumerType.GetCustomAttribute<DistributedObjectNameAttribute>();
                    if (objectNameAttr != null && !string.IsNullOrEmpty(objectNameAttr.Name))
                    {
                        objectName = objectNameAttr.Name;
                    }
                    if (distribSubObjectNameList.ContainsKey(dEvent.Topic))
                    {
                        if (!distribSubObjectNameList[dEvent.Topic].ContainsKey(objectName))
                        {
                            distribSubObjectNameList[dEvent.Topic].Add(objectName, consumerType);
                        }
                        else
                        {
                            if (distribSubObjectNameList[dEvent.Topic][objectName] != consumerType)
                            {
                                throw new EventException("该类型对象名重复");
                            }
                        }
                    }
                    else
                    {
                        var objectNameDic = new Dictionary<string, Type>();
                        objectNameDic.Add(objectName, consumerType);
                        distribSubObjectNameList.Add(dEvent.Topic, objectNameDic);
                    }
                    if (distribSubList.ContainsKey(dEvent.Topic))
                    {
                        if (distribSubList[dEvent.Topic].ContainsKey(objectName))
                        {
                            distribSubList[dEvent.Topic][objectName].Add(Tuple.Create(dEvent.HandleType, consumerInstance));
                        }
                        else
                        {
                            distribSubList[dEvent.Topic].Add(objectName, new List<Tuple<HandleType, object>> { Tuple.Create(dEvent.HandleType, consumerInstance) });
                        }
                    }
                    else
                    {
                        Dictionary<string, List<Tuple<HandleType, object>>> dict = new Dictionary<string, List<Tuple<HandleType, object>>>();
                        dict.Add(objectName, new List<Tuple<HandleType, object>> { Tuple.Create(dEvent.HandleType, consumerInstance) });
                        distribSubList.Add(dEvent.Topic, dict);
                    }
                    logger.Info(string.Format("解析分布式订阅类型完成，订阅对象类型：{0}，订阅类型：{1}，订阅主题：{2}，对象名：{3}", consumer.FullName, item.FullName, dEvent.Topic, objectName));
                }
            }
        }
        public SubscriptionService()
        {
            init();
        }
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            var key = typeof(IConsumer<T>);
            if (subList.ContainsKey(key))
            {
                return subList[key].Select(c => (IConsumer<T>)c).ToList();
            }
            return new List<IConsumer<T>>();
        }


        public IList<DistributedEventConsumer> GetDistributedSubscriptions(string topic, string objectName)
        {
            if (distribSubList.ContainsKey(topic) && distribSubList[topic].ContainsKey(objectName))
            {
                return distribSubList[topic][objectName].Select(c => new DistributedEventConsumer(c.Item1, c.Item2)).ToList();
            }
            return new List<DistributedEventConsumer>();
        }

		public bool HasTopicConsumer(string topic)
		{
			return distribSubList.ContainsKey(topic);
		}

		public IList<string> GetDistributedSubscriptionTopics()
		{
			return distribSubList.Keys.ToList();
		}

        public Type GetTopicConsumerType(string topic, string objectName)
        {
            if (distribSubObjectNameList.ContainsKey(topic) && distribSubObjectNameList[topic].ContainsKey(objectName))
            {
                return distribSubObjectNameList[topic][objectName];
            }
            return null;
        }

        public void RegisterConsumerType(Type consumerType)
        {
            InternalRegisterConsumerType(consumerType);
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
