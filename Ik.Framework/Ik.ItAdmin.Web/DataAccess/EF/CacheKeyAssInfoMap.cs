using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class CacheKeyAssInfoMap : IkEntityTypeConfiguration<CacheKeyAssInfoEntity>
    {
        public CacheKeyAssInfoMap()
        {
            this.ToTable("cache_ass_info");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("ass_id");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Code).HasColumnName("code");
            this.Property(t => t.Desc).HasColumnName("desc");

            this.Property(t => t.CreateTime).HasColumnName("create_time");
        }
    }
}