using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ik.Framework.DataAccess.DataMapping;




namespace Ik.Framework.Test.DataAcessList
{
    [TestClass]
    public class DataAccessFactoryTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            
            DataAccessFactory factory = new DataAccessFactory("OperationSupport");

            var access = factory.GetDataAccess<IVersionContentDao>();
           
            var data = access.GetVersionContentByCode(new Guid("2A62CB5F-FB1B-079A-14BC-05E2096FE81F"));
            //var data1 = access.GetVersionContentListByCategoryCode(new Guid("60ce0ab6-9c00-14be-17e6-63cdffef9136"));
            //var data2 = access.GetVersionContentListByCategoryCode2(new Guid("60ce0ab6-9c00-14be-17e6-63cdffef9136"));
            //var data3 = access.GetVersionContentByPageList(new Services.Common.Interface.VersionContents.ContentVersionParams { CategoryCode = new Guid("60ce0ab6-9c00-14be-17e6-63cdffef9136"), PageIndex = 1, PageSize = 30, RequireTotalCount = true });
        }
    }
}
