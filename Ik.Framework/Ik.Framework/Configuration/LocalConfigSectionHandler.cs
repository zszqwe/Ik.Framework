using Ik.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ik.Framework.Configuration
{
    public class LocalConfigSectionHandler<T> : IConfigurationSectionHandler where T : class
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return XmlSerializerManager.XmlDeserialize<T>(section);
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
