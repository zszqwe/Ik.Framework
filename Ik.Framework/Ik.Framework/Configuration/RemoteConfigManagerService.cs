using Ik.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IkZooKeeperNet;
using Ik.Framework.Common.Extension;
using Ik.Framework.Common.Serialization;
using Ik.Framework.ZooKeeperNet;

namespace Ik.Framework.Configuration
{
    public class RemoteConfigManagerService : BackgroundServiceBase, IConfigManager, IWatcher
    {
        public const string LogModelName_RemoteConfigService = "配置管理服务";
        private static ILog logger = LogManager.GetLogger(RemoteConfigManagerService.LogModelName_RemoteConfigService);
        public const string CONFIG_ROOT = "remote_config";
        private IkZooKeeperClient _client = null;
        private ConfigInfo _configInfo = null;
        private DeployInfo _envInfo = AppInfo.DeployEnvInfo;
        private ConcurrentDictionary<string, Action<string, byte[]>> registerEvents = new ConcurrentDictionary<string, Action<string, byte[]>>();
        private string _defaultEvnKey = "default";

        protected override void StartService()
        {
            logger.Info("配置管理服务开始");
            this.ServiceName = "配置管理服务";
            this._configInfo = ConfigManager.GetConfigInfo();
            if (string.IsNullOrEmpty(this._configInfo.ConfigName))
            {
                throw new ConfigInitException("配置标识不能为空");
            }
            if (string.IsNullOrEmpty(this._configInfo.RemoteConfigServerAddress))
            {
                throw new ConfigInitException("配置服务地址不能为空");
            }
            this._client = new IkZooKeeperClient(this._configInfo.RemoteConfigServerAddress, TimeSpan.FromSeconds(15), this);
            logger.Info(string.Format("配置管理服务开始完成，配置标识：{0}，配置版本：{1}，配置服务地址：{2}", this._configInfo.ConfigName, this._configInfo.VersionInfo, this._configInfo.RemoteConfigServerAddress));
        }

        protected override void ShutDownService()
        {
            logger.Info("配置管理服务开始结束");
            this._client.Dispose();
            logger.Info("配置管理服务开始完成");
        }

        public bool Add<T>(string configName, Version ver,string envKey, string key, T value)
        {
            if (!IsRunning)
            {
                return false;
            }
            EnsureZooKeeperPath(configName.ToLower(), ver, envKey.ToLower(), key.ToLower());
            var path = BuildZooKeeperPath(configName.ToLower(), ver, envKey.ToLower(), key.ToLower());
            var data = Serialize(value);
            try
            {
                return _client.EnsureSetData(path, data);
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("配置设置失败，配置名称：{0}，配置版本：{1}，key：{2}", this._configInfo.ConfigName, this._configInfo.VersionInfo, key), ex);
            }
            return false;
        }

        public bool Add(string configName, Version ver,string envKey, string key, string value)
        {
            if (!IsRunning)
            {
                return false;
            }
            EnsureZooKeeperPath(configName.ToLower(), ver, envKey.ToLower(), key.ToLower());
            var path = BuildZooKeeperPath(configName.ToLower(), ver, envKey.ToLower(), key.ToLower());
            var data = Encoding.UTF8.GetBytes(value);
            try
            {
                return _client.EnsureSetData(path, data);
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("配置设置失败，配置名称：{0}，配置版本：{1}，key：{2}", this._configInfo.ConfigName, this._configInfo.VersionInfo, key), ex);
            }
            return false;
        }

        public bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            if (!IsRunning)
            {
                return false;
            }
            var path = BuildZooKeeperPath(this._configInfo.ConfigName.ToLower(), this._configInfo.VersionInfo, this._envInfo.DeployKey.ToLower(), key.ToLower());
            try
            {
                if (_client.EnsureExists(path, false))
                {
                    var data = _client.EnsureGetData(path);
                    if (data != null)
                    {
                        value = Deserialize<T>(data);
                        return true;
                    }
                }
                else
                {
                    var defautEnv = BuildZooKeeperPath(this._configInfo.ConfigName.ToLower(), this._configInfo.VersionInfo, _defaultEvnKey, key.ToLower());
                    if (_client.EnsureExists(defautEnv, false))
                    {
                        var data = _client.EnsureGetData(defautEnv);
                        if (data != null)
                        {
                            value = Deserialize<T>(data);
                            return true;
                        }
                    }
                }
            }
            catch (IkZooKeeperNet.KeeperException.NoNodeException)
            {
                
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("配置获取失败，配置名称：{0}，配置版本：{1}，key：{2}", this._configInfo.ConfigName, this._configInfo.VersionInfo, key), ex);
            }

            return false;
        }


        public bool Remove<T>(string configName, Version ver,string envKey, string key)
        {
            if (!IsRunning)
            {
                return false;
            }
            var path = BuildZooKeeperPath(configName.ToLower(), ver,envKey.ToLower(), key.ToLower());
            try
            {
                return _client.EnsureDelete(path);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("配置删除失败，配置名称：{0}，配置版本：{1}，key：{2}", configName, ver, key), ex);
            }
            return false;
        }

        public T Get<T>(string key)
        {
            T value;
            return TryGet<T>(key, out value) ? value : default(T);
        }


        public bool RegisterConfigChangedEvent<T>(string key, ConfigChangedEvent<T> changed)
        {
            if (!IsRunning)
            {
                return false;
            }
            var path = BuildZooKeeperPath(this._configInfo.ConfigName.ToLower(), this._configInfo.VersionInfo,this._envInfo.DeployKey.ToLower(), key.ToLower());
            var flag = _client.EnsureExists(path, true);
            if (flag)
            {
                registerEvents.AddOrUpdate(path, new Action<string, byte[]>((p, v) =>
                {
                    T value = Deserialize<T>(v);
                    changed(p, key, value);
                }), (p, a) => a);
                return true;
            }
            return false;
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDataChanged)
            {
                var path = @event.Path;
                Action<string, byte[]> action;
                if (registerEvents.TryGetValue(path, out action))
                {
                    byte[] data = null;
                    try
                    {
                        data = _client.EnsureGetData(path);
                    }
                    catch(Exception ex) 
                    {
                        logger.Error(string.Format("监听中配置获取失败，配置名称：{0}，配置版本：{1}，key：{2}", _configInfo.ConfigName, _configInfo.VersionInfo, path), ex);
                    }
                    logger.Info("配置已变化，Key：" + path);
                    action(path, data);
                    try
                    {
                        var flag = _client.EnsureExists(path, true);
                        if (!flag)
                        {
                            logger.Error(string.Format("监听配置失败，配置名称：{0}，配置版本：{1}，key：{2}", _configInfo.ConfigName, _configInfo.VersionInfo, path));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format("监听配置失败，配置名称：{0}，配置版本：{1}，key：{2}", _configInfo.ConfigName, _configInfo.VersionInfo, path), ex);
                    }
                }
            }
        }

        private string BuildZooKeeperPath()
        {
            return "/" + CONFIG_ROOT;
        }

        private string BuildZooKeeperPath(string configName)
        {
            if (string.IsNullOrEmpty(configName))
            {
                throw new ArgumentNullException("配置标识不能为空");
            }
            return "/" + CONFIG_ROOT + "/" + configName;
        }

        private string BuildZooKeeperPath(string configName, Version ver)
        {
            if (string.IsNullOrEmpty(configName))
            {
                throw new ArgumentNullException("配置标识不能为空");
            }
            return "/" + CONFIG_ROOT + "/" + configName + "/" + ver.ToString();
        }

        private string BuildZooKeeperPath(string configName, Version ver, string envKey)
        {
            if (string.IsNullOrEmpty(configName))
            {
                throw new ArgumentNullException("配置标识不能为空");
            }
            if (string.IsNullOrEmpty(envKey))
            {
                throw new ArgumentNullException("环境标识不能为空");
            }
            return "/" + CONFIG_ROOT + "/" + configName + "/" + ver.ToString() + "/" + envKey;
        }

        private string BuildZooKeeperPath(string configName, Version ver, string envKey, string key)
        {
            if (string.IsNullOrEmpty(configName))
            {
                throw new ArgumentNullException("配置标识不能为空");
            }
            if (string.IsNullOrEmpty(envKey))
            {
                throw new ArgumentNullException("环境标识不能为空");
            }
            return "/" + CONFIG_ROOT + "/" + configName + "/" + ver.ToString() + "/" + envKey + "/" + key;
        }

        private void EnsureZooKeeperPath(string configName, Version ver,string envKey, string key)
        {
            if (!_client.EnsureExists("/", false))
            {
                _client.EnsureCreate("/");
            }
            var root = BuildZooKeeperPath();
            if (!_client.EnsureExists(root, false))
            {
                _client.EnsureCreate(root);
            }
            var levelName = BuildZooKeeperPath(configName);
            if (!_client.EnsureExists(levelName, false))
            {
                _client.EnsureCreate(levelName);
            }

            var levelVer = BuildZooKeeperPath(configName,ver);
            if (!_client.EnsureExists(levelVer, false))
            {
                _client.EnsureCreate(levelVer);
            }
            var levelEnv = BuildZooKeeperPath(configName, ver, envKey);
            if (!_client.EnsureExists(levelEnv, false))
            {
                _client.EnsureCreate(levelEnv);
            }

            var levelValue = BuildZooKeeperPath(configName, ver, envKey, key);
            if (!_client.EnsureExists(levelValue, false))
            {
                _client.EnsureCreate(levelValue);
            }
        }

        private byte[] Serialize<T>(T value)
        {
            return JsonSerializerManager.BsonSerialize(value);
        }

        private T Deserialize<T>(byte[] value)
        {
            return JsonSerializerManager.BsonDeSerialize<T>(value);
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
