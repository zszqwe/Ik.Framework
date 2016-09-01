
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(InkeyException exception)
        {
            Exception = exception;
        }

        public InkeyException Exception { get; set; }
    }
}
