using Ik.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ik.Framework.Configuration
{
    public class LocalConfigSectionHandler<T> : IConfigurationSectionHandler where T : class
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return XmlSerializerManager.XmlDeserialize<T>(section);
        }
    }
}
