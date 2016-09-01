using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public class InkeyResult
    {
        public string Desc { get; set; }
        public int Code { get; set; }

        public bool IsSuccess
        {
            get 
            {
                return this.Code == InkeyErrorCodes.CommonSuccess;
            }
        }

        public InkeyResult()
        {
            //默认操作成功
            this.Code = InkeyErrorCodes.CommonSuccess;
            this.Desc = InkeyErrorCodes.CommonSuccessDesc;
        }

        public InkeyResult(int code)
            : this(code, null)
        {
        }

        public InkeyResult(int code, string desc)
        {
            this.Code = code;
            this.Desc = desc;
        }

        public InkeyResult CopyStatus(InkeyResult result)
        {
            this.Code = result.Code;
            this.Desc = result.Desc;
            return this;
        }
    }

    public class InkeyResult<T> : InkeyResult
    {
        public InkeyResult()
        {
        }

        public InkeyResult(int code)
            : base(code, null)
        {
        }

        public InkeyResult(int code, string desc) : base(code, desc) 
        { }



        public T Data { get; set; }

        public new InkeyResult<T> CopyStatus(InkeyResult result)
        {
            this.Code = result.Code;
            this.Desc = result.Desc;
            return this;
        }
    }
}
