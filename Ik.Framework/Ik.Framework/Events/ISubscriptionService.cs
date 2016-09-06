using System;
using System.Collections.Generic;

namespace Ik.Framework.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        IList<IConsumer<T>> GetSubscriptions<T>();

        IList<DistributedEventConsumer> GetDistributedSubscriptions(string topic, string objectName);

		bool HasTopicConsumer(string topic);

		IList<string> GetDistributedSubscriptionTopics();

        Type GetTopicConsumerType(string topic, string objectName);

       void  RegisterConsumerType(Type consumerType);

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
