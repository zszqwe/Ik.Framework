using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching
{
    public class CasValue<T>
    {
        /// <summary>
        /// 真正的储存的缓存对象实例
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public ulong Cas { get; set; }
    }

    public class StateValue<T>
    {
        public T Result { get; set; }
    }
}
