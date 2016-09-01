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
