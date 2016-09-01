using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Dtos
{
    public class AuthRoleInfo
    {
        Guid RoleId { get; set; }

        string RoleCode { get; set; }

        string RoleName { get; set; }

        string RoleDesc { get; set; }

        public bool RoleIsEnable { get; set; }

        public bool RoleIsDelete { get; set; }

        public DateTime RoleCreateTime { get; set; }

        public DateTime RoleUpdateTime { get; set; }
    }
}
