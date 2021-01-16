using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Modbus.Common
{
    public class NumericString
    {
        #region 判断字符串是否由数字组成
        /// <summary>
        /// 判断字符串是否由数字组成
        /// </summary>
        public static bool IsNumeric(string s)
        {
            string pattern = @"^\-?[0-9]+$";
            return Regex.IsMatch(s, pattern);
        }
        #endregion
    }
}
