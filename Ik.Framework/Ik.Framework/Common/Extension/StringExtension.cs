using Ik.Framework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class StringExtension
    {
        #region 值类型转换

        /// <summary>
        /// 将字符串转换为指定的类型，如果转换不成功，返回默认值。
        /// </summary>
        /// <typeparam name="T">结构体类型或枚举类型</typeparam>
        /// <param name="str">需要转换的字符串</param>
        /// <returns>返回指定的类型。</returns>
        public static T ParseTo<T>(this string str) where T : struct
        {
            return ParseTo<T>(str, default(T));
        }

        /// <summary>
        /// 将字符串转换为指定的类型，如果转换不成功，返回默认值。
        /// </summary>
        /// <typeparam name="T">结构体类型或枚举类型</typeparam>
        /// <param name="str">需要转换的字符串</param>
        /// <param name="defaultValue">如果转换失败，需要使用的默认值</param>
        /// <returns>返回指定的类型。</returns>
        public static T ParseTo<T>(this string str, T defaultValue) where T : struct
        {
            var val = ParseToNullable<T>(str);
            if (val.HasValue)
                return val.Value;
            else
                return defaultValue;
        }

        /// <summary>
        /// 将字符串转换为指定的类型，如果转换不成功，返回null
        /// </summary>
        /// <typeparam name="T">结构体类型或枚举类型</typeparam>
        /// <param name="str">需要转换的字符串</param>
        /// <returns>返回指定的类型</returns>
        public static T? ParseToNullable<T>(this string str) where T : struct
        {
            if (string.IsNullOrEmpty(str))
                return null;

            Type t = typeof(T);
            if (t.IsEnum)
                return ToEnum<T>(str);
            else
                return (T?)ParseTo(str, t.FullName);
        }

        private static object ParseTo(string str, string type)
        {
            switch (type)
            {
                case "System.Boolean":
                    return ToBoolean(str);
                case "System.SByte":
                    return ToSByte(str);
                case "System.Byte":
                    return ToByte(str);
                case "System.UInt16":
                    return ToUInt16(str);
                case "System.Int16":
                    return ToInt16(str);
                case "System.uInt32":
                    return ToUInt32(str);
                case "System.Int32":
                    return ToInt32(str);
                case "System.UInt64":
                    return ToUInt64(str);
                case "System.Int64":
                    return ToInt64(str);
                case "System.Single":
                    return ToSingle(str);
                case "System.Double":
                    return ToDouble(str);
                case "System.Decimal":
                    return ToDecimal(str);
                case "System.DateTime":
                    return ToDateTime(str);
                case "System.Guid":
                    return ToGuid(str);
                default:
                    throw new NotSupportedException(string.Format("The string of \"{0}\" can not be parsed to {1}", str, type));
            }
        }
        public static sbyte? ToSByte(string value)
        {
            sbyte ret;
            if (sbyte.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static byte? ToByte(string value)
        {
            byte ret;
            if (byte.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static ushort? ToUInt16(string value)
        {
            ushort ret;
            if (ushort.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static short? ToInt16(string value)
        {
            short ret;
            if (short.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static uint? ToUInt32(string value)
        {
            uint ret;
            if (uint.TryParse(value, out ret))
                return ret;
            return null;
        }

        public static ulong? ToUInt64(string value)
        {
            ulong ret;
            if (ulong.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static long? ToInt64(string value)
        {
            long ret;
            if (long.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static float? ToSingle(string value)
        {
            float ret;
            if (float.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static double? ToDouble(string value)
        {
            double ret;
            if (double.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static decimal? ToDecimal(string value)
        {
            decimal ret;
            if (decimal.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static bool? ToBoolean(string value)
        {
            bool ret;
            if (bool.TryParse(value, out ret))
                return ret;
            return null;
        }
        public static T? ToEnum<T>(string str) where T : struct
        {
            T result;
            if (Enum.TryParse<T>(str, true, out result))
            {
                if (Enum.IsDefined(typeof(T), result))
                {
                    return result;
                }
            }
            return null;
        }
        public static Guid? ToGuid(string str)
        {
            Guid result;
            if (Guid.TryParse(str, out result))
                return result;
            return null;
        }
        public static DateTime? ToDateTime(string value)
        {
            DateTime ret;
            if (DateTime.TryParse(value, out ret))
                return ret;
            return null;
        }

        public static int? ToInt32(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            int v;
            if (int.TryParse(input, out v))
                return v;

            return null;
        }

        #endregion
    }
}
