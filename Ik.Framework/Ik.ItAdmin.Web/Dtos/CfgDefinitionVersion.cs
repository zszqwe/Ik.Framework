using Ik.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class CfgDefinition
    {
        /// <summary>
        /// 配置定义Id
        /// </summary>
        public Guid DefId { get; set; }
        /// <summary>
        /// 项目配置名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 配置标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 配置定义说明
        /// </summary>
        public string DefDesc { get; set; }

    }

    public class CfgDefinitionVersion : CfgDefinition
    {

        /// <summary>
        /// 配置版本Id
        /// </summary>
        public Guid DefVerId { get; set; }

        /// <summary>
        /// 配置版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 配置版本说明
        /// </summary>
        public string DefVerDesc { get; set; }
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
