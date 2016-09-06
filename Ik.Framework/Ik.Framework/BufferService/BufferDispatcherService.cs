using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public class BufferDispatcherService : BackgroundServiceBase,IBufferTransport
    {
        private static ILog logger = LogManager.GetLogger(BufferServiceManager.LogModelName_BufferService);
        private IEnumerable<BufferProcessorService> processorList = null;
        private List<BufferService> bufferServices = new List<BufferService>();
        private int transportBufferCount = 2;
        private int transportWorkerCount = 2;
        private Random random = new Random();

        public BufferDispatcherService(IEnumerable<BufferProcessorService> processors)
        {
            processorList = new List<BufferProcessorService>(processors);
        }


        protected override void StartService()
        {
            transportBufferCount = ConfigManager.Instance.TryGet<int>("Buffer_DispatcherCount", out transportBufferCount) ? transportBufferCount : 2;
            transportWorkerCount = ConfigManager.Instance.TryGet<int>("Buffer_WorkerCount", out transportWorkerCount) ? transportWorkerCount : 5;
            logger.Info(string.Format("数据缓冲服务开始，分配器：{0}，线程数：{1}", transportBufferCount, transportWorkerCount));
            int index= 0;
            foreach (var item in processorList)
            {
                item.ServiceName = "BufferProcessor_" + index;
                item.ServiceException += serviceException;
                item.Start();
                if (!item.IsRunning)
                {
                    throw new BufferServiceException(string.Format("数据缓冲处理器加载失败，名称{0}", item.GetType().FullName));
                }
                logger.Info(string.Format("数据缓冲处理器加载完成，名称{0}", item.GetType().FullName));
                index++;
            }
            for (int i = 0; i < transportBufferCount; i++)
            {
                BufferService bs = new BufferService();
                bs.NumberOfWorkerThreads = transportWorkerCount;
                bs.ServiceName = "BufferService_" + i;
                bs.ServiceException += serviceException;
                bs.BufferItemReceived += (s, a) => Parallel.ForEach(processorList, p => p.Process(a.Item));
                bs.Start();
                if (!bs.IsRunning)
                {
                    throw new BufferServiceException("数据缓冲分配器启动失败");
                }
                bufferServices.Add(bs);
            }
            logger.Info("数据缓冲分配器启动成功");
        }

        void serviceException(object sender, ErrorEventArgs e)
        {
            OnServiceException(e.Exception);
        }

        protected override void ShutDownService()
        {
            logger.Info("数据缓冲服务结束开始");
            foreach (var item in bufferServices)
            {
                item.Stop();
            }
            logger.Info("数据缓冲分配器结束完成");
            foreach (var item in processorList)
            {
                item.Stop();
            }
            logger.Info("数据缓冲服务结束完成");
        }

        public bool Transport(BufferItem item)
        {
            if (!IsRunning)
            {
                return false;
            }
            int next = 0;
            if (transportBufferCount > 1)
            {
                next = random.Next(0, transportBufferCount);
            }
            var transport = bufferServices[next];
            return transport.Transport(item);
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
