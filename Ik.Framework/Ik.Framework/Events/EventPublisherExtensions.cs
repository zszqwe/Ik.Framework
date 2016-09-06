

namespace Ik.Framework.Events
{
    public static class EventPublisherExtensions
    {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity, string lable = "") where T : class
        {
            eventPublisher.Publish(new EntityInserted<T>(entity), lable);
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity, string lable = "") where T : class
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity), lable);
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity, string lable = "") where T : class
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity), lable);
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
