using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    /// <summary>
    /// 别名。
    /// </summary>
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 指定别名。
        /// </summary>
        /// <param name="name">名称</param>
        public AliasAttribute(string name)
        {
            this.Name = name;
        }
    }

}
