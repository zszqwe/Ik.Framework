using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Configuration
{
    public class ConfigInitException :InkeyException
    {
        public ConfigInitException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {
 
        }
    }

    public class ConfigOperateException : InkeyException
    {
        public ConfigOperateException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

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
