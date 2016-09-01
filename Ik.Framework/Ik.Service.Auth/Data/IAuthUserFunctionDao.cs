using Ik.Framework.DataAccess;
using Ik.Framework.DependencyManagement;
using Ik.Service.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Data
{
    public interface IAuthUserFunctionDao
    {
        IList<AuthUserGroupFunction> GetUserGroupFunctionListByRefUserCode(string refUserCode);

        IList<AuthUserFunction> GetUserFunctionListByRefUserCode(string refUserCode);


    }
}
