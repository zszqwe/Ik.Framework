using Ik.Framework.Caching.CacheKeyManager;
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
    public class CacheKeyItemInfoEntity : BaseEntity
    {
        public Guid AppId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public CacheKeyScope KeySocpe { get; set; }

        public CacheEnvType CacheType { get; set; }

        public string ModelName { get; set; }

        public string RefValueType { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual CacheKeyAppInfoEntity CacheKeyAppInfo { get; set; }

    }
}