using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Security
{
    /// <summary>
    /// 字符串应如何转成字节数组
    /// </summary>
    public enum StrToBytesType
    {
        UTF8 = 0,
        Hex = 1,
        Base64 = 2
    }
}
