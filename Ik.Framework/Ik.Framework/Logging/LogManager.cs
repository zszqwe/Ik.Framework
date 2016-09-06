using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Framework.Logging
{
    public class LogManager
    {
        private static ILog logger = LogManager.GetLogger("系统日志");
        private static string _defaultAppName = Environment.MachineName;
        private static LogConfig _config = new LogConfig();
        private static Dictionary<string, LogLevel> _logLevelDict = new Dictionary<string, LogLevel>();
        private static Dictionary<string, LogWorkType> _logWorkTypeDict = new Dictionary<string, LogWorkType>();
        private static LogLevel _defaultLevel = LogLevel.INFO | LogLevel.ERROR | LogLevel.WARN;
        private static LogWorkType _defaultWorkType = LogWorkType.Local;


        public static void Initialize()
        {
            var appName = AppInfo.AppName;
            if (!string.IsNullOrEmpty(appName))
            {
                _defaultAppName = appName;
            }
            var config = ConfigManager.Instance.Get<LogConfig>(LogConfigurationSectionHandler.ConfigSectionName);
            if (config != null)
            {
                _config = config;
                ApplyConfig();
            }
        }

        public static void Initialize(LogLevel defaultLevel, LogWorkType defaultWorkType)
        {
            _defaultLevel = defaultLevel;
            _defaultWorkType = defaultWorkType;
            Initialize();
        }

        public static void EnableRemoteConfig()
        {
            if (ConfigManager.RemoteConfigService.IsRunning)
            {
                ConfigManager.Instance.RegisterConfigChangedEvent<LogConfig>(LogConfigurationSectionHandler.ConfigSectionName, new ConfigChangedEvent<LogConfig>((k, p, newConfig) =>
                {
                    logger.Info("日志配置已变化，值：" + newConfig.ToJsonString());
                    _config = newConfig;
                    ApplyConfig();
                }));
            }
        }

        private static void ApplyConfig()
        {
            if (_config != null)
            {
                Dictionary<string, LogLevel> newLogLevelDict = new Dictionary<string, LogLevel>();
                Dictionary<string, LogWorkType> logWorkTypeDict = new Dictionary<string, LogWorkType>();
                foreach (var item in _config.Rules)
                {
                    LogLevel newLevel = (LogLevel)Enum.Parse(typeof(LogLevel), item.Level);
                    string channelKey = "local";

                    string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", item.AppName, item.ModelName, item.BusinessName, item.Lable, item.ServerName, channelKey);
                    if (newLogLevelDict.ContainsKey(key))
                    {
                        LogLevel level = newLogLevelDict[key];
                        newLogLevelDict[key] = level | newLevel;
                    }
                    else
                    {
                        newLogLevelDict.Add(key, newLevel);
                    }
                    logWorkTypeDict[key] = (LogWorkType)Enum.Parse(typeof(LogWorkType), item.WorkType);

                }
                _logLevelDict = newLogLevelDict;
                _logWorkTypeDict = logWorkTypeDict;
                _defaultLevel = (LogLevel)Enum.Parse(typeof(LogLevel), _config.DefaultLevel);
                _defaultWorkType = (LogWorkType)Enum.Parse(typeof(LogWorkType), _config.DefaultWorkType);
            }
        }

        public static string DefaultAppName
        {
            get { return _defaultAppName; }
            set {
                LogManager._defaultAppName = value;
            }
        }

        public static ILog GetLogger()
        {
            return new DefaultLogger(null, null, null);
        }

        public static ILog GetLogger(string modelName)
        {
            return new DefaultLogger(modelName, null, null);
        }

        public static ILog GetLogger(string modelName, string businessName)
        {
            return new DefaultLogger(modelName, businessName, null);
        }

        public static ILog GetLogger(string modelName, string businessName, string lable)
        {
            return new DefaultLogger(modelName, businessName, lable);
        }

        internal static LogLevel? GetLogLevel(string appName, string modelName, string businessName, string lable, string serverName)
        {
            string key = string.Format("{0}_{1}_{2}_{3}_{4}_local", appName, modelName, businessName, lable, serverName);
            if (_logLevelDict.ContainsKey(key))
            {
                return _logLevelDict[key];
            }
            return null;
        }

        internal static bool MatchLogLevel(LogLevel logLevel, string appName, string modelName, string businessName, string lable, string serverName)
        {
            if (_logLevelDict.Count == 0)
            {
                return _defaultLevel.HasFlag(logLevel);
            }
            var level = LogManager.GetLogLevel(appName, modelName, businessName, lable, serverName);
            if (level.HasValue)
            {
                if (level.Value.HasFlag(logLevel))
                {
                    return true;
                }
                return false;
            }
            level = LogManager.GetLogLevel(appName, modelName, businessName, lable, "");
            if (level.HasValue)
            {
                if (level.Value.HasFlag(logLevel))
                {
                    return true;
                }
                return false;
            }
            level = LogManager.GetLogLevel(appName, modelName, businessName, "", "");
            if (level.HasValue)
            {
                if (level.Value.HasFlag(logLevel))
                {
                    return true;
                }
                return false;
            }
            level = LogManager.GetLogLevel(appName, modelName, "", "", "");
            if (level.HasValue)
            {
                if (level.Value.HasFlag(logLevel))
                {
                    return true;
                }
                return false;
            }
            level = LogManager.GetLogLevel(appName, "", "", "", "");
            if (level.HasValue)
            {
                if (level.Value.HasFlag(logLevel))
                {
                    return true;
                }
                return false;
            }
            return _defaultLevel.HasFlag(logLevel);
        }

        internal static LogWorkType? GetLogWorkType(string appName, string modelName, string businessName, string lable, string serverName)
        {
            string key = string.Format("{0}_{1}_{2}_{3}_{4}_local", appName, modelName, businessName, lable, serverName);
            if (_logWorkTypeDict.ContainsKey(key))
            {
                return _logWorkTypeDict[key];
            }
            return null;
        }

        internal static LogWorkType MatchLogWorkType(string appName, string modelName, string businessName, string lable, string serverName)
        {
            if (_logWorkTypeDict.Count == 0)
            {
                return _defaultWorkType;
            }
            var workType = LogManager.GetLogWorkType(appName, modelName, businessName, lable, serverName);
            if (workType.HasValue)
            {
                return workType.Value == LogWorkType.None? _defaultWorkType :workType.Value;
            }
            workType = LogManager.GetLogWorkType(appName, modelName, businessName, lable, "");
            if (workType.HasValue)
            {
                return workType.Value == LogWorkType.None ? _defaultWorkType : workType.Value;
            }
            workType = LogManager.GetLogWorkType(appName, modelName, businessName, "", "");
            if (workType.HasValue)
            {
                return workType.Value == LogWorkType.None ? _defaultWorkType : workType.Value;
            }
            workType = LogManager.GetLogWorkType(appName, modelName, "", "", "");
            if (workType.HasValue)
            {
                return workType.Value == LogWorkType.None ? _defaultWorkType : workType.Value;
            }
            workType = LogManager.GetLogWorkType(appName, "", "", "", "");
            if (workType.HasValue)
            {
                return workType.Value == LogWorkType.None ? _defaultWorkType : workType.Value;
            }
            return _defaultWorkType;
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
