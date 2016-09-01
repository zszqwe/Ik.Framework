using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Security
{
    /// <summary>
    /// 可选Hash算法
    /// </summary>
    public enum HashCryptoType
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        SHA256 = 3,
        SHA384 = 4,
        SHA512 = 5,
        HMACMD5 = 6,
        HMACSHA1 = 7,
        HMACSHA256 = 8,
        HMACSHA384 = 9,
        HMACSHA512 = 10,
    }
}
