using Ik.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    internal class BufferService : BackgroundServiceBase,IBufferTransport
    {
        private static ILog logger = LogManager.GetLogger("数据缓冲服务");
        protected readonly IList<WorkerThread> workerThreads = new List<WorkerThread>();
        private int numberOfWorkerThreads = 1;
        public event EventHandler<BufferItemEventArgs> BufferItemReceived;
        private BlockingCollection<BufferItem> sourceArrays = new BlockingCollection<BufferItem>(5000);

        public bool IsWorkerThreadBusy
        {
            get { return workerThreads.Count > 0; }
        }

        public int NumberOfWorkerThreads
        {
            get
            {
                if (IsRunning)
                {
                    lock (workerThreads)
                    {
                        return workerThreads.Count;
                    }
                }
                return numberOfWorkerThreads;
            }
            set
            {
                ChangeNumberOfWorkerThreads(value);
            }
        }

        /// <summary>
        /// 更改监听线程数
        /// </summary>
        /// <param name="targetNumberOfWorkerThreads">线程数</param>
        public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads)
        {
            lock (workerThreads)
            {
                numberOfWorkerThreads = targetNumberOfWorkerThreads;
                if (IsRunning == false)
                {
                    return;
                }
                int current = workerThreads.Count;
                if (targetNumberOfWorkerThreads == current)
                {
                    return;
                }
                if (targetNumberOfWorkerThreads < current)
                {
                    for (int i = targetNumberOfWorkerThreads; i < current; i++)
                    {
                        workerThreads[i].Stop();
                    }
                    return;
                }

                if (targetNumberOfWorkerThreads > current)
                {
                    for (int i = current; i < targetNumberOfWorkerThreads; i++)
                    {
                        AddWorkerThread().Start();
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 添加一个线程监听
        /// </summary>
        /// <returns></returns>
        protected WorkerThread AddWorkerThread()
        {
            lock (workerThreads)
            {
                var result = new WorkerThread(Receive);

                workerThreads.Add(result);

                result.Stopped += delegate(object sender, EventArgs e)
                {
                    var wt = sender as WorkerThread;
                    lock (workerThreads)
                    {
                        workerThreads.Remove(wt);
                    }
                };

                return result;
            }
        }

        //消息接收完成事件
        protected virtual void OnTransportMessageReceived(BufferItemEventArgs args)
        {
            EventHandler<BufferItemEventArgs> evt = BufferItemReceived;
            if (evt != null)
            {
                evt(this, args);
            }
        }

        /// <summary>
        /// 监听消息
        /// </summary>
        protected virtual void Receive()
        {
            BufferItem item = null;
            try
            {

                //从监听地址中获取一个消息
                item = sourceArrays.Take();

                //触发消息接收事件
                OnTransportMessageReceived(new BufferItemEventArgs(item));
            }
            catch (Exception ex)
            {
                if (!IgnoreException(ex))
                {
                    OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure, "数据缓冲服务监听消息错误", ex));
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        protected virtual bool IgnoreException(Exception ex)
        {
            return ex is ThreadAbortException;
        }

        protected override void StartService()
        {
            for (int i = 0; i < numberOfWorkerThreads; i++)
            {
                AddWorkerThread().Start();
            }
        }

        protected override void ShutDownService()
        {
            logger.Info("数据缓冲服务结束开始");
            while (true)
            {
                if (sourceArrays.Count == 0)
                {
                    break;
                }
                Thread.Sleep(200);
            }
            
            sourceArrays.CompleteAdding();

            var list = workerThreads.ToList();
            foreach (var item in list)
            {
                item.Stop();
            }
            while (IsWorkerThreadBusy)
            {
                Thread.Sleep(200);
            }
            logger.Info("数据缓冲服务结束完成");
        }

        public bool Transport(BufferItem item)
        {
            if (!IsRunning)
            {
                return false;
            }
            sourceArrays.Add(item);
            return true;
        }
    }
}
