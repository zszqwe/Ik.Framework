using Ik.Framework.DataAccess;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    [AutoDataService(AutoDataServiceType.EF)]
    [DataSource(DataSources.DataSource_ItAdmin)]
    public class AuthGroupInfoEntity : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        private ICollection<AuthUserInfoEntity> _authUserInfos;

        public virtual ICollection<AuthUserInfoEntity> AuthUserInfos
        {
            get { return _authUserInfos ?? (_authUserInfos = new List<AuthUserInfoEntity>()); }
            protected set { _authUserInfos = value; }
        }

        private ICollection<AuthRoleInfoEntity> _authRoleInfos;

        public virtual ICollection<AuthRoleInfoEntity> AuthRoleInfos
        {
            get { return _authRoleInfos ?? (_authRoleInfos = new List<AuthRoleInfoEntity>()); }
            protected set { _authRoleInfos = value; }
        }
    }
}