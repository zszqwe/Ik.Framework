using Ik.Framework.DataAccess;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.DependencyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    [AutoDataService(AutoDataServiceType.EF)]
    [DataSource(DataSources.DataSource_ItAdmin)]
    public class CacheValueTypeEntity : BaseEntity
    {
        public Guid AssId { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string ClassContext { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual CacheKeyAssInfoEntity CacheKeyAssInfo { get; set; }

    }
}