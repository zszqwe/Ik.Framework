using Ik.Framework.DataAccess.DataMapping;
using Ik.ItAdmin.Web.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Services
{
    public class OperationSupportDataAccessFactory
    {
        private static IDataAccessFactory factory = null;
        private static object lockObj = new object();
        public const string DataSourceName = DataSources.DataSource_ItAdmin;

        public static IDataAccessFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lockObj)
                    {
                        if (factory == null)
                        {
                            factory = new DataAccessFactory(DataSourceName);
                        }
                    }
                }
                return factory;
            }
        }
    }
}