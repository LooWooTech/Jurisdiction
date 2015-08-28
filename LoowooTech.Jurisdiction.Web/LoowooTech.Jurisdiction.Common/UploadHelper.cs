using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LoowooTech.Jurisdiction.Common
{
    public static class UploadHelper
    {
        public static string[] GetValue(this HttpContextBase context,string PropertyName)
        {
            return context.Request.Form[PropertyName].Split(',');
        }

        
    }
}
