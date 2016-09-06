
namespace Ik.Framework.Events
{
    /// <summary>
    /// Evnt publisher
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        void Publish<T>(T eventMessage, string lable = "");

        bool PublishDistributed<T>(string topic, T eventMessage, string lable = "");

        bool PublishDistributed<T>(string topic, T eventMessage, DistributedConsumerType consumerType, string lable = "");
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
