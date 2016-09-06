using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ik.Framework.Caching;

namespace Ik.Framework.Test.Caching
{
    [TestClass]
    public class MemcachedManagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            MemcachedManager m = new MemcachedManager();
            var c = m.Get<string>("api_keyTest");
            if (string.IsNullOrEmpty(c))
            {
                m.Set("api_keyTest", "test");
            }
            c = m.Get<string>("api_keyTest");
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
