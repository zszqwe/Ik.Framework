﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Dtos
{
    public class AuthUserInfo
    {
        Guid UserId { get; set; }

        string RefOrgUserCode { get; set; }

        public bool UserIsEnable { get; set; }

        public bool UserIsDelete { get; set; }

        public DateTime UserCreateTime { get; set; }
    }

    public class AuthUserGroupFunction : AuthUserFunction
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class AuthUserFunction
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        public Guid FunctionId { get; set; }

        public Guid ParentFunctionId { get; set; }

        public string FuncCode { get; set; }

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
