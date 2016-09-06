using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IkCaching.Configuration;
using System.ComponentModel;

namespace IkCaching.Configuration
{
	public class LoggerSection : ConfigurationSection
	{
		[ConfigurationProperty("factory", IsRequired = true)]
		[InterfaceValidator(typeof(ILogFactory)), TypeConverter(typeof(TypeNameConverter))]
		public Type LogFactory
		{
			get { return (Type)base["factory"]; }
			set { base["factory"] = value; }
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
