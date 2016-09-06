using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth
{
    public class UserPermission
    {
        public UserPermission()
        {
            this.Groups = new List<UserGroupInfo>();
            this.Roles = new List<RoleInfo>();
            this.Permissions = new List<AppPermission>();
        }
        public Guid UserId { get; set; }

        public string RefUserCode { get; set; }

        public IList<UserGroupInfo> Groups { get; private set; }

        public IList<RoleInfo> Roles { get; private set; }

        public IList<AppPermission> Permissions { get; private set; }

    }

    public class AppPermission
    {
        public string PermissionCode { get; set; }

        public Guid FunctionId { get; set; }

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
