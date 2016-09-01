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
    public interface IAuthAppFunctionDao
    {
        IList<AuthFunctionInfo> GetAllAuthAppFunctionList(string appCode, string funcCode);

        IList<AuthFunctionInfo> GetSubAuthAppFunctionList(string appCode, string funcParentCode);

        IList<AuthFunctionInfo> GetAuthAppRootFunctionList(string appCode);

        AuthFunctionInfo GetFunctionByCode(string appCode, string funcCode);
    }
}
