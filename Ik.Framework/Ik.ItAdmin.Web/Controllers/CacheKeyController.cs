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
    public class CacheKeyController : BaseController
    {
        private CacheKeyManagerService _cacheKeyManagerService = null;
        public CacheKeyController(CacheKeyManagerService cacheKeyManagerService)
        {
            this._cacheKeyManagerService = cacheKeyManagerService;
        }

        #region AppInfo
        public ActionResult AppInfoManager()
        {
            return View("AppInfoList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetCacheKeyAppInfoListData(GridQuery filter)
        {
            var result = this._cacheKeyManagerService.GetCacheKeyAppInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult CacheKeyAppInfoAdd()
        {
            return View("AppInfoAdd");
        }

        public ActionResult CacheKeyAppInfoEdit(Guid id)
        {
            var model = this._cacheKeyManagerService.GetCacheKeyAppInfo(id);
            return View("AppInfoEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveAuthAppInfo(CacheKeyAppInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._cacheKeyManagerService.AddOrUpdateAllCacheKeyAppInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CacheKeyAppInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._cacheKeyManagerService.DeleteAllCacheKeyAppInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }
        #endregion

        #region keyitem
        public ActionResult AppKeyItemInfoManager()
        {
            return View("AppKeyItemInfoTreeList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetAppKeyItemInfoManagerList()
        {
            var result = this._cacheKeyManagerService.GetCacheKeyAppInfoList();
            return Json(result);
        }

        public ActionResult KeyItemInfoManagerList(Guid id)
        {
            var appInfo = this._cacheKeyManagerService.GetCacheKeyAppInfo(id);
            ViewBag.AppName = string.Format("{0}({1})", appInfo.Name, appInfo.Code);
            ViewBag.AppId = id;
            return View("AppKeyItemList");
        }

        public ActionResult GetCacheKeyAppKeyItemInfoListData(GridQuery filter)
        {
            var result = this._cacheKeyManagerService.GetCacheKeyItemInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult CacheKeyItemInfoAdd(Guid id)
        {
            var model = new CacheKeyItemInfoModel();
            model.AppId = id;
            return View("KeyItemInfoAdd", model);
        }

        public ActionResult CacheKeyItemInfoEdit(Guid id)
        {
            var model = this._cacheKeyManagerService.GetCacheKeyItemInfo(id);
            return View("KeyItemInfoEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveCacheKeyItemInfo(CacheKeyItemInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._cacheKeyManagerService.AddOrUpdateAllCacheKeyItemInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }
        [AjaxInkeyResultHandleError]
        public ActionResult CacheKeyItemInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._cacheKeyManagerService.DeleteAllCacheKeyItemInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }
        #endregion

        #region valueType

        public ActionResult ValueTypeInfoManagerList(Guid id)
        {
            var assInfo = this._cacheKeyManagerService.GetCacheKeyAssInfo(id);
            ViewBag.AssName = string.Format("{0}({1})", assInfo.Name, assInfo.Code);
            ViewBag.AssId = id;
            return View("ValueTypeInfoList");
        }

        public ActionResult GetCacheKeyValueTypeInfoListData(GridQuery filter)
        {
            var result = this._cacheKeyManagerService.GetCacheKeyValueTypeInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult CacheKeyValueTypeInfoAdd(Guid id)
        {
            var model = new CacheKeyValueTypeInfoModel();
            model.AssId = id;
            return View("ValueTypeAdd", model);
        }

        public ActionResult CacheKeyValueTypeInfoEdit(Guid id)
        {
            var model = this._cacheKeyManagerService.GetCacheKeyValueTypeInfo(id);
            return View("ValueTypeEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveCacheKeyValueTypeInfo(CacheKeyValueTypeInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._cacheKeyManagerService.AddOrUpdateAllCacheKeyValueTypeInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }
        [AjaxInkeyResultHandleError]
        public ActionResult CacheKeyValueTypeInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._cacheKeyManagerService.DeleteAllCacheValueTypeItemInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }

        public ActionResult AssValueTypeInfoManager()
        {
            return View("AssValueTypeInfoTreeList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetAssValueTypeInfoManagerList()
        {
            var result = this._cacheKeyManagerService.GetCacheKeyAssInfoList();
            return Json(result);
        }

        #endregion

        #region cacheManager

        public ActionResult CacheManagerEdit(Guid id)
        {
            var model = this._cacheKeyManagerService.GetCacheKeyItemInfo(id);
            return View(model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CacheManager(string actionType, Guid itemId, string sType, string cacheResult)
        {
            InkeyResult<string> result = this._cacheKeyManagerService.CacheManager(actionType, itemId, sType, this.Request.Form, cacheResult);
            
            return Json(result);
        }

        public ActionResult AnyCacheManagerEdit()
        {
            return View("CacheManagerAnyEdit");
        }

        #endregion


        #region AssInfo
        public ActionResult AssInfoManager()
        {
            return View("AssInfoList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetCacheKeyAssInfoListData(GridQuery filter)
        {
            var result = this._cacheKeyManagerService.GetCacheKeyAssInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult CacheKeyAssInfoAdd()
        {
            return View("AssInfoAdd");
        }

        public ActionResult CacheKeyAssInfoEdit(Guid id)
        {
            var model = this._cacheKeyManagerService.GetCacheKeyAssInfo(id);
            return View("AssInfoEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveCacheKeyAssInfo(CacheKeyAssInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._cacheKeyManagerService.AddOrUpdateAllCacheKeyAssInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CacheKeyAssInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._cacheKeyManagerService.DeleteAllCacheKeyAssInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }
        #endregion
    }
}