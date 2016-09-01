using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Net.Http.Headers;
using Ik.Service.Auth;
using System.Web;

namespace Ik.WebFramework.Api
{
    public static class HttpContextExtensions
    {
        private const string IdentityInfo_Context_Key = "Context_Key_IdentityInfo";
        private const string RequestInfo_Context_Key = "Context_Key_RequestInfo";
        private const string Cookies_Context_Key = "Context_Key_Cookies";
        
        public static string GetSimpleCookie(this HttpRequestMessage request, string key)
        {

            var cookies = request.Headers.GetCookies();
            if (cookies != null)
            {
                foreach (var cookieHeader in cookies)
                {
                    var cookieStates = cookieHeader.Cookies;
                    if (cookieStates != null)
                    {
                        foreach (var cookieState in cookieStates)
                        {
                            if (cookieState.Name == key)
                            {
                                return cookieState.Value;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static void SetTempSimpleCookie(this HttpRequestMessage request, string domain, string key, string value, DateTime expired)
        {
            Dictionary<string, CookieHeaderValue> cookies = null;
            object obj;
            if (request.Properties.TryGetValue(Cookies_Context_Key, out obj))
            {
                cookies = (Dictionary<string, CookieHeaderValue>)obj;
            }
            else
            {
                cookies = new Dictionary<string, CookieHeaderValue>();
                request.Properties.Add(Cookies_Context_Key, cookies);
            }
            var cookie = new CookieHeaderValue(key, value);
            cookie.Expires = new DateTimeOffset(expired);
            cookie.Path = "/";
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            if (!cookies.ContainsKey(key))
            {
                cookies.Add(key, cookie);
            }
            else
            {
                cookies[key] = cookie;
            }
        }

        public static void SetSimpleCookie(this HttpResponseMessage response, HttpRequestMessage request)
        {
            Dictionary<string, CookieHeaderValue> cookies = null;
            object obj;
            if (request.Properties.TryGetValue(Cookies_Context_Key, out obj))
            {
                cookies = (Dictionary<string, CookieHeaderValue>)obj;
                response.Headers.AddCookies(cookies.Values.ToList());
            }
        }

        public static IList<CookieHeaderValue> GetAllAddedSimpleCookie(this HttpRequestMessage request)
        {
            Dictionary<string, CookieHeaderValue> cookies = null;
            object obj;
            if (request.Properties.TryGetValue(Cookies_Context_Key, out obj))
            {
                cookies = (Dictionary<string, CookieHeaderValue>)obj;
                return cookies.Values.ToList();
            }
            return new List<CookieHeaderValue>();
        }

        public static IdentityInfo GetIdentityInfo(this HttpRequestMessage request)
        {
            IdentityInfo identityInfo = null;
            object obj;
            if (request.Properties.TryGetValue(IdentityInfo_Context_Key, out obj))
            {
                return (IdentityInfo)obj;
            }
            return identityInfo;
        }

        public static void SetIdentityInfo(this HttpRequestMessage request, IdentityInfo identityInfo)
        {
            if (!request.Properties.ContainsKey(IdentityInfo_Context_Key))
            {
                request.Properties.Add(IdentityInfo_Context_Key, identityInfo);
            }
            else
            {
                request.Properties[IdentityInfo_Context_Key] = identityInfo;
            }
        }

        public static ApiRequestProcessInfo GetRequestProcessInfo(this HttpRequestMessage request)
        {
            ApiRequestProcessInfo apiRequestInfo = null;
            object obj;
            if (request.Properties.TryGetValue(RequestInfo_Context_Key, out obj))
            {
                return (ApiRequestProcessInfo)obj;
            }
            return apiRequestInfo;
        }

        public static void SetRequestProcessInfo(this HttpRequestMessage request, ApiRequestProcessInfo apiRequestInfo)
        {
            if (!request.Properties.ContainsKey(IdentityInfo_Context_Key))
            {
                request.Properties.Add(RequestInfo_Context_Key, apiRequestInfo);
            }
            else
            {
                request.Properties[RequestInfo_Context_Key] = apiRequestInfo;
            }
        }

        public static string GetHeaderValue(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> headers = null;
            if (request.Headers.TryGetValues(key, out headers))
            {
                return headers.FirstOrDefault();
            }
            return null;
        }

        public static T GetHeaderValue<T>(this HttpRequestMessage request, string key) where T : struct
        {
            IEnumerable<string> headers = null;
            if (request.Headers.TryGetValues(key, out headers))
            {
                var value = headers.FirstOrDefault();
                if (string.IsNullOrEmpty(value))
                {
                    return default(T);
                }
                else
                {
                    return value.ParseTo<T>();
                }
            }
            else
                return default(T);
        }

        public static ClientIpSource GetAllIpAddress(this HttpRequestMessage request)
        {
            var ips = new ClientIpSource();

            // 获取反向代理过来的IP
            IEnumerable<string> values;
            if (request.Headers.TryGetValues("X-Real-IP", out values))
            {
                ips.HttpClientIp = values.FirstOrDefault();
            }
            if (request.Headers.TryGetValues("request_port", out values))
            {
                ips.IsHttps = values.FirstOrDefault() == "443";
            }
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                object ctxObj = request.Properties["MS_HttpContext"];
                if (ctxObj is HttpContextBase)
                {
                    var ctx = (HttpContextBase)ctxObj;
                    ips.NginxClientIp = ctx.Request.UserHostAddress;
                }
            }
            return ips;
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            var ips = GetAllIpAddress(request);
            return ips.HttpClientIp;
        }
    }
}
