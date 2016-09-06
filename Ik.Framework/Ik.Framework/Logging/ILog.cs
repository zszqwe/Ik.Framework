using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Logging
{
    public interface ILog
    {
        void Debug(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime);
        void Debug(string message, string content, string appName, string modelName, string businessName, string lable, int code);
        void Debug(string message, string content, string appName, string modelName, string businessName, string lable);

        void Debug(string message, string content, string appName, string modelName, string businessName);

        void Debug(string message, string content, string appName, string modelName);

        void Debug(string message, string content, string appName);

        void Debug(string message, string appName);

        void Debug(string message, int code);

        void Debug(string message, int code, string content);

        void Debug(string message, int code, Exception ex);
        void Debug(string message, Exception ex);

        void Debug(string message);

        void Error(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime);
        void Error(string message, string content, string appName, string modelName, string businessName, string lable, int code);

        
        void Error(string message, string content, string appName, string modelName, string businessName, string lable);

        void Error(string message, string content, string appName, string modelName, string businessName);

        void Error(string message, string content, string appName, string modelName);

        void Error(string message, string content, string appName);

        void Error(string message, int code);

        void Error(string message, int code, string content);

        void Error(string message,int code, Exception ex);
        void Error(string message, Exception ex);

        void Error(string message, string appName);

        void Error(string message);

        void Info(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime);
        void Info(string message, string content, string appName, string modelName, string businessName, string lable, int code);
        void Info(string message, string content, string appName, string modelName, string businessName, string lable);

        void Info(string message, string content, string appName, string modelName, string businessName);

        void Info(string message, string content, string appName, string modelName);

        void Info(string message, string content, string appName);

        void Info(string message,int code, string content);

        void Info(string message, string appName);

        void Info(string message, int code);

        void Info(string message);

        void Info(string message, int code, Exception ex);
        void Info(string message, Exception ex);
        void Warn(string message, string content, string appName, string modelName, string businessName, string lable, int code, DateTime logTime);
        void Warn(string message, string content, string appName, string modelName, string businessName, string lable, int code);

        void Warn(string message, string content, string appName, string modelName, string businessName, string lable);

        void Warn(string message, string content, string appName, string modelName, string businessName);

        void Warn(string message, string content, string appName, string modelName);

        void Warn(string message, string content, string appName);
        void Warn(string message,int code, string content);
        void Warn(string message, Exception ex);
        void Warn(string message,int code, Exception ex);
        void Warn(string message, string appName);
        void Warn(string message, int code);
        void Warn(string message);

        bool CheckLogLevel(LogLevel level, string appName, string modelName, string businessName, string lable, string serverName);
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
