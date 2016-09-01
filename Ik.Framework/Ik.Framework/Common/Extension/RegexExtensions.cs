using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class RegexExtensions
    {
        // 不要用/d，/d会匹配中文的数字
        private const string MobileNumberPattern = "^[1][3|4|5|7|8][0-9][0-9]{8}$";

        public static bool IsMobileNumber(this string input)
        {
            if (input == null || input.Length != 11)
                return false;

            return Regex.IsMatch(input, MobileNumberPattern, RegexOptions.Compiled);
        }

    }
}
