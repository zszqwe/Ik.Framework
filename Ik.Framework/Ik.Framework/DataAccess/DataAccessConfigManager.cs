using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    public class DataAccessConfigManager
    {
        public  DataSource GetDataSource(string dataSourceName)
        {
            var config = ConfigManager.Instance.Get<DataSourcesConfig>(DataSourcesConfigurationSectionHandler.ConfigSectionName);
            if (config == null)
            {
                throw new DataAccessException("获取数据源配置失败");
            }
            foreach (var item in config.DataSources)
            {
                if (item.Name.ToLower() == dataSourceName.ToLower())
                {
                    return item;
                }
            }
            return null;
        }
    }
}
