using Ik.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class JsonExtension
    {
        /// <summary>
        /// 对象转换为JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            return JsonSerializerManager.JsonSerializer(obj);
        }

        /// <summary>
        /// JSON字符串转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string jsonData)
        {
            return JsonSerializerManager.JsonDeserialize<T>(jsonData);
        }

        /// <summary>
        /// 格式化传入的JSON字符串，如果不是JSON格式字符串将原样输出
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static string FormartJsonString(this string jsonData)
        {
            return JsonSerializerManager.JsonFormat(jsonData);
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
