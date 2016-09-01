using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching.CacheKeyManager
{
    public enum CacheKeyScope
    {
        Project = 0,
        Global = 1
    }

    public enum CacheEnvType
    {
        Memcached = 0, Redis = 1
    }
}
