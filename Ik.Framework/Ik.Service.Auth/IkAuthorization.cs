using Ik.Framework;
using Ik.Framework.Configuration;
using Ik.Service.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Collections.Concurrent;
using Ik.Framework.Caching;
using Ik.Framework.Logging;
using Ik.Framework.Common.Serialization;
using Ik.Framework.Events;

namespace Ik.Service.Auth
{
    [EventAutoSubscribeAttribute(false)]
    [DistributedEvent(Framework.Events.HandleType.Asyn, AuthorizationEventTopics.Topic_Authorization)]
    public class IkAuthorization : IIkAuthorization, IConsumer<AuthUpdatedEvent>
    {
        private static ILog _logger = LogManager.GetLogger("权限服务");
        private ConcurrentDictionary<string, Dictionary<string, AuthFunctionInfo>> _rootFuncs = new ConcurrentDictionary<string, Dictionary<string, AuthFunctionInfo>>();
        private ConcurrentDictionary<string, UserPermission> _userPermissionList = new ConcurrentDictionary<string, UserPermission>();
        private MemcachedManager cacheManager = null;
        public const string _cache_distribute_token_key = "identity_{token}_v1";
        public const string _cache_distribute_token_touch_key = "identity_{token}_touch";
        public const int _identity_distribute_expire_minutes = 60;
        private string _appCode = string.Empty;
        private Type _identityInfoType = null;
        public void Initialize()
        {
            cacheManager = new MemcachedManager(AuthServiceCacheFactory.CacheKeyFormat);
            var authService = new AuthAppService();
            this._appCode = AppInfo.AppCode;
            if (string.IsNullOrEmpty(this._appCode))
            {
                throw new AuthException("应用标识未配置");
            }
            var appRootFuncList = authService.GetAuthAppRootFunctionList(this._appCode);
            foreach (var item in appRootFuncList)
            {
                var allFuncList = authService.GetAllAuthFunctionList(this._appCode, item.FuncCode);
                allFuncList.Add(item);
                _rootFuncs.AddOrUpdate(item.FuncCode, allFuncList.ToDictionary(s => s.FuncCode), (key, func) => func);
            }

            if (SubscriptionManager.IsSubscriberContextInitialize)
            {
                SubscriptionManager.Subscriber.RegisterConsumerType(typeof(IkAuthorization));
            }
        }

        public virtual IdentityInfo CreateIdentityInfo()
        {
            return new IdentityInfo();
        }

        public IdentityInfo Authenticate(string refUserCode)
        {
            var up = GetUserPermission(refUserCode);
            if (up.Permissions.Count == 0)
            {
                throw new AuthException("未授权用户");
            }
            var tokenInfo = new TokenInfo { AccessTime = DateTime.Now, RefUserCode = refUserCode };
            var token = TokenHelper.EncryptToken(tokenInfo);
            IdentityInfo identityInfo = CreateIdentityInfo();
            this._identityInfoType = identityInfo.GetType();
            identityInfo.RefUserCode = refUserCode;
            identityInfo.Token = token;
            identityInfo.ExpireTime = DateTime.Now.AddMinutes(_identity_distribute_expire_minutes);
            bool flag = cacheManager.Set(_cache_distribute_token_key, identityInfo, _identity_distribute_expire_minutes, token);
            if (!flag)
            {
                throw new AuthException("令牌缓存失败");
            }
            cacheManager.Set(_cache_distribute_token_touch_key, 1, _identity_distribute_expire_minutes - 5, token);
            return identityInfo;
        }

        public bool IsAuthenticate(string token, out IdentityInfo identityInfo)
        {
            identityInfo = null;
            string cacheString;
            if (cacheManager.RawIsSet(_cache_distribute_token_key, out cacheString, token))
            {
                if (this._identityInfoType == null)
                {
                    this._identityInfoType = CreateIdentityInfo().GetType();
                }
                identityInfo = (IdentityInfo)JsonSerializerManager.JsonDeserialize(cacheString, this._identityInfoType);
                string touchValue;
                if (!cacheManager.RawIsSet(_cache_distribute_token_touch_key, out touchValue, token))
                {
                    cacheManager.RawSet(_cache_distribute_token_key, cacheString, _identity_distribute_expire_minutes, token);
                    cacheManager.Set(_cache_distribute_token_touch_key, 1, _identity_distribute_expire_minutes - 5, token);
                }
                return true;
            }
            return false;
        }

        public void Logout(string token)
        {
            bool flag = cacheManager.Remove(_cache_distribute_token_key, token);
            if (!flag)
            {
                _logger.Error("注销令牌失败，或许多次注销");
            }
        }

        public UserPermission GetUserPermission(string refUserCode)
        {
            UserPermission up = _userPermissionList.GetOrAdd(refUserCode, (code =>
            {
                UserPermission permission = new UserPermission();
                permission.RefUserCode = code;
                var authService = new AuthUserService();
                var funcGroupList = authService.GetUserGroupFunctionListByRefUserCode(refUserCode);
                foreach (var item in funcGroupList.GroupBy(func => func.GroupId))
                {
                    permission.Groups.Add(new UserGroupInfo { Id = item.Key, Name = item.First().GroupName });
                }

                foreach (var item in funcGroupList.GroupBy(func => func.RoleId))
                {
                    permission.Roles.Add(new RoleInfo { Id = item.Key, Name = item.First().RoleName });
                }

                permission.Permissions.AddRange(funcGroupList.Select(func => new AppPermission { FunctionId = func.FunctionId, PermissionCode = func.FuncCode }).ToList());

                var fucnList = authService.GetUserFunctionListByRefUserCode(refUserCode);

                foreach (var item in fucnList.GroupBy(func => func.RoleId))
                {
                    bool hasRole = false;
                    foreach (var role in permission.Roles)
                    {
                        if (role.Id == item.Key)
                        {
                            hasRole = true;
                            break;
                        }
                    }
                    if (!hasRole)
                    {
                        permission.Roles.Add(new RoleInfo { Id = item.Key, Name = item.First().RoleName });
                    }
                }

                permission.Permissions.AddRange(fucnList.Select(func => new AppPermission { FunctionId = func.FunctionId, PermissionCode = func.FuncCode }).ToList());

                if (funcGroupList.Count == 0 && fucnList.Count > 0)
                {
                    permission.UserId = fucnList.First().UserId;
                }
                else if (funcGroupList.Count > 0 && fucnList.Count == 0)
                {
                    permission.UserId = fucnList.First().UserId;
                }
                else if (funcGroupList.Count > 0 && fucnList.Count > 0)
                {
                    permission.UserId = fucnList.First().UserId;
                }
                return permission;
            }));

            return up;
        }

        public TokenInfo GetTokenInfo(string tokenString)
        {
            return TokenHelper.DecryptToken(tokenString);
        }


        public bool CheckPermission(string funcCode, UserPermission up)
        {
            var authFunctionDict = _rootFuncs.GetOrAdd(funcCode, (code) =>
            { 
                 var authService = new AuthAppService();
                 var function = authService.GetFunctionByCode(this._appCode, code);
                 var funcList = authService.GetAllAuthFunctionList(this._appCode, function.FuncCode);
                 return funcList.ToDictionary(s => s.FuncCode);
            });
            foreach (var item in up.Permissions)
            {
                if (authFunctionDict.ContainsKey(item.PermissionCode))
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateNotificationAll()
        {
            SubscriptionManager.Publisher.PublishDistributed<AuthUpdatedEvent>(AuthorizationEventTopics.Topic_Authorization, new AuthUpdatedEvent());
        }

        public void HandleEvent(string lable, AuthUpdatedEvent eventMessage)
        {
            _rootFuncs.Clear();
            _userPermissionList.Clear();
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
