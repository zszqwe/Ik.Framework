using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.SerialNumber
{
    public class SerialNumberStateDto
    {
        public ulong NextValue { get; set; }

        public bool SetState { get; set; }
    }
}
