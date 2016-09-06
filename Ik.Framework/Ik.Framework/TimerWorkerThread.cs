using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ik.Framework
{
    public class TimerWorkerThread
    {
        private readonly object toLock = new object();
        private readonly Action methodToRunInLoop;
        private readonly Thread thread;
        private CancellationTokenSource cancelTokenSource;
        private int millisecondsTimeout;

        public TimerWorkerThread(Action methodToRunInLoop, int millisecondsTimeout)
        {
            this.methodToRunInLoop = methodToRunInLoop;
            thread = new Thread(Loop);
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Name = String.Format("Worker.{0}", thread.ManagedThreadId);
            thread.IsBackground = true;
            this.millisecondsTimeout = millisecondsTimeout;
            this.cancelTokenSource = new CancellationTokenSource();
        }


        public event EventHandler Stopped;


        public void Start()
        {
            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }

        public void Stop()
        {
            this.cancelTokenSource.Cancel();
        }

        public void Kill()
        {
            thread.Abort();
        }

        protected void Loop()
        {
            while (!this.cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    cancelTokenSource.Token.WaitHandle.WaitOne(this.millisecondsTimeout);
                    methodToRunInLoop();
                }
                catch { }
            }

            if (Stopped != null)
            {
                Stopped(this, null);
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
