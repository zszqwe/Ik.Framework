using Ik.ItAdmin.Web.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Ik.Framework.Common.Paging;
using Ik.ItAdmin.Web.Models;

namespace Ik.ItAdmin.Web.Infrastructure
{
    public static class RoleTreeExtensions
    {
        public static MvcHtmlString RoleTree(this HtmlHelper helper, RoleFunctionInfoTreeModel model)
        {
            var htmlString = ReAppear(model);
            return new MvcHtmlString(htmlString);

        }

        private static string px = "<span style=\"height:25px;width:20px;float:left;\">&nbsp;</span>";
        private static string tl = "<span>{0}</span>";
        private static string ico_container = "<span style=\"height: 20px; width: 25px; float: left; \"><img src=\"/Content/image/commonstyle/wiscofile.gif\" /></span>";
        private static string ico_func = "<span style=\"height: 20px; width: 25px; float: left; \"><img src=\"/Content/image/commonstyle/wiscodate.gif\" /></span>";
        private static string checkBoxtl = "<input type=\"checkbox\" class=\"funcs_cls\" value=\"{0}\" {1} style=\"margin-left:5px;margin-right:5px;\"/>";
        private static int level = 0;

        private static string ReAppear(RoleFunctionInfoTreeModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (model.FunctionInfos.Count == 0)
            {
                sb.Append("<!--no data-->");
                return sb.ToString();
            }
            sb.AppendLine("<table id=\"rtree_" + model.AppId + "\" appId=\"" + model.AppId + "\" cellpadding=\"0\" cellspacing=\"0\" width=\"98%\">");
            ReAppearTr(model.AddedFunctionInfos, model.FunctionInfos, sb);
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private static string GetPx(int level)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < level; j++)
            {
                sb.Append(px);
            }
            return sb.ToString();
        }

        private static void ReAppearTr(Dictionary<Guid, AuthFunctionInfoEntity> AddedFunctionInfos, IList<AuthFunctionInfoEntity> funcs, StringBuilder sb)
        {
            level++;
            List<AuthFunctionInfoEntity> leafFuncs = new List<AuthFunctionInfoEntity>();
            List<AuthFunctionInfoEntity> nodeFuncs = new List<AuthFunctionInfoEntity>();
            foreach (var item in funcs)
            {
                if (item.SubAuthFunctionInfos.Count == 0)
                {
                    leafFuncs.Add(item);
                }
                else
                {
                    nodeFuncs.Add(item);
                }
            }
            if (leafFuncs.Count > 0)
            {
                PagedList<AuthFunctionInfoEntity> pagelists = new PagedList<AuthFunctionInfoEntity>(leafFuncs, 1, 5, leafFuncs.Count);
                int pages = pagelists.TotalPages;
                for (int i = 0; i < pages; i++)
                {
                    sb.AppendLine("<tr><td>");
                    sb.Append(GetPx(level));
                    sb.Append(ico_func);
                    IList<AuthFunctionInfoEntity> pageList = pagelists;
                    if (i > 0)
                    {
                        pageList = new PagedList<AuthFunctionInfoEntity>(leafFuncs, i++, 5);
                    }
                    foreach (var item in pageList)
                    {
                        if (AddedFunctionInfos.ContainsKey(item.Id))
                        {
                            sb.AppendFormat(checkBoxtl, item.Id, "checked=\"checked\"");
                        }
                        else
                        {
                            sb.AppendFormat(checkBoxtl, item.Id, "");
                        }
                        sb.AppendFormat(tl, item.Name);
                    }
                    sb.AppendLine("</td></tr>");
                }
                
            }
            foreach (var item in nodeFuncs)
            {
                sb.AppendLine("<tr><td>");
                for (int i = 0; i < level; i++)
                {
                    sb.Append(px);
                }
                sb.Append(ico_container);
                sb.AppendFormat(tl, item.Name);

                sb.AppendLine("</td></tr>");
                ReAppearTr(AddedFunctionInfos,item.SubAuthFunctionInfos.ToList(), sb);
            }
            level--;
        }
    }


}