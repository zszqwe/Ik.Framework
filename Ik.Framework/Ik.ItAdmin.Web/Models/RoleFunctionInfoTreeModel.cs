using Ik.ItAdmin.Web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class RoleFunctionInfoTreeModel
    {
        public RoleFunctionInfoTreeModel()
        {
            FunctionInfos = new List<AuthFunctionInfoEntity>();
        }

        public Guid AppId { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public Guid RoleId { get; set; }

        public IList<AuthFunctionInfoEntity> FunctionInfos { get; set; }

        public Dictionary<Guid, AuthFunctionInfoEntity> AddedFunctionInfos { get; set; }
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
