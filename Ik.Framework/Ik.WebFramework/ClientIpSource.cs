using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework
{
    public class ClientIpSource
    {
        public string NginxClientIp { get; set; }
        public string HttpClientIp { get; set; }
        public bool IsHttps { get; set; }
    }
}
