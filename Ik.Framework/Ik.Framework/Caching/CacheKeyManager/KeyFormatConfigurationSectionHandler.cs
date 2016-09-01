using Ik.Framework.Caching.CacheKeyManager;
using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching
{
    public class KeyFormatConfigurationSectionHandler : LocalConfigSectionHandler<CacheKeyConfig>
    {
        public const string ConfigSectionName = "keyConfigDefines";
    }
}
