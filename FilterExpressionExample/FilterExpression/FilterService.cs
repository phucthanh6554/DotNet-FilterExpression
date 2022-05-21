using FilterExpression.Constants;
using System;
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
        public Expression<Func<T, bool>> Filter<T>(string fe)
        {
            List<object> Output = new List<object>();
            Stack<string> Stack = new Stack<string>();

            string str = "";
            var operators = new List<string> { "(", "&", "|" };

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
                if (Output[i].GetType() == typeof(string) && i > 1)
                {
                    var val1 = (Expression)Output[i - 2];
                    var val2 = (Expression)Output[i - 1];

                    if (Output[i].ToString() == "&")
                        Output[i] = Expression.And(val1, val2);
                    else if (Output[i].ToString() == "|")
                        Output[i] = Expression.Or(val1, val2);

                    Output[i - 2] = null;

                    if (i - 3 > -1)
                    {
                        Output[i - 1] = Output[i - 3];
                        Output[i - 3] = null;
                    }
                    else
                    {
                        Output[i - 1] = null;
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
                //ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, strData[0]);
                var parsedValue = ParseValue(strData[2].Trim('`'), property.Type);

                var value = Expression.Constant(parsedValue, property.Type);

                MethodInfo func;

                switch (strData[1].ToLower())
                {
                    case Operators.Equal:
                        result = Expression.Equal(property, value);
                        break;
                    case Operators.LessThan:
                        result = Expression.LessThan(property, value);
                        break;
                    case Operators.LessThanAndEqual:
                        result = Expression.LessThanOrEqual(property, value);
                        break;
                    case Operators.GreaterThan:
                        result = Expression.GreaterThan(property, value);
                        break;
                    case Operators.GreaterThanAndEqual:
                        result = Expression.GreaterThanOrEqual(property, value);
                        break;
                    case Operators.StartsWith:
                        func = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

                        if (func == null)
                            throw new Exception("Cannot find startswith function");

                        result = Expression.Call(property, func, value);
                        break;
                    case Operators.Contains:
                        func = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

                        if (func == null)
                            throw new Exception("Cannot find startswith function");

                        result = Expression.Call(property, func, value);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private object ParseValue(string value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return int.Parse(value);
                case TypeCode.Double:
                    return double.Parse(value);
                case TypeCode.Decimal:
                    return decimal.Parse(value);
                case TypeCode.DateTime:
                    return DateTime.Parse(value);
                case TypeCode.String:
                default:
                    return value.ToString();
            }

            throw new Exception("Unsupported Value Type");

        }
    }
}
