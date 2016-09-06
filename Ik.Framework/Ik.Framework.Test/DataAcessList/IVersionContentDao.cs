

using Ik.Framework.Common.Paging;
using Ik.Framework.DataAccess.DataMapping;
using Ik.Framework.Test.DataAcessList.VersionContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Test.DataAcessList
{
    public interface IVersionContentDao
    {
        /// <summary>
        /// 获取指定分类下所有相关版本的内容，时间过期，分类状态=1，内容状态=1的都会被过滤
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [DataMappingInterception(typeof(myInterception))]
        IList<VersionContentDto> GetVersionContentListByCategoryCode(Guid categoryCode);

        IEnumerable<VersionContentDto> GetVersionContentListByCategoryCode2(Guid categoryCode);
        /// <summary>
        /// 获取指定内容code返回内容，时间过期，分类状态=1，内容状态=1的都会被过滤
        /// </summary>
        /// <param name="contentCode"></param>
        /// <returns></returns>
        VersionContentDto GetVersionContentByCode(Guid contentCode);

        IPagedList<VersionContentDto> GetVersionContentByPageList(ContentVersionParams filter);
    }

    public class myInterception : IDataMappingInterception
    {

        public void BeforeExecute(string methodName, object args)
        {
            throw new NotImplementedException();
        }

        public void EndExecute(string methodName, object args, object value)
        {
            throw new NotImplementedException();
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
