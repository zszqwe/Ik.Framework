using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework
{
    [System.Web.Http.ModelBinding.ModelBinder(typeof(Ik.WebFramework.Api.GridQueryWebApiBinder))]
    [System.Web.Mvc.ModelBinder(typeof(Ik.WebFramework.Mvc.GridQueryMvcBinder))]
    public class GridQuery
    {
        public GridQuery()
        {
            this.SortInfos = new List<SortInfo>();
            this.FilterInfos = new List<FilterInfo>();
            this.PostData = new Dictionary<string, string>();
        }
        /// <summary>
        /// 是否有筛选条件
        /// </summary>
        public bool HasFilter { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 跳过索引
        /// </summary>
        public int SkipIndex
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }
        /// <summary>
        /// 是否有排序条件
        /// </summary>
        public bool HasSort { get; set; }
        /// <summary>
        /// 排序信息
        /// </summary>
        public IList<SortInfo> SortInfos { get; private set; }
        /// <summary>
        /// 筛选信息
        /// </summary>
        public IList<FilterInfo> FilterInfos { get; private set; }
        /// <summary>
        /// 回传的所以参数
        /// </summary>
        public IDictionary<string, string> PostData { get; private set; }
    }

    public class SortInfo
    {
        public string Name { get; set; }
        public SortType Type { get; set; }
    }

    public enum SortType
    {
        Asc, Desc
    }

    public class FilterInfo
    {
        public FilterInfo()
        {
            this.Values = new List<string>();
        }
        public FilterComposeType ComposeType { get; set; }

        public string Name { get; set; }

        public FilterOperateType OperateType { get; set; }

        public bool HasMultipleValue { get; set; }

        public string Value { get; set; }

        public IList<string> Values { get; private set; }
    }

    public enum FilterComposeType
    {
        And,
        Or
    }

    public enum FilterOperateType
    {
        Eq, //等于
        Ne, //不等
        Lt, //小于
        Le,//小于等于
        Gt,//大于
        Ge,//大于等于
        Bw,//开始于
        Bn,//不开始于
        In,//属于
        Ni,//不属于
        Ew,//结束于
        En,//不结束于
        Cn,//包含
        Nc,//不包含
        Nu,//不存在
        Nn, //存在
    }
}
