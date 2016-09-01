using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    [Serializable]
    public class InkeyException : Exception
    {
        public InkeyException()
        {
 
        }

        public InkeyException(int code, string desc)
            : base(desc)
        {
            this.Code = code;
            this.Desc = desc;
        }

        public InkeyException(int code, string desc, Exception ex)
            : base(desc, ex)
        {
            this.Code = code;
            this.Desc = desc;
        }
        

        public int Code { get; set; }
        public string Desc { get; set; }
    }
}
