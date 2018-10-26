using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects
{
    public class Helper
    {
        public static string AsString(params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in args)
            {
                if (item is IEnumerable)
                {
                    var enumerable = item as IEnumerable;
                    foreach (var i in enumerable)
                    {
                        sb.Append(i.ToString());
                        sb.Append(",");
                    }

                    sb.Length -= 1;
                    sb.Append("__");
                }
                else
                {
                    sb.Append(item.ToString());
                    sb.Append("__");
                }
            }

            if (sb.Length > 2) sb.Length -= 2;
            return sb.ToString();
        }
    }
}
