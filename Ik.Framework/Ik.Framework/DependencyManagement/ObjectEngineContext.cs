using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DependencyManagement
{
    public class ObjectEngineContext
    {
        public const string LogModelName_ObjectService = "对象容器服务";
        private static ILog logger = LogManager.GetLogger(ObjectEngineContext.LogModelName_ObjectService);
        public static bool _isInited = false;

        #region Methods

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(Func<IEngine> create, bool forceRecreate = false)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                logger.Info("对象容器服务开始初始化");
                Singleton<IEngine>.Instance = create();
                Singleton<IEngine>.Instance.Initialize();
                logger.Info("对象容器服务开始初始化完成");
            }
            return Singleton<IEngine>.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (!_isInited)
                {
                    throw new IocException("对象容器服务上下文未初始化");
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
