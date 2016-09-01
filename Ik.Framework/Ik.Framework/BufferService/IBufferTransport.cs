using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public interface IBufferTransport
    {
        bool Transport(BufferItem item);
    }
}
