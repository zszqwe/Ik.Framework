using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    /// <summary>
    /// 标识该方法能支持框架根据版本路由调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class VersionRoute : Attribute
    {
        public VersionRoute()
        {

        }

        public VersionRoute(string acceptVersion, string targetAction)
        {
            Version version;
            if (!Version.TryParse(acceptVersion, out version))
            {
                throw new ArgumentException("版本号格式错误");
            }
            this.AcceptVersion = version; ;
            this.TargetAction = targetAction;
        }

        /// <summary>
        /// 接口调用的目标方法
        /// </summary>
        public string TargetAction { get; set; }

        /// <summary>
        /// 此方法对应的版本
        /// </summary>

        public Version AcceptVersion { get; set; }
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
