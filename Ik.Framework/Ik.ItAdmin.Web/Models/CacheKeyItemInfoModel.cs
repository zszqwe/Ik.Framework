using Ik.Framework.Caching.CacheKeyManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class CacheKeyItemInfoModel
    {
        public Guid? ItemId { get; set; }
        public Guid? AppId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "标识")]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        public string Desc { get; set; }

        public CacheKeyScope KeySocpe { get; set; }

        public CacheEnvType CacheType { get; set; }

        public string ModelName { get; set; }

        public string RefValueType { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string AppName { get; set; }

        public string AppCode { get; set; }
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
