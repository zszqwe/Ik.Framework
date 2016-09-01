using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Events
{
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class EventAutoSubscribeAttribute : Attribute
    {
        public EventAutoSubscribeAttribute(bool isAutoSubscribe)
        {
            this.IsAutoSubscribe = isAutoSubscribe;
        }
        public bool IsAutoSubscribe { get; private set; }
    }
}
