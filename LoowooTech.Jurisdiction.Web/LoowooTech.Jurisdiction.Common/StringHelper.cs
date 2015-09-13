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
            if (!string.IsNullOrEmpty(str))
            {
                var key = str.Split(ch);
                foreach (var item in key)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        list.Add(item);
                    }
                }
            }
            
            return list;
        }
        public static Dictionary<string, List<LoowooTech.Jurisdiction.Models.Group>> Sort(this Dictionary<string, List<LoowooTech.Jurisdiction.Models.Group>> Dict)
        {
            var newDict = new Dictionary<string, List<LoowooTech.Jurisdiction.Models.Group>>();
            foreach (var key in Dict.Keys)
            {
                newDict.Add(key, Dict[key].OrderBy(e => e.Name).ToList());
                //Dict[key] = Dict[key].OrderBy(e => e.Name).ToList();
            }
            return newDict;
        }


        public static Dictionary<string, List<T[]>> DictToTable<T>(this Dictionary<string, List<T>> Dict)
        {
            var TheDict = new Dictionary<string, List<T[]>>();
            foreach (var key in Dict.Keys)
            {
                TheDict.Add(key, Dict[key].ListToTable());
            }
            return TheDict;
        }
        public static List<T[]> ListToTable<T>(this List<T> List,int Count=2)
        {
            var value = new T[Count];
            int Index = 0;
            var Reborn = new List<T[]>();
            foreach (var item in List)
            {
                if (Index == Count)
                {
                    Reborn.Add(value);
                    value = new T[Count];
                    Index = 0;
                }
                value[Index] = item;
                Index++;
            }
            if (value != null)
            {
                Reborn.Add(value);
            }
            //if (!string.IsNullOrEmpty(value[0]))
            //{
            //    Reborn.Add(value);
            //}
            return Reborn;
        }

        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

       

        
    }
}
