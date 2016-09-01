using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ik.ItAdmin.Web.Dtos
{
    public class GroupUserInfoTreeNode
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
    }
}