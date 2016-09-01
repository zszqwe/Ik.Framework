using Ik.Framework;
using Ik.Framework.Common.Paging;
using Ik.Framework.Common.Utilities;
using Ik.Framework.DependencyManagement;
using Ik.ItAdmin.Web.DataAccess;
using Ik.ItAdmin.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Services
{
    [AutoBizServiceAttribute]
    public class CfgManagerService 
    {
        ICfgManagerDao _cfgManagerDao = null;

        public CfgManagerService(ICfgManagerDao cfgManagerDao)
        {
            this._cfgManagerDao = cfgManagerDao;
        }

        public IList<CfgDefinitionVersionTreeNode> GetCfgDefinitionVersionList()
        {
            var list = _cfgManagerDao.GetCfgDefinitionVersionList();
            var group = list.GroupBy(c => c.DefId);
            var treeNodeList = new List<CfgDefinitionVersionTreeNode>();
            int index = 1;
            foreach (var item in group)
            {
                var node = new CfgDefinitionVersionTreeNode();
                node.Id = index;
                node.DefId = item.Key;
                int count = 0;
                string name = string.Empty;
                foreach (var itemValue in item)
                {
                    name = itemValue.Title;
                    if (itemValue.DefVerId == Guid.Empty)
                    {
                        continue;
                    }
                    count++;
                    index++;
                    name = itemValue.Title + "(" + itemValue.Name + ")";
                    var nodeVersion = new CfgDefinitionVersionTreeNode();
                    nodeVersion.Id = index;
                    nodeVersion.ParentId = node.Id;
                    nodeVersion.DefVerId = itemValue.DefVerId;
                    nodeVersion.Name = itemValue.Version;
                    nodeVersion.Level = 1;
                    treeNodeList.Add(nodeVersion);
                    
                }
                node.Name = name;
                if (count > 0)
                {
                    node.HasChild = true;
                }
                treeNodeList.Add(node);
                index++;
            }
            return treeNodeList;
        }

        public CfgDefinition GetCfgDefinition(Guid id)
        {
            return _cfgManagerDao.GetCfgDefinitionById(id);
        }

        public CfgDefinitionVersion GetCfgDefinitionVersion(Guid id)
        {
            return _cfgManagerDao.GetCfgDefinitionVersionById(id);
        }

        public void AddOrUpdateAllCfgDefinition(CfgDefinition model)
        {
            if (model.DefId != Guid.Empty)
            {
                _cfgManagerDao.UpdateAllCfgDefinition(model);
            }
            else
            {
                model.DefId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgDefinition(model);
            }
        }

        public void AddOrUpdateAllCfgDefinitionVersion(CfgDefinitionVersion model)
        {
            if (model.DefVerId != Guid.Empty)
            {
                _cfgManagerDao.UpdateAllCfgDefinitionVersion(model);
            }
            else
            {
                model.DefVerId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgDefinitionVersion(model);
            }
        }

        public IList<CfgEnvironment> GetCfgEnvironmentList()
        {
            return _cfgManagerDao.GetCfgEnvironmentList();
        }

        public void AddOrUpdateAllCfgEnvironment(CfgEnvironment model)
        {
            if (model.EnvId != Guid.Empty)
            {
                _cfgManagerDao.UpdateAllCfgEnvironment(model);
            }
            else
            {
                model.EnvId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgEnvironment(model);
            }
        }

        public CfgEnvironment GetCfgEnvironment(Guid id)
        {
            return _cfgManagerDao.GetCfgEnvironmentById(id);
        }

        public IPagedList<CfgItemVersionEnvironment> GetCfgItemListByPager(CfgItemVersionEnvironmentParam filter)
        {
            return _cfgManagerDao.GetCfgItemListByPager(filter);
        }

        public CfgItem GetCfgItem(Guid id)
        {
            return _cfgManagerDao.GetCfgItemById(id);
        }

        public IList<CfgItemVersion> GetCfgItemVersionList(Guid id)
        {
            return _cfgManagerDao.GetCfgItemVersionList(id);
        }

        public void AddCfgItemAndVersion(CfgItemVersionEnvironment model)
        {
            using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
                var itemId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgItem(new CfgItem { ItemId = itemId, Key = model.Key, ItemDesc = model.ItemDesc, ItemType = model.ItemType, DefId = model.DefId });
                var itemVerId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgItemVersion(new CfgItemVersion { ItemVerId = itemVerId, ItemId = itemId, Version = model.Version });
                var itemVerEnvValId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgItemVersionEnvironment(new CfgItemVersionEnvironment { ItemVerEnvValId = itemVerEnvValId, ItemVerId = itemVerId, EnvId = model.EnvId, Value = model.Value, UpdateTime = DateTime.Now });
                scope.Complete();
            }
        }

        public void AddCfgVersionAndEnvValue(CfgItemVersionEnvironment model)
        {
            using (var scope = OperationSupportDataAccessFactory.Instance.CreateReadOnlyContextScope())
            {
                var itemVerId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgItemVersion(new CfgItemVersion { ItemVerId = itemVerId, ItemId = model.ItemId, Version = model.Version });
                var itemVerEnvValId = Guid.NewGuid();
                _cfgManagerDao.InsertCfgItemVersionEnvironment(new CfgItemVersionEnvironment { ItemVerEnvValId = itemVerEnvValId, ItemVerId = itemVerId, EnvId = model.EnvId, Value = model.Value, UpdateTime = DateTime.Now });
            }
        }

        public void AddCfgEnvValue(CfgItemVersionEnvironment model)
        {
            var itemVerEnvValId = Guid.NewGuid();
            _cfgManagerDao.InsertCfgItemVersionEnvironment(new CfgItemVersionEnvironment { ItemVerEnvValId = itemVerEnvValId, ItemVerId = model.ItemVerId, EnvId = model.EnvId, Value = model.Value, UpdateTime = DateTime.Now });
        }

        public void EditCfgItem(CfgItem model)
        {
            _cfgManagerDao.UpdateAllCfgItem(model);
        }

        public void EditCfgItemVerion(CfgItemVersion model)
        {
            _cfgManagerDao.UpdateAllCfgItemVersion(model);
        }

        public CfgItemVersion GetCfgItemVersion(Guid id)
        {
            return _cfgManagerDao.GetCfgItemVersionById(id);
        }

        public void EditCfgItemVerionEnvironment(CfgItemVersionEnvironment model)
        {
            _cfgManagerDao.UpdateAllCfgItemVersionEnvironment(model);
        }

        public CfgItemVersionEnvironment GetCfgItemVersionEnvironment(Guid id)
        {
            return _cfgManagerDao.GetCfgItemVersionEnvironmentById(id);
        }

        

        public IPagedList<CfgItemVersionEnvironment> GetCfgDefinitionItemListByPager(CfgItemVersionEnvironmentParam filter)
        {
            return _cfgManagerDao.GetCfgDefinitionItemListByPager(filter);
        }

        public void SetCfgDefinitionItem(Guid id, Guid defVerId)
        {
            _cfgManagerDao.InsertCfgDefinitionItem(id, defVerId);
        }

        public void DeleteAllCfgItem(IList<Guid> idList)
        {
            using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
                _cfgManagerDao.DeleteAllCfgItem(idList);
                scope.Complete();
            }
        }

        public void DeleteAllCfgDefinitionItem(IList<Guid> idList)
        {
             using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
            _cfgManagerDao.DeleteAllCfgDefinitionItem(idList);
                 scope.Complete();
            }
        }

        public void DeleteAllCfgItemVersion(IList<Guid> idList)
        {
             using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
            _cfgManagerDao.DeleteAllCfgItemVersion(idList);
                 scope.Complete();
            }
        }

        public void DeleteAllCfgItemVersionEnvironment(IList<Guid> idList)
        {
             using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
            _cfgManagerDao.DeleteAllCfgItemVersionEnvironment(idList);
                 scope.Complete();
            }
        }

        public void DeleteCfgDefinition(Guid id)
        {
             using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
            _cfgManagerDao.DeleteCfgDefinition(id);
                 scope.Complete();
            }
        }

        public void DeleteCfgDefinitionVersion(Guid id)
        {
             using (var scope = OperationSupportDataAccessFactory.Instance.CreateTransactionScope())
            {
            _cfgManagerDao.DeleteCfgDefinitionVersion(id);
            scope.Complete();
            }
        }

        public void PublishAllCfgDefinitonItem(Guid id)
        {
            var itemList = _cfgManagerDao.GetCfgDefinitionVersionItemList(id);
            if (itemList.Count > 0)
            {
                Ik.Framework.Configuration.RemoteConfigManagerService service = (Ik.Framework.Configuration.RemoteConfigManagerService)Ik.Framework.Configuration.ConfigManager.RemoteConfigService;
                if (!service.IsRunning)
                {
                    throw new InkeyException(InkeyErrorCodes.CommonFailure,"配置服务未启动");
                }
                var definitionVersion = GetCfgDefinitionVersion(id);
                foreach (var item in itemList)
                {
                    service.Add(definitionVersion.Name, VersionHelper.FormatVersion(definitionVersion.Version), item.EnvKey, item.Key, item.Value);
                }
            }
        }

        public IPagedList<CfgItemVersionEnvironment> GetCfgDefinitionVersionItemListByPager(CfgItemVersionEnvironmentParam filter)
        {
            return _cfgManagerDao.GetCfgDefinitionVersionItemListByPager(filter);
        }


        public IPagedList<CfgItemVersionEnvironment> GetSelectCfgItemListByPager(CfgItemVersionEnvironmentParam filter)
        {
            return _cfgManagerDao.GetSelectCfgItemListByPager(filter);
        }
    }
}