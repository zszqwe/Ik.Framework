using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
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
    /// Json序列化
    /// </summary>
    public static class JsonSerializerManager
    {
        #region Json序列化

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T data)
        {
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.SerializeObject(data, settings);
        }

        /// <summary>
        ///  JSON反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object JsonDeserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string JsonFormat(string json)
        {
            try
            {
                StringReader sr = new StringReader(json);
                Newtonsoft.Json.JsonTextReader tr = new Newtonsoft.Json.JsonTextReader(sr);
                StringBuilder sb = new StringBuilder();
                bool hasInArray = false;
                bool hasPropertyStart = false;
                while (tr.Read())
                {
                    switch (tr.TokenType)
                    {
                        case Newtonsoft.Json.JsonToken.Boolean:
                            if (hasInArray)
                            {
                                sb.AppendFormat("{0},", (bool)tr.Value ? "true" : "false");
                            }
                            else
                            {
                                sb.AppendFormat("{0}", (bool)tr.Value ? "true" : "false");
                            }
                            break;
                        case Newtonsoft.Json.JsonToken.Bytes:
                            break;
                        case Newtonsoft.Json.JsonToken.Comment:
                            break;
                        case Newtonsoft.Json.JsonToken.Date:
                            sb.AppendFormat("\"{0}\"", string.Format("{0:s}.{0:fffffff}{0:zzz}", tr.Value));
                            break;
                        case Newtonsoft.Json.JsonToken.EndArray:
                            hasInArray = false;
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append("]");
                            break;
                        case Newtonsoft.Json.JsonToken.EndConstructor:
                            break;
                        case Newtonsoft.Json.JsonToken.EndObject:
                            sb.Append("}");
                            break;
                        case Newtonsoft.Json.JsonToken.Float:
                            if (hasInArray)
                            {
                                sb.AppendFormat("{0},", tr.Value);
                            }
                            else
                            {
                                sb.AppendFormat("{0}", tr.Value);
                            }
                            break;
                        case Newtonsoft.Json.JsonToken.Integer:
                            if (hasInArray)
                            {
                                sb.AppendFormat("{0},", tr.Value);
                            }
                            else
                            {
                                sb.AppendFormat("{0}", tr.Value);
                            }
                            break;
                        case Newtonsoft.Json.JsonToken.None:
                            break;
                        case Newtonsoft.Json.JsonToken.Null:
                            sb.Append("null");
                            break;
                        case Newtonsoft.Json.JsonToken.PropertyName:
                            if (hasPropertyStart)
                            {
                                sb.AppendFormat("\"{0}\":", tr.Value);
                            }
                            else
                            {
                                sb.AppendFormat(",\"{0}\":", tr.Value);
                            }
                            hasPropertyStart = false;
                            break;
                        case Newtonsoft.Json.JsonToken.Raw:
                            break;
                        case Newtonsoft.Json.JsonToken.StartArray:
                            hasInArray = true;
                            sb.Append("[");
                            break;
                        case Newtonsoft.Json.JsonToken.StartConstructor:
                            break;
                        case Newtonsoft.Json.JsonToken.StartObject:
                            hasPropertyStart = true;
                            sb.Append("{");
                            break;
                        case Newtonsoft.Json.JsonToken.String:
                            if (hasInArray)
                            {
                                sb.AppendFormat("\"{0}\",", tr.Value);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\"", tr.Value);
                            }
                            break;
                        case Newtonsoft.Json.JsonToken.Undefined:
                            break;
                        default:
                            break;
                    }
                }
                return sb.ToString();
            }
            catch { }
            return json;
        }

        #endregion

        #region Bson序列化

        /// <summary>
        /// Bson二进制序列化
        /// </summary>
        /// <param name="data">要序列化的对象</param>
        /// <returns>byte[]</returns>
        public static byte[] BsonSerialize(object data)
        {
            using (var stream = new MemoryStream())
            {
                var bsonWriter = new BsonWriter(stream);
                var serializer = new JsonSerializer();
                serializer.Serialize(bsonWriter, data);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Bson反序列化二进制对象
        /// </summary>
        /// <typeparam name="T">要Bson反序列成的类型</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T BsonDeSerialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var reader = new BsonReader(stream);
                if (typeof(ICollection).IsAssignableFrom(typeof(T)))
                {
                    reader.ReadRootValueAsArray = true;
                }
                var serializer = new JsonSerializer(); ;
                return serializer.Deserialize<T>(reader);
            }
        }

        public static object BsonDeSerialize(byte[] data, Type type)
        {
            using (var stream = new MemoryStream(data))
            {
                var reader = new BsonReader(stream);
                if (typeof(ICollection).IsAssignableFrom(type))
                {
                    reader.ReadRootValueAsArray = true;
                }
                var serializer = new JsonSerializer(); ;
                return serializer.Deserialize(reader, type);
            }
        }

        /// <summary>
        /// Bson反序列化Stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T BsonDeSerialize<T>(Stream stream)
        {
            using (var reader = new BsonReader(stream))
            {
                if (typeof(ICollection).IsAssignableFrom(typeof(T)))
                {
                    reader.ReadRootValueAsArray = true;
                }
                var serializer = new JsonSerializer(); ;
                return serializer.Deserialize<T>(reader);
            }
        }


        #endregion
    }
}
