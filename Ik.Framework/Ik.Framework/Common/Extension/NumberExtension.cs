using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class NumberExtension
    {
        public static decimal ToDecimal2(this decimal obj)
        {
            return decimal.Parse(obj.ToString("#0.00"));
        }
    }
}
