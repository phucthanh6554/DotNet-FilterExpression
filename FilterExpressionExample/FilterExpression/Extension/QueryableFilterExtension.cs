using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Extension
{
    public static class QueryableFilterExtension
    {
        private static FilterService _filterService = new FilterService();

        public static IQueryable<T> Filter<T>(this IQueryable<T> queryable, string fe)
        {
            var filter = _filterService.Filter<T>(fe);

            return queryable.Where(filter);
        }
    }
}
