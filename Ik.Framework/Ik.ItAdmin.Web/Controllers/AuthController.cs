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
    public class AuthController : BaseController
    {
        private AuthManagerService _authManagerService = null;
        public AuthController(AuthManagerService authManagerService)
        {
            this._authManagerService = authManagerService;
        }

        #region AppInfo
        public ActionResult AppInfoManager()
        {
            return View("AppInfoList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetAuthAppInfoListData(GridQuery filter)
        {
            var result = this._authManagerService.GetAuthAppInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult AuthAppInfoAdd()
        {
            return View("AppInfoAdd");
        }

        public ActionResult AuthAppInfoEdit(Guid id)
        {
            var model = this._authManagerService.GetAuthAppInfo(id);
            return View("AppInfoEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveAuthAppInfo(AuthAppInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._authManagerService.AddOrUpdateAllAuthAppInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult AuthAppInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._authManagerService.DeleteAllAuthAppInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }
        #endregion

        #region function

        public ActionResult AppFunctionInfoManager()
        {
            return View("AppFunctionInfoTreeList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetAppFunctionInfoManagerList()
        {
            var result = this._authManagerService.GetAuthAppInfoList();
            return Json(result);
        }

        public ActionResult FunctionInfoManagerList(Guid id)
        {
            var appInfo = this._authManagerService.GetAuthAppInfo(id);
            ViewBag.AppName = string.Format("{0}({1})", appInfo.Name, appInfo.Code);
            ViewBag.AppId = id;
            ViewBag.ParentFunctionId = Guid.Empty;
            return View("FunctionInfoList");
        }

        public ActionResult GetAuthFunctionInfoListData(Guid appId, Guid parentId)
        {
            JqGridData<AuthFunctionInfoModel> list = new JqGridData<AuthFunctionInfoModel>();
            list.PageIndex = 1;
            list.TotalPages = 1;
            list.TotalRecords = 10000;
            list.DataList = this._authManagerService.GetSubAuthFunctionInfoList(appId, parentId);
            return Json(list);
        }

        public ActionResult AuthFunctionInfoAdd(Guid parentId,Guid appId)
        {
            var model = new AuthFunctionInfoModel();
            model.ParentId = parentId;
            model.AppId = appId;
            return View("AppFunctionInfoAdd");
        }

        public ActionResult AuthFunctionInfoEdit(Guid id)
        {
            var model = this._authManagerService.GetAuthFunctionInfo(id);
            return View("AppFunctionInfoEdit", model);
        }

        [AjaxInkeyResultHandleError]
        public ActionResult SaveAuthFunctionInfo(AuthFunctionInfoModel model)
        {
            InkeyResult result = new InkeyResult();
            string errorMessage;
            if (this.VerifyModle(out errorMessage))
            {
                this._authManagerService.AddOrUpdateAllAuthFunctionInfo(model);
            }
            else
            {
                result.Code = -101;
                result.Desc = errorMessage;
            }
            return Json(result);
        }

            [AjaxInkeyResultHandleError]
        public ActionResult AuthFunctionInfoDeleteAll(string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            var flag = this._authManagerService.DeleteAllAuthFunctionInfo(idList);
            if (!flag)
            {
                result.Code = -101;
                result.Desc = "该应用下还有相关的权限资源，不能删除";
            }
            return Json(result);
        }

        #endregion

        #region userinfo

            public ActionResult AppUserInfoManager()
            {
                return View("UserInfoList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetAuthUserInfoListData(GridQuery filter)
            {
                var result = this._authManagerService.GetAuthUserInfoListByPager(filter);
                return Json(result);
            }

            public ActionResult AuthUserInfoAdd()
            {
                return View("UserInfoAdd");
            }

            public ActionResult AuthUserInfoEdit(Guid id)
            {
                var model = this._authManagerService.GetAuthUserInfo(id);
                return View("UserInfoEdit", model);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult SaveAuthUserInfo(AuthUserInfoModel model)
            {
                InkeyResult result = new InkeyResult();
                string errorMessage;
                if (this.VerifyModle(out errorMessage))
                {
                    this._authManagerService.AddOrUpdateAllAuthUserInfo(model);
                }
                else
                {
                    result.Code = -101;
                    result.Desc = errorMessage;
                }
                return Json(result);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult AuthUserInfoDeleteAll(string ids)
            {
                InkeyResult result = new InkeyResult();
                IList<Guid> idList = ids.Split(',').Select(i =>
                {
                    return Guid.Parse(i);
                }).ToList();
                var flag = this._authManagerService.DeleteAllAuthUserInfo(idList);
                if (!flag)
                {
                    result.Code = -101;
                    result.Desc = "该应用下还有相关的权限资源，不能删除";
                }
                return Json(result);
            }

        #endregion

        #region groupinfo

            public ActionResult AppGroupInfoManager()
            {
                return View("GroupInfoList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetAuthGroupInfoListData(GridQuery filter)
            {
                var result = this._authManagerService.GetAuthGroupInfoListByPager(filter);
                return Json(result);
            }

            public ActionResult AuthGroupInfoAdd()
            {
                return View("GroupInfoAdd");
            }

            public ActionResult AuthGroupInfoEdit(Guid id)
            {
                var model = this._authManagerService.GetAuthGroupInfo(id);
                return View("GroupInfoEdit", model);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult SaveAuthGroupInfo(AuthGroupInfoModel model)
            {
                InkeyResult result = new InkeyResult();
                string errorMessage;
                if (this.VerifyModle(out errorMessage))
                {
                    this._authManagerService.AddOrUpdateAllAuthGroupInfo(model);
                }
                else
                {
                    result.Code = -101;
                    result.Desc = errorMessage;
                }
                return Json(result);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult AuthGroupInfoDeleteAll(string ids)
            {
                InkeyResult result = new InkeyResult();
                IList<Guid> idList = ids.Split(',').Select(i =>
                {
                    return Guid.Parse(i);
                }).ToList();
                var flag = this._authManagerService.DeleteAllAuthGroupInfo(idList);
                if (!flag)
                {
                    result.Code = -101;
                    result.Desc = "该应用下还有相关的权限资源，不能删除";
                }
                return Json(result);
            }

            public ActionResult AppGroupUserInfoManager()
            {
                return View("GroupUserInfoTreeList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetGroupUserInfoTreeListData()
            {
                var result = this._authManagerService.GetGroupUserInfoTreeList();
                return Json(result);
            }

            public ActionResult GroupUserInfoManagerList(Guid id)
            {
                ViewBag.GroupId = id;
                var group = this._authManagerService.GetAuthGroupInfo(id);
                ViewBag.GroupName = group.Name;

                return View("GroupUserInfoList");
            }

        [AjaxInkeyResultHandleError]
            public ActionResult GetAuthGroupUserInfoListData(GridQuery filter)
            {
                var result = this._authManagerService.GetAuthGroupUserInfoListByPager(filter);
                return Json(result);
            }

        public ActionResult SelectAuthUserInfoToGroup(Guid id)
        {
            ViewBag.GroupId = id;
            return View();
        }

        [AjaxInkeyResultHandleError]
        public ActionResult AddAuthUserInfoToGroup(Guid groupId, string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            this._authManagerService.AddAuthUserInfoToGroup(groupId, idList);
            return Json(result);
        }

        public ActionResult RemoveAuthUserInfoToGroup(Guid groupId,string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            this._authManagerService.RemoveAuthUserInfoToGroup(groupId, idList);
            return Json(result);
        }

        public ActionResult AppGroupRoleInfoManager()
        {
            return View("GroupRoleInfoTreeList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetGroupRoleInfoTreeListData()
        {
            var result = this._authManagerService.GetGroupRoleInfoTreeList();
            return Json(result);
        }

        public ActionResult GroupRoleInfoManagerList(Guid id)
        {
            ViewBag.GroupId = id;
            var group = this._authManagerService.GetAuthGroupInfo(id);
            ViewBag.GroupName = group.Name;

            return View("GroupRoleInfoList");
        }

        [AjaxInkeyResultHandleError]
        public ActionResult GetAuthGroupRoleInfoListData(GridQuery filter)
        {
            var result = this._authManagerService.GetAuthGroupRoleInfoListByPager(filter);
            return Json(result);
        }

        public ActionResult SelectAuthRoleInfoToGroup(Guid id)
        {
            ViewBag.GroupId = id;
            return View();
        }

        [AjaxInkeyResultHandleError]
        public ActionResult AddAuthRoleInfoToGroup(Guid groupId, string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            this._authManagerService.AddAuthRoleInfoToGroup(groupId, idList);
            return Json(result);
        }

        public ActionResult RemoveAuthRoleInfoToGroup(Guid groupId, string ids)
        {
            InkeyResult result = new InkeyResult();
            IList<Guid> idList = ids.Split(',').Select(i =>
            {
                return Guid.Parse(i);
            }).ToList();
            this._authManagerService.RemoveAuthRoleInfoToGroup(groupId, idList);
            return Json(result);
        }

        #endregion

            #region RoleInfo
            public ActionResult AppRoleInfoManager()
            {
                return View("RoleInfoList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetAuthRoleInfoListData(GridQuery filter)
            {
                var result = this._authManagerService.GetAuthRoleInfoListByPager(filter);
                return Json(result);
            }

            public ActionResult AuthRoleInfoAdd()
            {
                return View("RoleInfoAdd");
            }

            public ActionResult AuthRoleInfoEdit(Guid id)
            {
                var model = this._authManagerService.GetAuthRoleInfo(id);
                return View("RoleInfoEdit", model);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult SaveAuthRoleInfo(AuthRoleInfoModel model)
            {
                InkeyResult result = new InkeyResult();
                string errorMessage;
                if (this.VerifyModle(out errorMessage))
                {
                    this._authManagerService.AddOrUpdateAllAuthRoleInfo(model);
                }
                else
                {
                    result.Code = -101;
                    result.Desc = errorMessage;
                }
                return Json(result);
            }

            [AjaxInkeyResultHandleError]
            public ActionResult AuthRoleInfoDeleteAll(string ids)
            {
                InkeyResult result = new InkeyResult();
                IList<Guid> idList = ids.Split(',').Select(i =>
                {
                    return Guid.Parse(i);
                }).ToList();
                var flag = this._authManagerService.DeleteAllAuthRoleInfo(idList);
                if (!flag)
                {
                    result.Code = -101;
                    result.Desc = "该应用下还有相关的权限资源，不能删除";
                }
                return Json(result);
            }

            


            public ActionResult AppRoleUserInfoManager()
            {
                return View("RoleUserInfoTreeList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetRoleUserInfoTreeListData()
            {
                var result = this._authManagerService.GetRoleUserInfoTreeList();
                return Json(result);
            }

            public ActionResult RoleUserInfoManagerList(Guid id)
            {
                ViewBag.RoleId = id;
                var role = this._authManagerService.GetAuthRoleInfo(id);
                ViewBag.RoleName = role.Name;

                return View("RoleUserInfoList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetAuthRoleUserInfoListData(GridQuery filter)
            {
                var result = this._authManagerService.GetAuthRoleUserInfoListByPager(filter);
                return Json(result);
            }

            public ActionResult SelectAuthUserInfoToRole(Guid id)
            {
                ViewBag.RoleId = id;
                return View();
            }

            [AjaxInkeyResultHandleError]
            public ActionResult AddAuthUserInfoToRole(Guid roleId, string ids)
            {
                InkeyResult result = new InkeyResult();
                IList<Guid> idList = ids.Split(',').Select(i =>
                {
                    return Guid.Parse(i);
                }).ToList();
                this._authManagerService.AddAuthUserInfoToRole(roleId, idList);
                return Json(result);
            }

            public ActionResult RemoveAuthUserInfoToRole(Guid roleId, string ids)
            {
                InkeyResult result = new InkeyResult();
                IList<Guid> idList = ids.Split(',').Select(i =>
                {
                    return Guid.Parse(i);
                }).ToList();
                this._authManagerService.RemoveAuthUserInfoToRole(roleId, idList);
                return Json(result);
            }

            public ActionResult AppRoleFunctionInfoManager()
            {
                return View("RoleFunctionInfoTreeList");
            }

            [AjaxInkeyResultHandleError]
            public ActionResult GetRoleFunctionInfoTreeListData(GridQuery filter)
            {
                var result = this._authManagerService.GetRoleFunctionInfoTreeList();
                return Json(result);
            }

            public ActionResult RoleFunctionInfoManagerList(Guid id)
            {
                ViewBag.RoleId = id;
                var role = this._authManagerService.GetAuthRoleInfo(id);
                ViewBag.RoleName = role.Name;

                var model = this._authManagerService.GetRoleFunctionInfoTree(id);
                return View("RoleFunctionInfoList", model);
            }

            public ActionResult SaveFunctionInfoToRole(RoleFunctionInfoTreeModel model)
            {
                InkeyResult result = new InkeyResult();
                this._authManagerService.SaveAuthFunctionInfoToRole(model);
                return Json(result);
            }
        
            #endregion

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
