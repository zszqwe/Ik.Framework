using Ik.Framework.Caching.CacheKeyManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    public class IKCacheFactory : IKeyConfigRegistrar
    {
        public CacheKeyConfig RegisterConfig()
        {
            return KeyConfigManager.GetKeyConfigFormAssembly(typeof(IKCacheFactory).Assembly);
        }

        private static ICacheKeyFormat cacheKeyFormat = null;
        public static ICacheKeyFormat CacheKeyFormat
        {
            get
            {
                if (cacheKeyFormat == null)
                {
                    cacheKeyFormat = new DefaultKeyFormat(KeyConfigManager.GetFormatObjcetDictFromAssembly(typeof(IKCacheFactory).Assembly));
                }
                return cacheKeyFormat;
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
