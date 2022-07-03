using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Extension
{
    public static class ListFilterExtension
    {
        private static FilterService _filterService = new FilterService();

        public static List<T> Filter<T>(this List<T> list, string fe)
        {
            var filter = _filterService.Filter<T>(fe);

            return list.Where(filter.Compile()).ToList();
        }
    }
}
