using Ik.Framework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    public static class AppInfo
    {
        public static DeployInfo DeployEnvInfo { get; private set; }
        public static string AppCode { get; private set; }

        public static string AppName { get; private set; }
        public static Version AppVer { get; private set; }
        public static string MachineName { get; private set; }

        static AppInfo()
        {
            DeployEnvInfo = DeployInfo.Instance;

            AppCode = ConfigurationManager.AppSettings["app_code"];
            if (!string.IsNullOrEmpty(AppCode))
                AppCode = AppCode.ToLower();
            else
                AppCode = "";

            var appVer = ConfigurationManager.AppSettings["app_ver"];
            if (!string.IsNullOrEmpty(appVer))
            {
                AppVer = VersionHelper.FormatVersion(appVer);
            }
            else
            {
                AppVer = new Version("1.0.0.0");
            }

            var appName = ConfigurationManager.AppSettings["app_name"];
            if (!string.IsNullOrEmpty(appName))
            {
                AppName = appName;
            }

            MachineName = Environment.MachineName.ToLower();
        }
    }

    public class DeployInfo : DefaultSingleton<DeployInfo>
    {
        public bool IsDev { get; private set; }
        public bool IsTest { get; private set; }
        public bool IsPrePublish { get; private set; }
        public bool IsFormal { get; private set; } 
        public bool IsStressTest { get; private set; }
        public bool IsOther { get; private set; }

        public string DeployKey { get; private set; }

        public DeployInfo()
        {
            DeployKey = ConfigurationManager.AppSettings["app_env"];
            if (string.IsNullOrEmpty(DeployKey))
            {
                DeployKey = "DevServer";
            }
            switch (DeployKey.ToLower())
            {
                case "devserver": 
                    IsDev = true; 
                    break;
                case "testserver": 
                    IsTest = true; 
                    break;
                case "prepublishserver": 
                    IsPrePublish = true; 
                    break;
                case "formalserver": 
                    IsFormal = true; 
                    break;
                case "stresstestserver": 
                    IsStressTest = true; 
                    break;
                default: 
                    IsOther = true;
                    break;
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
