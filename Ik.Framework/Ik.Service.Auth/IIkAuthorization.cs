using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public interface IIkAuthorization
    {
        void Initialize();

        IdentityInfo Authenticate(string refUserCode);

        UserPermission GetUserPermission(string refUserCode);

        bool CheckPermission(string funcCode, UserPermission up);

        TokenInfo GetTokenInfo(string tokenString);

        void Logout(string token);

        bool IsAuthenticate(string token, out IdentityInfo identityInfo);

        void UpdateNotificationAll();
    }
}
