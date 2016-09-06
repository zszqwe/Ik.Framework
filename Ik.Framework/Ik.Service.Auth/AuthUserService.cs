using Ik.Service.Auth.Data;
using Ik.Service.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;

namespace Ik.Service.Auth
{
    public class AuthUserService
    {
        private IAuthUserFunctionDao appFunctionDao = AuthDataAccessFactory.Instance.GetDataAccess<IAuthUserFunctionDao>();
        public IList<AuthUserGroupFunction> GetUserGroupFunctionListByRefUserCode(string refUserCode)
        {
            return appFunctionDao.GetUserGroupFunctionListByRefUserCode(refUserCode);
        }

        public IList<AuthUserFunction> GetUserFunctionListByRefUserCode(string refUserCode)
        {
            return appFunctionDao.GetUserFunctionListByRefUserCode(refUserCode); 
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
