using Ik.Framework.BufferService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Logging
{

    public class LogEntity : BufferItem
    {
        public LogEntity()
        {
            this.Id = Guid.NewGuid();
            this.CreateTime = DateTime.Now;
        }

        public Guid Id { get; set; }


        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 日志标签
        /// </summary>
        public string Lable { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 机器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 日志码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string Content { get; set; }


        public override string ToString()
        {
            return string.Format("日志级别：{0}，日志时间：{1}，项目名称：{2},模块名称：{3},业务名称：{4},机器名称：{5},日志标签：{6},日志码：{7}。\r\n日志消息：{8}。\r\n详细内容：{9}", this.Level, this.CreateTime, this.AppName, this.ModelName, this.BusinessName, this.ServerName, this.Lable, this.Code, this.Message, this.Content);
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
