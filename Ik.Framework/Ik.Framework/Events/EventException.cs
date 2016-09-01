using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    public class EventException :InkeyException
    {
        public EventException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {
 
        }

        public EventException(string desc, Exception ex)
            : base(InkeyErrorCodes.CommonFailure, desc, ex)
        {

        }
    }
}
