using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class AuthUserInfoMap : IkEntityTypeConfiguration<AuthUserInfoEntity>
    {
        public AuthUserInfoMap()
        {
            this.ToTable("auth_user_info");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("user_id");
            this.Property(t => t.RefOrgUserCode).HasColumnName("ref_org_user_code");
            this.Property(t => t.IsEnable).HasColumnName("is_enable");
            this.Property(t => t.Desc).HasColumnName("desc");
            this.Property(t => t.CreateTime).HasColumnName("create_time");

            this.HasMany(t => t.AuthGroupInfos)
                .WithMany(t => t.AuthUserInfos)
                .Map(m =>
                {
                    m.ToTable("auth_user_group");
                    m.MapLeftKey("user_id");
                    m.MapRightKey("group_id");
                });

            this.HasMany(t => t.AuthRoleInfos)
                .WithMany(t => t.AuthUserInfos)
                .Map(m =>
                {
                    m.ToTable("auth_user_role");
                    m.MapLeftKey("user_id");
                    m.MapRightKey("role_id");
                });
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
