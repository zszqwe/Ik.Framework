
namespace Ik.Framework.Common.Paging
{
    /// <summary>
    /// 分页查询接口
    /// </summary>
    public interface IPageParameter
    {
        /// <summary>
        /// 当前页，1开始。
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 记录开始行号，1开始。
        /// </summary>
        int StartIndex { get; }

        /// <summary>
        /// 记录结束行号
        /// </summary>
        int EndIndex { get; }

        /// <summary>
        /// 是否需要记录总数
        /// </summary>
        bool RequireTotalCount { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        int TotalCount { get; set; }

        int MaxRecordCount { get; }
    }
}
