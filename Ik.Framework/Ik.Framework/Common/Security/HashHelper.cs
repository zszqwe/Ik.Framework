using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;
using Ik.Framework.Common.Extension;

namespace Ik.Framework.Common.Security
{
    /// <summary>
    /// 哈希算法帮助类
    /// </summary>
    public class HashHelper
    {
        /// <summary>
        /// 将Input转成UTF-8字节数组后计算Hash
        /// </summary>
        /// <param name="format">可选Hash算法</param>
        /// <param name="input">要计算Hash的字符串输入</param>
        /// <param name="salt">要计算Hash的加密key</param>
        /// <param name="saltToBytesType">加密key是何种类型的字符串</param>
        /// <returns>Hex表示的计算结果</returns>
        public static string Encrypt(HashCryptoType format, string input, string salt, StrToBytesType saltToBytesType = StrToBytesType.Hex)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("The input can not be none.");

            if (format == HashCryptoType.None)
                return input;

            var inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = null;
            if (!string.IsNullOrEmpty(salt))
            {
                switch (saltToBytesType)
                {
                    case StrToBytesType.UTF8: saltBytes = Encoding.UTF8.GetBytes(salt); break;
                    case StrToBytesType.Hex: saltBytes = salt.HexToBinary(); break;
                    case StrToBytesType.Base64: saltBytes = Convert.FromBase64String(salt); break;
                    default: throw new ArgumentException("Can't resolve saltToBytesType.", "saltToBytesType");
                }
            }

            var inArray = HashEncrypt(format, inputBytes, saltBytes);
            return inArray.BinaryToHex();
        }

        /// <summary>
        /// 将Input转成UTF-8字节数组后计算Hash
        /// </summary>
        /// <param name="format">可选Hash算法</param>
        /// <param name="input">要计算Hash的字符串输入</param>
        /// <param name="salt">要计算Hash的加密key</param>
        /// <param name="saltToBytesType">加密key是何种类型的字符串</param>
        /// <returns>Hex表示的计算结果</returns>
        public static string Encrypt(HashCryptoType format, string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("The input can not be none.");

            if (format == HashCryptoType.None)
                return input;

            var inputBytes = Encoding.UTF8.GetBytes(input);
            var inArray = Encrypt(format, inputBytes, null);
            return inArray.BinaryToHex();
        }


        /// <summary>
        /// 计算Hash
        /// </summary>
        /// <param name="format">可选Hash算法</param>
        /// <param name="input">要计算Hash的输入</param>
        /// <param name="salt">要计算Hash的加密key</param>
        /// <returns>计算结果</returns>
        public static byte[] Encrypt(HashCryptoType format, byte[] input, byte[] salt)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentException("The input can not be none.");

            if (format == HashCryptoType.None)
                return input;

            var inArray = HashEncrypt(format, input, salt);
            return inArray;
        }

        /// <summary>
        /// 生成用于加密或解密的随机Key。
        /// </summary>
        /// <param name="length">Key的字节数组长度</param>
        /// <returns>返回Key的字节数组</returns>
        public static byte[] GenerateCryptKey(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("length must greater than zero");
            }
            byte[] data = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(data);
            return data;
        }

        private static byte[] HashEncrypt(HashCryptoType format, byte[] input, byte[] salt)
        {
            byte[] inArray = null;
            var hashAlgorithm = HashAlgorithm.Create(format.ToString());
            if (hashAlgorithm is KeyedHashAlgorithm)
            {
                var algorithm = (KeyedHashAlgorithm)hashAlgorithm;
                if (salt != null && salt.Length > 0)
                    algorithm.Key = salt;
                inArray = algorithm.ComputeHash(input);
            }
            else
            {
                byte[] buffer;
                if (salt != null && salt.Length > 0)
                {
                    buffer = new byte[input.Length + salt.Length];
                    Buffer.BlockCopy(input, 0, buffer, 0, input.Length);
                    Buffer.BlockCopy(salt, 0, buffer, input.Length, salt.Length);
                }
                else
                {
                    buffer = input;
                }
                inArray = hashAlgorithm.ComputeHash(buffer);
            }
            return inArray;
        }

    }
}
