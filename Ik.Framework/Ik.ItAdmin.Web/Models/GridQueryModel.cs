using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ik.ItAdmin.Web.Models
{
    [ModelBinder(typeof(GridQueryModelBinder))]
    public class GridQueryModel
    {
        public GridQueryModel()
        {
            this.SortInfos = new List<SortInfo>();
            this.FilterInfos = new List<FilterInfo>();
            this.PostData = new Dictionary<string, string>();
        }
        public bool HasFilter { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public bool HasSort { get; set; }

        public IList<SortInfo> SortInfos { get; private set; }
        public IList<FilterInfo> FilterInfos { get; private set; }

        public IDictionary<string, string> PostData { get; private set; }
    }

    public class SortInfo
    {
        public string Name{ get; set; }
        public SortType Type { get; set; }
    }

    public enum SortType
    {
        Asc, Desc
    }

    public class FilterInfo
    {
        public FilterInfo()
        {
            this.Values = new List<string>();
        }
        public FilterComposeType ComposeType { get; set; }

        public string Name { get; set; }

        public FilterOperateType OperateType { get; set; }

        public bool HasMultipleValue { get; set; }

        public string Value { get; set; }

        public IList<string> Values { get; private set; }
    }

    public enum FilterComposeType
    {
        And, 
        Or
    }

    public enum FilterOperateType
    {
        Eq, //等于
        Ne, //不等
        Lt, //小于
        Le,//小于等于
        Gt,//大于
        Ge,//大于等于
        Bw,//开始于
        Bn,//不开始于
        In,//属于
        Ni,//不属于
        Ew,//结束于
        En,//不结束于
        Cn,//包含
        Nc,//不包含
        Nu,//不存在
        Nn, //存在
    }

    public class GridQueryModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var request = controllerContext.HttpContext.Request;
                GridQueryModel model = new GridQueryModel();

                model.HasFilter = bool.Parse(request["_search"] ?? "false");
                model.PageIndex = int.Parse(request["page"] ?? "1");
                model.PageSize = int.Parse(request["rows"] ?? "10");
                var sidx = request["sidx"];
                if (!string.IsNullOrEmpty(sidx))
                {
                    model.HasSort = true;
                    var list = sidx.Split(',');
                    foreach (var item in list)
                    {
                        if (item.IndexOf("asc") > -1)
                        {
                            model.SortInfos.Add(new SortInfo { Name = item.Replace("asc","").Trim(), Type = SortType.Asc });
                        }
                        else if (item.IndexOf("desc") > -1)
                        {
                            model.SortInfos.Add(new SortInfo { Name = item.Replace("desc", "").Trim(), Type = SortType.Desc });
                        }
                        else
                        {
                            SortType type = SortType.Asc;
                            if (request["sord"] == "desc")
                            {
                                type = SortType.Desc;
                            }
                            model.SortInfos.Add(new SortInfo { Name = item.Trim(), Type = type });
                        }
                    }
                }

                var filters = request["filters"];
                if (!string.IsNullOrEmpty(filters))
                {
                    JToken token = JToken.Parse(filters);
                    var groupOp = (JValue)token.SelectToken("groupOp");
                    FilterComposeType composeType = FilterComposeType.And;
                    if ((string)groupOp.Value == "OR")
                    {
                        composeType = FilterComposeType.Or;
                    }

                    var rules = token.SelectToken("rules");
                    foreach (JToken item in rules)
                    {
                        FilterInfo filterInfo = new FilterInfo();
                        filterInfo.ComposeType = composeType;
                        var field = (JValue)item.SelectToken("field");
                        filterInfo.Name = (string)field.Value;
                        var op = (JValue)item.SelectToken("op");
                        var operateType = (FilterOperateType)Enum.Parse(typeof(FilterOperateType), (string)op.Value, true);
                        filterInfo.OperateType = operateType;
                        var data = item.SelectToken("data");
                        if (data.Type == JTokenType.Array)
                        {
                            filterInfo.HasMultipleValue = true;
                            foreach (JToken vItem in data.Children())
                            {
                                filterInfo.Values.Add((string)((JValue)vItem).Value);
                            }
                        }
                        else
                        {
                            filterInfo.Value = (string)((JValue)data).Value;
                        }
                        model.FilterInfos.Add(filterInfo);
                    }
                }
                return model;
            }
            catch
            {
                return null;
            }
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
