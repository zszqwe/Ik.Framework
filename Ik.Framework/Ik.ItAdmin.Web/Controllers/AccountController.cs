using Ik.Framework;
using Ik.ItAdmin.Web.Infrastructure;
using Ik.ItAdmin.Web.Models;
using Ik.Service.Auth;
using Ik.WebFramework.Mvc;
using Ik.WebFramework.Mvc.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ik.ItAdmin.Web.Controllers
{
    public class AccountController : BaseController
    {
         [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [AjaxInkeyResultHandleError]
        public JsonResult Login(LoginViewModel model)
        {
            var result = new InkeyResult();
            AccountInfo accountInfo = (AccountInfo)IkAuthorizationContext.Current.Authenticate("13568851221");
            accountInfo.UserId = 564878;
            accountInfo.UserName = "系统管理员";
            var authorizeContainer = new InkeyAuthorizeContainer(this.HttpContext.Request);
            authorizeContainer.SetToken(accountInfo);
            return Json(result);
        }

         [HttpGet]
        public ActionResult Logout()
        {
            var identityInfo = this.HttpContext.Request.GetIdentityInfo();
            IkAuthorizationContext.Current.Logout(identityInfo.Token);
            return View();
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
