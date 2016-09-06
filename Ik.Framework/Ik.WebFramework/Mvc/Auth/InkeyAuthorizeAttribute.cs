using Ik.Framework;
using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ik.WebFramework.Mvc.Auth
{
    public class InkeyAuthorizeAttribute : AuthorizeAttribute
    {
        private string _funcCode = "";
        private const string Context_Authorize_IsCache_Key = "Context_Authorize_IsCache";
        public InkeyAuthorizeAttribute()
        {
 
        }

        public InkeyAuthorizeAttribute(string funcCode)
        {
            this._funcCode = funcCode;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            InkeyAuthorizeContainer container = new InkeyAuthorizeContainer(httpContext.Request);
            string tokenString = container.GetToken();
            if (string.IsNullOrEmpty(tokenString))
            {
                return false;
            }
            IdentityInfo identityInfo = null;
            if (httpContext.Items.Contains(Context_Authorize_IsCache_Key))
            {
                identityInfo = (IdentityInfo)httpContext.Items[Context_Authorize_IsCache_Key];
            }
            if (identityInfo == null && IkAuthorizationContext.Current.IsAuthenticate(tokenString, out identityInfo))
            {
                httpContext.Items[Context_Authorize_IsCache_Key] = identityInfo;
            }

            if (identityInfo != null)
            {
                if (!string.IsNullOrEmpty(this._funcCode))
                {
                    var up = IkAuthorizationContext.Current.GetUserPermission(identityInfo.RefUserCode);
                    var flag = IkAuthorizationContext.Current.CheckPermission(this._funcCode, up);
                    if (!flag)
                    {
                        return false;
                    }
                }
                httpContext.Request.SetIdentityInfo(identityInfo);
                Authorized(httpContext, identityInfo);
                return true;
            }
            return false;
        }

        public virtual void Authorized(HttpContextBase httpContext, IdentityInfo identityInfo)
        {

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var result = new InkeyResult();
                result.Code = 401;
                result.Desc = "未授权";
                filterContext.HttpContext.SkipAuthorization = true;
                filterContext.Result = new UnauthorizedResult(result);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
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
