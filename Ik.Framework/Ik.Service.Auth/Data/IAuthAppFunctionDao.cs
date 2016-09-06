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
