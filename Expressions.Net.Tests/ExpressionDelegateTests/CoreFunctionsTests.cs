using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Xunit;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	[Trait("ExpressionDelegate", "Theories")]
	public class CoreFunctionsTests : TestBase
	{
		[Theory(DisplayName = "Verify signature ")]
		[ClassData(typeof(CoreFunctionsTestSource))]
		public void TestExpressionWithExpectedResult(string Expression, string Returns)
		{
			// Arrange
			var expressionFunction = ExpressionEngine.CompileExpressionToDelegate(Expression);
			var variables = ExpressionEngine.CreateVariables(TestConstants.Variables, null);
			
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