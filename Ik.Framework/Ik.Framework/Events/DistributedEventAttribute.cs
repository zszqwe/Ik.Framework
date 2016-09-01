using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DistributedEventAttribute : Attribute
    {
        public DistributedEventAttribute(HandleType handleType, string topic)
        {
            this.HandleType = handleType;
			this.Topic = topic;
			if(string.IsNullOrEmpty(this.Topic))
			{
                throw new EventException("订阅主题不能为空");
			}
        }
        public HandleType HandleType { private set; get; }

		public string Topic { private set; get; }
    }

    public enum HandleType
    {
        Sync,Asyn
    }
}
