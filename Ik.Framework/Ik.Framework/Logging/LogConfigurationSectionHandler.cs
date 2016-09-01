using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Logging
{
    class LogConfigurationSectionHandler : LocalConfigSectionHandler<LogConfig>
    {
        public const string ConfigSectionName = "logConfig";
    }
}
