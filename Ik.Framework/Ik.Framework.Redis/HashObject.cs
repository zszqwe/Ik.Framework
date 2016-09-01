using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class HashObject
    {
        public HashObject(HashKeyObject key, HashValueObject value)
        {
            this.Key = key;
            this.Value = value;
        }
        public HashKeyObject Key { get; set; }

        public HashValueObject Value { get; set; }
    }
}
