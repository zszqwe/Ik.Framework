using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    internal class LocalConfigManager
    {
        public static bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            var type = typeof(T);
            if (type == typeof(string) || type.IsValueType)
            {
                var rawValue = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(rawValue))
                {
                    if (type == typeof(string))
                    {
                        value = (T)((object)rawValue);
                    }
                    if (type.IsGenericType && type.GetGenericTypeDefinition()== typeof(Nullable<>))
                    {
                        value = (T)Convert.ChangeType(rawValue, type.GetGenericArguments()[0]);
                    }
                    else
                    {
                        value = (T)Convert.ChangeType(rawValue, type);
                    }
                    return true;
                }
            }
            else
            {
                object section = ConfigurationManager.GetSection(key);
                if (section != null)
                {
                    value = (T)section;
                    return true;
                }
            }
            return false;
        }
    }
}
