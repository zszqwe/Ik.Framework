using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    /// <summary>
    /// 异常错误码定义
    /// </summary>
    public static class InkeyErrorCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int CommonSuccess = 100;

        /// <summary>
        /// 执行意外失败
        /// </summary>
        public const int CommonFailure = -100;

        /// <summary>
        /// 业务意外失败
        /// </summary>
        public const int CommonBusinessFailure = 99999;

        /// <summary>
        /// 成功的默认描述
        /// </summary>
        public const string CommonSuccessDesc = "成功";

        /// <summary>
        /// 失败的默认描述
        /// </summary>
        public const string CommonFailureDesc = "未知错误，请联系管理员";
    }
}
