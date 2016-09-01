using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    public class DistributedObjectNameAttribute : Attribute
    {
        public DistributedObjectNameAttribute(string name)
        {
			this.Name = name;
        }

		public string Name { private set; get; }
    }
}
