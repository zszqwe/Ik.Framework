using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching
{
    public class CachingException :InkeyException
    {
        public CachingException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {
 
        }
    }
}
