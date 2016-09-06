using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ik.Framework.BufferService;
using System.Collections.Generic;
using Ik.Framework.Logging;


namespace Ik.Framework.Test.BufferService
{
    [TestClass]
    public class BufferServiceManagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            BufferServiceManager.AddBufferProcessorService(new List<BufferProcessorService> { new  LogBufferProcessorService(),new SilverDollarBufferProcessorService()});
            BufferServiceManager.DispatcherService.Start();
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Tasks.Task.Run(new Action(() =>
                {
                    BufferServiceManager.TransportService.Transport(new LogBufferItem { Id = Guid.NewGuid(), Message = "测试_" + DateTime.Now });
                }));
                System.Threading.Tasks.Task.Run(new Action(() =>
                {
                    BufferServiceManager.TransportService.Transport(new SilverDollarBufferItem { Id = Guid.NewGuid(), SilverDollarName = "银元名称_" + DateTime.Now });
                }));

                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
            BufferServiceManager.DispatcherService.Stop();
        }
    }

    public class LogBufferProcessorService : BufferProcessorService
    {
        public override void BufferProcess(IList<BufferItem> items)
        {
            
        }

        public override bool FilterBuffer(BufferItem item)
        {
            return item is LogBufferItem;
        }
    }

    public class SilverDollarBufferProcessorService : BufferProcessorService
    {
        public override void BufferProcess(IList<BufferItem> items)
        {

        }

        public override bool FilterBuffer(BufferItem item)
        {
            return item is SilverDollarBufferItem;
        }
    }

    public class LogBufferItem : BufferItem
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 日志标签
        /// </summary>
        public string Lable { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 机器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 日志码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string Content { get; set; }
    }


    public class SilverDollarBufferItem : BufferItem
    {
        public Guid Id { get; set; }

        public string SilverDollarName { get; set; }

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
