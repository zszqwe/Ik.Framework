using Ik.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Framework.BufferService
{
    public abstract class BufferProcessorService : BackgroundServiceBase
    {
        protected readonly IList<TimerWorkerThread> workerThreads = new List<TimerWorkerThread>();
        private BlockingCollection<BufferItem> sourceArrays = new BlockingCollection<BufferItem>(int.MaxValue);
        private int enforceProcessTime = 8000;
        private DateTime lastProcessTime = DateTime.Now;

        public int EnforceProcessTime
        {
            get { return enforceProcessTime; }
            set { enforceProcessTime = value; }
        }
        private int noticeProcessCount = 100;

        public int NoticeProcessCount
        {
            get { return noticeProcessCount; }
            set { noticeProcessCount = value; }
        }

        public abstract void BufferProcess(IList<BufferItem> items);

        public abstract bool FilterBuffer(BufferItem item);

        public void Process(BufferItem item)
        {
            if (!FilterBuffer(item))
            {
                return;
            }
            if (noticeProcessCount == 1)
            {
                BufferProcess(new List<BufferItem> { item });
            }
            else
            {
                sourceArrays.Add(item);
                if (sourceArrays.Count >= NoticeProcessCount)
                {
                    lastProcessTime = DateTime.Now;
                    BatchProcessMessage();
                }
            }
        }

        private void BatchProcessMessage()
        {
            if (sourceArrays.Count == 0)
            {
                return;
            }
            var takeCount = NoticeProcessCount;
            if (sourceArrays.Count < takeCount)
            {
                takeCount = sourceArrays.Count;
            }
            var items = TakeBufferItems(takeCount);
            if (items.Count > 0)
            {
                try
                {
                    BufferProcess(items);
                }
                catch (Exception ex)
                {
                    foreach (var item in items)
                    {
                        OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure, string.Format("消息写入失败:{0}", item.ToJsonString()), ex));
                    }
                }

            }
        }

        private IList<BufferItem> TakeBufferItems(int count)
        {
            List<BufferItem> items = new List<BufferItem>();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    CancellationTokenSource cancelTokenSource = new CancellationTokenSource(300);
                    var item = sourceArrays.Take(cancelTokenSource.Token);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure, "当前缓存服务已关闭", ex));
                    break;

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnServiceException(new InkeyException(InkeyErrorCodes.CommonFailure, "获取缓冲数据项失败", ex));
                    break;
                }
            }

            return items;
        }

        private TimerWorkerThread AddWorkerThread()
        {
            lock (workerThreads)
            {
                var result = new TimerWorkerThread(() =>
                {
                    if (DateTime.Now - lastProcessTime < TimeSpan.FromMilliseconds(enforceProcessTime * 0.7))
                    {
                        return;
                    }
                    BatchProcessMessage();
                }, enforceProcessTime);

                workerThreads.Add(result);

                result.Stopped += delegate(object sender, EventArgs e)
                {
                    var wt = sender as TimerWorkerThread;
                    lock (workerThreads)
                    {
                        workerThreads.Remove(wt);
                    }
                };

                return result;
            }
        }

        protected override void StartService()
        {
            var worker = AddWorkerThread();
            worker.Start();
        }

        protected override void ShutDownService()
        {
            sourceArrays.CompleteAdding();
            var list = workerThreads.ToList();
            foreach (var item in list)
            {
                item.Stop();
            }
            while (workerThreads.Count > 0)
            {
                Thread.Sleep(200);
            }
            if (sourceArrays.Count > 0)
            {
                BatchProcessMessage();
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
