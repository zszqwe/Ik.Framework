using Ik.Service.Auth.Data;
using Ik.Service.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public class AuthAppService
    {
        private IAuthAppFunctionDao appFunctionDao = AuthDataAccessFactory.Instance.GetDataAccess<IAuthAppFunctionDao>();
        public IList<AuthFunctionInfo> GetAuthAppRootFunctionList(string appCode)
        {
            return appFunctionDao.GetAuthAppRootFunctionList(appCode);
        }

        public IList<AuthFunctionInfo> GetAllAuthFunctionList(string appCode, string funcCode)
        {
            return appFunctionDao.GetAllAuthAppFunctionList(appCode, funcCode);
        }

        public AuthFunctionInfo GetFunctionByCode(string appCode, string funcCode)
        {
            return appFunctionDao.GetFunctionByCode(appCode, funcCode);
        }
    }
}
