using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class CollectionExtension
    {
        /// <summary>
        /// 添加集合到现有集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ICollection<T> AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
            return source;
        }
    }

    public static class NameValueCollectionExtensions
    {
        public static T GetValue<T>(this NameValueCollection collection, string key) where T : struct
        {
            var value = collection[key];
            if (!string.IsNullOrEmpty(value))
            {
                return value.ParseTo<T>();
            }
            else
                return default(T);
        }

        public static string GetValue(this NameValueCollection collection, string key)
        {
            return collection[key];
        }
    }

}

