using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Paging
{
    /// <summary>
    /// 分页列表数据，当前页码从1开始
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        private int totalCount = 0;
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            totalCount = source.Count();
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            this.totalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source);
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount
        {
            get
            {
                return totalCount;
            }
            set
            {
                totalCount = value;
                TotalPages = TotalCount / PageSize;

                if (TotalCount % PageSize > 0)
                    TotalPages++;
            }
        }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }

        public object ExtraData { get; set; }


        public PagedListDto<T> GetPagedListDto()
        {
            return new PagedListDto<T>
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount,
                TotalPages = this.TotalPages,
                HasPreviousPage = this.HasPreviousPage,
                HasNextPage = this.HasNextPage,
                ExtraData = this.ExtraData,
                PageData = this
            };
        }

        public PagedListDto<T2> GetPagedListDto<T2>(Func<T, T2> set)
        {
            PagedListDto<T2> pageList = new PagedListDto<T2>
            {
                PageIndex = this.PageIndex,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount,
                TotalPages = this.TotalPages,
                HasPreviousPage = this.HasPreviousPage,
                HasNextPage = this.HasNextPage,
                ExtraData = this.ExtraData
            };
            List<T2> list = new List<T2>();
            foreach (var item in this)
            {
                list.Add(set(item));
            }
            pageList.PageData = list;
            return pageList;
        }

        public IPagedList<T2> ToPagedList<T2>(Func<T, T2> set)
        {
            List<T2> list = new List<T2>();
            foreach (var item in this)
            {
                list.Add(set(item));
            }
            return new PagedList<T2>(list, this.PageIndex, this.PageSize, this.TotalCount);
        }
    }
}
