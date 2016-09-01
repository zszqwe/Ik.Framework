using Ik.Framework;
using Ik.ItAdmin.Web.Dtos;
using Ik.ItAdmin.Web.Infrastructure;
using Ik.ItAdmin.Web.Models;
using Ik.ItAdmin.Web.Services;
using Ik.Service.Auth;
using Ik.WebFramework;
using Ik.WebFramework.Mvc;
using Ik.WebFramework.Mvc.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ik.ItAdmin.Web.Controllers
{
    public class SerialController : BaseController
    {
        private SerialNumberDefineService _serialNumberDefineService = null;
        public SerialController(SerialNumberDefineService serialNumberDefineService)
        {
            this._serialNumberDefineService = serialNumberDefineService;
        }

        public ActionResult SerialDefineInfoManager()
        {
            return View("SerialDefineList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetSerialDefineInfoListData(GridQuery filter)
        {
            var result = this._serialNumberDefineService.GetSerialDefineInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult SerialDefineInfoAdd()
        {
            return View("SerialDefineAdd");
        }

        public ActionResult SerialDefineInfoEdit(Guid id)
        {
            var model = this._serialNumberDefineService.GetSerialDefineInfo(id);
            return View("SerialDefineEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveSerialDefineInfo(SerialDefineInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._serialNumberDefineService.AddOrUpdateAllSerialDefineInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SerialDefineInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._serialNumberDefineService.DeleteAllSerialDefineInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }

    }
}