using System;

namespace Expressions.Net.Assembly
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
