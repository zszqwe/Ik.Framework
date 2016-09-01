

using Ik.Framework.Common.Paging;
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
}
