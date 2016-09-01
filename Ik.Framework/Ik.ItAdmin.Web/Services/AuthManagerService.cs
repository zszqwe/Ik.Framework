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

namespace Ik.ItAdmin.Web.Services
{
     [AutoBizServiceAttribute]
    public class AuthManagerService
    {
        private IRepository<AuthAppInfoEntity> _efAuthAppInfoRepository = null;
        private IRepository<AuthFunctionInfoEntity> _efAuthFunctionInfoRepository = null;
        private IRepository<AuthGroupInfoEntity> _efAuthGroupInfoRepository = null;
        private IRepository<AuthRoleInfoEntity> _efAuthRoleInfoRepository = null;
        private IRepository<AuthUserInfoEntity> _efAuthUserInfoRepository = null;
        public AuthManagerService(IRepository<AuthAppInfoEntity> efAuthAppInfoRepository
            , IRepository<AuthFunctionInfoEntity> efAuthFunctionInfoRepository
            , IRepository<AuthGroupInfoEntity> efAuthGroupInfoRepository
            , IRepository<AuthRoleInfoEntity> efAuthRoleInfoRepository
            , IRepository<AuthUserInfoEntity> efAuthUserInfoRepository)
        {
            this._efAuthAppInfoRepository = efAuthAppInfoRepository;
            this._efAuthFunctionInfoRepository = efAuthFunctionInfoRepository;
            this._efAuthGroupInfoRepository = efAuthGroupInfoRepository;
            this._efAuthRoleInfoRepository = efAuthRoleInfoRepository;
            this._efAuthUserInfoRepository = efAuthUserInfoRepository;
        }

        #region appInfo

        public IPagedList<AuthAppInfoModel> GetAuthAppInfoListByPager(GridQuery filter)
        {
            var list = this._efAuthAppInfoRepository.TableNoTracking.OrderByDescending(a => a.UpdateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efAuthAppInfoRepository.TableNoTracking.Count();

            return list.Select(app => {
                return new AuthAppInfoModel
                {
                    AppId = app.Id,
                    Code = app.Code,
                    CreateTime = app.CreateTime,
                    Desc = app.Desc,
                    IsEnable = app.IsEnable,
                    Name = app.Name,
                    UpdateTime = app.UpdateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }

        public IList<AppFunctionInfoTreeNode> GetAuthAppInfoList()
        {
            var list = this._efAuthAppInfoRepository.TableNoTracking.OrderByDescending(a => a.UpdateTime).ToList();
            return list.Select(app =>
            {
                var node = new AppFunctionInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = app.Id;
                node.AppId = app.Id;
                node.Name = app.Name;
                node.Code = app.Code;
                return node;
            }).ToList();
        }

        public void AddOrUpdateAllAuthAppInfo(AuthAppInfoModel model)
        {
            if (model.AppId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var appInfo = this._efAuthAppInfoRepository.GetById(model.AppId.Value);
                    appInfo.Code = model.Code;
                    appInfo.Name = model.Name;
                    appInfo.Desc = model.Desc;
                    appInfo.IsEnable = model.IsEnable;
                    appInfo.UpdateTime = DateTime.Now;
                    this._efAuthAppInfoRepository.Update(appInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efAuthAppInfoRepository.Insert(new AuthAppInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    IsEnable = model.IsEnable,
                    Name = model.Name,
                    UpdateTime = DateTime.Now
                });
            }
        }

        public AuthAppInfoModel GetAuthAppInfo(Guid id)
        {
            var app = this._efAuthAppInfoRepository.GetById(id);
            return new AuthAppInfoModel {
                AppId = app.Id,
                Code = app.Code,
                CreateTime = app.CreateTime,
                Desc = app.Desc,
                IsEnable = app.IsEnable,
                Name = app.Name,
                UpdateTime = app.UpdateTime
            };
        }

        public bool DeleteAllAuthAppInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var appInfo = this._efAuthAppInfoRepository.GetById(item);
                    var count = this._efAuthFunctionInfoRepository.TableNoTracking.Where(func => func.AppId == appInfo.Id).Count();
                    if (count > 0)
                    {
                        return false;
                    }
                    this._efAuthAppInfoRepository.Delete(appInfo);
                }
                scope.SaveChanges();
            }
            return true;
        }

        #endregion

        #region function

        public AuthFunctionInfoModel GetAuthFunctionInfo(Guid id)
        {
            var func = this._efAuthFunctionInfoRepository.GetById(id);
            return new AuthFunctionInfoModel
            {
                FunctionId = func.Id,
                Code = func.Code,
                CreateTime = func.CreateTime,
                Desc = func.Desc,
                IsEnable = func.IsEnable,
                Name = func.Name,
                UpdateTime = func.UpdateTime,
                ParentId = func.ParentId
            };
        }

        public IList<AuthFunctionInfoModel> GetSubAuthFunctionInfoList(Guid appId, Guid parentId)
        {
            var db = this._efAuthFunctionInfoRepository.TableNoTracking;
            if (parentId == Guid.Empty)
            {
                var list = db.Where(func => func.ParentId == parentId && func.AppId == appId).ToList().Select(func =>
                {
                    var model = new AuthFunctionInfoModel();
                    model.ParentId = parentId;
                    model.FunctionId = func.Id;
                    model.Code = func.Code;
                    model.Name = func.Name;
                    model.Desc = func.Desc;
                    model.IsEnable = func.IsEnable;
                    model.CreateTime = func.CreateTime;
                    model.UpdateTime = func.UpdateTime;
                    model.ParentParentId = parentId;
                    return model;
                }).ToList();
                return list;
            }
            var functionInfo = db.Include(func => func.SubAuthFunctionInfos).Where(func => func.AppId ==appId &&  func.Id == parentId).FirstOrDefault();
            if (functionInfo == null)
            {
                return new List<AuthFunctionInfoModel>();
            }
            return functionInfo.SubAuthFunctionInfos.Select(func => 
            {
                var model = new AuthFunctionInfoModel();
                model.ParentId = parentId;
                model.FunctionId = func.Id;
                model.Code = func.Code;
                model.Name = func.Name;
                model.Desc = func.Desc;
                model.IsEnable = func.IsEnable;
                model.CreateTime = func.CreateTime;
                model.UpdateTime = func.UpdateTime;
                model.ParentParentId = functionInfo.ParentId;
                return model;
            }).ToList();
        }

        public void AddOrUpdateAllAuthFunctionInfo(AuthFunctionInfoModel model)
        {
            if (model.FunctionId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var funcInfo = this._efAuthFunctionInfoRepository.GetById(model.FunctionId.Value);
                    funcInfo.Code = model.Code;
                    funcInfo.Name = model.Name;
                    funcInfo.Desc = model.Desc;
                    funcInfo.UpdateTime = DateTime.Now;
                    funcInfo.IsEnable = model.IsEnable;
                    this._efAuthFunctionInfoRepository.Update(funcInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efAuthFunctionInfoRepository.Insert(new AuthFunctionInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    CreateTime = DateTime.Now,
                    Desc = model.Desc,
                    IsEnable = model.IsEnable,
                    Name = model.Name,
                    UpdateTime = DateTime.Now,
                    ParentId = model.ParentId,
                    AppId = model.AppId
                });
            }
        }


        public bool DeleteAllAuthFunctionInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var db = this._efAuthFunctionInfoRepository.TableNoTracking;
                    var count = db.Where(func => func.ParentId == item).Count();
                    if (count > 0)
                    {
                        return false;
                    }
                    var funcInfo = db.Include(func => func.AuthRoleInfos).Where(func => func.Id == item).FirstOrDefault();
                    if (funcInfo != null)
                    {
                        funcInfo.AuthRoleInfos.Clear();
                        this._efAuthFunctionInfoRepository.Delete(funcInfo);
                    }
                }
                scope.SaveChanges();
            }
            return true;
        }

        #endregion




        #region userinfo

        public IPagedList<AuthUserInfoModel> GetAuthUserInfoListByPager(GridQuery filter)
        {
            var list = this._efAuthUserInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efAuthUserInfoRepository.TableNoTracking.Count();

            return list.Select(user =>
            {
                return new AuthUserInfoModel
                {
                    UserId = user.Id,
                    RefOrgUserCode = user.RefOrgUserCode,
                    CreateTime = user.CreateTime,
                    Desc = user.Desc,
                    IsEnable = user.IsEnable
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }
        public void AddOrUpdateAllAuthUserInfo(AuthUserInfoModel model)
        {
            if (model.UserId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var userInfo = this._efAuthUserInfoRepository.GetById(model.UserId.Value);
                    userInfo.RefOrgUserCode = model.RefOrgUserCode;
                    userInfo.IsEnable = model.IsEnable;
                    userInfo.Desc = model.Desc;
                    this._efAuthUserInfoRepository.Update(userInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efAuthUserInfoRepository.Insert(new AuthUserInfoEntity
                {
                    Id = Guid.NewGuid(),
                    RefOrgUserCode = model.RefOrgUserCode,
                    CreateTime = DateTime.Now,
                    IsEnable = model.IsEnable,
                    Desc = model.Desc,
                });
            }
        }

        public AuthUserInfoModel GetAuthUserInfo(Guid id)
        {
            var user = this._efAuthUserInfoRepository.GetById(id);
            return new AuthUserInfoModel
            {
                UserId = user.Id,
                RefOrgUserCode = user.RefOrgUserCode,
                CreateTime = user.CreateTime,
                IsEnable = user.IsEnable,
                Desc = user.Desc
            };
        }

        public bool DeleteAllAuthUserInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var db = this._efAuthUserInfoRepository.TableNoTracking;

                    var userInfo = db.Include(u => u.AuthRoleInfos).Include(u => u.AuthGroupInfos).Where(u => u.Id == item).FirstOrDefault();
                    if (userInfo != null)
                    {
                        userInfo.AuthRoleInfos.Clear();
                        userInfo.AuthRoleInfos.Clear();
                        this._efAuthUserInfoRepository.Delete(userInfo);
                    }
                }
                scope.SaveChanges();
            }
            return true;
        }

        #endregion


        #region groupinfo

        public IPagedList<AuthGroupInfoModel> GetAuthGroupInfoListByPager(GridQuery filter)
        {
            var list = this._efAuthGroupInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efAuthGroupInfoRepository.TableNoTracking.Count();

            return list.Select(group =>
            {
                return new AuthGroupInfoModel
                {
                    GroupId = group.Id,
                    Name = group.Name,
                    Code = group.Code,
                    CreateTime = group.CreateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }
        public void AddOrUpdateAllAuthGroupInfo(AuthGroupInfoModel model)
        {
            if (model.GroupId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var groupInfo = this._efAuthGroupInfoRepository.GetById(model.GroupId.Value);
                    groupInfo.Code = model.Code;
                    groupInfo.Name = model.Name;
                    this._efAuthGroupInfoRepository.Update(groupInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efAuthGroupInfoRepository.Insert(new AuthGroupInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Code = model.Code,
                    CreateTime = DateTime.Now
                });
            }
        }

        public AuthGroupInfoModel GetAuthGroupInfo(Guid id)
        {
            var group = this._efAuthGroupInfoRepository.GetById(id);
            return new AuthGroupInfoModel
            {
                GroupId = group.Id,
                Name = group.Name,
                Code = group.Code,
                CreateTime = group.CreateTime
            };
        }

        public bool DeleteAllAuthGroupInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var db = this._efAuthGroupInfoRepository.TableNoTracking;

                    var userInfo = db.Include(u => u.AuthRoleInfos).Include(u => u.AuthUserInfos).Where(u => u.Id == item).FirstOrDefault();
                    if (userInfo != null)
                    {
                        userInfo.AuthRoleInfos.Clear();
                        userInfo.AuthUserInfos.Clear();
                        this._efAuthGroupInfoRepository.Delete(userInfo);
                    }
                }
                scope.SaveChanges();
            }
            return true;
        }

        public IList<GroupUserInfoTreeNode> GetGroupUserInfoTreeList()
        {
            var list = this._efAuthGroupInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(group =>
            {
                var node = new GroupUserInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = group.Id;
                node.GroupId = group.Id;
                node.Name = group.Name;
                node.Code = group.Code;
                return node;
            }).ToList();
        }

        public IPagedList<AuthUserInfoModel> GetAuthGroupUserInfoListByPager(GridQuery filter)
        {
            Guid groupId = Guid.Parse(filter.PostData["groupId"]);
            var groupInfo = this._efAuthGroupInfoRepository.TableNoTracking.Where(g => g.Id == groupId).FirstOrDefault();
            var count = groupInfo.AuthUserInfos.Count();
            var list = groupInfo.AuthUserInfos.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            return list.Select(user =>
            {
                return new AuthUserInfoModel
                {
                    UserId = user.Id,
                    RefOrgUserCode = user.RefOrgUserCode,
                    IsEnable = user.IsEnable,
                    CreateTime = user.CreateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);
        }

        public void AddAuthUserInfoToGroup(Guid groupId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var groupInfo = this._efAuthGroupInfoRepository.Table.Where(g => g.Id == groupId).FirstOrDefault();
                var userInfoList = this._efAuthUserInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                groupInfo.AuthUserInfos.AddRange(userInfoList);
                this._efAuthGroupInfoRepository.Update(groupInfo);
                scope.SaveChanges();
            }
            

        }

        public void RemoveAuthUserInfoToGroup(Guid groupId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var groupInfo = this._efAuthGroupInfoRepository.Table.Where(g => g.Id == groupId).FirstOrDefault();
                var userInfoList = this._efAuthUserInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                foreach (var item in userInfoList)
                {
                    groupInfo.AuthUserInfos.Remove(item);
                }
                
                this._efAuthGroupInfoRepository.Update(groupInfo);
                scope.SaveChanges();
            }
        }

        public IList<GroupRoleInfoTreeNode> GetGroupRoleInfoTreeList()
        {
            var list = this._efAuthGroupInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(group =>
            {
                var node = new GroupRoleInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = group.Id;
                node.GroupId = group.Id;
                node.Name = group.Name;
                node.Code = group.Code;
                return node;
            }).ToList();
        }

        public IPagedList<AuthRoleInfoModel> GetAuthGroupRoleInfoListByPager(GridQuery filter)
        {
            Guid groupId = Guid.Parse(filter.PostData["groupId"]);
            var groupInfo = this._efAuthGroupInfoRepository.TableNoTracking.Where(g => g.Id == groupId).FirstOrDefault();
            var count = groupInfo.AuthRoleInfos.Count();
            var list = groupInfo.AuthRoleInfos.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            return list.Select(role =>
            {
                return new AuthRoleInfoModel
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Code = role.Code,
                    Desc = role.Desc,
                    IsEnable = role.IsEnable,
                    UpdateTime = role.UpdateTime,
                    CreateTime = role.CreateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);
        }

        public void AddAuthRoleInfoToGroup(Guid groupId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var groupInfo = this._efAuthGroupInfoRepository.Table.Where(g => g.Id == groupId).FirstOrDefault();
                var roleInfoList = this._efAuthRoleInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                groupInfo.AuthRoleInfos.AddRange(roleInfoList);
                this._efAuthGroupInfoRepository.Update(groupInfo);
                scope.SaveChanges();
            }


        }

        public void RemoveAuthRoleInfoToGroup(Guid groupId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var groupInfo = this._efAuthGroupInfoRepository.Table.Where(g => g.Id == groupId).FirstOrDefault();
                var roleInfoList = this._efAuthRoleInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                foreach (var item in roleInfoList)
                {
                    groupInfo.AuthRoleInfos.Remove(item);
                }

                this._efAuthGroupInfoRepository.Update(groupInfo);
                scope.SaveChanges();
            }
        }

        #endregion

        #region roleinfo

        public IPagedList<AuthRoleInfoModel> GetAuthRoleInfoListByPager(GridQuery filter)
        {
            var list = this._efAuthRoleInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            int count = this._efAuthRoleInfoRepository.TableNoTracking.Count();

            return list.Select(role =>
            {
                return new AuthRoleInfoModel
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Code = role.Code,
                    Desc = role.Desc,
                    IsEnable = role.IsEnable,
                    UpdateTime = role.UpdateTime,
                    CreateTime = role.CreateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);

        }
        public void AddOrUpdateAllAuthRoleInfo(AuthRoleInfoModel model)
        {
            if (model.RoleId.HasValue)
            {
                using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
                {
                    var roleInfo = this._efAuthRoleInfoRepository.GetById(model.RoleId.Value);
                    roleInfo.Code = model.Code;
                    roleInfo.Name = model.Name;
                    roleInfo.IsEnable = model.IsEnable;
                    roleInfo.Desc = model.Desc;
                    roleInfo.UpdateTime = DateTime.Now;
                    this._efAuthRoleInfoRepository.Update(roleInfo);
                    scope.SaveChanges();
                }
            }
            else
            {
                this._efAuthRoleInfoRepository.Insert(new AuthRoleInfoEntity
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Code = model.Code,
                    Desc = model.Desc,
                    IsEnable = model.IsEnable,
                    UpdateTime = DateTime.Now,
                    CreateTime = DateTime.Now
                });
            }
        }

        public AuthRoleInfoModel GetAuthRoleInfo(Guid id)
        {
            var role = this._efAuthRoleInfoRepository.GetById(id);
            return new AuthRoleInfoModel
            {
                RoleId = role.Id,
                Name = role.Name,
                Code = role.Code,
                Desc = role.Desc,
                IsEnable = role.IsEnable,
                UpdateTime = role.UpdateTime,
                CreateTime = role.CreateTime
            };
        }

        public bool DeleteAllAuthRoleInfo(IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                foreach (var item in idList)
                {
                    var db = this._efAuthRoleInfoRepository.TableNoTracking;

                    var roleInfo = db.Include(u => u.AuthFunctionInfos).Include(u => u.AuthUserInfos).Include(u => u.AuthGroupInfos).Where(u => u.Id == item).FirstOrDefault();
                    if (roleInfo != null)
                    {
                        roleInfo.AuthFunctionInfos.Clear();
                        roleInfo.AuthUserInfos.Clear();
                        roleInfo.AuthGroupInfos.Clear();
                        this._efAuthRoleInfoRepository.Delete(roleInfo);
                    }
                }
                scope.SaveChanges();
            }
            return true;
        }

        public IList<RoleUserInfoTreeNode> GetRoleUserInfoTreeList()
        {
            var list = this._efAuthRoleInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(role =>
            {
                var node = new RoleUserInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = role.Id;
                node.RoleId = role.Id;
                node.Name = role.Name;
                node.Code = role.Code;
                return node;
            }).ToList();
        }

        public IPagedList<AuthUserInfoModel> GetAuthRoleUserInfoListByPager(GridQuery filter)
        {
            Guid roleId = Guid.Parse(filter.PostData["roleId"]);
            var roleInfo = this._efAuthRoleInfoRepository.TableNoTracking.Where(g => g.Id == roleId).FirstOrDefault();
            var count = roleInfo.AuthUserInfos.Count();
            var list = roleInfo.AuthUserInfos.OrderByDescending(a => a.CreateTime).Skip(filter.SkipIndex).Take(filter.PageSize).ToList();
            return list.Select(user =>
            {
                return new AuthUserInfoModel
                {
                    UserId = user.Id,
                    RefOrgUserCode = user.RefOrgUserCode,
                    IsEnable = user.IsEnable,
                    CreateTime = user.CreateTime
                };
            }).ToPageList(filter.PageIndex, filter.PageSize, count);
        }

        public void AddAuthUserInfoToRole(Guid roleId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var roleInfo = this._efAuthRoleInfoRepository.Table.Where(g => g.Id == roleId).FirstOrDefault();
                var userInfoList = this._efAuthUserInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                roleInfo.AuthUserInfos.AddRange(userInfoList);
                this._efAuthRoleInfoRepository.Update(roleInfo);
                scope.SaveChanges();
            }


        }

        public void RemoveAuthUserInfoToRole(Guid roleId, IList<Guid> idList)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var roleInfo = this._efAuthRoleInfoRepository.Table.Where(g => g.Id == roleId).FirstOrDefault();
                var userInfoList = this._efAuthUserInfoRepository.Table.Where(u => idList.Contains(u.Id)).ToList();
                foreach (var item in userInfoList)
                {
                    roleInfo.AuthUserInfos.Remove(item);
                }

                this._efAuthRoleInfoRepository.Update(roleInfo);
                scope.SaveChanges();
            }
        }

        public IList<RoleFunctionInfoTreeNode> GetRoleFunctionInfoTreeList()
        {
            var list = this._efAuthRoleInfoRepository.TableNoTracking.OrderByDescending(a => a.CreateTime).ToList();
            return list.Select(role =>
            {
                var node = new RoleFunctionInfoTreeNode();
                node.ParentId = Guid.Empty;
                node.Id = role.Id;
                node.RoleId = role.Id;
                node.Name = role.Name;
                node.Code = role.Code;
                return node;
            }).ToList();
        }

        public IList<RoleFunctionInfoTreeModel> GetRoleFunctionInfoTree(Guid roleId)
        {
            IList<RoleFunctionInfoTreeModel> model = this._efAuthAppInfoRepository.TableNoTracking.OrderByDescending(a => a.UpdateTime).ToList().Select(app =>
            {
                return new RoleFunctionInfoTreeModel
                {
                    AppId = app.Id,
                    Code = app.Code,
                    Name = app.Name,
                };
            }).ToList();
            foreach (var item in model)
            {
                var funcInfos = this._efAuthFunctionInfoRepository.TableNoTracking.Where(func => func.ParentId == Guid.Empty && func.AppId == item.AppId).ToList();
                item.FunctionInfos.AddRange(funcInfos);
            }

            var roleInfo = this._efAuthRoleInfoRepository.TableNoTracking.Where(g => g.Id == roleId).FirstOrDefault();

            foreach (var item in model)
            {
                item.AddedFunctionInfos = roleInfo.AuthFunctionInfos.Where(func=>func.AppId == item.AppId).ToDictionary(func=>func.Id);
            }
            
            return model;
        }

        public void SaveAuthFunctionInfoToRole(RoleFunctionInfoTreeModel model)
        {
            using (var scope = AutoEfRepositoryFactory.GetEfRepositoryFactory(DataSources.DataSource_ItAdmin).CreateReadWriteContextScope())
            {
                var roleInfo = this._efAuthRoleInfoRepository.Table.Where(g => g.Id == model.RoleId).FirstOrDefault();
                var appFuncs = roleInfo.AuthFunctionInfos.Where(func=>func.AppId == model.AppId).ToList();
                foreach (var item in appFuncs)
                {
                    roleInfo.AuthFunctionInfos.Remove(item);
                }

                List<AuthFunctionInfoEntity> roleFuncs = new List<AuthFunctionInfoEntity>();
                Dictionary<Guid, AuthFunctionInfoEntity> added = new Dictionary<Guid,AuthFunctionInfoEntity>();
                foreach (var item in model.FunctionInfos)
                {
                    var func = this._efAuthFunctionInfoRepository.GetById(item.Id);
                    var parents = GetALLParents(func, added);
                    roleFuncs.Add(func);
                    roleFuncs.AddRange(parents);
                }
                roleInfo.AuthFunctionInfos.AddRange(roleFuncs);
                this._efAuthRoleInfoRepository.Update(roleInfo);
                scope.SaveChanges();
            }
        }

        private List<AuthFunctionInfoEntity> GetALLParents(AuthFunctionInfoEntity self, Dictionary<Guid, AuthFunctionInfoEntity> added)
        {
            List<AuthFunctionInfoEntity> list = new List<AuthFunctionInfoEntity>();
            GetALLParent(list, self.ParentFunctionInfo, added);
            return list;
        }

        private void GetALLParent(List<AuthFunctionInfoEntity> list ,AuthFunctionInfoEntity self, Dictionary<Guid, AuthFunctionInfoEntity> added)
        {
            if (self == null)
            {
                return;
            }
            if (!added.ContainsKey(self.Id))
            {
                list.Add(self);
                added.Add(self.Id, self);
            }
            GetALLParent(list, self.ParentFunctionInfo, added);
        }

        #endregion




        
    }
}