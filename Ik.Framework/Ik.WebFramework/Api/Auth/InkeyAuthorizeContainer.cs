using Ik.Framework.Configuration;
using Ik.Service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ik.WebFramework.Api;

namespace Ik.WebFramework.Api.Auth
{
    public class InkeyAuthorizeContainer
    {
        private const string _token_key = "token";
        private HttpRequestMessage _request = null;
        private HttpResponseMessage _response = null;
        private static string _cookieDomain = ConfigManager.Instance.Get<string>("auth_cookie_domain");

        public InkeyAuthorizeContainer(HttpRequestMessage request)
            : this(request, null)
        { }

        public InkeyAuthorizeContainer(HttpResponseMessage response)
            : this(null, response)
        { }

        public InkeyAuthorizeContainer(HttpRequestMessage request, HttpResponseMessage response)
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
