using Ik.ItAdmin.Web.Infrastructure;
using Ik.WebFramework.Mvc;
using Ik.WebFramework.Mvc.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ik.ItAdmin.Web.Controllers
{
    [InkeyAuthorize("ItAdminRoot")]
    public class BaseController : Controller
    {
        public AccountInfo UserInfo
        {
            get
            {
                return (AccountInfo)this.HttpContext.Request.GetIdentityInfo();
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            var result = new InkeyJsonResult();
            result.Data = data;
            result.ContentEncoding = contentEncoding;
            result.ContentType = contentType;
            return result;
        }

        protected bool VerifyModle(out string errorMessage)
        {
            errorMessage = null;
            if (this.ModelState.IsValid)
            {
                return true;
            }
            foreach (var item in this.ModelState.Values)
            {
                if (item.Errors.Count > 0)
                {
                    errorMessage = item.Errors[0].ErrorMessage;
                    break;
                }
            }
            return false;
        }
    }
}