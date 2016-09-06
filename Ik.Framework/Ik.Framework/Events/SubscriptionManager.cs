using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    public class SubscriptionManager
    {
        public const string LogModelName_SubscriptionService = "事件发布订阅服务";
        private static ILog logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
		private static DistributedSubscriptionService _subscriptionService = null;
        private static object _lockObj = new object();

		public static IBackgroundService DistributedEventService
        {
            get
            {
                if (_subscriptionService == null)
                {
                    lock (_lockObj)
                    {
                        if (_subscriptionService == null)
                        {
                            logger.Info("分布式事件发布订阅服务开始");
                            _subscriptionService = new DistributedSubscriptionService();
                        }
                    }
                }
                return _subscriptionService;
            }
        }

        public static IEventTransport EventTransport
        {
            get
            {
                return (IEventTransport)DistributedEventService;
            }
        }

        public static void SubscriberContextInitialize(Func<ISubscriptionService> create, bool forceRecreate = false)
        {
            LocalSubscribeContext.Initialize(create, forceRecreate);
        }

        public static void PublisherContextInitialize(Func<ISubscriptionService, IEventPublisher> create, ISubscriptionService subscriptionService, bool forceRecreate = false)
        {
            LocalPublisherContext.Initialize(create, subscriptionService, forceRecreate);
        }

        public static void SubscriberContextInitialize(bool forceRecreate = false)
        {
            LocalSubscribeContext.Initialize(forceRecreate);
        }

        public static void PublisherContextInitialize(bool forceRecreate = false)
        {
            LocalPublisherContext.Initialize(forceRecreate);
        }

        public static bool IsPublisherContextInitialize
        {
            get
            {
                return LocalPublisherContext.IsInited;
            }
        }

        public static bool IsSubscriberContextInitialize
        {
            get
            {
                return LocalSubscribeContext.IsInited;
            }
        }


        public static ISubscriptionService Subscriber
        {
            get
            {
                return LocalSubscribeContext.Current;
            }
        }

        public static IEventPublisher Publisher
        {
            get
            {
                return LocalPublisherContext.Current;
            }
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
