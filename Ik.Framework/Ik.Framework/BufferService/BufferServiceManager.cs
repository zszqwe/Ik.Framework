using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.BufferService
{
    public class BufferServiceManager
    {
        public const string LogModelName_BufferService = "数据缓冲服务";
        private static ILog logger = LogManager.GetLogger(BufferServiceManager.LogModelName_BufferService);
        private static BufferDispatcherService _dispatcherService = null;
        private static object _lockObj = new object();
        private static IList<BufferProcessorService> _processors = null;
        private static bool _isInited = false;

        public static void AddBufferProcessorService(IList<BufferProcessorService> processors)
        {
            _processors = processors;
        }

        public static IBackgroundService DispatcherService
        {
            get
            {
                if (_dispatcherService == null)
                {
                    lock (_lockObj)
                    {
                        if (_dispatcherService == null)
                        {
                            logger.Info("数据缓存服务，数据处理器自动注册开始");
                            AutoRegisterService();
                            if (_processors == null || _processors.Count == 0)
                            {
                                throw new BufferServiceException("数据缓冲服务处理器不能为空");
                            }
                            _dispatcherService = new BufferDispatcherService(_processors);
                            _isInited = true;
                        }
                    }
                }
                return _dispatcherService;
            }
        }

        public static bool HasInited
        {
            get
            {
                return _isInited;
            }
        }

        private static void AutoRegisterService()
        {
            var _typeFinder = new AppDomainTypeFinder();
            var bsTypes = _typeFinder.FindClassesOfType<IBufferServiceRegistrar>();
            var bsInstances = new List<IBufferServiceRegistrar>();
            foreach (var bsType in bsTypes)
            {
                bsInstances.Add((IBufferServiceRegistrar)Activator.CreateInstance(bsType));
            }
            bsInstances = bsInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var bsRegistrar in bsInstances)
            {
                if (_processors == null)
                {
                    _processors = new List<BufferProcessorService>();
                    bsRegistrar.Register(_processors);
                }
                else
                {
                    bsRegistrar.Register(_processors);
                }
                logger.Info(string.Format("数据缓存服务，数据处理器注册完成，类型：{0}", bsRegistrar.GetType().FullName));
            }
            logger.Info(string.Format("数据缓存服务，已完成{0}个数据处理器注册", bsInstances.Count));
        }

        public static IBufferTransport TransportService
        {
            get
            {
                return (IBufferTransport)DispatcherService;
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
