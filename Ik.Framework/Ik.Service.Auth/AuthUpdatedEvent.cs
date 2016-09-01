
using System;
namespace Ik.Framework.Events
{
    /// <summary>
    /// A container for entities that are updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuthUpdatedEvent:IEventTrack
    {
        private Guid eventId = Guid.NewGuid();
        public Guid EventId
        {
            get 
            {
                return eventId;
            }
        }
    }
}
