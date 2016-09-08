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
        public static string XmlSerializer<T>(T serialObject)
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
        public static T XmlDeserialize<T>(byte[] data)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(data))
            {
                return (T)xs.Deserialize(stream);
            }
        }

        public static T XmlDeserialize<T>(Stream stream)
        {
            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(stream);
        }

        public static T XmlDeserialize<T>(string xmlData)
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                var xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(sr);
            }
        }

        public static T XmlDeserialize<T>(XmlNode section) 
        {
            var ser = new XmlSerializer(typeof(T));
            var xmlReader = new XmlNodeReader(section);
            return (T)ser.Deserialize(xmlReader);
        }

    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
