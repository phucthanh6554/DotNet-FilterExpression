using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive
{
    internal class NotEqualDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "ne";
            }
        }

        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            return Expression.NotEqual(property, value);
        }
    }
}
