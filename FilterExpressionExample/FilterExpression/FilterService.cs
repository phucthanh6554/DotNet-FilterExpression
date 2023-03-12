using FilterExpression.Directive;
using FilterExpression.DirectiveDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FilterExpression
{
    public class FilterService
    {
        private static DirectiveDispatchService _dispatchService = new DirectiveDispatchService();
        private static ParseValueService _parseValueService = new ParseValueService();

        public Expression<Func<T, bool>> Filter<T>(string fe)
        {
            if (string.IsNullOrEmpty(fe) || string.IsNullOrWhiteSpace(fe))
                return x => true;

            List<object> Output = new List<object>();
            Stack<string> Stack = new Stack<string>();

            string str = "";
            var operators = new List<string> { "(", "&", "|", "!" };

            int singleQuoteCount = 0;

            var parameter = Expression.Parameter(typeof(T), "x");

            foreach (var c in fe)
            {
                if (operators.Contains(c.ToString()))
                {
                    Stack.Push(c.ToString());
                    continue;
                }

                if (c == '`')
                    singleQuoteCount++;

                if (singleQuoteCount > 0 && singleQuoteCount % 2 == 0)
                {
                    var expression = StringToExpression<T>(str, ref parameter);

                    Output.Add(expression);

                    str = "";
                    singleQuoteCount = 0;
                    continue;
                }
                else if (c == ')')
                {
                    Output.AddRange(GetStack(ref Stack));
                }
                else
                {
                    str += c;
                }
            }

            Output.AddRange(GetStack(ref Stack));

            for (var i = 0; i < Output.Count; i++)
            {
                if(Output[i].GetType() == typeof(string) && Output[i].ToString() == "!" && i > 0)
                {
                    var expression = (Expression)Output[i - 1];

                    expression = Expression.Not(expression);

                    Output[i] = expression;

                    if (i - 2 > -1)
                    {
                        Output[i - 1] = Output[i - 2];
                        Output[i - 2] = null!;
                    }
                    else
                    {
                        Output[i - 1] = null!;
                    }
                }
                else if (Output[i].GetType() == typeof(string) && i > 1)
                {
                    var val1 = (Expression)Output[i - 2];
                    var val2 = (Expression)Output[i - 1];

                    if (Output[i].ToString() == "&")
                        Output[i] = Expression.And(val1, val2);
                    else if (Output[i].ToString() == "|")
                        Output[i] = Expression.Or(val1, val2);

                    Output[i - 2] = null!;

                    if (i - 3 > -1)
                    {
                        Output[i - 1] = Output[i - 3];
                        Output[i - 3] = null!;
                    }
                    else
                    {
                        Output[i - 1] = null!;
                    }
                }
            }

            var a = (Expression)Output[Output.Count - 1];

            return Expression.Lambda<Func<T, bool>>((Expression)Output[Output.Count - 1], parameter);
        }

        private List<object> GetStack(ref Stack<string> stack)
        {
            var result = new List<object>();

            var current = "";

            while (current != "(" && stack.Count != 0)
            {
                current = stack.Pop();

                if (current != "(")
                    result.Add(current);
            }

            return result;
        }

        private Expression StringToExpression<T>(string str, ref ParameterExpression parameter)
        {
            var strData = str.Trim().Split(' ', 3);

            Expression result = null;

            if (strData.Length == 3)
            {
                var propertiesStr = strData[0].Split('.');

                //ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertiesStr[0]);

                for(int i = 1; i < propertiesStr.Length; i++)
                {
                    property = Expression.Property(property, propertiesStr[i]);
                }

                ConstantExpression value = _parseValueService.GetConstantExpression(strData[2], property.Type);

                IFilterDirective filterDirective = _dispatchService.GetDirective(strData[1]);

                return filterDirective.GenerateExpression(ref property, value);               
            }

            return result;
        }
    }
}
