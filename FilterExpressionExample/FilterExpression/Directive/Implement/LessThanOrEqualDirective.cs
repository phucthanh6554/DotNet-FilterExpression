using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive.Implement
{
    internal class LessThanOrEqualDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "le";
            }
        }

        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            return Expression.LessThanOrEqual(property, value); 
        }
    }
}
