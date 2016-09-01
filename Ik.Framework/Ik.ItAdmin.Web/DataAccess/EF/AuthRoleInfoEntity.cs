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
    public class AuthRoleInfoEntity: BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsEnable { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        private ICollection<AuthUserInfoEntity> _authUserInfos;

        public virtual ICollection<AuthUserInfoEntity> AuthUserInfos
        {
            get { return _authUserInfos ?? (_authUserInfos = new List<AuthUserInfoEntity>()); }
            protected set { _authUserInfos = value; }
        }

        private ICollection<AuthGroupInfoEntity> _authGroupInfos;

        public virtual ICollection<AuthGroupInfoEntity> AuthGroupInfos
        {
            get { return _authGroupInfos ?? (_authGroupInfos = new List<AuthGroupInfoEntity>()); }
            protected set { _authGroupInfos = value; }
        }

        private ICollection<AuthFunctionInfoEntity> _authFunctionInfos;

        public virtual ICollection<AuthFunctionInfoEntity> AuthFunctionInfos
        {
            get { return _authFunctionInfos ?? (_authFunctionInfos = new List<AuthFunctionInfoEntity>()); }
            protected set { _authFunctionInfos = value; }
        }
    }
}