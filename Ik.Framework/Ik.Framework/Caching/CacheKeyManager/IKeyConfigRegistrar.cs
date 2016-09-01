using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching.CacheKeyManager
{
    public interface IKeyConfigRegistrar
    {
        CacheKeyConfig RegisterConfig();
    }
}
