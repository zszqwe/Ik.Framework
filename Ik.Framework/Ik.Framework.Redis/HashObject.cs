using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class HashObject
    {
        public HashObject(HashKeyObject key, HashValueObject value)
        {
            this.Key = key;
            this.Value = value;
        }
        public HashKeyObject Key { get; set; }

        public HashValueObject Value { get; set; }
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
