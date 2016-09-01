using Ik.Framework.Common.Paging;
using Ik.Framework.DataAccess.EF;
using Ik.ItAdmin.Web.DataAccess.EF;
using Ik.ItAdmin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ik.Framework.Common.Extension;
using Ik.WebFramework;
using Ik.Framework.DependencyManagement;
using Ik.ItAdmin.Web.DataAccess;
using Ik.ItAdmin.Web.Dtos;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Ik.Framework.Caching.CacheKeyManager;
using System.Collections.Specialized;
using Ik.Framework;
using Ik.Framework.Caching;
using Newtonsoft.Json;
using Ik.Framework.Redis;
using Ik.Framework.Common.Serialization;

namespace Ik.ItAdmin.Web.Services
{
     [AutoBizServiceAttribute]
    public class CacheKeyManagerService
    {
        private IRepository<CacheKeyAppInfoEntity> _efCacheKeyAppInfoRepository = null;
        private IRepository<CacheKeyItemInfoEntity> _efCacheKeyItemInfoRepository = null;
        private IRepository<CacheValueTypeEntity> _efCacheValueTypeRepository = null;
        private IRepository<CacheKeyAssInfoEntity> _efCacheKeyAssInfoRepository = null;
        public CacheKeyManagerService(IRepository<CacheKeyAppInfoEntity> efCacheKeyAppInfoRepository
            , IRepository<CacheKeyItemInfoEntity> efCacheKeyItemInfoRepository
            , IRepository<CacheKeyAssInfoEntity> efCacheKeyAssInfoRepository
            , IRepository<CacheValueTypeEntity> efCacheValueTypeRepository)
        {
            this._efCacheKeyAppInfoRepository = efCacheKeyAppInfoRepository;
            this._efCacheKeyItemInfoRepository = efCacheKeyItemInfoRepository;
            this._efCacheValueTypeRepository = efCacheValueTypeRepository;
            this._efCacheKeyAssInfoRepository = efCacheKeyAssInfoRepository;
        }

        #region appInfo

        public IPagedList<CacheKeyAppInfoModel> GetCacheKeyAppInfoListByPager(GridQuery filter)
        {
            var list = this._efCacheKeyAppInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efCacheKeyAppInfoRepository.TableNoTracking.Count();

            return list.Select(app =>
            {
                return new CacheKeyAppInfoModel
                {
                    AppId = app.Id,
                    Code = app.Code,
                    CreateTime = app.CreateTime,
                    Desc = app.Desc,
                    Name = app.Name
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public IList<AppKeyItemInfoTreeNode> GetCacheKeyAppInfoList()
        {
            var list = this._efCacheKeyAppInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(app =>
            {
                var node = new AppKeyItemInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = app.Id;
                node.AppId = app.Id;
                node.Name = app.Name;
                node.Code = app.Code;
                return node;
            }).ToList();
        }

        public void AddOrUpdateAllCacheKeyAppInfo(CacheKeyAppInfoModel model)
        {
            if (model.AppId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var appInfo = this._efCacheKeyAppInfoRepository.GetById(model.AppId.Value);
                    appInfo.Code = model.Code;
                    appInfo.Name = model.Name;
                    appInfo.Desc = model.Desc;
                    this._efCacheKeyAppInfoRepository.Update(appInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efCacheKeyAppInfoRepository.Insert(new CacheKeyAppInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    Name = model.Name
                });
            }
        }

        public CacheKeyAppInfoModel GetCacheKeyAppInfo(Guid id)
        {
            var app = this._efCacheKeyAppInfoRepository.GetById(id);
            return new CacheKeyAppInfoModel
            {
                AppId = app.Id,
                Code = app.Code,
                CreateTime = app.CreateTime,
                Desc = app.Desc,
                Name = app.Name
            };
        }

        public bool DeleteAllCacheKeyAppInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var appInfo = this._efCacheKeyAppInfoRepository.GetById(item);
                    var count = this._efCacheKeyItemInfoRepository.TableNoTracking.Where(key => key.AppId == appInfo.Id).Count();
                    if (count > 0)
                    {
                        return false;
                    }
                    this._efCacheKeyAppInfoRepository.Delete(appInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }

        #endregion

        #region keyitem
        public IPagedList<CacheKeyItemInfoModel> GetCacheKeyItemInfoListByPager(GridQuery filter)
        {
            Guid appId = Guid.Parse(filter.PostData["appId"]);
            var list = this._efCacheKeyItemInfoRepository.TableNoTracking.Where(item => item.AppId == appId).OrderByDescending(a => a.UpdateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efCacheKeyItemInfoRepository.TableNoTracking.Count();

            return list.Select(item =>
            {
                return new CacheKeyItemInfoModel
                {
                    AppId = item.AppId,
                    Code = item.Code,
                    CreateTime = item.CreateTime,
                    Desc = item.Desc,
                    Name = item.Name,
                    UpdateTime = item.UpdateTime,
                    ItemId = item.Id,
                    KeySocpe = item.KeySocpe,
                    CacheType = item.CacheType,
                    RefValueType = item.RefValueType,
                    ModelName = item.ModelName
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public CacheKeyItemInfoModel GetCacheKeyItemInfo(Guid id)
        {
            var item = this._efCacheKeyItemInfoRepository.GetById(id);
            return new CacheKeyItemInfoModel
            {
                AppId = item.AppId,
                Code = item.Code,
                CreateTime = item.CreateTime,
                Desc = item.Desc,
                Name = item.Name,
                UpdateTime = item.UpdateTime,
                ItemId = item.Id,
                KeySocpe = item.KeySocpe,
                CacheType = item.CacheType,
                ModelName = item.ModelName,
                RefValueType = item.RefValueType,
                AppName = item.CacheKeyAppInfo.Name,
                AppCode = item.CacheKeyAppInfo.Code
            };
        }

        public void AddOrUpdateAllCacheKeyItemInfo(CacheKeyItemInfoModel model)
        {
            if (model.ItemId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var itemInfo = this._efCacheKeyItemInfoRepository.GetById(model.ItemId.Value);
                    itemInfo.Code = model.Code;
                    itemInfo.Name = model.Name;
                    itemInfo.Desc = model.Desc;
                    itemInfo.UpdateTime = DateTime.Now;
                    itemInfo.KeySocpe = model.KeySocpe;
                    itemInfo.CacheType = model.CacheType;
                    itemInfo.ModelName = model.ModelName;
                    itemInfo.RefValueType = model.RefValueType;
                    this._efCacheKeyItemInfoRepository.Update(itemInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efCacheKeyItemInfoRepository.Insert(new CacheKeyItemInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    Name = model.Name,
                    UpdateTime = DateTime.Now,
                    ModelName = model.ModelName,
                    KeySocpe = model.KeySocpe,
                    CacheType = model.CacheType,
                    RefValueType = model.RefValueType,
                    AppId = model.AppId.Value
                });
            }
        }

        public bool DeleteAllCacheKeyItemInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var itemInfo = this._efCacheKeyItemInfoRepository.GetById(item);
                    this._efCacheKeyItemInfoRepository.Delete(itemInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }
        #endregion

        #region value Type

        public IPagedList<CacheKeyValueTypeInfoModel> GetCacheKeyValueTypeInfoListByPager(GridQuery filter)
        {
            var assId = Guid.Parse(filter.PostData["assId"]);
            var list = this._efCacheValueTypeRepository.TableNoTracking.Where(a=>a.AssId == assId).OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efCacheValueTypeRepository.TableNoTracking.Count();

            return list.Select(item =>
            {
                return new CacheKeyValueTypeInfoModel
                {
                    Code = item.Code,
                    CreateTime = item.CreateTime,
                    Desc = item.Desc,
                    Name = item.Name,
                    ValueTypeId = item.Id
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public CacheKeyValueTypeInfoModel GetCacheKeyValueTypeInfo(Guid id)
        {
            var item = this._efCacheValueTypeRepository.GetById(id);
            return new CacheKeyValueTypeInfoModel
            {
                Code = item.Code,
                CreateTime = item.CreateTime,
                Desc = item.Desc,
                Name = item.Name,
                ValueTypeId = item.Id,
                ClassContext = item.ClassContext
            };
        }

        public IList<CacheKeyValueTypeInfoModel> GetCacheKeyValueTypeInfoList()
        {
            var list = this._efCacheValueTypeRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(item =>
           {
               return new CacheKeyValueTypeInfoModel
               {
                   Code = item.Code,
                   CreateTime = item.CreateTime,
                   Desc = item.Desc,
                   Name = item.Name,
                   ValueTypeId = item.Id,
                   ClassContext = item.ClassContext,
                   ValueTypeAssemblyName = item.CacheKeyAssInfo.Code
               };
           }).ToList();
        }

        public void AddOrUpdateAllCacheKeyValueTypeInfo(CacheKeyValueTypeInfoModel model)
        {
            if (model.ValueTypeId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var itemInfo = this._efCacheValueTypeRepository.GetById(model.ValueTypeId.Value);
                    itemInfo.Code = model.Code;
                    itemInfo.Name = model.Name;
                    itemInfo.Desc = model.Desc;
                    itemInfo.ClassContext = model.ClassContext;
                    this._efCacheValueTypeRepository.Update(itemInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efCacheValueTypeRepository.Insert(new CacheValueTypeEntity
                {
                    Id = Guid.NewGuid(),
                    AssId = model.AssId.Value,
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    Name = model.Name,
                    ClassContext = model.ClassContext
                });
            }
        }

        public bool DeleteAllCacheValueTypeItemInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var itemInfo = this._efCacheValueTypeRepository.GetById(item);
                    this._efCacheValueTypeRepository.Delete(itemInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }
        #endregion


        #region cacheManager

        public InkeyResult<string> CacheManager(string actionType, Guid itemId, string sType, NameValueCollection forms, string cacheResult)
        {
            InkeyResult<string> result = new InkeyResult<string>();
            var model = GetCacheKeyItemInfo(itemId);
            var valueTypes = GetCacheKeyValueTypeInfoList();
            KeyConfigFormatObjcet formats = new KeyConfigFormatObjcet(model.AppCode,new List<KeyItem> {  new KeyItem { Key = model.Code, Scope = model.KeySocpe } },
                valueTypes.Select(v => new KeyTypeDefine { ValueType = v.Code, ValueClassCode = v.ClassContext,ValueTypeAssemblyName = v.ValueTypeAssemblyName }).ToList());
            var formart = formats.FormatObjcets[model.Code];
            List<string> pValueList = new List<string>();
            foreach (var item in formart.KeyList)
            {
                string pName = item.TrimStart('{').TrimEnd('}');
                string pValue = forms[pName];
                if (string.IsNullOrEmpty(pValue))
                {
                    result.Code = InkeyErrorCodes.CommonBusinessFailure;
                    result.Desc = "参数不能为空，参数名称：" + pName;
                    return result;
                }
                pValueList.Add(pValue);
            }
            string vType = null;
            if (!string.IsNullOrEmpty(model.RefValueType))
            {
                foreach (var item in valueTypes)
                {
                    if (item.Code == model.RefValueType)
                    {
                        vType = string.Format("{0},{1}", item.Code, item.ValueTypeAssemblyName);
                        break;
                    }
                }
            }
            if (model.CacheType == CacheEnvType.Memcached)
            {
                return GetMemcachedResult(actionType, sType, vType, cacheResult, formart, pValueList);
            }
            return GetRedisResult(actionType, sType, vType, cacheResult, formart, pValueList);
        }

        private static InkeyResult<string> GetMemcachedResult(string actionType, string sType,string vType, string cacheResult, FormatObjcet formart, List<string> pValueList)
        {
            InkeyResult<string> result = new InkeyResult<string>();
            var cacheManager = new MemcachedManager(new CacheManagerActionCacheKeyFormat());
            var flag = false;
            switch (actionType)
            {
                case "View":
                    object value = string.Empty;
                        flag = cacheManager.RawIsSet(formart.Key, out value, pValueList.ToArray());
                        if (!flag)
                        {
                            result.Code = InkeyErrorCodes.CommonBusinessFailure;
                            result.Desc = "未设置缓存值";
                            return result;
                        }
                        else
                        {
                            result.Data = value is string ? (string)value : value.ToJsonString();
                        }
                    break;
                case "Update":
                    if (cacheManager.RawIsSet(formart.Key, out value, pValueList.ToArray()) && !(value is string))
                    {
                        value = JsonConvert.DeserializeObject(cacheResult, value.GetType());
                        flag = cacheManager.RawSet(formart.Key, value, pValueList.ToArray());
                    }
                    else
                    {
                        flag = cacheManager.RawSet(formart.Key, cacheResult, pValueList.ToArray());
                    }
                    if (!flag)
                    {
                        result.Code = InkeyErrorCodes.CommonBusinessFailure;
                        result.Desc = "请输入JSON";
                        return result;
                    }
                    else
                    {
                        result.Code = InkeyErrorCodes.CommonBusinessFailure;//前端需要弹出提示，所有设置这个值
                        result.Desc = "更新成功";
                    }
                    break;
                case "Delete":
                    flag = cacheManager.Remove(formart.Key, pValueList.ToArray());
                    if (!flag)
                    {
                        result.Code = InkeyErrorCodes.CommonBusinessFailure;
                        result.Desc = "缓存移除失败";
                        return result;
                    }
                    break;
            }
            return result;
        }

        private static InkeyResult<string> GetRedisResult(string actionType, string sType, string vType, string cacheResult, FormatObjcet formart, List<string> pValueList)
        {
            InkeyResult<string> result = new InkeyResult<string>();
            bool sJsonType = sType == "JSON";
            var cacheManager = new RedisManager(new CacheManagerActionCacheKeyFormat());
            var flag = false;
            switch (actionType)
            {
                case "View":
                     flag = cacheManager.Exists(formart.Key, pValueList.ToArray());
                     if (!flag)
                     {
                         result.Code = InkeyErrorCodes.CommonBusinessFailure;
                         result.Desc = "未设置缓存值";
                         return result;
                     }
                    if (sJsonType)
                    {
                        result.Data = cacheManager.GetOriginalValue(formart.Key, pValueList.ToArray());
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(vType))
                        {
                            result.Code = InkeyErrorCodes.CommonBusinessFailure;
                            result.Desc = "值类型未设置";
                            return result;
                        }
                        Type type = Type.GetType(vType);
                        if (type == null)
                        {
                            result.Code = InkeyErrorCodes.CommonBusinessFailure;
                            result.Desc = "值类型未定义，类型：" + vType;
                            return result;
                        }
                        byte[] oValue = cacheManager.RawGetOriginalValue(formart.Key, pValueList.ToArray());
                        object value = BinarySerializerManager.BinaryDeSerialize(oValue);
                        result.Data = value.ToJsonString();
                    }
                    break;
                case "Update":
                    flag = cacheManager.Exists(formart.Key, pValueList.ToArray());
                     if (!flag)
                     {
                         result.Code = InkeyErrorCodes.CommonBusinessFailure;
                         result.Desc = "未设置缓存值";
                         return result;
                     }
                     if (sJsonType)
                     {
                         flag = cacheManager.OriginalValueAdd(formart.Key, cacheResult, pValueList.ToArray());
                         if (!flag)
                         {
                             result.Code = InkeyErrorCodes.CommonBusinessFailure;
                             result.Desc = "更新失败";
                             return result;
                         }
                     }
                     else
                     {
                         if (string.IsNullOrEmpty(vType))
                         {
                             result.Code = InkeyErrorCodes.CommonBusinessFailure;
                             result.Desc = "值类型未设置";
                             return result;
                         }
                         Type type = Type.GetType(vType);
                         if (type == null)
                         {
                             result.Code = InkeyErrorCodes.CommonBusinessFailure;
                             result.Desc = "值类型未定义，类型：" + vType;
                             return result;
                         }
                         var oValue = JsonSerializerManager.JsonDeserialize(cacheResult, type);
                         if (oValue == null)
                         {
                             result.Code = InkeyErrorCodes.CommonBusinessFailure;
                             result.Desc = "json格式错误，类型：" + vType;
                             return result;
                         }
                         else
                         {
                             var value = BinarySerializerManager.BinarySerialize(oValue);
                             flag = cacheManager.RawOriginalValueAdd(formart.Key, value, pValueList.ToArray());
                             if (!flag)
                             {
                                 result.Code = InkeyErrorCodes.CommonBusinessFailure;
                                 result.Desc = "更新失败";
                                 return result;
                             }
                         }
                     }
                    result.Code = InkeyErrorCodes.CommonBusinessFailure;//前端需要弹出提示，所有设置这个值
                    result.Desc = "更新成功";
                    break;
                case "Delete":
                    flag = cacheManager.Remove(formart.Key, pValueList.ToArray());
                    if (!flag)
                    {
                        result.Code = InkeyErrorCodes.CommonBusinessFailure;
                        result.Desc = "缓存移除失败";
                        return result;
                    }
                    break;
            }
            return result;
        }


        /// <summary>
        /// 当前缓存操作不做缓存Key检查
        /// </summary>
        private class CacheManagerActionCacheKeyFormat : ICacheKeyFormat
        {

            public string FormatKey(string key, params object[] values)
            {
                FormatObjcet formatKey = new FormatObjcet();
                formatKey.Key = key;
                return formatKey.FormatKey(values);
            }


            public Type GetKeyDefineType(string key)
            {
                throw new NotImplementedException();
            }
        }
        #endregion


        #region assInfo

        public IPagedList<CacheKeyAssInfoModel> GetCacheKeyAssInfoListByPager(GridQuery filter)
        {
            var list = this._efCacheKeyAssInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efCacheKeyAssInfoRepository.TableNoTracking.Count();

            return list.Select(app =>
            {
                return new CacheKeyAssInfoModel
                {
                    AssId = app.Id,
                    Code = app.Code,
                    CreateTime = app.CreateTime,
                    Desc = app.Desc,
                    Name = app.Name
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public IList<AssValueTypeInfoTreeNode> GetCacheKeyAssInfoList()
        {
            var list = this._efCacheKeyAssInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(ass =>
            {
                var node = new AssValueTypeInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = ass.Id;
                node.AssId = ass.Id;
                node.Name = ass.Name;
                node.Code = ass.Code;
                return node;
            }).ToList();
        }

        public void AddOrUpdateAllCacheKeyAssInfo(CacheKeyAssInfoModel model)
        {
            if (model.AssId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var appInfo = this._efCacheKeyAssInfoRepository.GetById(model.AssId.Value);
                    appInfo.Code = model.Code;
                    appInfo.Name = model.Name;
                    appInfo.Desc = model.Desc;
                    this._efCacheKeyAssInfoRepository.Update(appInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efCacheKeyAssInfoRepository.Insert(new CacheKeyAssInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    Name = model.Name
                });
            }
        }

        public CacheKeyAssInfoModel GetCacheKeyAssInfo(Guid id)
        {
            var app = this._efCacheKeyAssInfoRepository.GetById(id);
            return new CacheKeyAssInfoModel
            {
                AssId = app.Id,
                Code = app.Code,
                CreateTime = app.CreateTime,
                Desc = app.Desc,
                Name = app.Name
            };
        }

        public bool DeleteAllCacheKeyAssInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var assInfo = this._efCacheKeyAssInfoRepository.GetById(item);
                    var count = this._efCacheValueTypeRepository.TableNoTracking.Where(key => key.AssId == assInfo.Id).Count();
                    if (count > 0)
                    {
                        return false;
                    }
                    this._efCacheKeyAssInfoRepository.Delete(assInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }

        #endregion
    }

    
}