using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework.Api
{
    public class ApiRequestProcessInfo : RequestProcessInfo
    {
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 终端平台
        /// </summary>
        public DevicePlatform ClientType { get; set; }

        /// <summary>
        /// 客户端网络类型
        /// </summary>
        public string ClientNetType { get; set; }

        /// <summary>
        /// 接口版本
        /// </summary>
        public string InterfaceVersion { get; set; }

        /// <summary>
        /// 终端版本
        /// </summary>
        public string ClientVersion { get; set; }


        public bool HasLocation
        {
            get
            {
                return this.Lat > 1 && this.Lng > 1;
            }
        }

        
        public string RegionCode { get; set; }

        public int ProcessState { get; set; }

        public string ProcessDesc { get; set; }

        public string Imei { get; set; }
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
