using Expressions.Net.Evaluation;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	/// <summary>
	/// All test cases for the <see cref="ExpressionDelegate"/> theory
	/// </summary>
	internal class CoreFunctionsTestSource : FunctionTestCaseResolverBase
	{
		public CoreFunctionsTestSource() 
			: base(FunctionsProvider.Default) { }

	}
}