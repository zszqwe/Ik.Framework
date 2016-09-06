using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public enum CfgItemType
    {
        Definition = 0, Global = 1
    }

    public class CfgItem
    {
        /// <summary>
        /// 配置数据项Id
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// 配置数据项标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 配置数据项描述
        /// </summary>
        public string ItemDesc { get; set; }

        public CfgItemType ItemType { get; set; }

        public Guid? DefId { get; set; }
    }

    public class CfgItemVersion : CfgItem
    {
        /// <summary>
        /// 配置数据项版本Id
        /// </summary>
        public Guid ItemVerId { get; set; }
        /// <summary>
        /// 配置数据项版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 配置数据项版本描述
        /// </summary>
        public string ItemVerDesc { get; set; }
    }

    public class CfgItemVersionEnvironment : CfgItemVersion
    {
        /// <summary>
        /// 配置数据项版本环境Id
        /// </summary>
        public Guid ItemVerEnvValId { get; set; }
        /// <summary>
        /// 环境Id
        /// </summary>
        public Guid EnvId { get; set; }
        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }
        /// <summary>
        /// 环境标识
        /// </summary>
        public string EnvKey { get; set; }
        /// <summary>
        /// 配置数据项值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 配置数据项描述
        /// </summary>
        public string ItemVerEnvValDesc { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string OptId
        {
            get
            {
                return string.Format("{0}#{1}#{2}#{3}", ItemId, ItemVerId, ItemVerEnvValId, DefItemId.HasValue ? DefItemId.Value : Guid.Empty);
            }
        }

        public Guid? DefItemId { get; set; }
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
