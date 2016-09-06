using Ik.Framework.BufferService;
using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework
{
    public class RequestProcessInfo : BufferItem
    {
        public RequestProcessInfo()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        /// <summary>
        /// 终端宽
        /// </summary>
        public int ClientWidth { get; set; }

        /// <summary>
        /// 终端高
        /// </summary>
        public int ClientHeight { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 来自http header记录的client ip
        /// </summary>
        public string ClientIP2 { get; set; }

        /// <summary>
        /// 客户端浏览器类型
        /// </summary>
        public string UserAgent { get; set; }

        public string RequestCookie { get; set; }

        public string ResponseCookie { get; set; }

        public DateTime RequestStartTime { get; set; }

        public string Headers { get; set; }

        public Dictionary<string, object> RequestArguments { get; set; }

        public bool IsHttps { get; set; }

        public string HttpMethod { get; set; }

        public string ResultContent { get; set; }

        public int HttpStatus { get; set; }

        public string RequestUrl { get; set; }

        public IdentityInfo IdentityInfo { get; set; }

        public int ElapsedMilliseconds { get; set; }
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
