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
