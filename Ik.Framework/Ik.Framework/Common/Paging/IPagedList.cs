using System;
using System.Collections.Generic;
using System.Linq;

namespace Ik.Framework.Common.Paging
{
    /// <summary>
    /// 分页列表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IList<T>, IPagedList 
    {
        /// <summary>
        /// 转换当前分页列表数据T到T2分页列表数据
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        IPagedList<T2> ToPagedList<T2>(Func<T, T2> set);

        /// <summary>
        /// 转换当前分页列表数据到PagedListDto<T>类型分页列表数据
        /// </summary>
        /// <returns></returns>
        PagedListDto<T> GetPagedListDto();

        /// <summary>
        /// 转换当前分页列表数据T到PagedListDto<T2>类型分页列表数据
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        PagedListDto<T2> GetPagedListDto<T2>(Func<T, T2> set);
    }

    /// <summary>
    /// 分页列表数据
    /// </summary>
    public interface IPagedList
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }

        object ExtraData { get; set; }
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
