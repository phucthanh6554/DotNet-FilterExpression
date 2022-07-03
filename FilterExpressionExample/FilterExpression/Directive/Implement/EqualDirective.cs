using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive
{
    internal class EqualDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "eq";
            }
        }

        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            return Expression.Equal(property, value);
        }
    }
}
