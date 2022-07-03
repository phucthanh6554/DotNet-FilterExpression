using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.Directive
{
    public interface IFilterDirective
    {
        string FilterSyntax { get; }

        Expression GenerateExpression(ref MemberExpression property, ConstantExpression value);
    }
}
