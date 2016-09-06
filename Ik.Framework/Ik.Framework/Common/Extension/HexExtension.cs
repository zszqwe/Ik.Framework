using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class HexExtension
    {
        /// <summary>
        /// 二进制转16进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BinaryToHex(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            var array = new char[checked(data.Length * 2)];
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                array[2 * i] = NibbleToHex((byte)(b >> 4));
                array[2 * i + 1] = NibbleToHex((byte)(b & 15));
            }
            return new string(array);
        }

        /// <summary>
        /// 16进制转二进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexToBinary(this string data)
        {
            if (data == null || data.Length % 2 != 0)
            {
                return null;
            }
            var array = new byte[data.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                int num = HexToInt(data[2 * i]);
                int num2 = HexToInt(data[2 * i + 1]);
                if (num == -1 || num2 == -1)
                {
                    return null;
                }
                array[i] = (byte)(num << 4 | num2);
            }
            return array;
        }

        /// <summary>
        /// 16进制转二进制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexToBinary(this char[] data)
        {
            if (data == null || data.Length % 2 != 0)
            {
                return null;
            }
            var array = new byte[data.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                int num = HexToInt(data[2 * i]);
                int num2 = HexToInt(data[2 * i + 1]);
                if (num == -1 || num2 == -1)
                {
                    return null;
                }
                array[i] = (byte)(num << 4 | num2);
            }
            return array;
        }

        private static char NibbleToHex(byte nibble)
        {
            return (char)((nibble < 10) ? (nibble + 48) : (nibble - 10 + 65));
        }
        private static int HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
            {
                return (int)(h - '0');
            }
            if (h >= 'a' && h <= 'f')
            {
                return (int)(h - 'a' + '\n');
            }
            if (h < 'A' || h > 'F')
            {
                return -1;
            }
            return (int)(h - 'A' + '\n');
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
