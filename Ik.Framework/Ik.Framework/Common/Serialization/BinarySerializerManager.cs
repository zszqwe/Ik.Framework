using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ik.Framework.Common.Serialization
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public static class BinarySerializerManager
    {
        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <param name="data">要序列化的对象</param>
        /// <returns>byte[]</returns>
        public static byte[] BinarySerialize<T>(T data)
        {
            var serializer = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, data);
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// 反序列化二进制对象
        /// </summary>
        /// <typeparam name="T">要二进制反序列成的类型</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T BinaryDeSerialize<T>(byte[] data)
        {
            var deserializer = new BinaryFormatter();
            using (var stream = new MemoryStream(data))
            {
                return (T)deserializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// 反序列化二进制对象
        /// </summary>
        /// <typeparam name="T">要二进制反序列成的类型</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T BinaryDeSerialize<T>(Stream stream)
        {
            var deserializer = new BinaryFormatter();
            return (T)deserializer.Deserialize(stream);
        }

        /// <summary>
        /// 反序列化二进制对象
        /// </summary>
        /// <typeparam name="T">要二进制反序列成的类型</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object BinaryDeSerialize(byte[] data)
        {
            var deserializer = new BinaryFormatter();
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize(stream);
            }
        }
    }
}
