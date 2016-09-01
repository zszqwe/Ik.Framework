using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Framework.Events
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DistributedEventPublisher : IDistributedEventPublisher
    {
		private readonly ISubscriptionService _subscriptionService = LocalSubscribeContext.Current;
        private static ILog _logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);

        public void Process(DistributedEvent dEvent)
        {
            string address = string.Empty;
            Type type = null;
            object eventMessage = null;
            try
            {
                address = SubscriptionManager.EventTransport.GetEventServerAddress();
                type = _subscriptionService.GetTopicConsumerType(dEvent.Topic, dEvent.ObjectName);
                eventMessage = dEvent.GetValue(type);
                if (!_subscriptionService.HasTopicConsumer(dEvent.Topic))
                {
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，执行请求远程通信，主题不存在，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, eventMessage);
                    return;
                }
            }
            catch(Exception ex)
            {
                _logger.Error("获取事件对象类型错误", ex);
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，准备执行请求远程通信错误，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent, ex);
                return;
            }
            EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，执行请求远程通信，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, eventMessage);
            try
            {
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，本地通知开始，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, eventMessage);
                var subscriptions = _subscriptionService.GetDistributedSubscriptions(dEvent.Topic, dEvent.ObjectName);
                subscriptions.ToList().ForEach(x => PublishToConsumer(address, dEvent.ObjectName, dEvent.Topic, x.Consumer, x.HandleType, eventMessage, dEvent.Lable));
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，本地通知完成，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, eventMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("获取订阅对象错误，主题：{0}，对象名称：{1}，数据：{2}", dEvent.Topic, dEvent.ObjectName, dEvent.GetJsonString()), ex);
            }
        }

        public void Heartbeat()
        {
            _logger.Info("执行心跳方法");
        }


        private void PublishToConsumer(string address,string objectName,string topic, object consumer, HandleType handleType, object eventMessage, string lable)
        {
            var type = consumer.GetType();
            if (handleType == HandleType.Sync)
            {
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，开始同步本地通知，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                try
                {
                    type.InvokeMember("HandleEvent", BindingFlags.Default | BindingFlags.InvokeMethod, null, consumer, new object[] { lable, eventMessage });
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，同步本地通知完成，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                }
                catch (Exception ex)
                {
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，同步本地通知错误，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage,ex);
                }
            }
            else
            {
                try
                {
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，开始执行异步本地通知，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                    Task.Run(() =>
                    {
                        EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，开始异步本地通知，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                        type.InvokeMember("HandleEvent", BindingFlags.Default | BindingFlags.InvokeMethod, null, consumer, new object[] { lable, eventMessage });
                        EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，异步本地通知完成，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                    }).ContinueWith((task) =>
                    {
                        if (task.Exception != null)
                        {
                            EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，异步本地通知错误，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage, task.Exception);
                        }
                    });
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，执行异步本地通知完成，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage);
                }
                catch (Exception ex)
                {
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，执行异步本地通知错误，地址：{0}，对象名：{1}", address, objectName), topic, lable, eventMessage, ex);
                }
            }
        }
    }
}
