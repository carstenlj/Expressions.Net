using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Xunit;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	[Trait("ExpressionDelegate", "Theories")]
	public class ExpressionFunctionTests : TestBase
	{
		[Theory(DisplayName = "Verify ")]
		[ClassData(typeof(ExpressionFunctionTestSource))]
		public void TestExpressionWithExpectedResult(string Expression, string Returns)
		{
			// Arrange
			var expressionFunction = ExpresisonFactory.CreateDelegate(Expression);
			var variables = ExpresisonFactory.CreateVariables(TestVariablesData, null);
			
			// Act
			var returnValueType = ValueTypeConverter.ConvertToValueType(Returns);
			var result = expressionFunction(variables);

			// Assert
			//Assert.Equal(Returns, result?.Type);
			Assert.True(returnValueType.CouldBe(result?.Type));
			Assert.True(result?.Type.CouldBe(returnValueType));

		}
	}
}