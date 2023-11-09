using System;

namespace Expressions.Net.Evaluation
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class OperatorFunctionAttribute : Attribute
	{
		public string Operator { get; }

		public OperatorFunctionAttribute(string @operator)
		{
			this.Operator = @operator;
		}
	}
}
