using Ik.Framework.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Service.Auth
{
    internal class TokenHelper
    {
        private const string _aes_key_token_str = "PdVXDIZdBSPCImYo6iuNbxnN/rdKay65x5OoFrpHYq4=";
        private const string _aes_iv_token_str = "LUDtqeTw36EbzBIho6RWNg==";
        internal static readonly byte[] _aes_key_token = Convert.FromBase64String(_aes_key_token_str);
        internal static readonly byte[] _aes_iv_token = Convert.FromBase64String(_aes_iv_token_str);

        internal static string EncryptToken(TokenInfo tokenInfo)
        {
            var tokenBytes = BitConverter.GetBytes(tokenInfo.AccessTime.ToBinary())
                .Concat(Encoding.UTF8.GetBytes(tokenInfo.RefUserCode))
                .ToArray();
            return AESHelper.EncryptBytes(tokenBytes, _aes_key_token, _aes_iv_token).BinaryToHex();
        }

        internal static TokenInfo DecryptToken(string token)
        {
            var tokenBin = token.HexToBinary();
            tokenBin = AESHelper.DecryptBytes(tokenBin, _aes_key_token, _aes_iv_token);

            return new TokenInfo
            {
                RefUserCode = Encoding.UTF8.GetString(tokenBin, 0, tokenBin.Length - 8),
                AccessTime = DateTime.FromBinary(BitConverter.ToInt64(tokenBin, tokenBin.Length - 8)),
            };
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
