using Ik.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    class AuthException: InkeyException
    {
        public AuthException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

        }

        public AuthException(string desc, Exception ex)
            : base(InkeyErrorCodes.CommonFailure, desc, ex)
        {

        }
    }
}
