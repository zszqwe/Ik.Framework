using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DependencyManagement
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false,Inherited = true)]
    public class AutoBizServiceAttribute : Attribute
    {
    }
}
