using Ik.Framework.Common.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class PagedListExtension
    {
        /// <summary>
        /// 根据分页参数返回分页对象（对数据集合内存分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPageList<T>(this IEnumerable<T> source, int pageIndex, int pageSize) where T : class ,new()
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据分页参数返回分页对象（对数据集合不分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static IPagedList<T> ToPageList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount) where T : class ,new()
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// 根据分页参数返回分页对象（对数据集合内存分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedListDto<T> ToPageListDto<T>(this IEnumerable<T> source, int pageIndex, int pageSize) where T : class ,new()
        {
            return new PagedList<T>(source, pageIndex, pageSize).GetPagedListDto();
        }

        /// <summary>
        /// 根据分页参数返回分页对象（对数据集合不分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static PagedListDto<T> ToPageListDto<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount) where T : class ,new()
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount).GetPagedListDto();
        }

        /// <summary>
        /// 根据分页参数返回分页对象（对数据集合不分页，不计算总页数等分页属性）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static PagedListDto<T> GetPageListDto<T>(this IEnumerable<T> source, int pageIndex, int pageSize) where T : class ,new()
        {
            return new PagedListDto<T> { PageData = new List<T>(source), PageIndex = pageIndex, PageSize = pageSize };
        }
    }
}
