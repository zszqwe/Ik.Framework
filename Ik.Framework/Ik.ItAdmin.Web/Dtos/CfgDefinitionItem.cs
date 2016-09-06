using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class CfgDefinitionItem
    {
        /// <summary>
        /// 数据项Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 配置数据项标识
        /// </summary>
        public string Key { get; set; }
       
        /// <summary>
        /// 配置数据项版本号
        /// </summary>
        public string ItemVersion { get; set; }

        /// <summary>
        /// 配置数据项版本环境Id
        /// </summary>
        public int ItemVerEnvValId { get; set; }
        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }
        /// <summary>
        /// 配置数据项值
        /// </summary>
        public string Value { get; set; }
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
