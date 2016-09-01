using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public interface IBufferServiceRegistrar
    {
        void Register(IList<BufferProcessorService> registed);

        int Order { get; }
    }
}
