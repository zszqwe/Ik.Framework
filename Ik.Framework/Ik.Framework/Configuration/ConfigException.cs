using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    public class ConfigInitException :InkeyException
    {
        public ConfigInitException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {
 
        }
    }

    public class ConfigOperateException : InkeyException
    {
        public ConfigOperateException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

        }
    }
}
