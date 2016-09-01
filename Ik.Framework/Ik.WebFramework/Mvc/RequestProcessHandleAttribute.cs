using Ik.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ik.Framework.Common.Extension;
using Ik.Framework.Logging;
using Ik.Framework.BufferService;
using System.Web.Mvc;
using System.Web;

namespace Ik.WebFramework.Mvc
{
    public class RequestProcessHandleAttribute : ActionFilterAttribute
    {
        private static ILog logger = LogManager.GetLogger();

        private bool _isBufferRequest;
        private bool _isNeedResultContent;
        public RequestProcessHandleAttribute(bool isBufferRequest = false, bool isNeedResultContent = false)
        {
            this._isBufferRequest = isBufferRequest;
            this._isNeedResultContent = isNeedResultContent;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var requestInfo = filterContext.HttpContext.Request.GetRequestProcessInfo();
            if (requestInfo == null)
            {
                requestInfo = BuildRequestInfo(filterContext);
                filterContext.HttpContext.Request.SetRequestProcessInfo(requestInfo);
            }
            base.OnActionExecuting(filterContext);
        }

        private RequestProcessInfo BuildRequestInfo(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var requestInfo = new RequestProcessInfo();

            requestInfo.IdentityInfo = filterContext.HttpContext.Request.GetIdentityInfo();

            requestInfo.RequestStartTime = System.DateTime.Now;

            requestInfo.UserAgent = request.UserAgent;
            requestInfo.RequestUrl = filterContext.HttpContext.Request.RawUrl.ToString();

            // 获取cookie
            if (request.Cookies.Count > 0)
            {
                foreach (string key in request.Cookies.Keys)
                {
                    HttpCookie item = request.Cookies[key];
                    requestInfo.RequestCookie += string.Format("{0}={1}~", item.Name, item.Value);
                }
                if (requestInfo.RequestCookie.Length > 0)
                {
                    requestInfo.RequestCookie = requestInfo.RequestCookie.TrimEnd('~');
                }
            }
            var ips = request.GetAllIpAddress();
            requestInfo.ClientIP = ips.HttpClientIp;
            requestInfo.ClientIP2 = ips.NginxClientIp;
            requestInfo.IsHttps = ips.IsHttps;

            //获取请求方法
            requestInfo.HttpMethod = filterContext.HttpContext.Request.HttpMethod;

            requestInfo.RequestArguments = new Dictionary<string, object>(filterContext.ActionParameters);
            return requestInfo;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var requestInfo = filterContext.HttpContext.Request.GetRequestProcessInfo();
            if (filterContext.Exception != null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var ajaxResultAttrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AjaxInkeyResultHandleErrorAttribute), false);
                    if (ajaxResultAttrs != null)
                    {
                        var ajaxResult = (AjaxInkeyResultHandleErrorAttribute)ajaxResultAttrs[0];
                        var inkeyException = filterContext.Exception as InkeyException;
                        var statusCode = 200;
                        if (inkeyException != null)
                        {
                            logger.Error(string.Format("请求错误，地址：{0}", filterContext.HttpContext.Request.RawUrl), filterContext.Exception);
                            if (ajaxResult.IsAjaxFormResult)
                            {
                                var result = new ContentResult { Content = new InkeyResult(inkeyException.Code, inkeyException.Desc).ToJsonString() };
                                filterContext.Result = result;
                            }
                            else
                            {
                                var result = new InkeyJsonResult();
                                result.Data = new InkeyResult(inkeyException.Code, inkeyException.Desc);
                                filterContext.Result = result;
                            }

                            requestInfo.ResultContent = filterContext.Exception.ToString();
                        }
                        else
                        {
                            Random rd = new Random();
                            int errorCode = rd.Next(900000, 9999999);
                            logger.Error(string.Format("请求错误，日志码{0}，地址：{1}", errorCode, filterContext.HttpContext.Request.RawUrl), filterContext.Exception);
                            if (ajaxResult.IsAjaxFormResult)
                            {
                                var result = new ContentResult { Content = new InkeyResult(InkeyErrorCodes.CommonFailure, InkeyErrorCodes.CommonFailureDesc + "，日志码：" + errorCode).ToJsonString() };
                                filterContext.Result = result;
                            }
                            else
                            {
                                var result = new InkeyJsonResult();
                                result.Data = new InkeyResult(InkeyErrorCodes.CommonFailure, InkeyErrorCodes.CommonFailureDesc + "，日志码：" + errorCode);
                                filterContext.Result = result;
                            }

                            requestInfo.ResultContent = filterContext.Exception.ToString();
                        }

                        filterContext.ExceptionHandled = true;
                        filterContext.HttpContext.Response.Clear();
                        filterContext.HttpContext.Response.StatusCode = statusCode;
                        filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    }
                }
                else
                {
                    requestInfo.ResultContent = filterContext.Exception.ToString();
                }
            }
            else
            {
                filterContext.HttpContext.Response.SetSimpleCookie(filterContext.HttpContext.Request);
                if (this._isNeedResultContent)
                {
                    //
                }
            }
            requestInfo.HttpStatus = (int)filterContext.HttpContext.Response.StatusCode;
            requestInfo.ElapsedMilliseconds = (int)((System.DateTime.Now - requestInfo.RequestStartTime).TotalMilliseconds);
            
            var responseCookies = filterContext.HttpContext.Request.GetAllAddedSimpleCookie();
            // 获取cookie
            if (responseCookies.Count > 0)
            {
                foreach (HttpCookie item in responseCookies)
                {
                    requestInfo.ResponseCookie += string.Format("{0}={1}~", item.Name, item.Value);
                }
                if (requestInfo.ResponseCookie.Length > 0)
                {
                    requestInfo.ResponseCookie = requestInfo.ResponseCookie.TrimEnd('~');
                }
            }

            BufferRequestProcessInfo(requestInfo);
            base.OnActionExecuted(filterContext);
        }

        private void BufferRequestProcessInfo(RequestProcessInfo requestInfo)
        {
            if (this._isBufferRequest)
            {
                BufferServiceManager.TransportService.Transport(requestInfo);
            }
        }
    }
}
