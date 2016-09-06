using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class AuthRoleInfoMap : IkEntityTypeConfiguration<AuthRoleInfoEntity>
    {
        public AuthRoleInfoMap()
        {
            this.ToTable("auth_role_info");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("role_id");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Code).HasColumnName("code");
            this.Property(t => t.Desc).HasColumnName("desc");

            this.Property(t => t.IsEnable).HasColumnName("is_enable");

            this.Property(t => t.CreateTime).HasColumnName("create_time");
            this.Property(t => t.UpdateTime).HasColumnName("update_time");

            this.HasMany(t => t.AuthUserInfos)
                .WithMany(t => t.AuthRoleInfos)
                .Map(m =>
                {
                    m.ToTable("auth_user_role");
                    m.MapLeftKey("role_id");
                    m.MapRightKey("user_id");
                });

            this.HasMany(t => t.AuthGroupInfos)
                .WithMany(t => t.AuthRoleInfos)
                .Map(m =>
                {
                    m.ToTable("auth_group_role");
                    m.MapLeftKey("role_id");
                    m.MapRightKey("group_id");
                });

            this.HasMany(t => t.AuthFunctionInfos)
                .WithMany(t => t.AuthRoleInfos)
                .Map(m =>
                {
                    m.ToTable("auth_role_function");
                    m.MapLeftKey("role_id");
                    m.MapRightKey("function_id");
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
