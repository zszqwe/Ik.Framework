using Ik.ItAdmin.Web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class RoleFunctionInfoTreeModel
    {
        public RoleFunctionInfoTreeModel()
        {
            FunctionInfos = new List<AuthFunctionInfoEntity>();
        }

        public Guid AppId { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public Guid RoleId { get; set; }

        public IList<AuthFunctionInfoEntity> FunctionInfos { get; set; }

        public Dictionary<Guid, AuthFunctionInfoEntity> AddedFunctionInfos { get; set; }
    }
}