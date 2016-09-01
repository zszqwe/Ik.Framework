using Ik.Framework.DependencyManagement;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    internal class LocalSubscribeContext
    {
        private static ILog logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
        private static bool _isInited = false;

        public static bool IsInited
        {
            get { return LocalSubscribeContext._isInited; }
            set { LocalSubscribeContext._isInited = value; }
        }

        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ISubscriptionService Initialize(Func<ISubscriptionService> create, bool forceRecreate = false)
        {
            if (Singleton<ISubscriptionService>.Instance == null || forceRecreate)
            {
                logger.Info("事件订阅上下文开始初始化");
                Singleton<ISubscriptionService>.Instance = create();
                _isInited = true;
            }
            return Singleton<ISubscriptionService>.Instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ISubscriptionService Initialize(bool forceRecreate = false)
        {
            if (Singleton<ISubscriptionService>.Instance == null || forceRecreate)
            {
                logger.Info("事件订阅上下文开始初始化");
                Singleton<ISubscriptionService>.Instance = new SubscriptionService();
                _isInited = true;
            }
            return Singleton<ISubscriptionService>.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static ISubscriptionService Current
        {
            get
            {
                if (!_isInited)
                {
                    throw new EventException("订阅服务上下文未初始化");
                }
                return Singleton<ISubscriptionService>.Instance;
            }
        }

        #endregion
    }
}
