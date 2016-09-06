using Ik.Framework.Logging;
using System;
using System.Linq;
using System.Reflection;
using Ik.Framework.Common.Extension;
using System.Text;

namespace Ik.Framework.Events
{
    /// <summary>
    /// Evnt publisher
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;
        private static string loggerBusinessName = "事件跟踪";
        private static ILog _logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
        private static ILog _loggerTrack = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService, loggerBusinessName);

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscriptionService"></param>
        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Publish to cunsumer
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="x">Event consumer</param>
        /// <param name="eventMessage">Event message</param>
        protected virtual void PublishToConsumer<T>(IConsumer<T> x, string lable, T eventMessage)
        {
            WriteTrackLog("开始处理本地事件通知",null, lable, eventMessage);
            try
            {
                x.HandleEvent(lable, eventMessage);
                WriteTrackLog("本地事件通知处理完成", null, lable, eventMessage);
            }
            catch (Exception ex)
            {
                WriteTrackLog("事件处理异常", null, lable, eventMessage, ex);
            }

        }

        internal static void WriteTrackLog(string optMessage, string topic, string lable, object eventMessage, Exception ex = null)
        {
            if (_loggerTrack.CheckLogLevel(LogLevel.INFO, LogManager.DefaultAppName, SubscriptionManager.LogModelName_SubscriptionService, loggerBusinessName, null, null))
            {
                Guid? id = null;
                IEventTrack eventTrack = eventMessage as IEventTrack;
                if (eventTrack != null)
                {
                    id = eventTrack.EventId;
                }

                StringBuilder sb = new StringBuilder();
                if (ex != null)
                {
                    sb.AppendLine(eventMessage.ToJsonString());
                    sb.AppendLine("错误信息开始");
                    sb.AppendLine("------------------------------------");
                    sb.AppendLine(ex.ToString());
                    sb.AppendLine("------------------------------------");
                    sb.AppendLine("错误信息完");
                }
                if (!string.IsNullOrEmpty(topic))
                {
                    if (ex != null)
                    {
                        _loggerTrack.Error(string.Format("{0}，主题：{1}，标签：{2}，Id：{3}", optMessage, topic, lable, id), 200, sb.ToString());
                    }
                    else
                    {
                        _loggerTrack.Info(string.Format("{0}，主题：{1}，标签：{2}，Id：{3}", optMessage, topic, lable, id), 200, sb.ToString());
                    }
                }
                else
                {
                    if (ex != null)
                    {
                        _loggerTrack.Error(string.Format("{0}，标签：{1}，Id：{2}", optMessage, lable, id), 200, sb.ToString());
                    }
                    else
                    {
                        _loggerTrack.Info(string.Format("{0}，标签：{1}，Id：{2}", optMessage, lable, id), 200, sb.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        public void Publish<T>(T eventMessage, string lable = "")
        {
            WriteTrackLog("发布本地事件通知",null, lable, eventMessage);
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => PublishToConsumer(x, lable, eventMessage));
        }

        public bool PublishDistributed<T>(string topic, T eventMessage, string lable = "")
		{
            return PublishDistributed<T>(topic, eventMessage, DistributedConsumerType.All, lable);
		}


        public bool PublishDistributed<T>(string topic, T eventMessage, DistributedConsumerType distributedConsumerType, string lable = "")
        {
            if (!SubscriptionManager.DistributedEventService.IsRunning)
            {
                throw new EventException("分布式事件处理服务未启动");
            }
            WriteTrackLog("发布分布式事件通知", topic, lable, eventMessage);

            //WriteTrackLog("发布分布式事件通知，本地通知开始", topic, lable, eventMessage);
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => PublishToConsumer(x, lable, eventMessage));
            //WriteTrackLog("发布分布式事件通知，本地通知完成", topic, lable, eventMessage);

            var consumerType = typeof(T);
            var objectName = consumerType.FullName;
            DistributedObjectNameAttribute objectNameAttr = consumerType.GetCustomAttribute<DistributedObjectNameAttribute>();
            if (objectNameAttr != null && !string.IsNullOrEmpty(objectNameAttr.Name))
            {
                objectName = objectNameAttr.Name;
            }
            var dEvent = new DistributedEvent(eventMessage, topic, lable, objectName);
            //WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信开始，对象名：{0}", objectName), topic, lable, eventMessage);
            return SubscriptionManager.EventTransport.SendEventMessage(dEvent, distributedConsumerType);
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
