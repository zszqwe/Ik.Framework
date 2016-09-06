using Ik.Framework;
using Ik.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ik.Framework.Common.Extension;

namespace Ik.WebFramework.Mvc
{
    public class AjaxInkeyResultHandleErrorAttribute : Attribute
    {
        private static ILog logger = LogManager.GetLogger();
        public bool IsAjaxFormResult { get; set; }
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
