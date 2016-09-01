using Ik.Framework.Common.Serialization;
using Ik.Framework.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ik.Framework.Caching.CacheKeyManager
{
    public class KeyConfigManager
    {
        public static CacheKeyConfig GetKeyConfig()
        {
            return ConfigManager.Instance.Get<CacheKeyConfig>(KeyFormatConfigurationSectionHandler.ConfigSectionName);
        }

        public static CacheKeyConfig GetKeyConfigFormAssembly(Assembly assembly)
        {
            XmlDocument keysXmlConfig = new XmlDocument();
            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".keyManager.config");
            keysXmlConfig.Load(stream);
            return XmlSerializerManager.XmlDeserialize<CacheKeyConfig>(keysXmlConfig.DocumentElement);
        }

        public static KeyConfigFormatObjcet GetFormatObjcetDictFromConfig()
        {
            CacheKeyConfig keysConfig = GetKeyConfig();
            return GetFormatObjcetDict(keysConfig);
        }

        public static KeyConfigFormatObjcet GetFormatObjcetDictFromConfig(CacheKeyConfig keysConfig)
        {
            return GetFormatObjcetDict(keysConfig);
        }

        public static KeyConfigFormatObjcet GetFormatObjcetDictFromAssembly(Assembly assembly)
        {
            CacheKeyConfig keysConfig = GetKeyConfigFormAssembly(assembly);
            return GetFormatObjcetDict(keysConfig);
        }

        private static KeyConfigFormatObjcet GetFormatObjcetDict(CacheKeyConfig keyConfig)
        {
            if (keyConfig == null)
            {
                throw new InkeyException(InkeyErrorCodes.CommonFailure, "缓存配置获取失败");
            }
            if (string.IsNullOrEmpty(keyConfig.ProjectName))
            {
                if (string.IsNullOrEmpty(AppInfo.AppCode))
                {
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, "缓存配置项目名称参数缺失");
                }
                keyConfig.ProjectName = AppInfo.AppCode;
            }
            if (string.IsNullOrEmpty(keyConfig.ProjectShowName))
            {
                if (string.IsNullOrEmpty(AppInfo.AppName))
                {
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, "缓存配置项目名称参数缺失");
                }
                keyConfig.ProjectShowName = AppInfo.AppName;
            }
            return new KeyConfigFormatObjcet(keyConfig.ProjectName, keyConfig.CacheKeys, keyConfig.CacheValueTypes);
        }

        public static IList<CacheKeyConfig> GetAllRegisterKeyConfig()
        {
            IList<CacheKeyConfig> list = new List<CacheKeyConfig>();
            var typeFinder = new AppDomainTypeFinder();
            var drTypes = typeFinder.FindClassesOfType<IKeyConfigRegistrar>();
            var drInstances = new List<IKeyConfigRegistrar>();
            foreach (var drType in drTypes)
            {
                drInstances.Add((IKeyConfigRegistrar)Activator.CreateInstance(drType));
            }
            foreach (var kcRegistrar in drInstances)
            {
                list.Add(kcRegistrar.RegisterConfig());
            }
            return list;
        }
    }
}
