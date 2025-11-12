using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    public class StringHelper
    {
        static bool ContainsNonHex(string input)
        {
            // 使用正則表達式檢查是否包含非十六進制字符
            string hexPattern = "^[0-9A-Fa-f]+$";
            return !Regex.IsMatch(input, hexPattern);
        }
    }
}
