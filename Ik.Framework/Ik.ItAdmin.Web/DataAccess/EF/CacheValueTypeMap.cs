using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class CacheValueTypeMap : IkEntityTypeConfiguration<CacheValueTypeEntity>
    {
        public CacheValueTypeMap()
        {
            this.ToTable("cache_value_types");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("value_type_id");
            this.Property(t => t.AssId).HasColumnName("ass_id");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Code).HasColumnName("code");
            this.Property(t => t.Desc).HasColumnName("desc");
            this.Property(t => t.ClassContext).HasColumnName("class_context");
            this.Property(t => t.CreateTime).HasColumnName("create_time");

            this.HasRequired(bc => bc.CacheKeyAssInfo)
                .WithMany(bp => bp.CacheValueTypes)
                .HasForeignKey(bc => bc.AssId);
        }
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
