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
    public class AuthFunctionInfoEntity : BaseEntity
    {
        public Guid ParentId { get; set; }
        public Guid AppId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsEnable { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual AuthAppInfoEntity AuthAppInfo { get; set; }

        public virtual AuthFunctionInfoEntity ParentFunctionInfo { get; set; }

        private ICollection<AuthFunctionInfoEntity> _subAuthFunctionInfos;

        public virtual ICollection<AuthFunctionInfoEntity> SubAuthFunctionInfos
        {
            get { return _subAuthFunctionInfos ?? (_subAuthFunctionInfos = new List<AuthFunctionInfoEntity>()); }
            protected set { _subAuthFunctionInfos = value; }
        }

        private ICollection<AuthRoleInfoEntity> _authRoleInfos;

        public virtual ICollection<AuthRoleInfoEntity> AuthRoleInfos
        {
            get { return _authRoleInfos ?? (_authRoleInfos = new List<AuthRoleInfoEntity>()); }
            protected set { _authRoleInfos = value; }
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
