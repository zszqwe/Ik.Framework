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
    public class AuthUserInfoEntity : BaseEntity
    {
        
        public string RefOrgUserCode { get; set; }

        public bool IsEnable { get; set; }
        public string Desc { get; set; }
        public DateTime CreateTime { get; set; }

        private ICollection<AuthGroupInfoEntity> _authGroupInfos;

        public virtual ICollection<AuthGroupInfoEntity> AuthGroupInfos
        {
            get { return _authGroupInfos ?? (_authGroupInfos = new List<AuthGroupInfoEntity>()); }
            protected set { _authGroupInfos = value; }
        }


        private ICollection<AuthRoleInfoEntity> _authRoleInfos;

        public virtual ICollection<AuthRoleInfoEntity> AuthRoleInfos
        {
            get { return _authRoleInfos ?? (_authRoleInfos = new List<AuthRoleInfoEntity>()); }
            protected set { _authRoleInfos = value; }
        }
    }
}