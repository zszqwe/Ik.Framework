using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public class BufferServiceException:InkeyException
    {
        public BufferServiceException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {
 
        }
    }
}
