using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Common
{
    public static class DateTimeHelper
    {
        public static DateTime GetRealTime(this AUser AUser)
        {
            return DateTime.Now.AddDays(AUser.Day).AddMonths(AUser.Month).AddYears(AUser.Year);
        }
    }
}
