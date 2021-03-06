﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Net.Http;

namespace Ik.WebFramework.Api
{

    public class GridQueryWebApiBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                if (bindingContext.ModelType != typeof(GridQuery))
                {
                    return false;
                }
                var request =  actionContext.Request.Content.ReadAsFormDataAsync().Result;
                GridQuery model = new GridQuery();
                foreach (string key in request.Keys)
                {
                    if (!model.PostData.ContainsKey(key))
                    {
                        model.PostData.Add(key, request[key]);
                    }
                }
                var keyValues = actionContext.Request.GetQueryNameValuePairs();
                foreach (var item in keyValues)
                {
                    if (!model.PostData.ContainsKey(item.Key))
                    {
                        model.PostData.Add(item.Key, item.Value);
                    }
                }
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
                            model.SortInfos.Add(new SortInfo { Name = item.Replace("asc", "").Trim(), Type = SortType.Asc });
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
                bindingContext.Model = model;
                return true;
            }
            catch
            {
                return false;
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
