using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class SortedSetValue<T>
    {
        public SortedSetValue(T value, double score)
        {
            this.Value = value;
            this.Score = score;
        }
        public T Value { get; set; }

        public double Score { get; set; }
    }
}
