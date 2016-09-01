using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Dtos
{
    public class AuthGroupInfo
    {
        Guid GroupId { get; set; }

        string GroupName { get; set; }

        public DateTime GroupCreateTime { get; set; }
    }
}
