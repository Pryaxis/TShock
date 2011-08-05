using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    public static class StringExt
    {
        //Can't name it Format :(
        public static String SFormat(this String str, params object[] args)
        {
            return String.Format(str, args);
        }
    }
}
