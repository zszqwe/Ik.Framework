using Ik.Framework.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    public class SerialDefineInfoMap : IkEntityTypeConfiguration<SerialDefineInfoEntity>
    {
        public SerialDefineInfoMap()
        {
            this.ToTable("seq_number");
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasColumnName("seq_id");
            this.Property(t => t.Key).HasColumnName("seq_key");
            this.Property(t => t.NextValue).HasColumnName("next_value");
            this.Property(t => t.PrefixValue).HasColumnName("prefix_value");
            this.Property(t => t.FormatLength).HasColumnName("format_length");
            this.Property(t => t.DateFormat).HasColumnName("date_format");
            this.Property(t => t.ApplyCapacity).HasColumnName("apply_capacity");
            this.Property(t => t.CheckThreshold).HasColumnName("check_threshold");
            this.Property(t => t.CreateTime).HasColumnName("create_time");
            this.Property(t => t.Desc).HasColumnName("desc");
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
