using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    internal class LocalPublisherContext
    {
        private static ILog logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
        private static bool _isInited = false;

        public static bool IsInited
        {
            get { return LocalPublisherContext._isInited; }
            set { LocalPublisherContext._isInited = value; }
        }

        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEventPublisher Initialize(Func<ISubscriptionService, IEventPublisher> create, ISubscriptionService subscriptionService, bool forceRecreate = false)
        {
            if (Singleton<IEventPublisher>.Instance == null || forceRecreate)
            {
                logger.Info("事件发布上下文开始初始化");
                Singleton<IEventPublisher>.Instance = create(subscriptionService);
                _isInited = true;
            }
            return Singleton<IEventPublisher>.Instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEventPublisher Initialize(bool forceRecreate = false)
        {
            if (Singleton<IEventPublisher>.Instance == null || forceRecreate)
            {
                logger.Info("事件发布上下文开始初始化");
                Singleton<IEventPublisher>.Instance =new EventPublisher(new SubscriptionService());
                _isInited = true;
            }
            return Singleton<IEventPublisher>.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEventPublisher Current
        {
            get
            {
                if (!_isInited)
                {
                    throw new EventException("订阅服务上下文未初始化");
                }
                return Singleton<IEventPublisher>.Instance;
            }
        }

        #endregion
    }
}
