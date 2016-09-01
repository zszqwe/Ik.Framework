

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