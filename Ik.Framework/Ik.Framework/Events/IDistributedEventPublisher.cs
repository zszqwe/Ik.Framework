using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    [ServiceContract]
    public interface IDistributedEventPublisher
    {
        [OperationContract]
        void Process(DistributedEvent dEvent);

        [OperationContract]
        void Heartbeat();
    }
}
