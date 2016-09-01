using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class AuthGroupInfoModel
    {
        public Guid? GroupId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "用户组标识")]

        public string Code { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "用户组名称")]
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}