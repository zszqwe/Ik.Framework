using Ik.Framework.DataAccess;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.DependencyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.DataAccess.EF
{
    [AutoDataService(AutoDataServiceType.EF)]
    [DataSource(DataSources.DataSource_ItAdmin)]
    public class SerialDefineInfoEntity : BaseEntity
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public long NextValue { get; set; }
        public string PrefixValue { get; set; }
        public int FormatLength { get; set; }
        public string DateFormat { get; set; }
        public int ApplyCapacity { get; set; }
        public int CheckThreshold { get; set; }
        public DateTime CreateTime { get; set; }

        public string Desc { get; set; }

    }
}