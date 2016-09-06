using System;
using System.Collections.Generic;
namespace Ik.Framework.SerialNumber
{
    public interface ISerialNumberService
    {
        string GetSerialNumber();

        IList<string> GetSerialNumber(int count);
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
