using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Service.Auth.Dtos
{
    public class AuthFunction
    {
        public Guid FunctionId { get; set; }

        public Guid ParentFunctionId { get; set; }

        public string FuncCode { get; set; }

        public string FuncName { get; set; }

        public string FuncDesc { get; set; }

        public bool FuncIsEnable { get; set; }

        public bool FuncIsDelete { get; set; }

        public DateTime FuncCreateTime { get; set; }

        public DateTime FuncUpdateTime { get; set; }

    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
