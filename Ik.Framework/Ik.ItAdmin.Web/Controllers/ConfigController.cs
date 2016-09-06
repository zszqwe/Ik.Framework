using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using Ik.ItAdmin.Web.Controllers;
using Ik.ItAdmin.Web.Services;
using Ik.ItAdmin.Web.Dtos;
using Ik.Framework;
using Ik.Framework.Common.Utilities;
using Ik.Framework.Common.Extension;
using Ik.Framework.DependencyManagement;
using Ik.WebFramework.Mvc.DependencyManagement;
using Ik.WebFramework.Mvc;
using Ik.WebFramework.Mvc.Auth;



namespace Inkey.ConfigManagerCenter.Controllers
{
    [InkeyAuthorize("ItAdminConfigManagerAll")]
    public class ConfigController : BaseController
    {
        private CfgManagerService _cfgManagerService = null;

        public ConfigController(CfgManagerService cfgManagerService)
        {
            this._cfgManagerService = cfgManagerService;
        }

        #region 配置、版本编辑
        public ActionResult ConfigManager()
        {
            return View("ConfigManagerTreeList");
        }

        [AjaxInkeyResultHandleError()]
        public ActionResult GetConfigManagerList()
        {
            var list = _cfgManagerService.GetCfgDefinitionVersionList();
            return Json(list);
        }

        public ActionResult CfgDefinitionView(Guid id)
        {
            var data = _cfgManagerService.GetCfgDefinition(id);
            return View(data);
        }

        public ActionResult CfgDefinitionVersionView(Guid id)
        {
            var data = _cfgManagerService.GetCfgDefinitionVersion(id);
            return View(data);
        }

        public ActionResult CfgDefinitionAdd()
        {
            var data = new CfgDefinition();
            return View("CfgDefinitionEdit", data);
        }

        public ActionResult CfgDefinitionEdit(Guid id)
        {
            var data = _cfgManagerService.GetCfgDefinition(id);
            return View("CfgDefinitionEdit", data);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveCfgDefinition(CfgDefinition model)
        {
            InkeyResult result = new InkeyResult();
            _cfgManagerService.AddOrUpdateAllCfgDefinition(model);
            return Json(result);
        }

        public ActionResult CfgDefinitionVersionAdd(Guid defId)
        {
            var cfgDef = _cfgManagerService.GetCfgDefinition(defId);
            if (cfgDef == null)
            {
                throw new InkeyException(InkeyErrorCodes.CommonFailure, "配置Id无效");
            }
            var data = new CfgDefinitionVersion();
            data.DefId = cfgDef.DefId;
            data.Name = cfgDef.Name;
            data.Title = cfgDef.Title;
            return View("CfgDefinitionVersionEdit", data);
        }

        public ActionResult CfgDefinitionVersionEdit(Guid id)
        {
            var data = _cfgManagerService.GetCfgDefinitionVersion(id);
            return View("CfgDefinitionVersionEdit", data);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveCfgDefinitionVersion(CfgDefinitionVersion model)
        {
            InkeyResult result = new InkeyResult();
            Version ver;
            if (Version.TryParse(model.Version, out ver))
            {
                model.Version = VersionHelper.FormatVersion(model.Version).ToString();
                _cfgManagerService.AddOrUpdateAllCfgDefinitionVersion(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = "版本号格式错误";
            }
            return Json(result);
        }

        #endregion

        public ActionResult GetCfgDefinitionItemListById(Guid id)
        {
            ViewBag.DefId = id;
            return View("GetCfgDefinitionItemListIndex");
        }

        public ActionResult GetCfgDefinitionItemList(Guid defId, string key, string ver, string env, int pageIndex = 1, int totalCount = 0)
        {
            var data = _cfgManagerService.GetCfgDefinitionItemListByPager(new CfgItemVersionEnvironmentParam { DefId = defId, Key = key, Version = ver, EnvName = env, PageIndex = pageIndex });
            return View("_CfgItemTable", data);
        }

        public ActionResult GetCfgDefinitionVersionItemListById(Guid id)
        {
            ViewBag.DefVerId = id;
            var data = _cfgManagerService.GetCfgDefinitionVersion(id);
            ViewBag.DefId = data.DefId;
            return View("GetCfgDefinitionVersionItemListIndex");
        }

        public ActionResult GetCfgDefinitionVersionItemList(Guid defVerId, string key, string ver, string env, int pageIndex = 1, int totalCount = 0)
        {
            var data = _cfgManagerService.GetCfgDefinitionVersionItemListByPager(new CfgItemVersionEnvironmentParam { DefVerId = defVerId, Key = key, Version = ver, EnvName = env, PageIndex = pageIndex });
            return View("_CfgItemTable", data);
        }

        #region 配置环境管理
        public ActionResult ConfigEnvManager()
        {
            return View("ConfigEnvManagerList");
        }

        public ActionResult GetCfgEnvList()
        {
            var data = _cfgManagerService.GetCfgEnvironmentList();
            return View("_CfgEnvTable",data);
        }

        public ActionResult CfgEnvAdd()
        {
            var data = new CfgEnvironment();
            return View("CfgEnvEdit", data);
        }

        public ActionResult CfgEnvEdit(Guid id)
        {
            var data = _cfgManagerService.GetCfgEnvironment(id);
            return View("CfgEnvEdit", data);
        }

         [AjaxInkeyResultHandleError]
        public ActionResult SaveCfgEnv(CfgEnvironment model)
        {
            InkeyResult result = new InkeyResult();
            _cfgManagerService.AddOrUpdateAllCfgEnvironment(model);
            return Json(result);
        }

        #endregion

         public ActionResult ConfigItemManager()
         {
             return View("ConfigItemManagerList");
         }

         public ActionResult GetCfgItemList(string key, string ver, string env, int pageIndex = 1, int totalCount = 0)
         {
             var data = _cfgManagerService.GetCfgItemListByPager(new CfgItemVersionEnvironmentParam { Key = key, Version = ver, EnvName = env, PageIndex = pageIndex });
             return View("_CfgItemTable", data);
         }

         public ActionResult CfgItemAdd(int itemType, Guid? defId)
         {
             var data = new CfgItemVersionEnvironment();
             data.ItemType = itemType == 1 ? CfgItemType.Global : CfgItemType.Definition;
             data.DefId = defId;
             IList<CfgEnvironment> envData = _cfgManagerService.GetCfgEnvironmentList();
             ViewBag.EnvSelectList = new SelectList(envData, "EnvId", "Name");
             return View("CfgItemAdd", data);
         }

         public ActionResult CfgItemEdit(string ids)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             var data = _cfgManagerService.GetCfgItem(idVals[0]); ;
             return View("CfgItemEdit", data);
         }

         public ActionResult CfgItemVerAdd(string ids)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             CfgItem item = _cfgManagerService.GetCfgItem(idVals[0]);
             var data = new CfgItemVersionEnvironment();
             data.ItemId = item.ItemId;
             data.Key = item.Key;
             IList<CfgEnvironment> envData = _cfgManagerService.GetCfgEnvironmentList();
             ViewBag.EnvSelectList = new SelectList(envData, "EnvId", "Name");
             return View("CfgItemVerAdd", data);
         }

         public ActionResult CfgItemVerEdit(string ids)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             var data = _cfgManagerService.GetCfgItemVersion(idVals[1]);
             return View("CfgItemVerEdit", data);
         }

         public ActionResult CfgItemVerEnvAdd(string ids)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             CfgItem item = _cfgManagerService.GetCfgItem(idVals[0]);
             var data = new CfgItemVersionEnvironment();
             data.ItemId = item.ItemId;
             data.Key = item.Key;
             IList<CfgItemVersion> itemVersions = _cfgManagerService.GetCfgItemVersionList(item.ItemId);
             ViewBag.VerSelectList = new SelectList(itemVersions, "ItemVerId", "Version");
             IList<CfgEnvironment> envData = _cfgManagerService.GetCfgEnvironmentList();
             ViewBag.EnvSelectList = new SelectList(envData, "EnvId", "Name");
             return View("CfgItemVerEnvAdd", data);
         }

         public ActionResult CfgItemVerEnvEdit(string ids)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             var data = _cfgManagerService.GetCfgItemVersionEnvironment(idVals[2]);
             IList<CfgEnvironment> envData = _cfgManagerService.GetCfgEnvironmentList();
             ViewBag.EnvSelectList = new SelectList(envData, "EnvId", "Name");
             return View("CfgItemVerEnvEdit", data);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult SaveCfgItem(CfgItemVersionEnvironment model)
         {
             InkeyResult result = new InkeyResult();
             Version ver;
             if (Version.TryParse(model.Version, out ver))
             {
                 model.Version = VersionHelper.FormatVersion(model.Version).ToString();
                 _cfgManagerService.AddCfgItemAndVersion(model);
             }
             else
             {
                 result.Code = -101;
                 result.Desc = "版本号格式错误";
             }

             
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult SaveCfgItemVer(CfgItemVersionEnvironment model)
         {
             InkeyResult result = new InkeyResult();
             Version ver;
             if (Version.TryParse(model.Version, out ver))
             {
                 model.Version = VersionHelper.FormatVersion(model.Version).ToString();
                 _cfgManagerService.AddCfgVersionAndEnvValue(model);
             }
             else
             {
                 result.Code = -101;
                 result.Desc = "版本号格式错误";
             }


             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult SaveCfgItemVerEnv(CfgItemVersionEnvironment model)
         {
             InkeyResult result = new InkeyResult();
             _cfgManagerService.AddCfgEnvValue(model);
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult SaveCfgItemEdit(CfgItem model)
         {
             InkeyResult result = new InkeyResult();
             _cfgManagerService.EditCfgItem(model);
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult SaveCfgItemVerEdit(CfgItemVersion model)
         {
             InkeyResult result = new InkeyResult();
             _cfgManagerService.EditCfgItemVerion(model);
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
        public ActionResult SaveCfgItemVerEnvEdit(CfgItemVersionEnvironment model)
         {
             InkeyResult result = new InkeyResult();
             model.UpdateTime = DateTime.Now;
             _cfgManagerService.EditCfgItemVerionEnvironment(model);
             return Json(result);
         }
         [AjaxInkeyResultHandleError]
         public ActionResult CfgItemDeleteAll(string ids)
         {
             IList<Guid> idList = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 Guid defItemId = Guid.Parse(s[3]);
                 if (defItemId != Guid.Empty)
                 {
                     return Guid.Empty;
                 }
                 else
                 {
                     return Guid.Parse(s[0]);
                 }
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             IList<Guid> idListForGolbal = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 return Guid.Parse(s[3]);
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             InkeyResult result = new InkeyResult();
             if (idList.Count > 0)
             {
                 _cfgManagerService.DeleteAllCfgItem(idList);
             }
             if (idListForGolbal.Count > 0)
             {
                 _cfgManagerService.DeleteAllCfgDefinitionItem(idListForGolbal);
             }
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult CfgItemVerDeleteAll(string ids)
         {
             IList<Guid> idList = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 Guid defItemId = Guid.Parse(s[3]);
                 if (defItemId != Guid.Empty)
                 {
                     return Guid.Empty;
                 }
                 else
                 {
                     return Guid.Parse(s[1]);
                 }
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             IList<Guid> idListForGolbal = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 return Guid.Parse(s[3]);
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             InkeyResult result = new InkeyResult();
             if (idList.Count > 0)
             {
                 _cfgManagerService.DeleteAllCfgItemVersion(idList);
             }
             if (idListForGolbal.Count > 0)
             {
                 result.Code = -101;
                 result.Desc = "全局数据项不能操作";
             }
             return Json(result);
         }

         [AjaxInkeyResultHandleError]
         public ActionResult CfgItemVerEnvDeleteAll(string ids)
         {
             IList<Guid> idList = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 Guid defItemId = Guid.Parse(s[3]);
                 if (defItemId != Guid.Empty)
                 {
                     return Guid.Empty;
                 }
                 else
                 {
                     return Guid.Parse(s[2]);
                 }
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             IList<Guid> idListForGolbal = ids.Split(',').Select(i =>
             {
                 var s = i.Split('#');
                 return Guid.Parse(s[3]);
             }).Where(i => i != Guid.Empty).Distinct().ToList();
             InkeyResult result = new InkeyResult();
             if (idList.Count > 0)
             {
                 _cfgManagerService.DeleteAllCfgItemVersionEnvironment(idList);
             }
             if (idListForGolbal.Count > 0)
             {
                 result.Code = -101;
                 result.Desc = "全局数据项不能操作";
             }
             return Json(result);
         }

         public ActionResult SelectCfgItemList(Guid defId, Guid defVerId)
         {
             ViewBag.DefVerId = defVerId;
             ViewBag.DefId = defId;
             return View();
         }

         public ActionResult GetSelectCfgItemList(Guid defId, string key, string ver, string env, int pageIndex = 1, int totalCount = 0)
         {
             var data = _cfgManagerService.GetSelectCfgItemListByPager(new CfgItemVersionEnvironmentParam { DefId = defId, Key = key, Version = ver, EnvName = env, PageIndex = pageIndex });
             return View("_CfgItemTable", data);
         }

        [AjaxInkeyResultHandleError]
         public ActionResult SelectCfgDefinitionItem(string ids, Guid defVerId)
         {
             var idVals = ids.Split('#').Select(id => Guid.Parse(id)).ToArray();
             InkeyResult result = new InkeyResult();
             _cfgManagerService.SetCfgDefinitionItem(idVals[2], defVerId);
             return Json(result);
         }

        [AjaxInkeyResultHandleError]
        public ActionResult CfgDefinitionDelete(Guid id)
        {
            InkeyResult result = new InkeyResult();
            _cfgManagerService.DeleteCfgDefinition(id);
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CfgDefinitionVersionDelete(Guid id)
        {
            InkeyResult result = new InkeyResult();
            _cfgManagerService.DeleteCfgDefinitionVersion(id);
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CfgDefinitonItemPublishAll(Guid id)
        {
            InkeyResult result = new InkeyResult();
            _cfgManagerService.PublishAllCfgDefinitonItem(id);
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult CfgDefinitionItemDeleteAll(string ids)
        {
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                var s = i.Split('#');
                return Guid.Parse(s[3]);
            }).Where(i => i != Guid.Empty).Distinct().ToList();
            InkeyResult result = new InkeyResult();
            if (idList.Count > 0)
            {
                _cfgManagerService.DeleteAllCfgDefinitionItem(idList);
            }
            return Json(result);
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
