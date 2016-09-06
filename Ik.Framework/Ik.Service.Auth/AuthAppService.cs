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

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
