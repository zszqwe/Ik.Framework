using Ik.Framework.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class EnumExtension
    {
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="enumData"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumData)
        {
            return EnumHelper.GetDescription(enumData);
        }
    }
}
