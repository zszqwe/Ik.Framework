using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Logging
{
    [Flags]
    public enum LogLevel
    {
        DEBUG = 1, 
        ERROR = 2, 
        WARN = 4, 
        INFO = 8,
        OFF = 16
    }
}
