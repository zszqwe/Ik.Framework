using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Net.Http.Headers;
using Ik.Service.Auth;
using System.Web;
using System.Collections;

namespace Ik.WebFramework.Mvc
{
    public static class HttpContextExtensions
    {
        private const string IdentityInfo_Context_Key = "Context_Key_IdentityInfo";
        private const string RequestInfo_Context_Key = "Context_Key_RequestInfo";
        private const string Cookies_Context_Key = "Context_Key_Cookies";
        public static string GetSimpleCookie(this HttpRequestBase request, string key)
        {

            HttpCookie cookie = request.Cookies.Get(key);
            if (cookie != null)
            {
                return cookie.Value;
            }
            return string.Empty;
        }

        public static void SetTempSimpleCookie(this HttpRequestBase request, string domain, string key, string value, DateTime expired)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            Dictionary<string, HttpCookie> cookies = null;
            if (items.Contains(Cookies_Context_Key))
            {
                cookies = (Dictionary<string, HttpCookie>)items[Cookies_Context_Key];
            }
            else
            {
                cookies = new Dictionary<string, HttpCookie>();
                items.Add(Cookies_Context_Key, cookies);
            }
            var cookie = new HttpCookie(key, value);
            cookie.Expires = expired;
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

        public static void SetSimpleCookie(this HttpResponseBase response, HttpRequestBase request)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            Dictionary<string, HttpCookie> cookies = null;
            if (items.Contains(Cookies_Context_Key))
            {
                cookies = (Dictionary<string, HttpCookie>)items[Cookies_Context_Key];
                foreach (var item in cookies.Values)
                {
                    response.Cookies.Add(item);
                }
            }
        }

        public static IList<HttpCookie> GetAllAddedSimpleCookie(this HttpRequestBase request)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            Dictionary<string, HttpCookie> cookies = null;
            if (items.Contains(Cookies_Context_Key))
            {
                cookies = (Dictionary<string, HttpCookie>)items[Cookies_Context_Key];
                cookies.Values.ToList();
            }
            return new List<HttpCookie>();
        }

        public static IdentityInfo GetIdentityInfo(this HttpRequestBase request)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            IdentityInfo identityInfo = null;
            if (items.Contains(IdentityInfo_Context_Key))
            {
                return (IdentityInfo)items[IdentityInfo_Context_Key];
            }
            return identityInfo;
        }

        public static void SetIdentityInfo(this HttpRequestBase request, IdentityInfo identityInfo)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            if (!items.Contains(IdentityInfo_Context_Key))
            {
                items.Add(IdentityInfo_Context_Key, identityInfo);
            }
            else
            {
                items[IdentityInfo_Context_Key] = identityInfo;
            }
        }

        public static RequestProcessInfo GetRequestProcessInfo(this HttpRequestBase request)
        {
            RequestProcessInfo requestInfo = null;
            IDictionary items = request.RequestContext.HttpContext.Items;
            if (items.Contains(RequestInfo_Context_Key))
            {
                return (RequestProcessInfo)items[RequestInfo_Context_Key];
            }
            return requestInfo;
        }

        public static void SetRequestProcessInfo(this HttpRequestBase request, RequestProcessInfo requestInfo)
        {
            IDictionary items = request.RequestContext.HttpContext.Items;
            if (!items.Contains(RequestInfo_Context_Key))
            {
                items.Add(RequestInfo_Context_Key, requestInfo);
            }
            else
            {
                items[RequestInfo_Context_Key] = requestInfo;
            }
        }

        public static ClientIpSource GetAllIpAddress(this HttpRequestBase request)
        {
            var ips = new ClientIpSource();

            // 获取反向代理过来的IP
            // 获取IP，有从TcpIP层获取的IP和http header获取的IP
            ips.NginxClientIp = request.UserHostAddress;
            ips.HttpClientIp = request.Headers["X-Real-IP"];
            ips.IsHttps = request.Headers["request_port"] == "443";
            return ips;
        }

        public static string GetClientIpAddress(this HttpRequestBase request)
        {
            var ips = GetAllIpAddress(request);
            return ips.HttpClientIp;
        }
    }
}
