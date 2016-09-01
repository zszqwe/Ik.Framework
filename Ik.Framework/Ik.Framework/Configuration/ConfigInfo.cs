using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    public class ConfigInfo
    {
        public string ConfigName { get; set; }

        public Version VersionInfo { get; set; }

        public string RemoteConfigServerAddress { get; set; }
    }
}
