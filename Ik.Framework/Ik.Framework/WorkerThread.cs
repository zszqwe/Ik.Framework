using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ik.Framework
{
    public class WorkerThread
    {
        private readonly object toLock = new object();
        private readonly Action methodToRunInLoop;
        private readonly Thread thread;
        private volatile bool stopRequested;


        public WorkerThread(Action methodToRunInLoop)
        {
            this.methodToRunInLoop = methodToRunInLoop;
            thread = new Thread(Loop);
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Name = String.Format("Worker.{0}", thread.ManagedThreadId);
            thread.IsBackground = true;
        }


        protected bool StopRequested
        {
            get
            {
                bool result;
                lock (toLock)
                {
                    result = stopRequested;
                }
                return result;
            }
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
            lock (toLock)
            {
                stopRequested = true;
            }
        }

        public void Kill()
        {
            thread.Abort();
        }

        protected void Loop()
        {
            while (!StopRequested)
            {
                try
                {
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
