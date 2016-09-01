using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public class IdentityInfo
    {
        public string RefUserCode { get; set; }
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
