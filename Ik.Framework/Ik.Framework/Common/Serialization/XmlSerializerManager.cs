using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ik.Framework.Common.Serialization
{
    /// <summary>
    /// XML序列化
    /// </summary>
    public static class XmlSerializerManager
    {
        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T serialObject) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8);
            ser.Serialize(writer, serialObject);
            writer.Close();
            return Encoding.UTF8.GetString(mem.ToArray());
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T">要XML反序列化成的类型</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(byte[] data) where T : class
        {
            var xs = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(data))
            {
                return xs.Deserialize(stream) as T;
            }
        }

        public static T XmlDeserialize<T>(Stream stream) where T : class
        {
            var xs = new XmlSerializer(typeof(T));
            return xs.Deserialize(stream) as T;
        }

        public static T XmlDeserialize<T>(string xmlData) where T : class
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                var xs = new XmlSerializer(typeof(T));
                return xs.Deserialize(sr) as T;
            }
        }

        public static T XmlDeserialize<T>(XmlNode section) where T : class
        {
            var ser = new XmlSerializer(typeof(T));
            var xmlReader = new XmlNodeReader(section);
            return ser.Deserialize(xmlReader) as T;
        }

    }
}
