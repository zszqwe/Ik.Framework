using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ik.Framework.Configuration;
using Ik.Framework.Logging;


namespace Ik.Framework.Test.Configuration
{
    [TestClass]
    public class RemoteConfigManagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            ConfigManager.RemoteConfigService.Start();
            var appName = ConfigManager.Instance.Get<string>("AppName");
            var config = ConfigManager.Instance.Get<LogConfig>("logConfig");
            ConfigManager.RemoteConfigService.Stop();
        }
    }
}
