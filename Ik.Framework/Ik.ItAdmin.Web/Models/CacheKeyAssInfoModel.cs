using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class CacheKeyAssInfoModel
    {
        public Guid? AssId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "标识")]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime CreateTime { get; set; }
    }
}