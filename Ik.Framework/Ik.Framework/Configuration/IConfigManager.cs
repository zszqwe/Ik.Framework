using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{

    public delegate void ConfigChangedEvent<T>(string key,string path, T value);
    public interface IConfigManager
    {
        bool TryGet<T>(string key, out T value);

        T Get<T>(string key);

        bool RegisterConfigChangedEvent<T>(string key, ConfigChangedEvent<T> changed);
    }
}
