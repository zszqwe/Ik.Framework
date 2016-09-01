
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IkZooKeeperNet;

namespace Ik.Framework.Configuration
{
    internal class RemoteConfigManager : DefaultSingleton<RemoteConfigManager>
    {
        private static RemoteConfigManagerService configService = new RemoteConfigManagerService();
        private static readonly object lockObj = new object();

        public static  bool TryGet<T>(string key, out T value)
        {
            return configService.TryGet<T>(key, out value);
        }

        public static bool RegisterConfigChangedEvent<T>(string key, ConfigChangedEvent<T> changed)
        {
            return configService.RegisterConfigChangedEvent<T>(key, changed);
        }

        public static IBackgroundService RemoteConfigService
        {
            get
            {
                return configService;
            }
        }
    }
}
