using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    public class DataSourcesConfigurationSectionHandler : LocalConfigSectionHandler<DataSourcesConfig>
    {
        public const string ConfigSectionName = "dataSourcesDefines";
    }
}
