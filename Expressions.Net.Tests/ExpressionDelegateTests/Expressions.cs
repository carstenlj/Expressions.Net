using Xunit;

namespace Expressions.Net.Tests
{
	[Trait("ExpressionDelegate", "Theories")]
	public class Expressions : TestBase
	{

		[InlineData("1 + 2 * 3", 7d)]
		[InlineData("('Hello' + ' ' + 'world').Length() / 2", 5.5d)] 
		[InlineData("item.name", "yolo max")]
		[InlineData("item.name.Length()", 8d)]
		[InlineData("!!item.success", true)]
		[InlineData("traits.Length()", 2d)]
		[Theory(DisplayName = "Can compile and evaluate to expected result ")]
		public void TestExpressionWithExprectedResult(string expr, object? expected)
		{
			// Arrange
			var expressionFunction = ExpressionEngine.CompileExpressionToDelegate(expr);
			var variables = ExpressionEngine.CreateVariables(TestConstants.Variables, null);

			// Act
			var result = expressionFunction(variables);

			// Assert
			Assert.NotNull(result);
			Assert.DoesNotContain("Invalid", result.GetType().Name);
			Assert.Equal(expected, result.Data);

		}
	}
}