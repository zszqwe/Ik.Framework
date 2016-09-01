using Ik.Framework.Common.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class CfgItemVersionEnvironmentParam: PageParameter
    {
        public Guid DefId { get; set; }

        public Guid DefVerId { get; set; }

        /// <summary>
        /// 配置数据项标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 配置数据项版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }
    }
}