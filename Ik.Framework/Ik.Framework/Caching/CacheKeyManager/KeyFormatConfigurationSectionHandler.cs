using Ik.Framework.Caching.CacheKeyManager;
using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Caching
{
    public class KeyFormatConfigurationSectionHandler : LocalConfigSectionHandler<CacheKeyConfig>
    {
        public const string ConfigSectionName = "keyConfigDefines";
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
