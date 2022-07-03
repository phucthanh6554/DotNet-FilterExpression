using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive.Implement
{
    internal class GreaterThanOrEqualDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "ge";
            }
        }

        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            return Expression.GreaterThanOrEqual(property, value);
        }
    }
}
