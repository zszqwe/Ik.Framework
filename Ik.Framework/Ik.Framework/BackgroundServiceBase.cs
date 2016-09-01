using Ik.Framework.Common;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ik.Framework
{
    /// <summary>
    /// 后台自动服务基类
    /// </summary>
    public abstract class BackgroundServiceBase : IBackgroundService
    {
        public long started;

        //服务异常事件
        public event EventHandler<ErrorEventArgs> ServiceException;

        /// <summary>
        /// 服务是否在运行
        /// </summary>
        public bool IsRunning
        {
            get { return Interlocked.Read(ref started) > 0; }
        }

        public string ServiceName { get; set; }

        /// <summary>
        /// 开始服务
        /// </summary>
        public virtual void Start()
        {
            if (IsRunning)
            {
                return;
            }
            try
            {
                StartService();
                Interlocked.Exchange(ref started, 1);
            }
            catch (Exception ex)
            {
                if (!OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure,"服务启动失败",ex)))
                {
                    ILog logger = LogManager.GetLogger("后台服务", ServiceName);
                    logger.Error("服务启动失败", ex);
                }
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            try
            {
                Interlocked.Exchange(ref started, 0);
                ShutDownService();
            }
            catch (Exception ex)
            {
                if (!OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure, "服务停止失败", ex)))
                {
                    ILog logger = LogManager.GetLogger("后台服务", ServiceName);
                    logger.Error("服务停止失败", ex);
                }
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }


        /// <summary>
        /// 重启服务
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        //触发服务启动异常
        protected virtual bool OnServiceException(InkeyException ex)
        {
            EventHandler<ErrorEventArgs> evt = ServiceException;
            if (evt != null)
            {
                evt(this, new ErrorEventArgs(ex));
                return true;
            }
            return false;
        }

        protected abstract void StartService();
        protected abstract void ShutDownService();
    }
}
