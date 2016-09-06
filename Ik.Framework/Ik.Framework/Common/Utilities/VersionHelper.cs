using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    public class VersionHelper
    {
        public static Version FormatVersion(string version)
        {
            var count = version.Count(c => c == '.');
            if (count == 1)
            {
                return new Version(version + ".0.0");
            }
            else if (count == 2)
            {
                return new Version(version + ".0");
            }
            return new Version(version);
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
