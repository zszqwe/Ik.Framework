using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Models
{
    public class JqGridDataModel<T>
    {
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }

        [JsonProperty("totalRecords")]
        public int TotalRecords { get; set; }

        [JsonProperty("dataList")]
        public IEnumerable<T> DataList { get; set; }
    }
}