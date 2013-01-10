using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFKCommonLibrary.ExtensionMethods
{
    public static class IEnumerableExtensions
    {
        public static string AsString(this IEnumerable<string> list, string seperator = ", ")
        {
            if (list == null || list.Count() == 0) return "";
            var retVal = "";
            foreach (var itm in list)
            {
                retVal += itm + seperator;
            }

            return retVal.Substring(0, retVal.Length - seperator.Length);
        }
    }
}
