using Ik.Framework.Common.Utilities;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    public class ConfigManager : DefaultSingleton<ConfigManager>, IConfigManager
    {
        public const string LogModelName_ConfigService = "配置服务";
        private static ILog logger = LogManager.GetLogger(ConfigManager.LogModelName_ConfigService);
        public static ConfigInfo GetConfigInfo()
        {
            ConfigInfo info = new ConfigInfo();
            var key = ConfigurationManager.AppSettings["config_key"];
            if (!string.IsNullOrEmpty(key))
            {
                info.ConfigName = key;
            }
            else
            {
                info.ConfigName = AppInfo.AppCode;
            }
            info.RemoteConfigServerAddress = ConfigurationManager.AppSettings["config_server_address"];
            string ver = ConfigurationManager.AppSettings["config_ver"];
            if (!string.IsNullOrEmpty(ver))
            {
                Version version = VersionHelper.FormatVersion(ver);
                info.VersionInfo = version;
            }
            else
            {
                info.VersionInfo = AppInfo.AppVer;
            }
            return info;
        }

        public static IBackgroundService RemoteConfigService
        {
            get
            {
                return RemoteConfigManager.RemoteConfigService;
            }
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (LocalConfigManager.TryGet<T>(key, out value))
            {
                return true;
            }
            if (RemoteConfigManager.TryGet<T>(key, out value))
            {
                return true;
            }
            return false;
        }

        public T Get<T>(string key)
        {
            T value;
            return TryGet<T>(key, out value) ? value : default(T);
        }

        public bool RegisterConfigChangedEvent<T>(string key, ConfigChangedEvent<T> changed)
        {
            return RemoteConfigManager.RegisterConfigChangedEvent<T>(key, changed);
        }
    }
}
