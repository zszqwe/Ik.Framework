
using Ik.Framework.Common.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Test.DataAcessList.VersionContents
{
    /// <summary>
    /// 版本类容
    /// </summary>
    public class VersionContentDto
    {
        /// <summary>
        /// 分类编码
        /// </summary>
        public Guid CategoryCode { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        public Guid CategoryParentCode { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 内容编码
        /// </summary>
        public Guid ContentCode { get; set; }
        /// <summary>
        /// 内容名称
        /// </summary>
        public string ContentName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string ContentDesc { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public int ContentType { get; set; }
        /// <summary>
        /// 内容值
        /// </summary>
        public string ContentText { get; set; }
        /// <summary>
        /// 内容创建时间
        /// </summary>
        public DateTime? ContentCreatedTime { get; set; }
        /// <summary>
        /// 内容开始时间
        /// </summary>
        public DateTime? ContentStartTime { get; set; }
        /// <summary>
        /// 内容结束时间
        /// </summary>
        public DateTime? ContentEndTime { get; set; }
        /// <summary>
        /// 内容状态
        /// </summary>
        public int ContentStatus { get; set; }
        /// <summary>
        /// 内容排序
        /// </summary>
        public int ContentSortId { get; set; }

        /// <summary>
        /// 分类版本号值
        /// </summary>
        public long CategoryVersionValue { get; set; }

        /// <summary>
        /// 内容版本号值
        /// </summary>
        public long ContentVersionValue { get; set; }

    }

    public class ContentVersionParams : PageParameter
    {
        public Guid CategoryCode { get; set; }
    }

}
