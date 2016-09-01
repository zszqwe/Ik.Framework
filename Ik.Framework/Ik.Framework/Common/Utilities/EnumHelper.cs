using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    public static class EnumHelper
    {
        public static IEnumerable<T> GetValues<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T必须是枚举类型");

            return Enum.GetValues(typeof(T)).OfType<T>();
        }

        public static IEnumerable<string> GetDescriptions<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T必须是枚举类型");
            var vals = GetValues<T>();

            return vals.Select(e => GetDescription(e as Enum)).ToList();
        }


        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();
            var val = en.ToString();
            var name = GetMemberInfoDesc(type, val);
            return string.IsNullOrEmpty(name) ? val : name;
        }

        private static string GetMemberInfoDesc(Type type,string name)
        {
            MemberInfo[] memInfo = type.GetMember(name);

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return null;
        }


        public static T StringToEnum<T>(string name)
        {

            string[] names = Enum.GetNames(typeof(T));

            if (((IList)names).Contains(name))
            {
                return (T)Enum.Parse(typeof(T), name);
            }

            return default(T);

        }
    }
}
