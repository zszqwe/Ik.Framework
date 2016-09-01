using Ik.Framework;
using Ik.Framework.Configuration;
using Ik.Service.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public class IkAuthorizationContext
    {
        public static bool _isInited = false;
        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IIkAuthorization Initialize(Func<IIkAuthorization> create, bool forceRecreate = false)
        {
            if (Singleton<IIkAuthorization>.Instance == null || forceRecreate)
            {
                Singleton<IIkAuthorization>.Instance = create();
                Singleton<IIkAuthorization>.Instance.Initialize();
                _isInited = true;
            }
            return Singleton<IIkAuthorization>.Instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IIkAuthorization Initialize(bool forceRecreate = false)
        {
            if (Singleton<IIkAuthorization>.Instance == null || forceRecreate)
            {
                Singleton<IIkAuthorization>.Instance = new IkAuthorization();
                Singleton<IIkAuthorization>.Instance.Initialize();
                _isInited = true;
            }
            return Singleton<IIkAuthorization>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>

        public static IIkAuthorization Current
        {
            get
            {
                if (!_isInited)
                {
                    throw new AuthException("权限服务上下文未初始化");
                }
                return Singleton<IIkAuthorization>.Instance;
            }
        }
    }
}
