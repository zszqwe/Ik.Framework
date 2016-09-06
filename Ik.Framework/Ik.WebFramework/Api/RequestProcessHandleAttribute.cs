using Ik.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using Ik.Framework.Common.Extension;
using Ik.Framework.Logging;
using Ik.Framework.BufferService;
using System.Net.Http.Headers;

namespace Ik.WebFramework.Api
{
    public class RequestProcessHandleAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private static ILog logger = LogManager.GetLogger();

        public const string _query_string_header_key = "__h";
        public const string _header_cookie_key = "_cookies";

        public const string _lng_key = "m-lng";
        public const string _lat_key = "m-lat";

        public const string _client_nettype_key = "m-nw";
        public const string _interface_version_key = "m-iv";
        public const string _client_version_key = "m-cv";
        public const string _client_type_key = "m-ct";
        public const string _client_width_key = "m-cw";
        public const string _client_height_key = "m-ch";
        public const string _install_source_key = "m-sr";
        public const string _encrypted_imei_key = "m-ii";
        public const string _token_key = "token";
        public const string _location_code = "m-lc";

        private bool _isBufferRequest;
        private bool _isNeedResultContent;
        public RequestProcessHandleAttribute(bool isBufferRequest = false, bool isNeedResultContent = false)
        {
            this._isBufferRequest = isBufferRequest;
            this._isNeedResultContent = isNeedResultContent;
        }
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var requestInfo = actionContext.Request.GetRequestProcessInfo();
            if (requestInfo == null)
            {
                requestInfo = BuildRequestInfo(actionContext);
                actionContext.Request.SetRequestProcessInfo(requestInfo);
            }
            base.OnActionExecuting(actionContext);
        }

        private ApiRequestProcessInfo BuildRequestInfo(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var requestInfo = new ApiRequestProcessInfo();
            requestInfo.IdentityInfo = request.GetIdentityInfo();
            requestInfo.RequestStartTime = System.DateTime.Now;
            requestInfo.Lng = request.GetHeaderValue<double>(_lng_key);
            requestInfo.Lat = request.GetHeaderValue<double>(_lat_key);
            requestInfo.ClientNetType = request.GetHeaderValue(_client_nettype_key);
            requestInfo.InterfaceVersion = request.GetHeaderValue(_interface_version_key);
            requestInfo.ClientVersion = request.GetHeaderValue(_client_version_key);
            requestInfo.ClientType = request.GetHeaderValue<DevicePlatform>(_client_type_key);
            requestInfo.ClientWidth = request.GetHeaderValue<int>(_client_width_key);
            requestInfo.ClientHeight = request.GetHeaderValue<int>(_client_height_key);

            requestInfo.UserAgent = request.Headers.UserAgent.ToString();
            requestInfo.RequestUrl = actionContext.Request.RequestUri.ToString();

            // 获取cookie
            IEnumerable<string> values;
            if (request.Headers.TryGetValues("Cookie", out values))
            {
                foreach (var item in values)
                {
                    requestInfo.RequestCookie += item + "~";
                }
                if (requestInfo.RequestCookie.Length > 0)
                {
                    requestInfo.RequestCookie = requestInfo.RequestCookie.TrimEnd('~');
                }
            }


            // 获取IP，有从TcpIP层获取的IP和http header获取的IP
            var ips = request.GetAllIpAddress();
            requestInfo.ClientIP = ips.HttpClientIp;
            requestInfo.ClientIP2 = ips.NginxClientIp;
            requestInfo.IsHttps = ips.IsHttps;

            //获取请求方法
            requestInfo.HttpMethod = request.Method.Method;

            requestInfo.RequestArguments = actionContext.ActionArguments;

            return requestInfo;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var apiRequestInfo = actionExecutedContext.Request.GetRequestProcessInfo();
            if (actionExecutedContext.Response != null)
            {
                var response = actionExecutedContext.Response;
                var objectContent = response.Content as ObjectContent;
                if (objectContent != null)
                {
                    InkeyResult result = objectContent.Value as InkeyResult;
                    if (result != null)
                    {
                        apiRequestInfo.ProcessState = result.Code;
                        apiRequestInfo.ProcessDesc = result.Desc;
                        if (this._isNeedResultContent)
                        {
                            apiRequestInfo.ResultContent = result.ToJsonString();
                        }
                    }
                }
                actionExecutedContext.Response.SetSimpleCookie(actionExecutedContext.Request);
            }
            else if (actionExecutedContext.Exception != null)
            {
                var inkeyException = actionExecutedContext.Exception as InkeyException;
                if (inkeyException != null)
                {
                    logger.Error(string.Format("请求错误，地址：{0}", actionExecutedContext.Request.RequestUri), actionExecutedContext.Exception);
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new InkeyResult(inkeyException.Code, inkeyException.Desc));
                    apiRequestInfo.ProcessState = inkeyException.Code;
                    apiRequestInfo.ProcessDesc = actionExecutedContext.Exception.Message;
                    apiRequestInfo.ResultContent = actionExecutedContext.Exception.ToString();
                }
                else
                {
                    Random rd = new Random();
                    int errorCode = rd.Next(900000, 9999999);
                    InkeyResult result = new InkeyResult(InkeyErrorCodes.CommonFailure, InkeyErrorCodes.CommonFailureDesc + "，日志码：" + errorCode);
                    logger.Error(string.Format("请求错误，日志码{0}，地址：{1}", errorCode, actionExecutedContext.Request.RequestUri), actionExecutedContext.Exception);
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, result);
                    apiRequestInfo.ProcessState = errorCode;
                    apiRequestInfo.ProcessDesc = actionExecutedContext.Exception.Message;
                    apiRequestInfo.ResultContent = actionExecutedContext.Exception.ToString();
                }
            }
            apiRequestInfo.HttpStatus = (int)actionExecutedContext.Response.StatusCode;
            apiRequestInfo.ElapsedMilliseconds = (int)((System.DateTime.Now - apiRequestInfo.RequestStartTime).TotalMilliseconds);
            

            var responseCookies = actionExecutedContext.Request.GetAllAddedSimpleCookie();
            // 获取cookie
            if (responseCookies.Count > 0)
            {
                foreach (CookieHeaderValue item in responseCookies)
                {
                    apiRequestInfo.ResponseCookie += string.Format("{0}={1}~", item.Cookies[0].Name, item.Cookies[0].Value);
                }
                if (apiRequestInfo.ResponseCookie.Length > 0)
                {
                    apiRequestInfo.ResponseCookie = apiRequestInfo.ResponseCookie.TrimEnd('~');
                }
            }
            BufferRequestProcessInfo(apiRequestInfo);
            base.OnActionExecuted(actionExecutedContext);
        }

        

        private void BufferRequestProcessInfo(ApiRequestProcessInfo apiRequestInfo)
        {
            if (this._isBufferRequest)
            {
                BufferServiceManager.TransportService.Transport(apiRequestInfo);
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
