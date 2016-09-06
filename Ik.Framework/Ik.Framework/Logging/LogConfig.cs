using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ik.Framework.Logging
{
    [XmlRoot("logConfig")]
    public class LogConfig
    {

        [XmlAttribute("defaultLevel")]
        public string DefaultLevel { get; set; }

        [XmlAttribute("defaultWorkType")]
        public string DefaultWorkType { get; set; }

        [XmlArray("rules")]
        [XmlArrayItem("rule")]
        public List<LogDispatchRule> Rules { get; set; }

    }

    public class LogDispatchRule
    {
        /// <summary>
        /// 日志标签
        /// </summary>
        [XmlAttribute("lable")]
        public string Lable { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [XmlAttribute("appName")]
        public string AppName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [XmlAttribute("modelName")]
        public string ModelName { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [XmlAttribute("businessName")]
        public string BusinessName { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [XmlAttribute("serverName")]
        public string ServerName { get; set; }

        [XmlAttribute("level")]
        public string Level { get; set; }

        [XmlAttribute("workType")]
        public string WorkType { get; set; }
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
