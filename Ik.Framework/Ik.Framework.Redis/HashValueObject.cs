using Ik.Framework.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Redis
{
    public class HashValueObject
    {
        private string _originalValue = null;
        private byte[] _originalRawValue = null;

        public HashValueObject(string originalValue)
        {
            this._originalValue = originalValue;
        }

        public HashValueObject(byte[] originalRawValue)
        {
            this._originalRawValue = originalRawValue;
        }
        public T GetValue<T>()
        {
            if (this._originalValue != null)
            {
                return JsonSerializerManager.JsonDeserialize<T>(this._originalValue);
            }
            return BinarySerializerManager.BinaryDeSerialize<T>(this._originalRawValue);
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
