using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression
{
    internal class ParseValueService
    {
        private object ParseValue(string value, Type type)
        {
            value = value.Trim('`', ' ');

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

        public ConstantExpression GetConstantExpression(string value, Type type)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null");

            if (value.Contains('['))
                return ParseValueArray(value, type);

            var parsedValue = ParseValue(value, type);

            return Expression.Constant(parsedValue, type);
        }

        private ConstantExpression ParseValueArray(string arrAsString, Type type)
        {
            var valueStrArr = arrAsString.Trim('`', '[', ']').Split(',');

            IList listValue = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            if (listValue == null)
                throw new Exception("Something went wrong");

            foreach (var v in valueStrArr)
                listValue.Add(ParseValue(v, type));

            return Expression.Constant(listValue);
        }
    }
}
