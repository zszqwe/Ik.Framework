using Ik.Framework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DependencyManagement
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class AutoDataServiceAttribute : Attribute
    {
        public AutoDataServiceAttribute()
        {
            ServiceType = AutoDataServiceType.Mapping;
        }

        public AutoDataServiceAttribute(AutoDataServiceType serviceType)
        {
            ServiceType = serviceType;
        }

        public AutoDataServiceType ServiceType { get; private set; }

    }

    public enum AutoDataServiceType
    {
        Mapping, EF
    }
}
