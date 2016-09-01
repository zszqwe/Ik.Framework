using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Infrastructure
{
    public class ItAdminIkAuthorization : IkAuthorization
    {
        public override IdentityInfo CreateIdentityInfo()
        {
            return new AccountInfo();
        }
    }
}