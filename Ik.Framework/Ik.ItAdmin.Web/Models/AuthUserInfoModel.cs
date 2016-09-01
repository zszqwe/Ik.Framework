using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class AuthUserInfoModel
    {
        public Guid? UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "用户标识")]
        public string RefOrgUserCode { get; set; }

        public bool IsEnable { get; set; }
        public string Desc { get; set; }
        public DateTime CreateTime { get; set; }
    }
}