using Ik.Framework.DataAccess;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.DependencyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    [AutoDataService(AutoDataServiceType.EF)]
    [DataSource(DataSources.DataSource_ItAdmin)]
    public class CacheKeyAppInfoEntity : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }


        public DateTime CreateTime { get; set; }

        private ICollection<CacheKeyItemInfoEntity> _cacheKeyItemInfos;

        public virtual ICollection<CacheKeyItemInfoEntity> CacheKeyItemInfos
        {
            get { return _cacheKeyItemInfos ?? (_cacheKeyItemInfos = new List<CacheKeyItemInfoEntity>()); }
            protected set { _cacheKeyItemInfos = value; }
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
