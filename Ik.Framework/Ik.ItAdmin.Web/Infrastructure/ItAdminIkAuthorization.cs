﻿using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Infrastructure
{
    public class ItAdminIkAuthorization : IkAuthorization
    {
        public override IdentityInfo CreateIdentityInfo()
        {
            return new AccountInfo();
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
