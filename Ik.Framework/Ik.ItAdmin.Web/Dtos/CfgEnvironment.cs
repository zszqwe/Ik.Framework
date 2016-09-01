using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class CfgEnvironment
    {
        /// <summary>
        /// 环境Id
        /// </summary>
        public Guid EnvId { get; set; }
        /// <summary>
        /// 环境名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 环境标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 配置数据项描述
        /// </summary>
        public string Desc { get; set; }
    }
}