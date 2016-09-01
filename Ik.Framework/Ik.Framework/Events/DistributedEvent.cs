using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using Ik.Framework.Common.Serialization;

namespace Ik.Framework.Events
{
    public class DistributedEvent
    {
        private object _value = null;
        public DistributedEvent()
		{
		 
		}

        public DistributedEvent(object value, string topic,string lable, string objectName)
        {
            this.ObjectName = objectName;
			this.Topic = topic;
            this._value = value;
            this.Playload = JsonSerializerManager.BsonSerialize(value);
            this.Lable = lable;
        }

        
        public string ObjectName { get; set; }

		public string Topic { set; get; }

        public string Lable { get; set; }

		public byte[] Playload { set; get; }

        public object GetValue(Type type)
        {
            if (this._value != null)
            {
                return this._value;
            }
            return JsonSerializerManager.BsonDeSerialize(this.Playload, type);
        }

        public object GetValue()
        {
            if (this._value != null)
            {
                return this._value;
            }
            return null;
        }

        public string GetJsonString()
        {
            if (this._value != null)
            {
                return this._value.ToJsonString();
            }
            return string.Empty;
        }
    }
}
