using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class RoleFunctionInfoTreeNode
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public Guid RoleId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
    }
}