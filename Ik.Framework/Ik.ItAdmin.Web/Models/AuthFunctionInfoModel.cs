using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class AuthFunctionInfoModel
    {
        public Guid AppId { get; set; }
        public Guid? FunctionId { get; set; }

        public Guid ParentId { get; set; }

        public Guid ParentParentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "标识")]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsEnable { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
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
