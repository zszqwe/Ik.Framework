using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Infrastructure
{
    public class AccountInfo : IdentityInfo
    {
        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}