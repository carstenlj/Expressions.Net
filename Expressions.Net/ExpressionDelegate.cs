using Expressions.Net.Evaluation;

namespace Expressions.Net
{
	/// <summary>
	/// Delegate representing a compiled expression
	/// </summary>
	/// <param name="variables">A map of named values that can be referenced by the expression</param>
	/// <returns>A single value as a result of the expression</returns>
	public delegate IValue ExpressionDelegate(IVariables? variables = null);
}
