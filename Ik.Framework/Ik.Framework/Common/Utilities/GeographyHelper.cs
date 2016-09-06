using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    public static class GeographyHelper
    {
        public const double InvalidDistance = 100000000;

        public static DbGeography Point(double longitude, double latitude)
        {
            string wellKnownText = string.Format("POINT({0} {1})", longitude, latitude);
            var point = DbGeography.FromText(wellKnownText);
            return point;
        }

        public static string DistanceFormat(double distance)
        {
            if (distance < 1000)
            {
                return string.Format("{0}米", (int)distance);
            }
            else if (distance < InvalidDistance)
            {
                return string.Format("{0}公里", (distance / 1000).ToString("0.0"));
            }
            else
            {
                return "未知距离";
            }
        }

        
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
