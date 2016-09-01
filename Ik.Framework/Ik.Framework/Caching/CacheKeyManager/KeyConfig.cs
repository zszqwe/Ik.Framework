using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ik.Framework.Caching.CacheKeyManager
{
    [XmlRoot(KeyFormatConfigurationSectionHandler.ConfigSectionName)]
    public class CacheKeyConfig
    {
        [XmlAttribute("projectName")]
        public string ProjectName { get; set; }

        [XmlAttribute("projectShowName")]
        public string ProjectShowName { get; set; }

        [XmlArray("cacheKeys")]
        [XmlArrayItem("key")]
        public List<KeyItem> CacheKeys { get; set; }
        [XmlArray("cacheValueTypes")]
        [XmlArrayItem("type")]
        public List<KeyTypeDefine> CacheValueTypes { get; set; }

    }

    public class KeyItem
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("desc")]
        public string Desc { get; set; }

        [XmlAttribute("scope")]
        public CacheKeyScope Scope { get; set; }

        [XmlAttribute("cacheType")]
        public string CacheType { get; set; }

        [XmlAttribute("modelName")]
        public string ModelName { get; set; }

        [XmlAttribute("refValueType")]
        public string RefValueType { get; set; }
    }

    public class KeyTypeDefine
    {
        [XmlAttribute("valueType")]
        public string ValueType { get; set; }

        [XmlElement("valueClassCode")]
        public string ValueClassCode { get; set; }

        [XmlElement("valueTypeAssemblyName")]
        public string ValueTypeAssemblyName { get; set; }
    }
}
