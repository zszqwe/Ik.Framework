using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public class TokenInfo
    {
        public string RefUserCode { get; set; }
        public DateTime AccessTime { get; set; }
    }
}
