using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DependencyManagement
{
    class IocException: InkeyException
    {
        public IocException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

        }

        public IocException(string desc, Exception ex)
            : base(InkeyErrorCodes.CommonFailure, desc, ex)
        {

        }
    }
}
