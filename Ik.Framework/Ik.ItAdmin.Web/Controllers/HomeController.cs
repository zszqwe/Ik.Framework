using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using Ik.ItAdmin.Web.Controllers;
using System.Text;
using Ik.Framework.Events;
using Ik.ItAdmin.Web.Services;
using System.Diagnostics;
using Ik.WebFramework.Mvc.Auth;
using Ik.ItAdmin.Web.Models;



namespace Inkey.ConfigManagerCenter.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult SearchList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSearchListData(GridQueryModel query)
        {
            Ik.ItAdmin.Web.Models.JqGridDataModel<object> data = new Ik.ItAdmin.Web.Models.JqGridDataModel<object>();
            data.PageIndex = 1;
            data.TotalPages = 20;
            data.TotalRecords = 400;
            string[] type = new string[] { "dianxin","yidong","liantong" };
            List<object> list = new List<object>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(new { Id = i, CreateDate = DateTime.Now, PhoneNo = "18615707951", CardNo = "512085456874614641", PhoneType = type[i%3], Remark = "备注" + i });
            }
            data.DataList = list;
            return Json(data);
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
