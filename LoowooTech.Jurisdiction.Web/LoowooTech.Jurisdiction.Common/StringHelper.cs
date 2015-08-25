using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoowooTech.Jurisdiction.Common
{
    public static class StringHelper
    {
        public static string Extract(this string str, int Index = 0, string Filter = "CN=")
        {
            var key = str.Split(',');
            return key[Index].Replace(Filter, "");
        }

        public  static string ToStr(List<string> List)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in List)
            {
                sb.Append(item + ";");
            }
            return sb.ToString();
        }

        public static List<string> StrToList(this string str,char ch)
        {
            List<string> list = new List<string>();
            var key = str.Split(ch);
            foreach (var item in key)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        
    }
}
