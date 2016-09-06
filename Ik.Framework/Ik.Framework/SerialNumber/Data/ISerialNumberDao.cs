using Ik.Framework.DataAccess;
using Ik.Framework.DependencyManagement;
using Ik.Framework.SerialNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.SerialNumber.Data
{
    public interface ISerialNumberDao
    {
        SerialNumberDto GetSerialNumberConfigByKey(string key);

        long GetSerialNumberNextValueByKey(string key);

        long GetIncrementSerialNumberByKey(string key, long lengthMaxValue);

        long GetIncrementCountSerialNumberByKey(string key, int count, long lengthMaxValue);

        SerialNumberStateDto GetApplyCacheSerialNumberCapacityByKey(string key, long serialNumber);
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
