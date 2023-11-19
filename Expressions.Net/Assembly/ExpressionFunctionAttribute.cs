using System;

namespace Expressions.Net.Assemblies
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ExpressionFunctionAttribute : Attribute
	{
		public string Signature { get; }

		public ExpressionFunctionAttribute(string signature)
		{
			this.Signature = signature;
		}
	}
}
