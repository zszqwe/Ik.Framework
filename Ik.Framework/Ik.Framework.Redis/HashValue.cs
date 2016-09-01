using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class HashValue<TKey, TValue>
    {
        public HashValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}
