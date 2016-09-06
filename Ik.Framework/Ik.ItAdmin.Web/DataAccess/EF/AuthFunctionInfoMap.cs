using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class AuthFunctionInfoMap : IkEntityTypeConfiguration<AuthFunctionInfoEntity>
    {
        public AuthFunctionInfoMap()
        {
            this.ToTable("auth_function_info");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("function_id");
            this.Property(t => t.ParentId).HasColumnName("parent_function_id");
            this.Property(t => t.AppId).HasColumnName("app_id");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Code).HasColumnName("code");
            this.Property(t => t.Desc).HasColumnName("desc");

            this.Property(t => t.IsEnable).HasColumnName("is_enable");

            this.Property(t => t.CreateTime).HasColumnName("create_time");
            this.Property(t => t.UpdateTime).HasColumnName("update_time");

            this.HasRequired(bc => bc.AuthAppInfo)
                .WithMany(bp => bp.AuthFunctionInfos)
                .HasForeignKey(bc => bc.AppId);

            this.HasMany(t => t.AuthRoleInfos)
                .WithMany(t => t.AuthFunctionInfos)
                .Map(m =>
                {
                    m.ToTable("auth_role_function");
                    m.MapLeftKey("function_id");
                    m.MapRightKey("role_id");
                });

            this.HasRequired(bc => bc.ParentFunctionInfo)
                .WithMany(bp => bp.SubAuthFunctionInfos)
                .HasForeignKey(bc => bc.ParentId);
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
