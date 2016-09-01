using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework
{
    public class JqGridData<T>
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
