using FilterExpression.Directive;
using FilterExpression.DirectiveDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression
{
    public class FilterService
    {
        private static DirectiveDispatchService _dispatchService = new DirectiveDispatchService();
        private static ParseValueService _parseValueService = new ParseValueService();

        public Expression<Func<T, bool>> Filter<T>(string fe)
        {
            if (string.IsNullOrEmpty(fe))
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
                    if (Stack.Any() && Stack.First() == "!" && c != '!')
                    {
                        Stack.Pop();
                        Stack.Push(c.ToString());
                        Stack.Push("!");
                        continue;
                    }

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

            var resultStack = new Stack();

            for (var i = 0; i < Output.Count; i++)
            {
                if (Output[i] is not string)
                    resultStack.Push(Output[i]);
                else if (Output[i] is string && (string)Output[i] != "!")
                {
                    var val1 = (Expression)resultStack.Pop()!;
                    var val2 = (Expression)resultStack.Pop()!;

                    if (Output[i].ToString() == "&")
                        resultStack.Push(Expression.And(val1, val2));
                    else if (Output[i].ToString() == "|")
                        resultStack.Push(Expression.Or(val1, val2));
                }
                else if (Output[i] is string && (string)Output[i] == "!")
                {
                    var val1 = (Expression)resultStack.Pop()!;
                    resultStack.Push(Expression.Not(val1));
                }
            }

            var resultExpression = resultStack.Pop();

            return Expression.Lambda<Func<T, bool>>((Expression)resultExpression!, parameter);
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
                //ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, strData[0]);

                ConstantExpression value = _parseValueService.GetConstantExpression(strData[2], property.Type);

                IFilterDirective filterDirective = _dispatchService.GetDirective(strData[1]);

                return filterDirective.GenerateExpression(ref property, value);               
            }

            return result;
        }
    }
}
