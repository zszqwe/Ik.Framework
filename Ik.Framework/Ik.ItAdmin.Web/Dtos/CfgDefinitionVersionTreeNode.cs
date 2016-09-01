using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class CfgDefinitionVersionTreeNode
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public Guid DefId { get; set; }

        public Guid DefVerId { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }

        public bool HasChild { get; set; }

        public int Level { get; set; }
    }
}