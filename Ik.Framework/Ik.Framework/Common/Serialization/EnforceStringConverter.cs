using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Serialization
{
    public interface IEnforceStringConverterFormat
    {
        string Format(object value);
    }

    public class DefaultEnforceStringConverterFormat : IEnforceStringConverterFormat
    {

        public virtual string Format(object value)
        {
            if (value != null)
            {
                return value.ToString();
            }
            return null;
        }
    }

    public class EnforceStringConverter : JsonConverter
    {
        private IEnforceStringConverterFormat format = null;
        public EnforceStringConverter()
        {
            format = new DefaultEnforceStringConverterFormat();
        }

        public EnforceStringConverter(IEnforceStringConverterFormat format)
        {
            this.format = format;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var flag = IsNullableType(objectType);
            Type realType = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (reader.TokenType == JsonToken.String)
            {
                string text = reader.Value.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    return Convert.ChangeType(text, realType);
                }
            }
            if (flag)
            {
                return null;
            }
            else
            {
                return 0;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(format.Format(value));
        }

        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
