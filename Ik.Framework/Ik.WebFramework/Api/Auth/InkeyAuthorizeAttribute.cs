using Ik.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using Ik.Service.Auth;
using Ik.WebFramework.Api;

namespace Ik.WebFramework.Api.Auth
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

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            InkeyAuthorizeContainer container = new InkeyAuthorizeContainer(actionContext.Request);
            string tokenString = container.GetToken();
            IEnumerable<string> headers;
            if (string.IsNullOrEmpty(tokenString) && actionContext.Request.Headers.TryGetValues("token", out headers))
            {
                tokenString = headers.FirstOrDefault();
            }
            IdentityInfo identityInfo = null;
            if (actionContext.Request.Properties.ContainsKey(Context_Authorize_IsCache_Key))
            {
                identityInfo = (IdentityInfo)actionContext.Request.Properties[Context_Authorize_IsCache_Key];
            }
            if (identityInfo == null && IkAuthorizationContext.Current.IsAuthenticate(tokenString, out identityInfo))
            {
                actionContext.Request.Properties[Context_Authorize_IsCache_Key] = identityInfo;
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
                actionContext.Request.SetIdentityInfo(identityInfo);
                Authorized(actionContext, identityInfo);
                return true;
            }
            return false;
        }

        public virtual void Authorized(HttpActionContext actionContext, IdentityInfo identityInfo)
        {
 
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var result = new InkeyResult();
            result.Desc = "无权限，需要登录后操作";
            result.Code = (int)HttpStatusCode.Unauthorized;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
        }
    }
}
