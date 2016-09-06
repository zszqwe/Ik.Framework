
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

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
