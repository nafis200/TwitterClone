using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Utilities.Extensions
{
   public static class StringExtensions
    {
        public static bool IsLong(this string value)
        {
            return value.Length > 10;
        }
    }
}