using Ik.Framework.Caching.CacheKeyManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class CacheKeyValueTypeInfoModel
    {
        public Guid? ValueTypeId { get; set; }

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

        public string ClassContext { get; set; }

        public DateTime CreateTime { get; set; }

        public string ValueTypeAssemblyName { get; set; }
    }
}