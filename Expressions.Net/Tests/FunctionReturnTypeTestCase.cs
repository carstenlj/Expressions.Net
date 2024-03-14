using Expressions.Net.Evaluation;

namespace Expressions.Net.Tests
{

	public class FunctionReturnTypeTestCase
	{
		public string Expression { get; set; }
		public IValueType Returns { get; set; }

		public FunctionReturnTypeTestCase(string expression, IValueType returns)
		{
			Expression = expression;
			Returns = returns;
		}
	}
}