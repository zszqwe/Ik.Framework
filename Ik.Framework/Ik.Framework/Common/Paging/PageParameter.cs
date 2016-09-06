using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Paging
{
    /// <summary>
    /// 自动分页参数，当前页码从1开始
    /// </summary>
    public class PageParameter : IPageParameter
    {
        private static int pageIndexMax = 100;
        private static int pageSizeMax = 200;
        private static int limitMaxTotalCount = 2000;
        private bool requireTotalCount = true;
        private int totalCount = 0;
        private int pageIndex = 0;

        public PageParameter()
        {
 
        }

        public PageParameter(int maxPageIndex, int maxPageSize, int maxTotalCount)
        {
            pageIndexMax = maxPageIndex;
            pageSizeMax = maxPageSize;
            limitMaxTotalCount = maxTotalCount;
        }

        /// <summary>
        /// 当前页，1开始。
        /// </summary>
        public int PageIndex
        {
            get
            {
                if (pageIndex < 1 || pageIndex > pageIndexMax)
                {
                    return pageIndexMax;
                }
                return pageIndex;
            }
            set
            {
                pageIndex = value;
            }
        }

        private int pageSize = 0;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSize < 1 || pageSize > pageSizeMax)
                {
                    return pageSizeMax;
                }
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// 记录开始行号，1开始。
        /// </summary>
        public int StartIndex
        {
            get
            {
                return (PageIndex - 1) * PageSize + 1;
            }
        }

        /// <summary>
        /// 记录结束行号
        /// </summary>
        public int EndIndex
        {
            get
            {
                return PageIndex * PageSize;
            }
        }

        public bool RequireTotalCount
        {
            get
            {
                return requireTotalCount; ;
            }
            set
            {
                requireTotalCount = value;
            }
        }

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

        public int TotalCount
        {
            get
            {
                return totalCount;
            }
            set
            {
                totalCount = value;
            }
        }


        public int MaxRecordCount
        {
            get 
            {
                return limitMaxTotalCount;
            }
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
