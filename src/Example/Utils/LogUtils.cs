using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example.Utils
{
    public static class LogUtils
    {

        public static string LogCollection<T>(IEnumerable<T> items)
        {
            return string.Join(", ", items);
        }
    }
}