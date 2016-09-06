using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class CacheKeyItemInfoMap : IkEntityTypeConfiguration<CacheKeyItemInfoEntity>
    {
        public CacheKeyItemInfoMap()
        {
            this.ToTable("cache_key_info");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("key_id");
            this.Property(t => t.AppId).HasColumnName("app_id");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Code).HasColumnName("code");
            this.Property(t => t.Desc).HasColumnName("desc");
            this.Property(t => t.KeySocpe).HasColumnName("key_scope");
            this.Property(t => t.CacheType).HasColumnName("cache_type");
            this.Property(t => t.ModelName).HasColumnName("model_name");
            this.Property(t => t.RefValueType).HasColumnName("ref_value_type");
            this.Property(t => t.CreateTime).HasColumnName("create_time");
            this.Property(t => t.UpdateTime).HasColumnName("update_time");

            this.HasRequired(bc => bc.CacheKeyAppInfo)
                .WithMany(bp => bp.CacheKeyItemInfos)
                .HasForeignKey(bc => bc.AppId);
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
