using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive.Implement
{
    internal class EndsWithDirective : IFilterDirective
    {
        public string FilterSyntax
        {
            get
            {
                return "endswith";
            }
        }

        public Expression GenerateExpression(ref MemberExpression property, ConstantExpression value)
        {
            var function = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

            if (value.Type != typeof(string))
                throw new ArgumentException("Value must be string type");

            if (function == null)
                throw new Exception("EndsWith function is not found");

            return Expression.Call(property, function, value);
        }
    }
}
