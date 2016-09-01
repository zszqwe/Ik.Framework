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
