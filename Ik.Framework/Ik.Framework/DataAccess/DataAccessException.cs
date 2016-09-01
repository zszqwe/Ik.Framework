using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    public class DataAccessException : InkeyException
    {
        public DataAccessException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

        }

        public DataAccessException(string desc, Exception ex)
            : base(InkeyErrorCodes.CommonFailure, desc, ex)
        {

        }
    }
}
