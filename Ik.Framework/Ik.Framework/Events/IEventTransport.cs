using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    public interface IEventTransport
    {
        bool SendEventMessage(DistributedEvent dEvent, DistributedConsumerType consumerType);

        string GetEventServerAddress();
    }
}
