using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Security
{
    public class RSAHelper
    {
        private static readonly RSASecurity rsa = new RSASecurity();

        /// <summary>
        /// 生成密钥
        /// </summary>
        public static RSAKey GenerateRSAKey()
        {
            return rsa.GenerateRSAKey();
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="dataToEncrypt">待加密数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string Encrypt(string dataToEncrypt, string publicKey)
        {
            return rsa.Encrypt(dataToEncrypt, publicKey);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="dataToEncrypt">待加密数据</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string Encrypt(byte[] dataToEncrypt, string publicKey)
        {
            return rsa.Encrypt(dataToEncrypt, publicKey);
        }

        /// <summary>
        /// 根据安全证书加密
        /// </summary>
        /// <param name="dataToEncrypt"></param>
        /// <param name="certfile"></param>
        /// <returns></returns>
        public static string X509CertEncrypt(string dataToEncrypt, string certfile)
        {
            return rsa.X509CertEncrypt(dataToEncrypt, certfile);
        }

        /// <summary>
        /// 根据安全证书加密
        /// </summary>
        /// <param name="dataToEncrypt">待加密数据</param>
        /// <param name="certfile">安全证书</param>
        /// <returns></returns>
        public static string X509CertEncrypt(byte[] dataToEncrypt, string certfile)
        {
            return rsa.X509CertEncrypt(dataToEncrypt, certfile);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedData">待解密数据</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedData, string privateKey)
        {
            return rsa.Decrypt(encryptedData, privateKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedData">待解密数据</param>
        /// <param name="keyfile">私钥文件</param>
        /// <param name="password">访问私钥文件密码</param>
        /// <returns></returns>
        public static string X509CertDecrypt(string encryptedData, string keyfile, string password)
        {
            return rsa.X509CertDecrypt(encryptedData, keyfile, password);
        }
    }
}
