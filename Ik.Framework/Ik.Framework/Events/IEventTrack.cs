using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    public interface IEventTrack
    {
         Guid EventId { get; }
    }
}
