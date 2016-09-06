using Ik.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class HashKeyObject
    {
        private string _originalKey = null;
        private byte[] _originalRawKey = null;

        public HashKeyObject(byte[] originalRawKey)
        {
            this._originalRawKey = originalRawKey;
        }

        public HashKeyObject(string originalKey)
        {
            this._originalKey = originalKey;
        }
        public T GetKey<T>()
        {
            if (this._originalKey != null)
            {
                return JsonSerializerManager.JsonDeserialize<T>(this._originalKey);
            }
            return BinarySerializerManager.BinaryDeSerialize<T>(this._originalRawKey);
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
