
using Ik.Framework.BufferService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLogger = Common.Logging;

namespace Ik.Framework.Logging
{
    internal class DefaultLogger : ILog
    {
        private static readonly CommonLogger.ILog loggerForDebug = CommonLogger.LogManager.GetLogger("DefaultLogger_Debug");
        private static readonly CommonLogger.ILog loggerForInfo = CommonLogger.LogManager.GetLogger("DefaultLogger_Info");
        private static readonly CommonLogger.ILog loggerForError = CommonLogger.LogManager.GetLogger("DefaultLogger_Error");
        private static readonly CommonLogger.ILog loggerForWarn = CommonLogger.LogManager.GetLogger("DefaultLogger_Warn");
        private string defaultModelName = string.Empty;
        private string defaultBusinessName = string.Empty;
        private string defaultLable = string.Empty;
        private string defaultServerName = Environment.MachineName;

        public DefaultLogger(string defaultModelName, string defaultBusinessName, string defaultLable)
        {
            this.defaultModelName = defaultModelName;
            this.defaultBusinessName = defaultBusinessName;
            this.defaultLable = defaultLable;
        }

        public void Debug(string message, string content, string appName, string modelName, string businessName, string lable, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                Code = code,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string content, string appName, string modelName, string businessName, string lable)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string content, string appName, string modelName, string businessName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string content, string appName, string modelName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string content, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message,int code, string content)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, int code, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName,
                Code = code
            });
        }

        public void Debug(string message, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string content, string appName, string modelName, string businessName, string lable, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string content, string appName, string modelName, string businessName, string lable)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string content, string appName, string modelName, string businessName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string content, string appName, string modelName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string content, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message,int code, string content)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Error(string message,int code, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = ex.ToString(),
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string content, string appName, string modelName, string businessName, string lable, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string content, string appName, string modelName, string businessName, string lable)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string content, string appName, string modelName, string businessName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string content, string appName, string modelName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string content, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message,int code, string content)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Info(string message, int code, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName,
                Code = code
            });
        }

        public void Info(string message, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string content, string appName, string modelName, string businessName, string lable,int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string content, string appName, string modelName, string businessName, string lable)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string content, string appName, string modelName, string businessName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string content, string appName, string modelName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string content, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message,int code, string content)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, int code)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = ex.ToString(),
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, int code, Exception ex)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = ex.ToString(),
                Code = code,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message, string appName)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                AppName = appName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Warn(string message)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                AppName = LogManager.DefaultAppName,
                ModelName = defaultModelName,
                BusinessName = defaultBusinessName,
                Lable = defaultLable,
                ServerName = defaultServerName
            });
        }

        public void Debug(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.DEBUG,
                Message = message,
                Content = content,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                Code = code,
                ServerName = defaultServerName,
                CreateTime = logTime
            });
        }

        public void Error(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.ERROR,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName,
                CreateTime = logTime
            });
        }

        public void Info(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.INFO,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName,
                CreateTime = logTime
            });
        }

        public void Warn(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime)
        {
            this.Write(new LogEntity
            {
                Level = LogLevel.WARN,
                Message = message,
                Content = content,
                Code = code,
                AppName = appName,
                ModelName = modelName,
                BusinessName = businessName,
                Lable = lable,
                ServerName = defaultServerName,
                CreateTime = logTime
            });
        }

        private void Write(LogEntity log)
        {
            var workType = LogManager.MatchLogWorkType(log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName);
            if (workType == LogWorkType.Local || workType == LogWorkType.LocalAndBuffer)
            {
                switch (log.Level)
                {
                    case LogLevel.DEBUG:
                        if (LogManager.MatchLogLevel(log.Level,log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                        {
                            loggerForDebug.Debug(log.ToString());
                        }
                        break;
                    case LogLevel.ERROR:
                        if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                        {
                            loggerForError.Error(log.ToString());
                        }
                        break;
                    case LogLevel.WARN:
                        if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                        {
                            loggerForWarn.Warn(log.ToString());
                        }
                        break;
                    case LogLevel.INFO:
                        if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                        {
                            loggerForInfo.Info(log.ToString());
                        }
                        break;
                }
            }

            if (workType == LogWorkType.Buffer || workType == LogWorkType.LocalAndBuffer)
            {
                if (BufferServiceManager.HasInited && BufferServiceManager.DispatcherService.IsRunning)
                {
                    switch (log.Level)
                    {
                        case LogLevel.DEBUG:
                            if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                            {
                                BufferServiceManager.TransportService.Transport(log);
                            }
                            break;
                        case LogLevel.ERROR:
                            if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                            {
                                BufferServiceManager.TransportService.Transport(log);
                            }
                            break;
                        case LogLevel.WARN:
                            if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                            {
                                BufferServiceManager.TransportService.Transport(log);
                            }
                            break;
                        case LogLevel.INFO:
                            if (LogManager.MatchLogLevel(log.Level, log.AppName, log.ModelName, log.BusinessName, log.Lable, log.ServerName))
                            {
                                BufferServiceManager.TransportService.Transport(log);
                            }
                            break;
                    }
                }
            }
        }





        public bool CheckLogLevel(LogLevel level,string appName, string modelName, string businessName, string lable, string serverName)
        {
            return LogManager.MatchLogLevel(level, appName, modelName, businessName, lable, serverName);
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
