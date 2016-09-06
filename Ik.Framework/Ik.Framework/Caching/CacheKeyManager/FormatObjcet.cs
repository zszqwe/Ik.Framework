using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ik.Framework.Caching.CacheKeyManager
{
    public class FormatObjcet 
    {
        private Type _type = null;
        public FormatObjcet(Type type = null)
        {
            this._type = type;
            KeyList = new List<string>();
        }


        private string key = "";
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                if (!string.IsNullOrEmpty(key))
                {
                    key = value.ToLower();
                    MatchCollection mc = Regex.Matches(key, @"{[^{}]+}");
                    {
                        foreach (Match m in mc)
                        {
                            KeyList.Add(m.Value);
                        }
                    }
                }
            }
        }

        public List<string> KeyList { get; private set; }

        public string FormatKey(params object[] values)
        {
            string result = null;
            if (KeyList.Count != values.Length)
            {
                return null;
            }
            result = key;
            for (int i = 0; i < KeyList.Count; i++)
            {
                if (values[i] == null)
                {
                    throw new InkeyException(InkeyErrorCodes.CommonFailure, "参数不能为空，参数名：" + KeyList[i]);
                }
                result = result.Replace(KeyList[i], values[i].ToString());
            }
            return result;
        }

        public Type GetKeyDefineType()
        {
            return this._type;
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
