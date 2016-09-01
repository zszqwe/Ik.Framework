using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ik.Framework.Caching
{
    [XmlRoot("memcached")]
    public class MemcachedConfig
    {
        [XmlAttribute("connectionTimeout")]
        public int ConnectionTimeout { get; set; }

        [XmlAttribute("deadTimeout")]
        public int DeadTimeout { get; set; }

        [XmlAttribute("maxPoolSize")]
        public int MaxPoolSize { get; set; }

        [XmlAttribute("ninPoolSize")]
        public int MinPoolSize { get; set; }

        [XmlArray("servers")]
        [XmlArrayItem("add")]
        public List<MemcachedServer> Servers { get; set; }

        [XmlArray("authParameters")]
        [XmlArrayItem("add")]
        public List<AuthenticationParameter> AuthParameters { get; set; }
    }

    public class AuthenticationParameter
    {
        [XmlAttribute("key")]
        public string Key { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public class MemcachedServer
    {
        [XmlAttribute("address")]
        public string Address { get; set; }

        [XmlAttribute("port")]
        public int Port { get; set; }

        [XmlAttribute("type")]
        public AddressType AddressType { get; set; }
    }

    public enum AddressType
    {
        Host, Ip
    }


}
