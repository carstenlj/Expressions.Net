using System;

namespace Expressions.Net.Reflection
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OperatorFunctionAttribute : Attribute
    {
        public string Operator { get; }

        public OperatorFunctionAttribute(string @operator)
        {
            Operator = @operator;
        }
    }
}
