using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    public class MaskHelper
    {
        public static string MaskBankCardNumber(string cardNumber)
        {
            int index = cardNumber.Length - 5;
            return MaskString(cardNumber, -1, index);
        }

        /// <summary>
        /// 身份证掩码
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static string MaskIdCardNumber(string idCardNumber)
        {
            return MaskString(idCardNumber, 1, idCardNumber.Length - 2);
        }

        /// <summary>
        /// 做掩码，start和end都是闭区间
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string MaskString(string str, int start, int end)
        {
            int index = 0;
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            foreach (char item in str)
            {
                if (index > end)
                {
                    flag = true;
                }
                if (index > start && !flag)
                {
                    sb.Append("*");
                }
                else
                {
                    sb.Append(item);
                }
                index++;
            }
            return sb.ToString();
        }
    }
}
