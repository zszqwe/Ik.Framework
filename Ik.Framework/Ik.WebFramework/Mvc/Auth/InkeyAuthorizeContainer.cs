using Ik.Framework.Configuration;
using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ik.WebFramework.Mvc.Auth
{
    public class InkeyAuthorizeContainer
    {
        private const string _token_key = "token";
        private HttpRequestBase _request = null;
        private HttpResponseBase _response = null;
        private static string _cookieDomain = ConfigManager.Instance.Get<string>("auth_cookie_domain");

        public InkeyAuthorizeContainer(HttpRequestBase request)
            : this(request, null)
        { }

        public InkeyAuthorizeContainer(HttpResponseBase response)
            : this(null, response)
        { }

        public InkeyAuthorizeContainer(HttpRequestBase request, HttpResponseBase response)
        {
            this._request = request;
            this._response = response;
        }

        public void SetToken(IdentityInfo identityInfo)
        {
            this._request.SetTempSimpleCookie(_cookieDomain, _token_key, identityInfo.Token, identityInfo.ExpireTime.AddDays(1));
        }

        public string GetToken()
        {
            return this._request.GetSimpleCookie(_token_key);
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
