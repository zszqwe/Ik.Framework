using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ik.Framework.Caching.CacheKeyManager
{
    public class KeyConfigFormatObjcet
    {
        private static ILog logger = LogManager.GetLogger("缓存Key管理");
        private Dictionary<string, FormatObjcet> _formatObjcetDict = new Dictionary<string, FormatObjcet>(StringComparer.InvariantCultureIgnoreCase);
        private string _projectName = null;
        private static Dictionary<string, Dictionary<string, string>> _typeDefines = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, Assembly> _typeAssDefines = new Dictionary<string, Assembly>();
        private static Dictionary<string, string> _typeInAssemblys = new Dictionary<string, string>();
        private static object lockObj = new object();

        public KeyConfigFormatObjcet(string projectName, IList<KeyItem> cacheKeyList, IList<KeyTypeDefine> cacheKeyTypeList)
        {
            this._projectName = projectName;

            var group = cacheKeyTypeList.GroupBy(t => t.ValueTypeAssemblyName);
            foreach (var item in group)
            {
                lock (lockObj)
                {
                    if (!_typeDefines.ContainsKey(item.Key))
                    {
                        _typeDefines.Add(item.Key, new Dictionary<string, string>());
                        StringBuilder sb = new StringBuilder();
                        var def = item.First();
                        foreach (var code in item)
                        {
                            _typeDefines[item.Key].Add(code.ValueType, code.ValueClassCode);
                            sb.AppendLine("");
                            sb.AppendLine("//代码开始");

                            sb.Append(code.ValueClassCode);
                            sb.AppendLine("//代码结束");
                        }
                        var assembly = GetDefineTypeAssembly(sb.ToString(), def.ValueTypeAssemblyName);
                        _typeAssDefines.Add(item.Key, assembly);
                    }
                    else
                    {
                        List<KeyTypeDefine> needAdd = new List<KeyTypeDefine>();
                        foreach (var code in item)
                        {
                            if (!_typeDefines[item.Key].ContainsKey(code.ValueType))
                            {
                                needAdd.Add(code);
                            }
                        }
                        if (needAdd.Count > 0)
                        {
                            var def = item.First();
                            StringBuilder sb = new StringBuilder();
                            Dictionary<string, string> vTypes = _typeDefines[item.Key];
                            foreach (KeyValuePair<string, string> code in vTypes)
                            {
                                sb.AppendLine("");
                                sb.AppendLine("//代码开始");

                                sb.Append(code.Value);
                                sb.AppendLine("//代码结束");
                            }
                            foreach (var code in needAdd)
                            {
                                _typeDefines[item.Key].Add(code.ValueType, code.ValueClassCode);
                                _typeInAssemblys.Add(code.ValueType,item.Key);
                                sb.AppendLine("");
                                sb.AppendLine("//代码开始");

                                sb.Append(code.ValueClassCode);
                                sb.AppendLine("//代码结束");
                            }
                            var assembly = GetDefineTypeAssembly(sb.ToString(), def.ValueTypeAssemblyName);
                            _typeAssDefines.Add(item.Key, assembly);
                        }
                    }
                }
            }
            foreach (var item in cacheKeyList)
            {
                if (_formatObjcetDict.ContainsKey(item.Key))
                {
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, "缓存Key已经添加，Key:" + item.Key);
                }
                _formatObjcetDict.Add(item.Key, GetFormatObjcet(projectName, item));
            }
        }

        private Assembly GetDefineTypeAssembly(string sources,string assName)
        {

            if (string.IsNullOrEmpty(sources))
            {
                return null;
            }
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.Core.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");
            cplist.ReferencedAssemblies.Add("System.Xml.dll");
            cplist.ReferencedAssemblies.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin" , "Newtonsoft.Json.dll"));
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ass");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string file = Path.Combine(folder, assName + ".dll");
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    string tempFile = Path.Combine(folder, assName + ".temp");
                    File.Move(file, tempFile);
                }
            }
            cplist.OutputAssembly = file;
            CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");



            CompilerResults cr = provider1.CompileAssemblyFromSource(cplist, sources);
            if (true == cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }
            Assembly.LoadFrom(file);
            return cr.CompiledAssembly;
        }

        public static FormatObjcet GetFormatObjcet(string projectName, KeyItem item)
        {
            if (string.IsNullOrEmpty(item.Key))
            {
                throw new InkeyException(InkeyErrorCodes.CommonFailure, "缓存配置缓存Key参数缺失");
            }
            string key = item.Key;
            if (item.Scope == CacheKeyScope.Project)
            {
                key = projectName + "_" + item.Key;
            }
            if (!AppInfo.DeployEnvInfo.IsFormal)
            {
                var fx = AppInfo.DeployEnvInfo.DeployKey;
                key = fx + "_" + key;
            }
            FormatObjcet formatObjcet = null;
            if (!string.IsNullOrEmpty(item.RefValueType) && _typeInAssemblys.ContainsKey(item.RefValueType))
            {
                var ass = _typeAssDefines[_typeInAssemblys[item.RefValueType]];
                Type type = ass.GetType(item.RefValueType);
                formatObjcet = new FormatObjcet(type);
            }
            else
            {
                formatObjcet = new FormatObjcet();
            }
            formatObjcet.Key = key;
            return formatObjcet;
        }

        public Dictionary<string, FormatObjcet> FormatObjcets
        {
            get
            {
                return _formatObjcetDict;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
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
