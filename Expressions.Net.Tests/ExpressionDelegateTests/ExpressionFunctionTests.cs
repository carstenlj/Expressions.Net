using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValueTypes;
using Xunit;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	[Trait("ExpressionDelegate", "Theories")]
	public class ExpressionFunctionTests : TestBase
	{
		[Theory(DisplayName = "Expression returns expected type ")]
		[ClassData(typeof(ExpressionFunctionTestSource))]
		public void TestExpressionWithExprectedResult(string Expression, IValueType Returns)
		{
			// Arrange
			var expressionFunction = ExpresisonFactory.CreateDelegate(Expression);
			var variables = ExpresisonFactory.CreateVariables(TestVariablesData, null);

			// Act
			var result = expressionFunction(variables);

			// Assert
			//Assert.Equal(Returns, result?.Type);
			Assert.True(Returns.CouldBe(result?.Type));
			Assert.True(result?.Type.CouldBe(Returns));

		}
	}
}