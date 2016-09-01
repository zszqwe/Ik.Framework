using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ik.Framework.Common.Extension;

namespace Ik.WebFramework.Mvc.Auth
{
    public class UnauthorizedResult : ActionResult
    {

        public UnauthorizedResult(object data)
        {
            this.Data = data;
        }
        public object Data { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.Write(Data.ToJsonString());
        }
    }
}
